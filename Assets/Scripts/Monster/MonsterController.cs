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
    private IEnumerator KnockBackCoroutine;

    //Test용
    public GameObject Player;

    [SerializeField] List<Astar_Node> FollowPath;


    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();

        FollowPath = MapManager.Instance.GetAstarPath(Player.transform, this.transform);
        FollowPath.RemoveAt(FollowPath.Count - 1);
    }

    private void Update()
    {
        if (FollowPath.Count > 0)
        {
            var path = FollowPath[FollowPath.Count - 1];
            Vector3 target = new Vector3(path.X * 10 + 5f, 0, path.Y * 10 + 5f);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(path.X * 10 + 5f, 0, path.Y * 10 + 5f), 0.2f);
            if (2f >= Vector3.Distance(transform.position, target))
            {
                FollowPath.RemoveAt(FollowPath.Count - 1);
            }
        }
        
    }

    private void OnEnable()
    {
        isStun = false;
        skinnedMeshRenderer.material = IdleMateriel;
        //StartCoroutine(FollowPlayer());

    }
    IEnumerator FollowPlayer()
    {
        while(!IsDie)
        {
            FollowPath = MapManager.Instance.GetAstarPath(Player.transform , this.transform);
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
        KnockBackCoroutine = KnockBackMove(KnockBackPosition);
        StartCoroutine(KnockBackCoroutine);
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
}