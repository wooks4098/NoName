using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    void Attack();
    void DashAtaack();
    void Skill();
}

public class SwordSkill : MonoBehaviour, ISkill
{
    public void Attack()
    {

    }

    public void DashAtaack()
    {
        throw new System.NotImplementedException();
    }

    public void Skill()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
