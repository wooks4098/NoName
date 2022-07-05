using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossJumpEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem[] CircleParticle;

    public void ShowEffect()
    {
        StartCoroutine(Startparticle());
    }

    IEnumerator Startparticle()
    {
        for(int i = 0; i<CircleParticle.Length; i++)
        {
            
            CircleParticle[i].Play();
            yield return new WaitForSeconds(0.4f);
        }
    }
}
