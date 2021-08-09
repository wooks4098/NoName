using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    IInput input;
    Player_Movement movement;


    private void Start()
    {
        input = GetComponent<IInput>();
        movement = GetComponent<Player_Movement>();

        //Input ¿¬°á
        input.OnMovementDirectionInput += movement.HandleMovementDirection;
        input.OnMovementInput += movement.HandleMovement;

        var skill = GetComponent<ISkill>();
        input.OnAttackInput += skill.Attack;

    }

    private void OnDestroy()
    {
        input.OnMovementDirectionInput -= movement.HandleMovementDirection;
        input.OnMovementInput -= movement.HandleMovement;
    }



}
