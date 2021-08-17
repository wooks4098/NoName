using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    Effect = 0,
}

[System.Serializable]
public class ObjectPool
{
    public ObjectType objectType;
    public GameObject[] Object;
    public GameObject Object_Prefab;
}
public class ObjectPoolManger : MonoBehaviour
{
    public ObjectPool[] objectPool;


    private void Awake()
    {
        Setobject();
    }

    void Setobject()
    {
        for (int i = 0; i < objectPool.Length; i++)
        {
            for(int j = 0; j< objectPool[i].Object.Length; j++)
            {
                objectPool[i].Object[j] = Instantiate(objectPool[i].Object_Prefab);
                objectPool[i].Object[j].SetActive(false);
            }
            
        }

    }


    //����� ������Ʈ ����
    public GameObject ReturnObject(ObjectType _object)
    {
        switch (_object)
        {
            case ObjectType.Effect:
                return findObject(objectPool[(int)_object].Object);

        }
        return null; //��밡���� ������Ʈ�� ����
    }

    //��밡���� ������Ʈ Ž��
    GameObject findObject(GameObject[] _obj)
    {
        for (int i = 0; i < _obj.Length; i++)
        {
            if (_obj[i].activeSelf == false)
                return _obj[i];
        }
        return null;
    }
}
