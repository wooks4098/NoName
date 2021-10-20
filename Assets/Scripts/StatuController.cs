using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CharacterType
{
    Player = 0,
    Monster,
}

public class StatuController : MonoBehaviour
{
    [SerializeField] CharacterType characterType;

    Weapon weapon = null;
    MonsterData monsterData = null;

    public Weapon GetWeapon { get { return weapon; } }
    public MonsterData GetMonsterData { get { return monsterData; } }





}
