using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_MonsterHit : MonoBehaviour
{
    [SerializeField] Material idle;
    [SerializeField] Material hit;
    [SerializeField] SkinnedMeshRenderer skin;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Weapon")
        {
            Debug.Log("Hit");
            StartCoroutine(Hit());
        }
    }
    IEnumerator Hit()
    {
        skin.material = hit;
        yield return new WaitForSeconds(0.2f);
        skin.material = idle;

    }

}
