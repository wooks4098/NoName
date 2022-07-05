using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Rush가 끝났는지 체크하는 스크립트
/// </summary>
public class RushEndCollider : MonoBehaviour
{
    [SerializeField] GolemController golemController;

    BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = this.GetComponent<BoxCollider>();
        boxCollider.enabled = false;
    }

    public void ColliderOn()
    {
        boxCollider.enabled = true;
    }

    public void ColliderOff()
    {
        boxCollider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Golem")
        {
            golemController.EndRush();
            boxCollider.enabled = false;

        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.transform.tag == "")
    //}
}
