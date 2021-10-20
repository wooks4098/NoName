using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "MonsterDatas/Make New MonsterData", order = 0)]
public class MonsterData : ScriptableObject
{
    //ü��
    public float HP;


    //����
    public float AttackRange;
    public float Damage;

    //�̵�����
    public float MoveSpeed;

}
