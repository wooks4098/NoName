using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterState
{ 
    Idle = 0,
    FindPlayer,
    FollowPlayer,
    Attack,
    Strun,
    Die
    //�������� ������ ���� �� ����
    //�������� �԰� ���� -> Idle���·� ��ȯ�ϴ°��� �Ŀ� �۾�
}

enum MonsterFollow
{
    None = 0, //�������� ����
    OnlyMove, //�������� ������
    OnlyFollow, //�÷��̾ ����ٴ�
    FindAndFollow, // Ž�� �� ����ٴ�
}

public class MonsterController : MonoBehaviour
{
    [SerializeField] MonsterData monsterData;

    [SerializeField] MonsterState monsterState;
    [SerializeField] MonsterFollow monsterFollow;

    Vector3 movementVector = Vector3.zero; //�̵� ����



    //���� ����� ���� Ȯ�� ����
    [SerializeField] bool isStun = false;//�����ߴ���
    [SerializeField] bool IsKnockBack = false;//�����ߴ���
    [SerializeField] bool IsDie = false;
    [SerializeField] bool isAttacking = false;
    [SerializeField] bool CanAttack = true;
    [SerializeField] bool AttackLookAtEnd = false;
    [SerializeField] bool isStrunning = false;

    //���� �ݶ��̴�
    [SerializeField] BoxCollider DamageCollider;


    //���� �ǰ�ȿ��
    [SerializeField] Transform DamageEffectTransfrom; //�ǰ�ȿ�� ��ġ
    [SerializeField] Material IdleMateriel;  //�⺻ Materiel
    [SerializeField] Material DamageMateriel; //�ǰ� Materiel
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    //�ڷ�ƾ
    private Coroutine CoKnockBackCoroutine;
    private Coroutine CoFindPathCorutine;
    private Coroutine CoAttackCoolTime;
    private Coroutine CoAttackLookat;


    [SerializeField] List<Astar_Node> FollowPath; //Ž�� ��Ʈ

    MosnterStatusController mosnterStatusController;
    [SerializeField] MonsterDamage monsterDamage;
    [SerializeField] CharacterController characterController;


    Animator animator;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mosnterStatusController = GetComponent<MosnterStatusController>();
        monsterState = MonsterState.Idle;
        //monsterData.ChangeAnimator(animator);
    }



    private void OnEnable()
    {
        MonsterSpawn();

    }

    //���� ������ �� �ʱ�ȭ
    void MonsterSpawn()
    {
        //animator.play() �ʱ� �ִϸ��̼� ����

        mosnterStatusController.SetHp();
        isStun = false;
        skinnedMeshRenderer.material = IdleMateriel;

        characterController.enabled = true;

        SetMonsterStartState();
        //�÷��̾� Ž������( ������ X -> ĳ���Ͱ� �濡 ������ ���� ����)
    }

    private void Update()
    {
        movementVector.y -= monsterData.gravity;


        switch (monsterState)
        {
            case MonsterState.Idle:
                movementVector = Vector3.zero;
                break;
            case MonsterState.FindPlayer:
                break;
            case MonsterState.FollowPlayer:
                FollowPlayer();
                break;
            case MonsterState.Attack:
                movementVector = Vector3.zero;
                Attack();
                break;
            case MonsterState.Die:
                movementVector = Vector3.zero;
                return;
        }

        characterController.Move(movementVector * Time.deltaTime);



    }

    void SetMonsterStartState()
    {
        switch(monsterFollow)
        {
            case MonsterFollow.None:
                ChangeMonsterState(MonsterState.Idle);
                break;
            case MonsterFollow.OnlyMove:
                ChangeMonsterState(MonsterState.Idle);
                break;
            case MonsterFollow.FindAndFollow:
                ChangeMonsterState(MonsterState.FindPlayer);
                break;
            case MonsterFollow.OnlyFollow:
                ChangeMonsterState(MonsterState.FollowPlayer);
                break;
            default:
                ChangeMonsterState(MonsterState.FollowPlayer);
                break;
        }
    }

    public void ChangeMonsterState(MonsterState Changestate)
    {
        if (mosnterStatusController.GetHp() <= 0 && Changestate == MonsterState.Die)
        {
            StopAllCoroutines();
            Debug.Log(monsterState);
            if (monsterState != MonsterState.Die)
            {
                //MapManager �׾��ٰ� �˷��ֱ�
                MapManager.Instance.RemoveRoomMonster(this);
                //MonsterManager.Instance.MonsterDieCheck();
                animator.SetTrigger("DieTrigger");
                Debug.Log("trigger");
            }
            //characterController.enabled = false;
            monsterState = MonsterState.Die;
            return;
        }

        //if (Changestate == MonsterState.Die)
        //{
        //    StopAllCoroutines();
        //}

        monsterState = Changestate;
        DamageColliderOff();
        if (Changestate == MonsterState.FollowPlayer)
        {
            if (CoFindPathCorutine != null)
                StopCoroutine(CoFindPathCorutine);
            CoFindPathCorutine = StartCoroutine(FindingPlayerPath());
        }


        }

    //Monster State ���� FollowPlayer�� ����
    public void ChangeMonsterStateFollow()
    {

        if (mosnterStatusController.GetHp() <= 0)
        {
            StopAllCoroutines();

            if (monsterState != MonsterState.Die)
            {
                animator.SetTrigger("DieTrigger");
                Debug.Log("trigger");
            }
            characterController.enabled = false;
            monsterState = MonsterState.Die;
            return;
        }
        if (!isStrunning)
            monsterState = MonsterState.FollowPlayer;
        isAttacking = false;
        AttackLookAt_End();
        DamageColliderOff();

        if (CoFindPathCorutine != null)
            StopCoroutine(CoFindPathCorutine);
        CoFindPathCorutine = StartCoroutine(FindingPlayerPath());

    }

    public void CharacterControllerOff()
    {
        characterController.enabled = false;

    }

    void Monster_Rotation(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        //ȸ������(���ʹϾ�)����
        Quaternion targetangle = Quaternion.LookRotation(direction);
        //�������� �Լ��� �̿��� �ε巯�� ȸ��
        animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation, targetangle, Time.deltaTime * 8.0f); //Time.deltaTime * 8.0f);
    }


    void Attack()
    {
        if (CanAttack == true && isAttacking == false)
        {
            animator.SetTrigger("Attack");
            isAttacking = true;
            CanAttack = false;
            AttackLookAtEnd = false;
            StartCoroutine(AttackCoolTime());
            StartCoroutine(AttackLookat());

            if (CoAttackCoolTime != null)
                StopCoroutine(CoAttackCoolTime);
            CoAttackCoolTime = StartCoroutine(AttackCoolTime());

            if (CoAttackLookat != null)
                StopCoroutine(CoAttackLookat);
            CoAttackLookat = StartCoroutine(AttackLookat());

}
    }
    //���ݽ� �����ð� Ÿ�� �ٶ󺸱�
    IEnumerator AttackLookat()
    {
        float LookatTime = 0.35f;

        while (0 < LookatTime)
        {
            if (AttackLookAtEnd)
            {
                AttackLookAtEnd = false;
                break;
            }
            Monster_Rotation(GameManager.Instance.GetPlayerPos());
            yield return null;
        }
    }

    //������ Ÿ�� �ٶ󺸱� ������
    public void AttackLookAt_End()
    {
        AttackLookAtEnd = true;
    }

    //���� ��Ÿ��
    IEnumerator AttackCoolTime()
    {
        yield return new WaitForSeconds(1.3f);
        CanAttack = true;
    }
    //���� �ݶ��̴� �ѱ�
    public void DamageColliderOn()
    {
        DamageCollider.enabled = true;
    }
    //���� �ݶ��̴� ����
    public void DamageColliderOff()
    {
        DamageCollider.enabled = false;
    }

    void FollowPlayer()
    {
        float Distance = 100;
        if (FollowPath.Count < 3)
            Distance = Vector3.Distance(transform.position, GameManager.Instance.GetPlayerPos());

        //���� ���ɰŸ��� State Attack�� ����
        if (Distance <= monsterData.AttackRange)
        {
            ChangeMonsterState(MonsterState.Attack);
            animator.SetBool("Walk", false);
            return;
        }


        if (FollowPath.Count < 3 && Distance < 15)
        {//Ÿ�ٰ� ����� ���
            //Debug.Log(Distance);
            Vector3 vec3dir = (GameManager.Instance.GetPlayerPos() - transform.position).normalized;
            //transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, MoveSpeed * Time.deltaTime);

            //characterController.Move(vec3dir * Time.deltaTime * mosnterStatusController.monsterData.MoveSpeed);
            movementVector = vec3dir * mosnterStatusController.monsterData.MoveSpeed;
            //transform.position += vec3dir * Time.deltaTime * mosnterStatusController.monsterData.MoveSpeed;


            Monster_Rotation(GameManager.Instance.GetPlayerPos());
            animator.SetBool("Walk", true);

            // Path�� ������ ĭ�� ���� ��� ����
            if (FollowPath.Count > 0 && transform.position.x >= FollowPath[0].X * 10 + 3 && transform.position.x <= FollowPath[0].X * 10 + 7
                && transform.position.z >= FollowPath[0].Y * 10 + 3 && transform.position.z <= FollowPath[0].Y * 10 + 7)
            {
                FollowPath.RemoveAt(0);
            }
            //Debug.Log("����� �߰�");
        }
        else
        {//Ÿ�ٰ� �ָ� �ִ� ��� A* Path����Ͽ� �̵�
            if (FollowPath.Count <= 0)
                return;
            var Path = FollowPath[0];
            Vector3 target = new Vector3(Path.X * 10 + 5f, transform.position.y, Path.Y * 10 + 5f);  //10�� Plane���� ���߿� ������ �����ؾ���
            //transform.position = Vector3.MoveTowards(transform.position, target, mosnterStatusController.monsterData.MoveSpeed * Time.deltaTime);

            Vector3 vec3dir = (target - transform.position).normalized;
            //characterController.Move(vec3dir * Time.deltaTime * mosnterStatusController.monsterData.MoveSpeed);
            movementVector = vec3dir * mosnterStatusController.monsterData.MoveSpeed;

            animator.SetBool("Walk", true);
            Monster_Rotation(target);
            //Debug.Log("A*  Count > 2");

            // Path�� ������ ĭ�� ���� ��� ����
            if (transform.position.x >= FollowPath[0].X * 10 + 3 && transform.position.x <= FollowPath[0].X * 10 + 7
                && transform.position.z >= FollowPath[0].Y * 10 + 3 && transform.position.z <= FollowPath[0].Y * 10 + 7)
            {
                FollowPath.RemoveAt(0);
            }
        }
    }

    //��ǥ���� �� ã��
    IEnumerator FindingPlayerPath()
    {

        while (!IsDie)
        {
            FollowPath = MapManager.Instance.GetAstarPath(this.transform, GameManager.Instance.GetPlayerTrans());
            if (FollowPath.Count > 0)
                FollowPath.RemoveAt(0);
            yield return new WaitForSeconds(0.3f);
        }
    }


    #region ������ �Ծ��� ��

    void DamageCrowdControl(Weapon weapon)
    {
        bool isStrun = weapon.attackData[(int)weapon.GetPlayerSkill()].IsStun;
        float StrunTime = isStrun ? weapon.attackData[(int)weapon.GetPlayerSkill()].Stuntime : 0;
        DamageAni(isStrun);
        DamageKnockBack(weapon.attackData[(int)weapon.GetPlayerSkill()].KnockBack, isStrun, StrunTime);

    }

    //�˹� 
    void DamageKnockBack(float KnockBackRange, bool isStrun, float SturnTime)
    {
     
        if (KnockBackRange == 0)
            return;
        if(isStrun)
            ChangeMonsterState(MonsterState.Strun);

        Vector3 KnockBackPosition = (transform.position - GameManager.Instance.GetPlayerPos()).normalized;
        KnockBackPosition *= KnockBackRange;
        KnockBackPosition.y = 0;

        KnockBackPosition += transform.position;

        if (CoKnockBackCoroutine != null)
            StopCoroutine(CoKnockBackCoroutine);
        CoKnockBackCoroutine = StartCoroutine(KnockBackMove(KnockBackPosition, isStrun, SturnTime));
    }

    //�˹� �̵�
    IEnumerator KnockBackMove(Vector3 KnockBackPosition, bool isStrun, float SturnTime)
    {

        float time = 0f;
        float timeCheck = 1f;
        IsKnockBack = true;
        var currPos = transform.position;
        Vector3 vec3dir;
        while (currPos != KnockBackPosition && timeCheck > time)//��ǥ ��ġ�� ���� or �ð��� ������ �ݺ��� Ż��
        {
            time += Time.deltaTime;
            currPos = Vector3.Lerp(currPos, KnockBackPosition, time);
            //transform.position = currPos;

            vec3dir = (KnockBackPosition - transform.position);

            movementVector = vec3dir * mosnterStatusController.monsterData.MoveSpeed;

            yield return null;
        }
        if (isStrun)
            StartCoroutine(DoStrun(SturnTime));
        IsKnockBack = false;
    }
    //����
    IEnumerator DoStrun(float SturnTime)
    {
        isStrunning = true;
        animator.SetBool("IsStrun", true);
        yield return new WaitForSeconds(SturnTime- 0.2F);
        animator.SetBool("IsStrun", false);
        isStrunning = false;
        yield return new WaitForSeconds(0.2F);
        ChangeMonsterStateFollow();
    }

    //������ �ִϸ��̼�
    void DamageAni(bool isStun)
    {
        //StunHit�� ��� ��쿩�� �ִϸ��̼� �߻�
        //Hit�� Attacking�߿��� �߻� X


        if ( isStun)
            animator.SetTrigger("StunHit");
        else if(!isAttacking)
            animator.SetTrigger("Hit");
    }


    //������ ȿ��
    void DamageEffect()
    {
        var particleobject = ObjectPoolManger.Instance.ReturnObject(ObjectType.Effect);
        if (particleobject == null)
            return;
        ParticleShow _particle = particleobject.GetComponent<ParticleShow>();
        _particle.ChangeTransfrom(DamageEffectTransfrom);
        _particle.ParticlePlay();
        StartCoroutine(ChangeDamageMaterial());
    }

    IEnumerator ChangeDamageMaterial()
    {
        skinnedMeshRenderer.material = IdleMateriel;
        yield return new WaitForSeconds(0.02f);
        skinnedMeshRenderer.material = DamageMateriel;
        yield return new WaitForSeconds(0.1f);
        skinnedMeshRenderer.material = IdleMateriel;
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        //GetComponent<Rigidbody>().isKinematic = true;
        if (other.tag == "Weapon")
        {
            if (monsterState == MonsterState.Die)
                return;
            DamageEffect();
            //���� �޴����� ���� ���� ��������
            Weapon weapon = FindObjectOfType<Player_Attack>().GetWeapon();
            DamageCrowdControl(weapon);
            mosnterStatusController.Damage(35);
            //mosnterStatusController.Die();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //GetComponent<Rigidbody>().isKinematic = false;

    }
    #region �����

    private void OnDrawGizmosSelected()
    {
        DrawGizoms_AstarPaht();
    }

    void DrawGizoms_AstarPaht()
    {
        Gizmos.color = Color.white;
        if (FollowPath.Count != 0) for (int i = 0; i < FollowPath.Count - 1; i++)
                Gizmos.DrawLine(new Vector3(FollowPath[i].X * 10 +5f , 0.5f, FollowPath[i].Y * 10 + 5f), new Vector3(FollowPath[i + 1].X * 10 +5f, 0.5f, FollowPath[i + 1].Y * 10 +5f));
    }

    #endregion
}