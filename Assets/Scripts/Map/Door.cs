using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] int roomNumber;


    //test
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] BoxCollider boxCollider;

    public void SetRoomNumber(int _number)
    {
        roomNumber = _number;
    }

    public void OpenDoor()
    {
        meshRenderer.enabled = true;
        boxCollider.isTrigger = true;
    }

    public void CloseDoor()
    {
        meshRenderer.enabled = false;
        boxCollider.isTrigger = false;
    }
}
