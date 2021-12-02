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

        //agent 자동 회전 종료
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
