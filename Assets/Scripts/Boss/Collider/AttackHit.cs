using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHit : MonoBehaviour
{
    [SerializeField] float Damage;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GameManager.Instance.PlayerDamage(Damage,false);
            Debug.Log("플레이어 데미지");
        }
    }
}
