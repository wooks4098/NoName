using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapMonster
{    
    public List<MonsterController> monsterList = new List<MonsterController>();
    public bool Clear = false;
}

public class MonsterManager : MonoBehaviour
{
    private static MonsterManager instance;
    public static MonsterManager Instance { get { return instance; } }

    [SerializeField] List<MapMonster> mapMonster = new List<MapMonster>();

    [SerializeField] int roomMonsterCount;

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

        for (int i = 0; i<rooms.Count; i++)
        {
            MapMonster newMapMonster = new MapMonster();
            float minx = rooms[i].bottomLeft.x + 5;
            float maxx = rooms[i].topRight.x - 5;
            float miny = rooms[i].bottomLeft.y + 5;
            float maxy = rooms[i].topRight.y - 5;

            SpawnPos = new Vector3(Random.Range(minx, maxx),0, Random.Range(miny,maxy));
            Monster = ObjectPoolManger.Instance.ReturnObject(ObjectType.Monster1);
            Monster.SetActive(true);
            Monster.transform.position = SpawnPos;

            newMapMonster.monsterList.Add(Monster.GetComponent<MonsterController>());
            SpawnPos = new Vector3(Random.Range(minx, maxx), 0, Random.Range(miny, maxy));
            Monster = ObjectPoolManger.Instance.ReturnObject(ObjectType.Monster2);
            Monster.SetActive(true);
            Monster.transform.position = SpawnPos;
            newMapMonster.monsterList.Add(Monster.GetComponent<MonsterController>());

            mapMonster.Add(newMapMonster);
        }
        for(int i = 0; i< mapMonster.Count; i++)
        {
            for (int j = 0; j < mapMonster[i].monsterList.Count; j++)
                mapMonster[i].monsterList[j].gameObject.SetActive(false);
        }
    }

    public void SetActiveMonster(int _roomNumber)
    {

        MapMonster roomMonster = mapMonster[_roomNumber];
        if (roomMonster.Clear)
            return;
        roomMonsterCount = roomMonster.monsterList.Count;
        for (int i = 0; i< roomMonster.monsterList.Count; i++)
        {
            roomMonster.monsterList[i].gameObject.SetActive(true);
        }
    }

    //방에서 몬스터가 죽은경우 체크 -> 방을 클리어했는지 확인 용도
    public void MonsterDieCheck()
    {
        roomMonsterCount--;
        if (roomMonsterCount <= 0)
        {
            MapManager.Instance.AllDoorOpen();
            mapMonster[MapManager.Instance.GetPlayerRoomNumber()].Clear = true;
        }
    }
}
