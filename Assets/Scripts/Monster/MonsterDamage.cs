using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���Ͱ� �������� �� �������� �ִ� ��ũ��Ʈ
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
