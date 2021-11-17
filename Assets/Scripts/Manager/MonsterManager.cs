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

    private void Start()
    {
        //MonsterSpawn();
    }

    public void MonsterSpawn()
    {
        List<Room> rooms = MapManager.Instance.GetRoominfo();
        Vector3 SpawnPos;
        GameObject Monster;
        for(int i = 0; i<rooms.Count; i++)
        {
            float minx = rooms[i].bottomLeft.x + 5;
            float maxx = rooms[i].topRight.x - 5;
            float miny = rooms[i].bottomLeft.y + 5;
            float maxy = rooms[i].topRight.y - 5;

            SpawnPos = new Vector3(Random.Range(minx, maxx),0, Random.Range(miny,maxy));
            Monster = ObjectPoolManger.Instance.ReturnObject(ObjectType.Monster1);
            Monster.SetActive(true);
            Monster.transform.position = SpawnPos;
            SpawnPos = new Vector3(Random.Range(minx, maxx), 0, Random.Range(miny, maxy));
            Monster = ObjectPoolManger.Instance.ReturnObject(ObjectType.Monster2);
            Monster.SetActive(true);
            Monster.transform.position = SpawnPos;


        }
    }

  
}
