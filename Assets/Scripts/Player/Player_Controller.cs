using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    // [SerializeField] IInput input;
    [SerializeField] Player_Movement movement;
    [SerializeField] Player_Attack player_Attack;
    [SerializeField] Player_Input player_input;
    [SerializeField] Player_StatuController player_StatuController;

    CharacterController characterController;
    public Player_Attack GetPlayer_Attack { get { return player_Attack; } }
    Animator ani;
    private void Start()
    {

        // input = player_input.GetComponent<IInput>();
        //movement = GetComponent<Player_Movement>();

        //Input 연결
        player_input.OnMovementDirectionInput += movement.HandleMovementDirection;
        player_input.OnMovementInput += movement.HandleMovement;

        //var skill = player_Attack.GetComponent<ISkill>();
        player_input.OnAttackInput += player_Attack.Attack;
        player_input.OnDodge += player_Attack.Dodge;
        player_input.QSkill += player_Attack.Qskill;

        ani = GetComponentInChildren<Animator>();
        characterController = GetComponentInChildren<CharacterController>();
    }

    //플레이어 스폰 생성시 상태 변경
    public void PlayerSpawn()
    {

        GameManager.Instance.PlayerDieChange(true);
        player_input.PlayerDie(false);

    }


    public void Die()
    {
        ani.SetTrigger("Die");
        player_input.PlayerDie(true);
        characterController.enabled = false;
        // 애니메이션 Any State도 막기
    }

    //private void OnDestroy()
    //{
    //    input.OnMovementDirectionInput -= movement.HandleMovementDirection;
    //    input.OnMovementInput -= movement.HandleMovement;
    //}

    public Player_StatuController GetPlayerStatusController()
    {
        return player_StatuController;
    }


}
