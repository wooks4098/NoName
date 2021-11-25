using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [SerializeField] Player_Controller PlayerRootPrefab;
    [SerializeField] GameObject PlayerRoot;
    [SerializeField] Transform Player;
    [SerializeField] GameObject Camera;
    [SerializeField] GameObject Monster;
    [SerializeField] GameObject Monster1;


    [SerializeField] Player_Controller playerController;
    bool IsplayerDie = false;
    bool test = false;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);


    }


    private void Update()
    {
        if(test == false && Input.GetMouseButtonDown(0))
        {
            test = true;
            CreatePlayer();
            MonsterManager.Instance.MonsterSpawn();
            //MonsterManager.Instance.SetActiveMonster(0);

        }
    }

    void CreatePlayer()
    {
        Room spawnRoom = MapManager.Instance.GetRoominfo(0);
        float minx = spawnRoom.bottomLeft.x + 5;
        float maxx = spawnRoom.topRight.x - 5;
        float miny = spawnRoom.bottomLeft.y + 5;
        float maxy = spawnRoom.topRight.y - 5;
        Vector3 spawnPos = new Vector3(Random.Range(minx, maxx), 0.3f, Random.Range(miny, maxy));
        playerController = Instantiate(PlayerRootPrefab, spawnPos, Quaternion.identity);
        //플레이어가 있는 방 번호 입력
        MapManager.Instance.SetPlayerRoomNumber(0);
        Player = playerController.transform.GetChild(0);
        playerController.gameObject.SetActive(true);    
    }

    public void PlayerDamage(float _Damage, bool isStrun)
    {
        if (IsplayerDie)
            return;
        playerController.GetPlayerStatusController().Damage(_Damage, isStrun);
    }

    public void PlayerDie()
    {
        IsplayerDie = true;
        playerController.Die();

    }

    public void PlayerDieChange(bool _Die)
    {
        IsplayerDie = _Die;

    }

    #region return

    public Vector3 GetPlayerPos()
    {
        return Player.position;
    }

    public Transform GetPlayerTrans()
    {
        return Player;
    }

    public bool GetIsPlayerDie()
    {
        if (IsplayerDie)
            return false;
        return IsplayerDie;
    }

    #endregion


}
