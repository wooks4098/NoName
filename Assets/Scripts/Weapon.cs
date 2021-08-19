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

    [Space(10f)] //���� ��ġ
    public WeaponType weaponType;
    public bool isRightHanded;

    //���� ��Ÿ��
    public float[] AttackAniTime; //�⺻���� ��� �ð�
    public float AttackMinTime = 0; //���� �⺻���� �����ִ� �ð���ġ(�ּ�)
    public float AttackMaxTime = 0; //���� �⺻���� �����ִ� �ð���ġ(�ִ�)
    public float DashTime = 0; //�뽬 ���� �����ѽð� (Runtime)
    public float DodgeCoolTime = 0; //ȸ�� ��Ÿ��
    public float QSkillCoolTime = 0; //Qskill ��Ÿ��

    //���ݺ� ���� (�˹�, ����)
    public AttackData[] attackData;


    const string weaponName = "Weapon";

    public void ChangeWeapon(Transform righthandTransform, Transform lefthandTransform, Animator animator,Weapon oldWeapon)
    {

        //������ ����ִ� ���� ����
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

    //���Ⱑ ���������� �޼����� �����ϴ� �Լ�
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
