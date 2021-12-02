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
    [SerializeField] GolemState state;
    [SerializeField] float WalkSpeed;
    [Space] [Space]

    [SerializeField] float attackRange;

    Vector3 PlayerPos;

    NavMeshAgent agent;
    Animator ani;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();

        //agent �ڵ� ȸ�� ����
        agent.updateRotation = false;
        agent.speed = WalkSpeed;

        state = GolemState.Wait;
    }
    private void Update()
    {
        PlayerPos = GameManager.Instance.GetPlayerPos();

        switch (state)
        {
            case GolemState.Follow:
                Follow();
                break;
            case GolemState.JumpAttack:
                JumpAttack();
                break;
            case GolemState.Attack:
                Attack();
                break;
            case GolemState.Throw:
                Throw();
                break;
            case GolemState.Rush:
                Rush();
                break;
        }
        Rotation();
    }

    void TargetMove(Vector3 target)
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

    void Follow()
    {

    }

    void JumpAttack()
    {

    }    

    void Attack()
    {
        if(agent.remainingDistance<= attackRange)
        {

        }

    }  

    void Throw()
    {

    }

    void Rush()
    {

    }


    public void ChangeState(GolemState _golemState)
    {
        state = _golemState;
        switch(state)
        {
            case GolemState.Attack:
            case GolemState.Follow:
                TargetMove(PlayerPos);
                break;
        }
    }
}
