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
    //데미지는 언제든 입을 수 있음
    //데미지를 입고 기절 -> Idle상태로 전환하는것은 후에 작업
}


public class MonsterController : MonoBehaviour
{
    [SerializeField] MonsterData monsterData;

    [SerializeField] MonsterState monsterState;


    //상태 변경시 조건 확인 변수
    [SerializeField] bool isStun = false;//기절했는지
    [SerializeField] bool IsKnockBack = false;//기절했는지
    [SerializeField] bool IsDie = false;
    [SerializeField] bool isAttacking = false;
    [SerializeField] bool CanAttack = true;
    [SerializeField] bool AttackLookAtEnd = false;
    [SerializeField] bool isStrunning = false;

    //공격 콜라이더
    [SerializeField] BoxCollider DamageCollider;


    //몬스터 피격효과
    [SerializeField] Transform DamageEffectTransfrom; //피격효과 위치
    [SerializeField] Material IdleMateriel;  //기본 Materiel
    [SerializeField] Material DamageMateriel; //피격 Materiel
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    //코루틴
    private Coroutine CoKnockBackCoroutine;
    private Coroutine CoFindPathCorutine;
    private Coroutine CoAttackCoolTime;
    private Coroutine CoAttackLookat;


    [SerializeField] List<Astar_Node> FollowPath; //탐색 루트

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



        //플레이어 탐색시작( 생성후 X -> 캐릭터가 방에 들어왔을 경우로 수정)
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

    //Monster State 상태 FollowPlayer로 변경
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
        //회전각도(쿼터니언)산출
        Quaternion targetangle = Quaternion.LookRotation(direction);
        //선형보간 함수를 이용해 부드러운 회전
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
    //공격시 일정시간 타겟 바라보기
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

    //공격중 타겟 바라보기 끝내기
    public void AttackLookAt_End()
    {
        AttackLookAtEnd = true;
    }

    //공격 쿨타임
    IEnumerator AttackCoolTime()
    {
        yield return new WaitForSeconds(1.3f);
        CanAttack = true;
    }
    //공격 콜라이더 켜기
    public void DamageColliderOn()
    {
        DamageCollider.enabled = true;
    }
    //공격 콜라이더 끄기
    public void DamageColliderOff()
    {
        DamageCollider.enabled = false;
    }

    void FollowPlayer()
    {
        float Distance = 100;
        if (FollowPath.Count < 3)
            Distance = Vector3.Distance(transform.position, MonsterManager.Instance.GetPlayerPos());

        //공격 가능거리면 State Attack로 변경
        if (Distance <= monsterData.AttackRange)
        {
            ChangeMonsterState(MonsterState.Attack);
            animator.SetBool("Walk", false);
            return;
        }


        if (FollowPath.Count < 3 && Distance < 15)
        {//타겟과 가까운 경우
            //Debug.Log(Distance);
            Vector3 vec3dir = (MonsterManager.Instance.GetPlayerPos() - transform.position).normalized;
            //transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, MoveSpeed * Time.deltaTime);

            transform.position += vec3dir * Time.deltaTime * mosnterStatusController.monsterData.MoveSpeed;


            Monster_Rotation(MonsterManager.Instance.GetPlayerPos());
            animator.SetBool("Walk", true);

            // Path의 마지막 칸에 들어올 경우 제거
            if (FollowPath.Count > 0 && transform.position.x >= FollowPath[0].X * 10 + 3 && transform.position.x <= FollowPath[0].X * 10 + 7
                && transform.position.z >= FollowPath[0].Y * 10 + 3 && transform.position.z <= FollowPath[0].Y * 10 + 7)
            {
                FollowPath.RemoveAt(0);
            }
            //Debug.Log("가까운 추격");
        }
        else
        {//타겟과 멀리 있는 경우 A* Path사용하여 이동
            if (FollowPath.Count <= 0)
                return;
            var Path = FollowPath[0];
            Vector3 target = new Vector3(Path.X * 10 + 5f, transform.position.y, Path.Y * 10 + 5f);  //10은 Plane길이 나중에 변수로 수정해야함
            transform.position = Vector3.MoveTowards(transform.position, target, mosnterStatusController.monsterData.MoveSpeed * Time.deltaTime);
            animator.SetBool("Walk", true);
            Monster_Rotation(target);
            //Debug.Log("A*  Count > 2");

            // Path의 마지막 칸에 들어올 경우 제거
            if (transform.position.x >= FollowPath[0].X * 10 + 3 && transform.position.x <= FollowPath[0].X * 10 + 7
                && transform.position.z >= FollowPath[0].Y * 10 + 3 && transform.position.z <= FollowPath[0].Y * 10 + 7)
            {
                FollowPath.RemoveAt(0);
            }
        }
    }

    //목표지점 길 찾기
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


    #region 데미지 입었을 때

    void DamageCrowdControl(Weapon weapon)
    {
        bool isStrun = weapon.attackData[(int)weapon.GetPlayerSkill()].IsStun;
        float StrunTime = isStrun ? weapon.attackData[(int)weapon.GetPlayerSkill()].Stuntime : 0;
        DamageAni(isStrun);
        DamageKnockBack(weapon.attackData[(int)weapon.GetPlayerSkill()].KnockBack, isStrun, StrunTime);

    }

    //넉백 
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

    //넉백 이동
    IEnumerator KnockBackMove(Vector3 KnockBackPosition, bool isStrun, float SturnTime)
    {

        float time = 0f;
        float timeCheck = 1f;
        IsKnockBack = true;
        var currPos = transform.position;
        while (currPos != KnockBackPosition && timeCheck > time)//목표 위치에 도착 or 시간이 지나면 반복문 탈출
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
    //스턴
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

    //데미지 애니메이션
    void DamageAni(bool isStun)
    {
        //StunHit은 어떠한 경우여도 애니메이션 발생
        //Hit은 Attacking중에는 발생 X


        if ( isStun)
            animator.SetTrigger("StunHit");
        else if(!isAttacking)
            animator.SetTrigger("Hit");
    }


    //데미지 효과
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
            //게임 메니저로 무기 종류 가져오기
            Weapon weapon = FindObjectOfType<Player_Attack>().GetWeapon();
            DamageCrowdControl(weapon);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        GetComponent<Rigidbody>().isKinematic = false;

    }
    #region 기즈모

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