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

        //agent �ڵ� ȸ�� ����
        agent.updateRotation = false;
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
                if (!agent.pathPending)
                    Attack();
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
        agent.SetDestination(PlayerPos.position);

    }
    void Follow()
    {
        agent.SetDestination(PlayerPos.position);

    }

    void EndFollow()
    {

    }

    #endregion


    #region Attack
    void StartAttack()
    {
        agent.SetDestination(PlayerPos.position);
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
            }
        }
        else
        {
            agent.SetDestination(PlayerPos.position);
            Rotation();
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
        SelectState();
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
        }
    }

    //� ���¸� ���� ���ϴ� �Լ�
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
