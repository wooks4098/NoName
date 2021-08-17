using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_MonsterHit : MonoBehaviour
{
    [SerializeField] Material idle;
    [SerializeField] Material hit;
    [SerializeField] SkinnedMeshRenderer skin;
    [SerializeField] ParticleSystem hitpar;
    [SerializeField] Transform particleTransfrom;
    public ObjectPoolManger poolmanager;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Weapon")
        {
            Debug.Log("Hit");
            GameObject _particle =  poolmanager.ReturnObject(ObjectType.Effect);
            _particle.SetActive(true);
            _particle.transform.position = particleTransfrom.position;
            _particle.GetComponent<ParticleSystem>().Play();
            //StartCoroutine(Hit());
        }
    }
    IEnumerator Hit()
    {
        skin.material = hit;
        yield return new WaitForSeconds(0.2f);
        skin.material = idle;

    }

}
