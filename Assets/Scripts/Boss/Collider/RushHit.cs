using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushHit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("�÷��̾� ������");
        }
    }
}
