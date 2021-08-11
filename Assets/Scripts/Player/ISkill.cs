using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UseSkill
{
    None = 0,
    AttackCombo,
    DashAttack,
    Dodge,
    QSkill,
}

public interface ISkill
{
    void Attack();
    void DashAtaack();

    void Dodge();
    void Qskill();

    bool IsAttack();
    bool isDashAttack();

    UseSkill isSkill();
}
