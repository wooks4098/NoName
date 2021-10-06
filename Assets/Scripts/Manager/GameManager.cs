using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Player;
    public GameObject Moster;
    public GameObject Camera;

    bool start = false;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !start)
        {
            Player.SetActive(true);
            Moster.SetActive(true);
            Camera.SetActive(true);

            start = true;
        }

    }
}
