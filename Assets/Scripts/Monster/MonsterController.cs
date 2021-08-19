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

    private IEnumerator KnockBackCoroutine;


    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();

    }

    private void OnEnable()
    {
        isStun = false;
        skinnedMeshRenderer.material = IdleMateriel;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon")
        {
            DamageEffect();
            Weapon weapon = FindObjectOfType<Player_Attack>().GetWeapon();
            DamageCrowdControl(weapon);
        }
    }

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
}