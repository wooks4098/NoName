using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_StatuController : MonoBehaviour
{
    //ü��
    public float Hp;

    //�̵�
    public float moverotationSpeed;
    public float attackrotationSpeed;
    public float movementSpeed;
    public float dodgeSpeed;
    public float RunSpeed;
    public float AttackSpeed;
    public float JumpPower;
    public float gravity;

    //����


    Player_Attack player_Attack;

    private void Awake()
    {
        player_Attack = GetComponent<Player_Attack>();
    }

    public void Damage(float _Damage, bool isStrun)
    {
        Hp -= _Damage;
        //Debug.Log("�÷��̾�" + _Damage + "��ŭ�� ��������");
    }
}