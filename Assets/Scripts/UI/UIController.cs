using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    private static UIController instance;
    public static UIController Instance { get { return instance; } }

    [SerializeField] PlayerUIController playerUI;

    private void Awake()
    {
        instance = this;
        //if (null == instance)
        //{
        //    instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else Destroy(gameObject);


    }


    #region PlayerUI    

    public void ChangeHp(float _Changehp)
    {
        playerUI.ChangeHpSlider(_Changehp);
    }

    #endregion

}
