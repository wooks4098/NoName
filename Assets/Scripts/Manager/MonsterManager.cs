using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private static MonsterManager instance;
    public static MonsterManager Instance { get { return instance; } }

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

  
}
