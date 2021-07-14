using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{

    float v;
    float h;
    float Speed = 2f;
    private void Update()
    {



        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        transform.position += new Vector3(h, 0, v).normalized * Time.deltaTime * Speed;

    }
}
