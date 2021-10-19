using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
   // [SerializeField] IInput input;
    [SerializeField] Player_Movement movement;
    [SerializeField] Player_Attack player_Attack;
    [SerializeField] Player_Input player_input;
    public Player_Attack GetPlayer_Attack { get { return player_Attack; } }

    private void Start()
    {

       // input = player_input.GetComponent<IInput>();
        //movement = GetComponent<Player_Movement>();

        //Input ¿¬°á
        player_input.OnMovementDirectionInput += movement.HandleMovementDirection;
        player_input.OnMovementInput += movement.HandleMovement;

        //var skill = player_Attack.GetComponent<ISkill>();
        player_input.OnAttackInput += player_Attack.Attack;
        player_input.OnDodge += player_Attack.Dodge;
        player_input.QSkill += player_Attack.Qskill;


    }

    //private void OnDestroy()
    //{
    //    input.OnMovementDirectionInput -= movement.HandleMovementDirection;
    //    input.OnMovementInput -= movement.HandleMovement;
    //}



}
