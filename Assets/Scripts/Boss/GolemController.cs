using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum GolemState
{
    Wait = 0,
    Follow, //따라가기
    JumpAttack, //점프 공격
    Attack, //평타 공격
    Throw, //던지기
    Rush, //돌진
}

public class GolemController : MonoBehaviour
{
    [SerializeField] GolemState BossState; //현재 상태
    [SerializeField] float WalkSpeed;
    [Space]
    [Space]
    
    //Attack
    [SerializeField] float attackRange;
    [SerializeField] float attackCoolTime;
    [SerializeField] bool canAttack = true;
    [SerializeField] GameObject AttackHitBox;
    //Vector3 PlayerPos;

    NavMeshAgent agent;
    Animator ani;

    //test
    [Space]
    [Space]
    [SerializeField] Vector3 PlayerPos;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();

        //agent 자동 회전 종료
        agent.updateRotation = false;
        agent.speed = WalkSpeed;

        BossState = GolemState.Wait;


    }

    private void OnEnable()
    {
        StartCoroutine(StartState());
    }

    IEnumerator StartState()
    {
        yield return new WaitForSeconds(2f);
        ChangeState(GolemState.Attack);

    }

    private void FixedUpdate()
    {
        PlayerPos = GameManager.Instance.GetPlayerPos();
        if (PlayerPos == null)
            return;
        switch (BossState)
        {
            case GolemState.Follow:
                Follow();
                break;
            case GolemState.Attack:
                if (!agent.pathPending)
                    Attack();
                break;
        }

    }




    void Rotation()
    {
        if (agent.hasPath)
        {
            //회전 미끄러짐 방지
            agent.acceleration = 60f;
            //에이전트의 이동방향
            Vector3 direction = agent.desiredVelocity;
            //회전각도(쿼터니언)산출
            Quaternion targetangle = Quaternion.LookRotation(direction);
            //선형보간 함수를 이용해 부드러운 회전
            ani.transform.rotation = Quaternion.Slerp(ani.transform.rotation, targetangle, Time.deltaTime * 8.0f);
            ani.transform.eulerAngles = new Vector3(0, ani.transform.rotation.eulerAngles.y, 0);
        }
    }


    #region Follow
    void StartFollow()
    {
        ani.SetBool("IsWalk", true);
        agent.SetDestination(PlayerPos);

    }
    void Follow()
    {
        agent.SetDestination(PlayerPos);

    }

    void EndFollow()
    {
        ani.SetBool("IsWalk", false);

    }

    #endregion


    #region Attack
    void StartAttack()
    {
        agent.SetDestination(PlayerPos);
        canAttack = true;
    }
    void Attack()
    {
        //Debug.Log(agent.remainingDistance);
        //플레이어를 따라가다 공격가능 범위면 공격해야한다
        if (agent.remainingDistance <= attackRange)
        {
            if(canAttack)
            {
                Debug.Log("공격");
                agent.speed = 0;
                canAttack = false;
                ani.SetTrigger("Attack");
                StartCoroutine(AttackCooltime());
                StartCoroutine(AttackRotation());
            }
            else
                ani.SetBool("IsWalk", false);

        }
        else
        {
            ani.SetBool("IsWalk", true);
            agent.SetDestination(PlayerPos);
            Rotation();
        }
    }
    //공격시 플레이어 바라보도록
    IEnumerator AttackRotation()
    {
        float time = 0;
        while(time <0.5f)
        {
            time += Time.deltaTime;
            Vector3 TargetVector = PlayerPos - transform.position;
            //선형보간 함수를 이용해 부드러운 회전
            ani.transform.rotation = Quaternion.Slerp(ani.transform.rotation, Quaternion.LookRotation(TargetVector).normalized, 1f);
            yield return null;
        }
       
    }
    IEnumerator AttackCooltime()
    {
        yield return new WaitForSeconds(4f);
        Debug.Log("공격쿨타임 끝");

        canAttack = true;
        agent.speed = WalkSpeed;
        EndAttack();
    }

    void EndAttack()
    {
        ani.SetBool("IsWalk", false);
        SelectState();
    }


    #region 공격 hitbox

    public void OpenHitbox()
    {
        AttackHitBox.SetActive(true);
    }

    public void CloseHitBox()
    {
        AttackHitBox.SetActive(false);
    }
    #endregion

    #endregion




    public void ChangeState(GolemState _golemState)
    {
        BossState = _golemState;
        switch (BossState)
        {
            case GolemState.Attack:
                StartAttack();
                break;
            case GolemState.Follow:
                StartFollow();
                break;
        }
    }

    //어떤 상태를 할지 정하는 함수
    void SelectState()
    {
        GolemState changeState = GolemState.Attack;

        ChangeState(changeState);
    }





    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
