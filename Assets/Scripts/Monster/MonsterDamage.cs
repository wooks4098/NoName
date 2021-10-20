using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 몬스터가 공격했을 때 데미지를 주는 스크립트
/// </summary>
public class MonsterDamage : MonoBehaviour
{

    MosnterStatusController monsterStatusController;

    private void Awake()
    {
        monsterStatusController = GetComponentInParent<MosnterStatusController>();
    }


    

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log(other.name);
            gameObject.SetActive(false);
            GameManager.Instance.PlayerDamage(monsterStatusController.monsterData.Damage, false);   
        }
    }
}
