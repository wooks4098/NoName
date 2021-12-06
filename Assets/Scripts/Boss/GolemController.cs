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
    [SerializeField] GolemState DongState; //해야할 상태
    [SerializeField] float WalkSpeed;
    [Space]
    [Space]

    //Attack
    [SerializeField] float attackRange;
    [SerializeField] float attackCoolTime;
    [SerializeField] bool canAttack = true;
    [SerializeField] bool isAttack = false;//공격중인지
    //Vector3 PlayerPos;

    NavMeshAgent agent;
    Animator ani;

    //test
    [Space]
    [Space]
    [SerializeField] Transform PlayerPos;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();

        //agent 자동 회전 종료
        //agent.updateRotation = false;
        agent.speed = WalkSpeed;

        BossState = GolemState.Wait;

        ChangeState(GolemState.Attack);
    }




    private void FixedUpdate()
    {
        //PlayerPos = GameManager.Instance.GetPlayerPos();

        switch (BossState)
        {
            case GolemState.Follow:
                Follow();
                break;
            case GolemState.Attack:
                Attack();
                break;
        }
        //Rotation();
    }

    void Follow()
    {
        agent.SetDestination(PlayerPos.position);
        switch(DongState)
        {
            case GolemState.Attack:
                if (canAttack)
                    ChangeState(GolemState.Attack);
                break;
        }
    }

    void TargetMove(bool hapathCheck, Vector3 target, float EndRange = 0.2f)
    {
        
            agent.SetDestination(target);
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
    void Attack()
    {
        if (!agent.pathPending && agent.remainingDistance <= attackRange && canAttack)
            StartCoroutine(AttackCoolTime());
        else
            ChangeState(GolemState.Follow);

    }

    IEnumerator AttackCoolTime()
    {
        isAttack = true;
        canAttack = false;
        agent.isStopped = true;
        ani.SetTrigger("Attack");
        yield return new WaitForSeconds(1f);
        isAttack = false;
        //ChangeState(GolemState.Follow);
        agent.isStopped = false;
        yield return new WaitForSeconds(attackCoolTime-1);
        canAttack = true;
    }

    void Throw()
    {

    }

    void Rush()
    {

    }


    public void ChangeState(GolemState _golemState)
    {
        BossState = _golemState;
        switch (BossState)
        {
            case GolemState.Attack:
                agent.SetDestination(PlayerPos.position);
                break;
            case GolemState.Follow:
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
