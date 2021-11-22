using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] int roomNumber;

    public void SetRoomNumber(int _number)
    {
        roomNumber = _number;
    }
}
