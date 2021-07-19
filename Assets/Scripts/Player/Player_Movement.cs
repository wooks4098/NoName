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
        if(controller.isGrounded)
        {
            //if(movementVector.magnitude > 0)
            //{
            //    //var animationSpeedMultiplier = SetCorrectAnimation();
            //    //RotatePlayer();
            //    //movementVector *= animationSpeedMultiplier;
            //}
        }
        movementVector.y -= gravity;
        controller.Move(movementVector * Time.deltaTime);
    }

    public void HandleMovement(Vector2 input)
    {
        if(controller.isGrounded)
        {
            if(input.y > 0)
            {
                RotatePlayer();
                movementVector = transform.forward * movementSpeed;
                animator.SetFloat("Battle_Walk", input.y);
            }
            else if(input.y == 0)
            {
                movementVector = Vector3.zero;
                animator.SetFloat("Battle_Walk", 0);
            }
            else if(input.y < 0)
            {
                movementVector = transform.forward * movementSpeed;
                //Debug.Log(input.y);
                animator.SetFloat("Battle_Walk", -input.y);
               
            }
        }
    }

    public void HandleMovementDirection(Vector3 direction)
    {
        float test = Quaternion.FromToRotation(Vector3.up, direction - transform.forward).eulerAngles.z;

        desiredRotationAngle = Vector3.Angle(transform.forward, direction);
        var crossProduct = Vector3.Cross(transform.forward, direction).y;
        if (crossProduct < 0)
        {
            desiredRotationAngle *= -1;
        }
        Debug.Log("test" + test);
        Debug.Log(desiredRotationAngle);
        //Debug.Log(desiredRotationAngle);
    }

    void RotatePlayer()
    {
        //Debug.Log(desiredRotationAngle);
        if(desiredRotationAngle > 10 || desiredRotationAngle < -10)
        {
            transform.Rotate(Vector3.up * desiredRotationAngle * rotationSpeed * Time.deltaTime);
        }
    }

    float SetCorrectAnimation()
    {
        float currentAnimationSpeed = animator.GetFloat("Battle_Walk");
        if(desiredRotationAngle > 10 || desiredRotationAngle < -10)
        {
            if(currentAnimationSpeed < 0.2f)
            {
                currentAnimationSpeed += Time.deltaTime * 2;
                currentAnimationSpeed = Mathf.Clamp(currentAnimationSpeed, 0, 0.2f);
            }
            //Debug.Log(currentAnimationSpeed);
        }
        else
        {
            if(currentAnimationSpeed < 1)
            {
                currentAnimationSpeed += Time.deltaTime * 2;
            }
            else
            {
                currentAnimationSpeed = 1;
            }
        }
        return currentAnimationSpeed;
    }
}
