using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MosnterStatusController : MonoBehaviour
{

    public MonsterData monsterData;

    [SerializeField] float Hp;

    MonsterController monsterController;

    private void Awake()
    {
        monsterController = GetComponent<MonsterController>();
    }
    public void SetHp()
    {
        Hp = monsterData.HP;
    }

    public void Damage(float _Damage)
    {
        Hp = Mathf.Max(Hp - _Damage, 0);

        if (Hp <= 0)
            Die();
        //Debug.Log("플레이어" + _Damage + "만큼의 피해입음");
    }
    public float GetHp()
    {
        return Hp;
    }
    public void Die()
    {
        Hp = 0;
        monsterController.ChangeMonsterState(MonsterState.Die);
    }
}
