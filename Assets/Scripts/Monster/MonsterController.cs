using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MonsterState
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


public class MonsterController : MonoBehaviour
{
    [SerializeField] MonsterData monsterData;

    [SerializeField] MonsterState monsterState;


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
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        mosnterStatusController = GetComponent<MosnterStatusController>();
        monsterState = MonsterState.Idle;
    }



    private void OnEnable()
    {
        isStun = false;
        skinnedMeshRenderer.material = IdleMateriel;



        //�÷��̾� Ž������( ������ X -> ĳ���Ͱ� �濡 ������ ���� ����)
        CoFindPathCorutine = StartCoroutine(FindingPlayerPath());

    }
    private void Update()
    {

        switch (monsterState)
        {
            case MonsterState.Idle:
                break;
            case MonsterState.FindPlayer:
                break;
            case MonsterState.FollowPlayer:
                FollowPlayer();
                break;
            case MonsterState.Attack:
                Attack();
                break;
        }




    }


    void ChangeMonsterState(MonsterState Changestate)
    {
        monsterState = Changestate;
        DamageColliderOff();

    }

    //Monster State ���� FollowPlayer�� ����
    public void ChangeMonsterStateFollow()
    {
        if(!isStrunning)
            monsterState = MonsterState.FollowPlayer;
        isAttacking = false;
        AttackLookAt_End();
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

            //if (CoAttackCoolTime != null)
            //    StopCoroutine(CoAttackCoolTime);
            //CoAttackCoolTime = StartCoroutine(AttackCoolTime());
            //if (CoAttackLookat != null)
            //    StopCoroutine(CoAttackCoolTime);
            //CoAttackLookat = StartCoroutine(AttackLookat());

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
            Monster_Rotation(MonsterManager.Instance.GetPlayerPos());
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
            Distance = Vector3.Distance(transform.position, MonsterManager.Instance.GetPlayerPos());

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
            Vector3 vec3dir = (MonsterManager.Instance.GetPlayerPos() - transform.position).normalized;
            //transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, MoveSpeed * Time.deltaTime);

            transform.position += vec3dir * Time.deltaTime * mosnterStatusController.monsterData.MoveSpeed;


            Monster_Rotation(MonsterManager.Instance.GetPlayerPos());
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
            transform.position = Vector3.MoveTowards(transform.position, target, mosnterStatusController.monsterData.MoveSpeed * Time.deltaTime);
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
        monsterState = MonsterState.FollowPlayer;
        while (!IsDie)
        {
            FollowPath = MapManager.Instance.GetAstarPath(this.transform, MonsterManager.Instance.GetPlayerTransfrom());
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

        Vector3 KnockBackPosition = (transform.position - MonsterManager.Instance.GetPlayerPos()).normalized;
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
        while (currPos != KnockBackPosition && timeCheck > time)//��ǥ ��ġ�� ���� or �ð��� ������ �ݺ��� Ż��
        {
            time += Time.deltaTime;
            currPos = Vector3.Lerp(currPos, KnockBackPosition, time);
            transform.position = currPos;

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
        GetComponent<Rigidbody>().isKinematic = true;
        if (other.tag == "Weapon")
        {
            DamageEffect();
            //���� �޴����� ���� ���� ��������
            Weapon weapon = FindObjectOfType<Player_Attack>().GetWeapon();
            DamageCrowdControl(weapon);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        GetComponent<Rigidbody>().isKinematic = false;

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