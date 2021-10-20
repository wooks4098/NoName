using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [SerializeField] GameObject PlayerRootPrefab;
    [SerializeField] GameObject PlayerRoot;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Camera;
    [SerializeField] GameObject Monster;


    Player_Attack player_Attack;
    Player_StatuController player_StatuController;

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
        }
    }

    void CreatePlayer()
    {
        PlayerRoot = Instantiate(PlayerRootPrefab, Vector3.zero, Quaternion.identity);
        Player = PlayerRoot.transform.GetChild(0).gameObject;
        Camera = PlayerRoot.transform.GetChild(1).gameObject;
        player_Attack = Player.GetComponent<Player_Attack>();
        player_StatuController = Player.GetComponent<Player_StatuController>();


        PlayerRoot.SetActive(true);
        Monster.SetActive(true);
    }

    public void PlayerDamage(float _Damage, bool isStrun)
    {
        player_StatuController.Damage(_Damage, isStrun);
    }

    #region return

    public Vector3 GetPlayerPos()
    {
        return Player.transform.position;
    }

    public Transform GetPlayerTrans()
    {
        return Player.transform;
    }


    #endregion


}
