using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    Effect = 0,
    Monster1,
    Monster2,
}

[System.Serializable]
public class ObjectPool
{
    public ObjectType objectType;
    public List<GameObject> Objects = new List<GameObject>();
    public GameObject Object_Prefab;
}
[System.Serializable]
public class ObjectPoolManger : MonoBehaviour
{
    private static ObjectPoolManger instance;
    [ArrayElementTitle("objectType")]
    public ObjectPool[] objectPool;

    public static ObjectPoolManger Instance { get { return instance; } }

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        Setobject();
    }

    void Setobject()
    {
        for (int i = 0; i < objectPool.Length; i++)
        {
            for (int j = 0; j < objectPool[i].Objects.Count; j++)
            {
                objectPool[i].Objects[j] = Instantiate(objectPool[i].Object_Prefab, transform);
                objectPool[i].Objects[j].SetActive(false);
            }
        }
    }

    //����� ������Ʈ ����
    public GameObject ReturnObject(ObjectType _object)
    {
        var objects = objectPool[(int)_object].Objects;
        var findobject = objects.Find(obj => !obj.activeSelf);
        if (null == findobject)
        {//��� ������̶�� ����
            findobject = Instantiate(objectPool[(int)_object].Object_Prefab, transform);
            objects.Add(findobject);
            findobject.SetActive(false);
        }
        return findobject;

        //return null; //��밡���� ������Ʈ�� ����
    }
}
