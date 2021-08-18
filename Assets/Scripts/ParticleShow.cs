using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleShow : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;
   
    public void ChangeTransfrom(Transform _transform)
    {
        transform.position = _transform.position;   
    }


    public void ParticlePlay()
    {
        gameObject.SetActive(true);
        StartCoroutine("Run");
    }
    IEnumerator Run()
    {
        particle.Play();
        while (particle.isPlaying)
        {
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
