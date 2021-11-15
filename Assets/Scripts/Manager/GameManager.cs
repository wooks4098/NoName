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
        }
    }

    void CreatePlayer()
    {
        playerController = Instantiate(PlayerRootPrefab, Vector3.zero, Quaternion.identity);
        //PlayerRoot = Instantiate(PlayerRootPrefab, Vector3.zero, Quaternion.identity);
        Player = playerController.transform.GetChild(0);
        //Camera = PlayerRoot.transform.GetChild(1).gameObject;

        //playerController = PlayerRoot.GetComponent<Player_Controller>();

        //PlayerRoot.SetActive(true);
        playerController.gameObject.SetActive(true);    
        Monster.SetActive(true);
        Monster1.SetActive(true);
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
