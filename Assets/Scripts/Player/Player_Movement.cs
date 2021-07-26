using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{

    CharacterController controller;
    Animator animator;

    public float rotationSpeed;
    public float movementSpeed;
    public float gravity = 20;
    Vector3 movementVector = Vector3.zero;
    private float desiredRotationAngle = 0;



    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        movementVector.y -= gravity;
        controller.Move(movementVector * Time.deltaTime);
    }

    public void HandleMovement(Vector2 input)
    {
        if (controller.isGrounded)
        {
            if (input.y == 0 && input.x == 0)
            {//Stop
                movementVector = Vector3.zero;
                animator.SetFloat("Battle_Walk", 0);
            }
            else
            {
                RotatePlayer_Front();
                movementVector = transform.forward * movementSpeed;
                animator.SetFloat("Battle_Walk", Mathf.Abs(input.y));
            }
        }
    }

    void Movement(Vector3 vec3, float aniSpeed)
    {
        movementVector = Vector3.zero;
        animator.SetFloat("Battle_Walk", 0);
    }

    public void HandleMovementDirection(Vector3 direction)
    {
        desiredRotationAngle = Vector3.Angle(transform.forward, direction);
        var crossProduct = Vector3.Cross(transform.forward, direction).y;
        if (crossProduct < 0)
        {
            desiredRotationAngle *= -1;
        }

    }

    void RotatePlayer_Front() //앞으로 갈때 플레이어 회전
    {
        //if (desiredRotationAngle_Front > 10 || desiredRotationAngle_Front < -10)
        {
            transform.Rotate(Vector3.up * desiredRotationAngle * rotationSpeed * Time.deltaTime);
        }
    }




}
