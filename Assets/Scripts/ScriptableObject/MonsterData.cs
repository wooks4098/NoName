using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "MonsterDatas/Make New MonsterData", order = 0)]
public class MonsterData : ScriptableObject
{
    [SerializeField] AnimatorOverrideController animatorOverride = null;


    //ü��
    public float HP;


    //����
    public float AttackRange;
    public float Damage;

    //�̵�����
    public float MoveSpeed;


    //public void ChangeAnimator(Animator animator)
    //{

    //    var ovverideController = animator.runtimeAnimatorController as AnimatorOverrideController;

    //    if (animatorOverride != null)
    //    {
    //        animator.runtimeAnimatorController = animatorOverride;

    //    }
    //}

}
