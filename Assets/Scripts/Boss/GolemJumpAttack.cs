using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 데미지 범위 ~5, 5.5 ~ 9, 10 ~ 13
/// </summary>
public class GolemJumpAttack : MonoBehaviour
{
    [SerializeField] ParticleSystem[] CircleParticle;

    public void StartJump()
    {
        StartCoroutine(Startparticle());
    }

    IEnumerator Startparticle()
    {
        for (int i = 0; i < CircleParticle.Length; i++)
        {
            CircleParticle[i].Play();
            PlayerDamageCheck(i);

             yield return new WaitForSeconds(0.4f);
        }
    }

    void PlayerDamageCheck(int particleNum)
    {
        float Distance = 
            Vector3.Distance(gameObject.transform.position, GameManager.Instance.GetPlayerPos());

        Debug.Log(Distance);
        switch (particleNum)
        {
            case 0:
                if (Distance < 5)
                    Debug.Log("플레이어 데미지");
                break;
            case 1:
                if (Distance > 5.5f && Distance < 9)
                    Debug.Log("플레이어 데미지");
                break;
            case 2:
                if (Distance > 10 && Distance < 13)
                    Debug.Log("플레이어 데미지");
                break;
        }
    }
}
