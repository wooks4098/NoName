using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player_Input : MonoBehaviour, IInput
{
    public Action<Vector2,bool> OnMovementInput { get; set; }

    public Action<Vector3> OnMovementDirectionInput { get; set; }
    public Action OnAttackInput { get; set; }
    public Action<Vector3> OnAttackDirection { get; set; }

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
        Vector2 input = GetMovementInput();
        GetMovementDirection(input);
    }

    #region 이동
    Vector2 GetMovementInput()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        OnMovementInput?.Invoke(input, GetComponent<ISkill>().IsAttack());
        return input;
    }

    Vector3 GetDirection()
    {
        var cameraFowardDirection = Camera.main.transform.forward;
        Vector3 directionToMoveIn = Vector3.Scale(cameraFowardDirection, (Vector3.right + Vector3.forward));
        //Debug.DrawRay(Camera.main.transform.position, cameraFowardDirection * 10, Color.red);
        //Debug.DrawRay(Camera.main.transform.position, directionToMoveIn * 10, Color.blue);
        return directionToMoveIn;
    }

    void GetMovementDirection(Vector2 input)
    {
        Vector3 directionToMoveIn = GetDirection();
        if (input.y != 0 || Math.Abs(LastInput.y) < Math.Abs(input.y )|| Math.Abs(input.y) == 1)
        {
            directionToMoveIn *= (input.y > 0) ? 1 : -1;

            if(input.x != 0 || Math.Abs(LastInput.x) < Math.Abs(input.x)|| Math.Abs(input.x) == 1)
            {
                directionToMoveIn = directionToMoveIn + (input.x >0 ? 1 : -1) * Camera.main.transform.right;
                test = false;
            }


            //if (input.x > 0 && (input.x > LastInput.x || input.x == 1))
            //{
            //    directionToMoveIn = directionToMoveIn + Camera.main.transform.right;
            //    
            //}
            //else if (input.x < 0 && (input.x < LastInput.x || input.x == -1))
            //{
            //    directionToMoveIn = directionToMoveIn - Camera.main.transform.right;
            //    test = false;
            //}
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
        OnMovementDirectionInput?.Invoke(directionToMoveIn.normalized);

    }
    #endregion

    #region 공격
    void GetAttackInput()
    {
        OnAttackInput?.Invoke();
    }

    void GetAttackDirection()
    {
        
    }


    #endregion

}
