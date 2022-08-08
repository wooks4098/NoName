using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    [SerializeField] int Damage;


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Golem")
        {
            other.GetComponent<GolemController>().Damage(-Damage);
            Debug.Log("trigger데미지");
        }
    }

    public int GetDamage()
    {
        return Damage;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.transform.tag == "Golem")
    //    {
    //        collision.gameObject.GetComponent<GolemController>().Damage(-Damage);
    //        Debug.Log("onco데미지");
    //    }
    //}
}
