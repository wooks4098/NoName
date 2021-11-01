using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "MonsterDatas/Make New MonsterData", order = 0)]
public class MonsterData : ScriptableObject
{
    [SerializeField] AnimatorOverrideController animatorOverride = null;


    //체력
    public float HP;


    //공격
    public float AttackRange;
    public float Damage;

    //이동관련
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
