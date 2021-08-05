using System;
using UnityEngine;

public interface IInput
{
    Action<Vector3> OnMovementDirectionInput { get; set; }
    Action<Vector2, bool> OnMovementInput { get; set; } //<이동, 스킬>

    Action OnAttackInput { get; set; }

    Action<Vector3> OnAttackDirection { get; set; }
}