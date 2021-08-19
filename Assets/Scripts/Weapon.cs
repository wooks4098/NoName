using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{ 
    Sword = 0,
    Hammer,
}
[System.Serializable]
public struct AttackData
{
    public float KnockBack;
    public float Stuntime;
    public bool IsStun;
}

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
public class Weapon : ScriptableObject
{

    [SerializeField] AnimatorOverrideController animatorOverride = null;
    [SerializeField] GameObject weaponPrefab = null;
    [SerializeField] BoxCollider Hitbox = null;

    [SerializeField] UseSkill PlayerSkill;

    [Space(10f)] //무기 수치
    public WeaponType weaponType;
    public bool isRightHanded;

    //공격 쿨타임
    public float[] AttackAniTime; //기본공격 모션 시간
    public float AttackMinTime = 0; //다음 기본공격 갈수있는 시간수치(최소)
    public float AttackMaxTime = 0; //다음 기본공격 갈수있는 시간수치(최대)
    public float DashTime = 0; //대쉬 공격 가능한시간 (Runtime)
    public float DodgeCoolTime = 0; //회피 쿨타임
    public float QSkillCoolTime = 0; //Qskill 쿨타임

    //공격별 상태 (넉백, 기절)
    public AttackData[] attackData;


    const string weaponName = "Weapon";

    public void ChangeWeapon(Transform righthandTransform, Transform lefthandTransform, Animator animator,Weapon oldWeapon)
    {

        //이전에 들고있던 무기 제거
        DestroyOleWepaon(righthandTransform, lefthandTransform,oldWeapon);

        if (weaponPrefab != null)
        {
            Transform handTransform;
            handTransform = GetTransform(righthandTransform, lefthandTransform);
            GameObject weapon = Instantiate(weaponPrefab, handTransform);
            Hitbox = weapon.GetComponent<BoxCollider>();
            weapon.name = weaponName;
            PlayerSkill = UseSkill.None;
        }

        var ovverideController = animator.runtimeAnimatorController as AnimatorOverrideController;

        if (animatorOverride != null)
        {
            animator.runtimeAnimatorController = animatorOverride;

        }
        //else if (ovverideController != null)
        //{
        //    animator.runtimeAnimatorController = ovverideController.runtimeAnimatorController;
        //}
    }

    private  void DestroyOleWepaon(Transform righthandTransform, Transform lefthandTransform, Weapon oldWeapon)
    {
        if (oldWeapon == null)
            return;
        Transform oldWeaponTransform = righthandTransform.Find(weaponName);
        //switch (oldWeapon.weaponType)
        //{
        //    case WeaponType.Sword:
        //    case WeaponType.Hammer:

        //        break;

        //}
        if (oldWeaponTransform == null)
            return;
        oldWeaponTransform.name = "DESTROYING";
        Destroy(oldWeaponTransform.gameObject);
    }

    //무기가 오른손인지 왼손인지 리턴하는 함수
    private Transform GetTransform(Transform righthandTransform, Transform lefthandTransform)
    {
        return isRightHanded ? righthandTransform : lefthandTransform;
    }

    public void ChangePlayerSkill(UseSkill _skill)
    {
        PlayerSkill = _skill;
    }

    public UseSkill GetPlayerSkill()
    {
        return PlayerSkill;
    }

    public BoxCollider GetHitbox()
    {
        return Hitbox;
    }

}
