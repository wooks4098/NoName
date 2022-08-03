using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushHit : MonoBehaviour
{
    [SerializeField] float Damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.Instance.PlayerDamage(Damage,false);
            Debug.Log("Rush�÷��̾� ������");
        }
    }
}
