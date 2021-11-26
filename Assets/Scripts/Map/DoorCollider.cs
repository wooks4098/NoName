using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollider : MonoBehaviour
{
    [SerializeField] int roomNumber;

    public void SetDoorNumber(int _number)
    {
        roomNumber = _number;
    }


    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            MapManager.Instance.PlayerExitRoom(roomNumber);
        }
    }
}
