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
    Rigidbody rigid;

    [SerializeField] GolemState BossState; //���� ����
    [SerializeField] float WalkSpeed;

    [Header("Follow")]
    [SerializeField] float FollowTime;//���󰡱� �ð�
    [SerializeField] float FollowSpeed;//���󰡱� �ӵ�
    [SerializeField] bool isFollow; //���󰡴� ������
    //Attack
    [Header("Attack")]
    [SerializeField] float attackRange;
    [SerializeField] float attackCoolTime;
    [SerializeField] bool canAttack = true;
    [SerializeField] GameObject AttackHitBox;

    //Rush
    [Header("Rush")]
    [SerializeField] float RushRange;
    [SerializeField] float RushSpeed;
    Vector3 RushDir;
    [SerializeField] bool IsEndRush = false;

    [Header("JumpAttack")]
    [SerializeField] float JumpUpPower;
    [SerializeField] float JumpDownPower;
    NavMeshAgent agent;
    Animator ani;
    [Space]
    [Space]
    [SerializeField] GameObject RushMovePos;

    //test
    [SerializeField] Vector3 PlayerPos;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
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
        yield return new WaitForSeconds(3f);
        ChangeState(GolemState.JumpAttack);

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
                //if (!agent.pathPending)
                //    Rush();
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
        agent.speed = FollowSpeed;
        StartCoroutine(FollowTimeCheck());
        isFollow = true;
    }
    void Follow()
    {
        if(isFollow)
        {
            agent.SetDestination(PlayerPos);
            Rotation();
        }
    }

    void EndFollow()
    {
        agent.speed = WalkSpeed;
        ani.SetBool("IsWalk", false);
        ChangeState(GolemState.Rush);
        isFollow = false;
    }

    IEnumerator FollowTimeCheck()
    {
        yield return new WaitForSeconds(FollowTime);
        EndFollow();
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
    void EndAttack()
    {
        ani.SetBool("IsWalk", false);
        ChangeState(GolemState.Rush);
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
        Debug.Log("Rush����");
        RushMovePos.GetComponent<RushEndCollider>().ColliderOn();
        agent.ResetPath();
        ani.SetTrigger("Rush");
        StartCoroutine(ReadyRushDir());
    }

    void Rush()
    {
        //Debug.Log("rush����");
        //if (!IsEndRush && (agent.velocity.sqrMagnitude >= 0.5f * 0.5f && agent.remainingDistance <= 0.5f))//�̵�����
        //{
        //    IsEndRush = true;
        //    EndRush();
        //}
    }

    public void EndRush()
    {
        Debug.Log("��");
        IsEndRush = true;
        agent.ResetPath();
        ani.SetTrigger("EndRush");
        agent.speed = WalkSpeed;
        ChangeState(GolemState.Follow);
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
            RushMovePos.transform.position = RushDir;
            yield return null;
        }
        Debug.Log("rush��");

        IsEndRush = false;
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


    #region JumpAttack

    void StartJumpAttack()
    {
        
        Debug.Log("����");
        agent.ResetPath();
        agent.enabled = false;
        rigid.isKinematic = false;
        ani.SetTrigger("JumpAttack");
    }

    void JumpAttack()
    {

    }

    public void EndJumpAttack()
    {
        agent.enabled = true;
        rigid.isKinematic = true;
    }

    public void  AniJump()
    {
        Debug.Log("����");
       rigid.AddForce(transform.up * JumpUpPower, ForceMode.Impulse);
    }
    //�ְ����� �� �Ʒ��� �� �ִ� �Լ�
    public void AniJumpDown()
    {
        rigid.AddForce(-transform.up * JumpDownPower, ForceMode.Impulse);

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
            case GolemState.JumpAttack:
                StartJumpAttack();
                break;
        }
    }

    //� ���¸� ���� ���ϴ� �Լ�
    void SelectState()
    {

        GolemState changeState = GolemState.Follow;
        switch (Random.Range(0,2))
        {
            case 0:
                changeState = GolemState.Attack;
                break;
            case 1:
                changeState = GolemState.Rush;
                break;
        }
        changeState = GolemState.Rush;
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
