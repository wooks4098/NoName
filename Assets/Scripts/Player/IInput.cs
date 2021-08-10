using System;
using UnityEngine;

public interface IInput
{
    Action<Vector3> OnMovementDirectionInput { get; set; }
    Action<Vector2, UseSkill> OnMovementInput { get; set; } //<이동, 스킬>

    Action OnAttackInput { get; set; }

    Action OnDodge { get; set; }

    Action QSkill { get; set; }

    Action<Vector3> OnAttackDirection { get; set; }
}