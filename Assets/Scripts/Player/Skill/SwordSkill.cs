using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    void Attack();
    void DashAtaack();
    void Skill();

    bool IsAttack();
}

public class SwordSkill : MonoBehaviour, ISkill
{
    [SerializeField] float Speed;
    [SerializeField] float AttackTimeCheck = 0; //기본공격 시간 측정용
    [SerializeField] float AttackTime = 0; //기본공격이 다음공격으로 갈수 있는 시간(-0.2 ~ 0.2)
    int AttackNum = 0;
    [SerializeField] bool isAttack = false;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0)&& !isAttack)
        {
            Attack();
        }
    }


    public void Attack()
    {
        isAttack = true;
        AttackTimeCheck = 0;
        StartCoroutine(AttackCombo());
    }

    IEnumerator AttackCombo()
    {
        PlayAnimation(AttackNum++);

        while (AttackTimeCheck <= AttackTime + 0.2f || AttackNum<3)
        {
            if (AttackTimeCheck > AttackTime + 0.2f)
                break;
            AttackTimeCheck += Time.deltaTime * Speed;
           if(Input.GetMouseButtonDown(0))
            {
                if (AttackTimeCheck >= AttackTime- 0.2f && AttackTimeCheck <= AttackTime+0.2f)
                {
                    if (AttackNum > 2)
                    {
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

    private void AttackReset()
    {
        AttackNum = 0;
        AttackTimeCheck = 0;
        isAttack = false;
    }

    public void DashAtaack()
    {
    }

    public void Skill()
    {
    }

    void PlayAnimation(int ani)
    {
        animator.SetFloat("Attack_Count", ani);
        animator.SetTrigger("Attack");
    }

    public bool IsAttack()
    {
        return isAttack;
    }
}
