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
    [SerializeField] GolemState DongState; //�ؾ��� ����
    [SerializeField] float WalkSpeed;
    [Space]
    [Space]

    //Attack
    [SerializeField] float attackRange;
    [SerializeField] float attackCoolTime;
    [SerializeField] bool canAttack = true;
    [SerializeField] bool isAttack = false;//����������
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
