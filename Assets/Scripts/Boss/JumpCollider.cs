using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCollider : MonoBehaviour
{
    [SerializeField] bool isHitPlayer;//플레이어가 충돌중인지

    public bool GetHitPlayer()
    {
        return isHitPlayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            isHitPlayer = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            isHitPlayer = false;
    }

}
