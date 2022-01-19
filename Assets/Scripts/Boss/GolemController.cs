using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum GolemState
{
    Wait = 0,
    Follow, //���󰡱�
    JumpAttack, //���� ����
    Attack, //��Ÿ ����
    Throw, //������
    Rush, //����
}

public class GolemController : MonoBehaviour
{
    [SerializeField] GolemState BossState; //���� ����
    [SerializeField] float WalkSpeed;
    [Space]
    [Space]
    
    //Attack
    [SerializeField] float attackRange;
    [SerializeField] float attackCoolTime;
    [SerializeField] bool canAttack = true;
    [SerializeField] GameObject AttackHitBox;
    [Space]
    [Space]
    //Rush

    Vector3 RushDir;
    [SerializeField] float RushRange;
    [SerializeField] float RushSpeed;
    //[SerializeField] bool IsEndRushReady = false;
    [SerializeField] bool IsEndRush = false;
    NavMeshAgent agent;
    Animator ani;
    [SerializeField] GameObject testCube;

    //test
    [Space]
    [Space]
    [SerializeField] Vector3 PlayerPos;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();

        //agent �ڵ� ȸ�� ����
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
        ChangeState(GolemState.Rush);

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
            case GolemState.Rush:
                if (!agent.pathPending)
                    Rush();
                break;
        }

    }




    void Rotation()
    {
        if (agent.hasPath)
        {
            //ȸ�� �̲����� ����
            agent.acceleration = 60f;
            //������Ʈ�� �̵�����
            Vector3 direction = agent.desiredVelocity;
            //ȸ������(���ʹϾ�)����
            Quaternion targetangle = Quaternion.LookRotation(direction);
            //�������� �Լ��� �̿��� �ε巯�� ȸ��
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
        //�÷��̾ ���󰡴� ���ݰ��� ������ �����ؾ��Ѵ�
        if (agent.remainingDistance <= attackRange)
        {
            if(canAttack)
            {
                Debug.Log("����");
                agent.speed = 0;
                canAttack = false;
                ani.SetTrigger("Attack");
                StartCoroutine(AttackCooltime());
                StartCoroutine(AttackLookAtPlayer());
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
    //���ݽ� �÷��̾� �ٶ󺸵���
    IEnumerator AttackLookAtPlayer()
    {
        float time = 0;
        while (time <= 0.3)
        {
            time += Time.deltaTime;
            LookPlayer();
            yield return null;

        }
        yield break;
    }
    IEnumerator AttackRotation()
    {
        float time = 0;
        while(time <0.5f)
        {
            time += Time.deltaTime;
            Vector3 TargetVector = PlayerPos - transform.position;
            //�������� �Լ��� �̿��� �ε巯�� ȸ��
            ani.transform.rotation = Quaternion.Slerp(ani.transform.rotation, Quaternion.LookRotation(TargetVector).normalized, 1f);
            yield return null;
        }
       
    }
    IEnumerator AttackCooltime()
    {
        yield return new WaitForSeconds(4f);
        Debug.Log("������Ÿ�� ��");

        canAttack = true;
        agent.speed = WalkSpeed;
        EndAttack();
    }

    void EndAttack()
    {
        ani.SetBool("IsWalk", false);
        ChangeState(GolemState.Rush);
        //SelectState();
    }


    #region ���� hitbox

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


    #region Rush

    void StartRush()
    {
        //Debug.Log("����");

        agent.ResetPath();
        IsEndRush = false;
        ani.SetTrigger("Rush");
        StartCoroutine(ReadyRushDir());
    }

    void Rush()
    {
        if (!IsEndRush && (agent.velocity.sqrMagnitude >= 0.1f * 0.1f && agent.remainingDistance <= 0.1f))//�̵�����
        {
            
            IsEndRush = true;
            EndRush();
        }
    }

    void EndRush()
    {
        Debug.Log("��");
        agent.ResetPath();
        ani.SetTrigger("EndRush");
        agent.speed = WalkSpeed;
        ChangeState(GolemState.Rush);
    }

    IEnumerator ReadyRushDir()
    {
        float time = 0;
        while(time < 3f)
        {
            time += Time.deltaTime;
            //Debug.Log("������");

            LookPlayer();
            RushDir = (PlayerPos - transform.position).normalized * RushRange + transform.position;
            testCube.transform.position = RushDir;
            yield return null;
        }
        agent.speed = RushSpeed;
        if (SetDestination(RushDir))
            agent.SetDestination(RushDir);
        else
            agent.SetDestination(PlayerPos);
    }

    //public void AniStartRush()
    //{
    //    IsEndRushReady = true;
    //}


    private bool SetDestination(Vector3 targetDestination)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetDestination, out hit, 1f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            return true;
        }
        return false;
    }
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
            case GolemState.Rush:
                StartRush();
                break;
        }
    }

    //� ���¸� ���� ���ϴ� �Լ�
    void SelectState()
    {
        GolemState changeState = GolemState.Attack;

        ChangeState(changeState);
    }



    void LookPlayer()
    {
        //������Ʈ�� �̵�����
        Vector3 direction = PlayerPos - transform.position;
        //ȸ������(���ʹϾ�)����
        Quaternion targetangle = Quaternion.LookRotation(direction);
        //�������� �Լ��� �̿��� �ε巯�� ȸ��
        ani.transform.rotation = Quaternion.Slerp(ani.transform.rotation, targetangle, Time.deltaTime * 8.0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
