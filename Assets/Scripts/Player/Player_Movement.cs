using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{

    CharacterController controller;
    Animator animator;

    public float moverotationSpeed;
    public float attackrotationSpeed;
    public float movementSpeed;
    public float RunSpeed;
    public float AttackSpeed;
    public float JumpPower;
    public float gravity = 20;
    Vector3 movementVector = Vector3.zero;
    private float desiredRotationAngle = 0;



    public bool isRun = false;
    public float RunTime = 0;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        movementVector.y -= gravity;
        if(controller.isGrounded)
        {
           
            Jump();
            RunCheck();
        }
        else
            RotatePlayer_Rotation(false);     
        controller.Move(movementVector * Time.deltaTime);
    }
    void Jump()
    {
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            movementVector.y = JumpPower;
        }
    }

    #region Move

    public void HandleMovement(Vector2 input, bool isAttack)
    {
        Debug.Log(input);
        if (!controller.isGrounded)
            return;

        if (input.y == 0 && input.x == 0)
        {//Stop
            if (isAttack) //공격중일 때
                RotatePlayer_Rotation(isAttack);
            if(isRun)
                StartCoroutine(RunToWalk_Speed());
            movementVector = Vector3.zero;
            animator.SetFloat("Battle_Walk", 0);

        }
        else
        {
            RotatePlayer_Rotation(isAttack);
            if (isAttack)
            {
                RotatePlayer_Rotation(isAttack);
                movementVector = transform.forward * AttackSpeed;
                animator.SetFloat("Battle_Walk", Mathf.Max(Mathf.Abs(input.x), Mathf.Abs(input.y)));
                RunTime = 0;
            }           
            else if (isRun)
            {//Run
                movementVector = transform.forward * RunSpeed;
                animator.SetBool("IsRun", true);
                RunTime += Time.deltaTime;

            }
            else
            {//Walk
                RunTime = 0;
                movementVector = transform.forward * movementSpeed;
                
                animator.SetFloat("Battle_Walk", Mathf.Max(Mathf.Abs(input.x), Mathf.Abs(input.y)));
            }
        }
    }

    void Movement(Vector3 vec3, float aniSpeed)
    {
        movementVector = Vector3.zero;
        animator.SetFloat("Battle_Walk", 0);
    }
    #endregion

    #region Run
    void RunCheck()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isRun == false)
        {
            isRun = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && isRun == true)
        {
            isRun = false;
            StartCoroutine(RunToWalk_Speed());
        }
    }
    IEnumerator RunToWalk_Speed() // Run -> walk 이동속도 줄여주는 함수
    {
        float FadeOut_Time = 0f;
        float FadeOut_TimeCheck = 0.5f;
        float ChangeMovementSpeed = movementSpeed;
        RunTime = 0;
        movementSpeed = RunSpeed;
        animator.SetBool("IsRun", false);
        while (movementSpeed > ChangeMovementSpeed)
        {
            FadeOut_Time += Time.deltaTime / FadeOut_TimeCheck;
            movementSpeed = Mathf.Lerp(RunSpeed, ChangeMovementSpeed, FadeOut_Time);
            yield return null;
        }
        movementSpeed = ChangeMovementSpeed;
        
        yield return null;
    }
    #endregion

    #region 회전
    public void HandleMovementDirection(Vector3 direction)
    {
        desiredRotationAngle = Vector3.Angle(transform.forward, direction);
        var crossProduct = Vector3.Cross(transform.forward, direction).y;
        if (crossProduct < 0)
        {
            desiredRotationAngle *= -1;
        }

    }

    void RotatePlayer_Rotation(bool isAttack) //앞으로 갈때 플레이어 회전
    {
        //if (desiredRotationAngle_Front > 10 || desiredRotationAngle_Front < -10)
        {
            transform.Rotate(Vector3.up * desiredRotationAngle * (isAttack == true? attackrotationSpeed : moverotationSpeed) * Time.deltaTime);
        }
    }


    #endregion

}
