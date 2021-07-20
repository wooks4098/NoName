using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player_Input : MonoBehaviour, IInput
{
    public Action<Vector2> OnMovementInput { get; set; }

    public Action<Vector3> OnMovementDirectionInput { get; set; }

    private void Start()
    {
        //마우스 커서를 게임 중앙좌표에 고정시키고 커서를 숨김
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        GetMovementInput();
        GetMovementDirection();
    }

    void GetMovementInput()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //Debug.Log(input.y);
        Debug.Log(input.x);
        OnMovementInput?.Invoke(input);
    }

    void GetMovementDirection()
    {
        var cameraFowardDirection = Camera.main.transform.forward;
        Debug.DrawRay(Camera.main.transform.position, cameraFowardDirection * 10, Color.red);
        var directionToMoveIn = Vector3.Scale(cameraFowardDirection, (Vector3.right + Vector3.forward));
        Debug.DrawRay(Camera.main.transform.position, directionToMoveIn * 10, Color.blue);
        OnMovementDirectionInput?.Invoke(directionToMoveIn.normalized);
    }

}
