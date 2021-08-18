using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player_Attack : MonoBehaviour, ISkill
{
    [SerializeField] Transform rightHandTransform = null;
    [SerializeField] Transform leftHandTransform = null;
    [SerializeField] Weapon weapon;
    [SerializeField] BoxCollider Hitbox = null;

    Animator animator;


    [Header("공격상태")]
    [SerializeField] float AttackTimeCheck = 0; //기본공격 시간 측정용
    //[SerializeField] float AttackCoolTime = 0; //기본공격이 다음공격으로 갈수 있는 시간(-0.2 ~ 0.2)
    //[SerializeField] float DashTime = 0; //대쉬 공격 가능한시간 (Runtime)
    //[SerializeField] float DodgeCoolTime = 0; //회피 쿨타임
    //[SerializeField] float QSkillCoolTime = 0; //Qskill 쿨타임
    int AttackNum = 0;
    [SerializeField] bool isAttack = false;
    [SerializeField] bool isDashAttack = false;
    [SerializeField] bool CanDashAttack = true;
    [SerializeField] bool Isdodge = false;
    [SerializeField] bool Candodge = true;
    [SerializeField] bool isQSkill = false;
    [SerializeField] bool CanQSkill = true;



    void Start()
    {
        animator = GetComponent<Animator>();
        ChangeWeapon();
    }

    void ChangeWeapon()
    {
        weapon.ChangeWeapon(rightHandTransform, leftHandTransform, animator, weapon);
        Hitbox = weapon.GetHitbox();
        HitboxOff();
    }

    public void HitboxOn()
    {
        Hitbox.enabled = true;
    }

    public void HitboxOff()
    {
        Hitbox.enabled = false;
    }

    void StopAllSkill()
    {
        StopAllCoroutines();
    }

    public void Attack() //기본공격
    {
        if (isAttack||Isdodge||isQSkill)
            return;
        if (GetComponent<Player_Movement>().RunTime > weapon.DashTime)
        {
            if (CanDashAttack)
            {
                DashAtaack();
            }
            else if (!isDashAttack)
                AttackCombo();
        }
        else
        {
            if (!isDashAttack)
                AttackCombo();
        }
    }
    public void DashAtaack() //대쉬 어택
    {
        weapon.ChangePlayerSkill(UseSkill.DashAttack);
        isDashAttack = true;
        CanDashAttack = false;
        animator.SetTrigger("DashAttack");
        StartCoroutine(DashAttackCooltime());
    }
    IEnumerator DashAttackCooltime() //대쉬 쿨타임
    {
        yield return new WaitForSeconds(1.1f);
        isDashAttack = false;
        yield return new WaitForSeconds(0.8f);
        CanDashAttack = true;
        yield return null;
    }

    public void AttackCombo() //기본공격
    {
        weapon.ChangePlayerSkill(UseSkill.AttackCombo);
        isAttack = true;
        AttackTimeCheck = 0;
        StartCoroutine(AttackCombostart());
    }

    IEnumerator AttackCombostart() //기본공격 콤보
    {
        PlayAnimation(AttackNum++);
        
        while (AttackTimeCheck <= weapon.AttackCoolTime + 0.15f || AttackNum < 2)
        {
            if (AttackTimeCheck > weapon.AttackCoolTime + 0.2f)
                break;
            AttackTimeCheck += Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                if (AttackTimeCheck >= weapon.AttackCoolTime - 0.1f && AttackTimeCheck <= weapon.AttackCoolTime + 0.2f)
                {
                    if (AttackNum > 2)
                    {
                        yield return new WaitForSeconds(0.5f);
                        AttackReset();
                        yield break;
                    }

                    PlayAnimation(AttackNum++);
                    AttackTimeCheck = 0;
                }
            }

            yield return null;
        }
        AttackReset();
    }

    void PlayAnimation(int ani) //기본공격 콤보 애니메이션
    {
        animator.SetFloat("Attack_Count", ani);
        animator.SetTrigger("Attack");
    }

    private void AttackReset() //기본공격 리셋
    {
        AttackNum = 0;
        AttackTimeCheck = 0;
        isAttack = false;
    }

    public void Dodge() //회피
    {
        if (!isDashAttack && Candodge && !Isdodge)
        {
            weapon.ChangePlayerSkill(UseSkill.Dodge);
            Isdodge = true;
            Candodge = false;
            animator.SetTrigger("Dodge");
            StartCoroutine(DodgeCooltime());
        }


    }

    IEnumerator DodgeCooltime()
    {
        yield return new WaitForSeconds(0.225f);
        Isdodge = false;
        isQSkill = false;
        isDashAttack = false;
        yield return new WaitForSeconds(weapon.DodgeCoolTime - 0.225f);
        Candodge = true;
    }

    public void Qskill()
    {
        if (CanQSkill && !isQSkill && !isDashAttack && !Isdodge)
        {
            weapon.ChangePlayerSkill(UseSkill.QSkill);
            isQSkill = true;
            CanQSkill = false;
            animator.SetTrigger("QSkill");
            StartCoroutine(QSkillCooltime());
        }
    }
    IEnumerator QSkillCooltime()
    {
        yield return new WaitForSeconds(1.1f);
        isQSkill = false;
        yield return new WaitForSeconds(weapon.QSkillCoolTime - 1.1f);
        CanQSkill = true;
    }


    public bool IsAttack()
    {
        return isAttack;
    }

    bool ISkill.isDashAttack()
    {
        return isDashAttack;
    }

    public UseSkill isSkill()
    {
        if (isDashAttack)
            return UseSkill.DashAttack;
        if (Isdodge)
            return UseSkill.Dodge;
        if (isQSkill)
            return UseSkill.QSkill;
        if (isAttack)
            return UseSkill.AttackCombo;
        return UseSkill.None;
    }
}
