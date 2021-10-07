using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField] Transform DamageEffectTransfrom;
    [SerializeField] Material IdleMateriel;
    [SerializeField] Material DamageMateriel;
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;

    [SerializeField] bool isStun = false;//기절했는지
    [SerializeField] bool IsKnockBack = false;//기절했는지
    [SerializeField] bool IsDie = false;

    [SerializeField] float MoveSpeed;

    private Coroutine KnockBackCoroutine;
    private Coroutine FindPathCorutine;

    //Test용
    public GameObject Player;

    [SerializeField] List<Astar_Node> FollowPath;


    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();

        //이동 테스트 코드
        //FollowPath = MapManager.Instance.GetAstarPath(this.transform,Player.transform);
        //FollowPath.RemoveAt(0);
    }

    //private void Start()
    //{
    //}

    private void OnEnable()
    {
        isStun = false;
        skinnedMeshRenderer.material = IdleMateriel;
        //FindPathCorutine = FindingPlayerPath();

        FindPathCorutine = StartCoroutine(FindingPlayerPath());

    }
    private void Update()
    {

        //이동 테스트 코드
        //if (FollowPath.Count > 0)
        //{
        //    var path = FollowPath[0];
        //    Vector3 target = new Vector3(path.X * 10 + 5f, 0, path.Y * 10 + 5f);
        //    transform.position = Vector3.MoveTowards(transform.position, new Vector3(path.X * 10 + 5f, 0, path.Y * 10 + 5f), 0.2f);
        //    if (2f >= Vector3.Distance(transform.position, target))
        //    {
        //        FollowPath.RemoveAt(0);
        //    }
        //}

        if (FollowPath.Count < 2 && Vector3.Distance(transform.position, Player.transform.position) < 10)
        {
            transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, MoveSpeed * Time.deltaTime);
            Monster_Rotation(Player.transform.position);
            Debug.Log("가까운 추격");
        }
        else 
        {
            if (FollowPath.Count <= 0)
                return;
            var Path = FollowPath[0];
            Vector3 target = new Vector3(Path.X * 10 + 5f, transform.position.y, Path.Y * 10 + 5f);  //10은 Plane길이 나중에 변수로 수정해야함
            transform.position = Vector3.MoveTowards(transform.position, target, MoveSpeed * Time.deltaTime);
            Monster_Rotation(target);
            Debug.Log("A*  Count > 2");
            if (transform.position.x >= FollowPath[0].X * 10 + 3 && transform.position.x <= FollowPath[0].X * 10 + 7
                && transform.position.z >= FollowPath[0].Y * 10 + 3 && transform.position.z <= FollowPath[0].Y * 10 + 7)
            {
                FollowPath.RemoveAt(0);
            }
        }



    }

    void Monster_Rotation(Vector3 target)
    {
        Vector3 direction =  (target - transform.position).normalized;
        //회전각도(쿼터니언)산출
        Quaternion targetangle = Quaternion.LookRotation(direction);
        //선형보간 함수를 이용해 부드러운 회전
        animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation, targetangle, Time.deltaTime * 8.0f); //Time.deltaTime * 8.0f);

        //animator.transform.rotation = Quaternion.LookRotation(direction);
    }
    IEnumerator FindingPlayerPath()
    {
        while(!IsDie)
        {
            FollowPath = MapManager.Instance.GetAstarPath(this.transform, Player.transform);
            if(FollowPath.Count > 0)
                 FollowPath.RemoveAt(0);
            yield return new WaitForSeconds(0.3f);
        }
    }


    #region 데미지 입었을 때

    void DamageCrowdControl(Weapon weapon)
    {
        DamageAni(weapon.attackData[(int)weapon.GetPlayerSkill()].IsStun);
        DamageKnockBack(weapon.attackData[(int)weapon.GetPlayerSkill()].KnockBack);

    }


    void DamageKnockBack(float KnockBackRange)
    {
        if (KnockBackRange == 0)
            return;
        Transform PlayerTransfrom = MonsterManager.Instance.GetPlayerTransfrom();
        Vector3 KnockBackPosition = (transform.position-PlayerTransfrom.position ).normalized;
        KnockBackPosition *= KnockBackRange;
        KnockBackPosition.y = 0;

        KnockBackPosition += transform.position;

        if(KnockBackCoroutine != null)
            StopCoroutine(KnockBackCoroutine);
        KnockBackCoroutine = StartCoroutine(KnockBackMove(KnockBackPosition));
    }

    IEnumerator KnockBackMove(Vector3 KnockBackPosition)
    {
        float time = 0f;
        float timeCheck = 1f;
        IsKnockBack = true;
        while (transform.position != KnockBackPosition)
        {
            time += Time.deltaTime / timeCheck;
            transform.position = Vector3.Lerp(transform.position, KnockBackPosition, time);
           

            yield return null;
        }
        IsKnockBack = false;
    }
    void DamageAni(bool isStun)
    {
        if (isStun)
            animator.SetTrigger("StunHit");
        else
            animator.SetTrigger("Hit");
    }
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
        if (other.tag == "Weapon")
        {
            DamageEffect();
            Weapon weapon = FindObjectOfType<Player_Attack>().GetWeapon();
            DamageCrowdControl(weapon);
        }
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