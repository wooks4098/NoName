using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_StatuController : MonoBehaviour
{
    //체력
    public float Hp;

    //이동
    public float moverotationSpeed;
    public float attackrotationSpeed;
    public float movementSpeed;
    public float dodgeSpeed;
    public float RunSpeed;
    public float AttackSpeed;
    public float JumpPower;
    public float gravity;

    //공격


    Player_Attack player_Attack;

    private void Awake()
    {
        player_Attack = GetComponent<Player_Attack>();
    }

    public void Damage(float _Damage, bool isStrun)
    {
        Hp = Mathf.Max(Hp - _Damage,0);
        //Hp = 0;
        if(Hp <=0)
        {
            GameManager.Instance.PlayerDie();
        }
        //Debug.Log("플레이어" + _Damage + "만큼의 피해입음");
    }
}
