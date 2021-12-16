using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Hp : MonoBehaviour
{
    [SerializeField] Player_StatuController Status;
    [SerializeField] Text HPtext;

    public void SetStatus(Player_StatuController _Status)
    {
        Status = _Status;
    }

    private void FixedUpdate()
    {
        if(Status != null)
             HPtext.text = "HP : " + Status.Hp.ToString();
    }
}
