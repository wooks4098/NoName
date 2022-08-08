using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���Ͱ� �������� �� �������� �ִ� ��ũ��Ʈ
/// </summary>
public class MonsterDamage : MonoBehaviour
{

    MosnterStatusController monsterStatusController;
    BoxCollider attackCollider;

    private void Awake()
    {
        monsterStatusController = GetComponentInParent<MosnterStatusController>();
        attackCollider = GetComponent<BoxCollider>();
    }

    
    

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log(other.name);
            attackCollider.enabled = false;
            //gameObject.SetActive(false);
            GameManager.Instance.PlayerDamage(-monsterStatusController.monsterData.Damage, false);   
        }
    }
}
