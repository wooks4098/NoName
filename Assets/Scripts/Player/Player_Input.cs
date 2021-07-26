using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player_Input : MonoBehaviour, IInput
{
    public Action<Vector2> OnMovementInput { get; set; }
    public Action<Vector3> OnMovementDirectionInput { get; set; }
    float lastX = 0;

    private void Start()
    {
        //마우스 커서를 게임 중앙좌표에 고정시키고 커서를 숨김
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        GetMovementDirection(GetMovementInput());
    }

    Vector2 GetMovementInput()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //Debug.Log(input.y);
        Debug.Log(input.x);
        OnMovementInput?.Invoke(input);
        return input;
    }

    void GetMovementDirection(Vector2 input)
    {
        

        var cameraFowardDirection = Camera.main.transform.forward;
        Debug.DrawRay(Camera.main.transform.position, cameraFowardDirection * 10, Color.red);
        Vector3 directionToMoveIn = Vector3.Scale(cameraFowardDirection, (Vector3.right + Vector3.forward));
        Debug.DrawRay(Camera.main.transform.position, directionToMoveIn * 10, Color.blue);
        if (input.y > 0)
        {
            if (input.x > 0 && (input.x > lastX || input.x == 1))
            {
                directionToMoveIn = directionToMoveIn + Camera.main.transform.right;
            }
            else if (input.x < 0 && (input.x < lastX || input.x == -1))
            {
                directionToMoveIn = directionToMoveIn - Camera.main.transform.right;
            }
        }
        else if (input.y < 0)
        {
            directionToMoveIn = -directionToMoveIn;
            if (input.x > 0 &&( input.x > lastX || input.x == 1))
            {
                directionToMoveIn = directionToMoveIn + Camera.main.transform.right;
            }
            else if (input.x < 0 &&( input.x < lastX || input.x == -1))
            {
                directionToMoveIn = directionToMoveIn - Camera.main.transform.right;
            }
            
        }
        else if(input.y == 0)
        {
            if (input.x < 0)
            {
                //Quaternion v3Rotation = Camera.main.transform.rotation;  // 회전각
                //v3Rotation *= Quaternion.Euler(0f, 90f, 0f);  // 회전각
                //Vector3 v3Direction = Vector3.forward; // 회전시킬 벡터(테스트용으로 world forward 썼음)
                //Vector3 v3RotatedDirection = v3Rotation * v3Direction;
                directionToMoveIn = Vector3.Scale(Camera.main.transform.right, (Vector3.right + Vector3.forward));

            }
            else if (input.x > 0)
            {
                Quaternion v3Rotation = Quaternion.Euler(0f, -90f, 0f);  // 회전각
                Vector3 v3Direction = Vector3.forward; // 회전시킬 벡터(테스트용으로 world forward 썼음)
                Vector3 v3RotatedDirection = v3Rotation * v3Direction;
                directionToMoveIn = v3RotatedDirection;
            }
        }

        lastX = input.x;


        //앞 Y>0
        //뒤 y <0
        //오 x>0
        //왼 x<0
        //if()
        OnMovementDirectionInput?.Invoke(directionToMoveIn.normalized);
    }

}
