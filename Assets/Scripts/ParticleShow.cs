using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleShow : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;
    private void Update()
    {
        if (particle.isPlaying == false)
            gameObject.SetActive(false);
    }
}
