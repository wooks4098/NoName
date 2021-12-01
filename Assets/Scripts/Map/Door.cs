using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] int roomNumber;


    //test
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] float height;
    [SerializeField] GameObject doorObject;

    public void SetRoomNumber(int _number)
    {
        roomNumber = _number;
    }

    public void OpenDoor()
    {
        //meshRenderer.enabled = false;
        boxCollider.isTrigger = true;
        StartCoroutine(DoorMove(true));
    }
    IEnumerator DoorMove(bool isOpen)
    {
        float FalltimeCheck = 0;
        float Falltime = 3;
        float MoveY;

        float Doory = doorObject.transform.position.y;
        while (FalltimeCheck <= Falltime)
        {
            FalltimeCheck += Time.deltaTime;

            if(isOpen)
            {
                MoveY = Mathf.Lerp(Doory, transform.position.y + height, FalltimeCheck / Falltime);
                MoveY = Mathf.Abs(MoveY) - doorObject.transform.position.y;
                doorObject.transform.position += new Vector3(0, MoveY, 0);

            }
            else
            {
                MoveY = Mathf.Lerp(doorObject.transform.position.y, transform.position.y, FalltimeCheck / Falltime);
                MoveY = doorObject.transform.position.y - Mathf.Abs(MoveY);
                doorObject.transform.position -= new Vector3(0, MoveY, 0);

            }
            yield return null;
        }
    }
    public void CloseDoor()
    {
        //meshRenderer.enabled = true;
        boxCollider.isTrigger = false;
        StartCoroutine(DoorMove(false));
    }
}
