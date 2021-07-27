using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player_Input : MonoBehaviour, IInput
{
    public Action<Vector2> OnMovementInput { get; set; }

    public Action<Vector3> OnMovementDirectionInput { get; set; }

    [SerializeField] Vector2 LastInput = Vector2.zero;
    [SerializeField] Vector2 _Input = Vector2.zero;

    public bool test = false;

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
        Debug.Log(input.y);
        _Input = input;
        OnMovementInput?.Invoke(input);
        return input;
    }

    void GetMovementDirection(Vector2 input)
    {
        

        var cameraFowardDirection = Camera.main.transform.forward;
        Debug.DrawRay(Camera.main.transform.position, cameraFowardDirection * 10, Color.red);
        Vector3 directionToMoveIn = Vector3.Scale(cameraFowardDirection, (Vector3.right + Vector3.forward));
        Debug.DrawRay(Camera.main.transform.position, directionToMoveIn * 10, Color.blue);
        if(input.y != 0 || Math.Abs(LastInput.y) < input.y || Math.Abs(input.y) == 1)
        {
            directionToMoveIn *= (input.y > 0) ? 1 : -1;
            if (input.x > 0 && (input.x > LastInput.x || input.x == 1))
            {
                directionToMoveIn = directionToMoveIn + Camera.main.transform.right;
                test = false;
            }
            else if (input.x < 0 && (input.x < LastInput.x || input.x == -1))
            {
                directionToMoveIn = directionToMoveIn - Camera.main.transform.right;
                test = false;
            }
        }
        else if(Math.Abs(LastInput.y) > input.y || input.y == 0)
        {
            if (input.x > 0 )
            {
                directionToMoveIn = -Vector3.Cross(directionToMoveIn, Vector3.up);
                test = true;
            }
            else if (input.x < 0)
            {
                directionToMoveIn = (Vector3.Cross(directionToMoveIn, Vector3.up));
            }
           
        }
        LastInput = input;

        //if (input.y > 0)
        //{
        //    if (input.x > 0 && (input.x > lastX || input.x == 1))
        //    {
        //        directionToMoveIn = directionToMoveIn + Camera.main.transform.right;
        //    }
        //    else if (input.x < 0 && (input.x < lastX || input.x == -1))
        //    {
        //        directionToMoveIn = directionToMoveIn - Camera.main.transform.right;
        //    }
        //}
        //else if (input.y < 0)
        //{
        //    directionToMoveIn = -directionToMoveIn;
        //    if (input.x > 0 &&( input.x > lastX || input.x == 1))
        //    {
        //        directionToMoveIn = directionToMoveIn + Camera.main.transform.right;
        //    }
        //    else if (input.x < 0 &&( input.x < lastX || input.x == -1))
        //    {
        //        directionToMoveIn = directionToMoveIn - Camera.main.transform.right;
        //    }

        //}
        //else if(input.y == 0)
        //{
        //    if (input.x > 0)
        //    {
        //        directionToMoveIn = -Vector3.Cross(directionToMoveIn, Vector3.up);

        //    }
        //    else if (input.x < 0)
        //    {
        //        directionToMoveIn = (Vector3.Cross(directionToMoveIn, Vector3.up));
        //    }
        //}




        //앞 Y>0
        //뒤 y <0
        //오 x>0
        //왼 x<0
        //if()
        OnMovementDirectionInput?.Invoke(directionToMoveIn.normalized);
    }

}
