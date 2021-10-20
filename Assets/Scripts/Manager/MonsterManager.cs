using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private static MonsterManager instance;
    public static MonsterManager Instance { get { return instance; } }

    [SerializeField] Transform PlayerTransfrom;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        //PlayerTransfrom = FindObjectOfType<Player_Controller>().transform;
    }

    public Transform GetPlayerTransfrom()
    {
        return GameManager.Instance.GetPlayerTrans();
    }

    public Vector3 GetPlayerPos()
    {
        return GameManager.Instance.GetPlayerPos();
    }
}
