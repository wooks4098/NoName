using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    [SerializeField] float Damage;


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Golem")
        {
            other.GetComponent<GolemController>().Damage(-Damage);
            Debug.Log("trigger������");
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.transform.tag == "Golem")
    //    {
    //        collision.gameObject.GetComponent<GolemController>().Damage(-Damage);
    //        Debug.Log("onco������");
    //    }
    //}
}
