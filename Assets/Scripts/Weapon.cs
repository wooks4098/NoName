using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{ 
    Sword = 0,
    Hammer,
}
[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
public class Weapon : ScriptableObject
{

    [SerializeField] AnimatorOverrideController animatorOverride = null;
    [SerializeField] GameObject weaponPrefab = null;
    [SerializeField] BoxCollider Hitbox = null;
    

    [Space(10f)]
    public WeaponType weaponType;
    public bool isRightHanded;

    public float AttackCoolTime = 0; //�⺻������ ������������ ���� �ִ� �ð�(-0.2 ~ 0.2)
    public float DashTime = 0; //�뽬 ���� �����ѽð� (Runtime)
    public float DodgeCoolTime = 0; //ȸ�� ��Ÿ��
    public float QSkillCoolTime = 0; //Qskill ��Ÿ��


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

    public BoxCollider GetHitbox()
    {
        return Hitbox;
    }

}
