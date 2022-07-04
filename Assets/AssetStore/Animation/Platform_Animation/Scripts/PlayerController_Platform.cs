using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Platform : MonoBehaviour
{
    Animator anim;

    [Header("Rotation speed")]
    public float speed_rot;

    [Header("Movement speed during jump")]
    public float speed_move;

    [Header("Time available for combo")]
    public int term;

    public bool isJump;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Rotate();
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        if (!isJump)
        {            
            Attack();
            
            Dodge();
            
            Jump();

            Block();
        }
    }

    Quaternion rot;
    bool isRun;

    
    void Rotate()
    {
        
        if (Input.GetKey(KeyCode.D))
        {            
            Move();            
            rot = Quaternion.LookRotation(Vector3.right);
        }

        
        else if (Input.GetKey(KeyCode.A))
        {            
            Move();
            rot = Quaternion.LookRotation(Vector3.left);
        }

        else
        {            
            anim.SetBool("Run", false);
        }
        
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, speed_rot * Time.deltaTime);

    }

    
    void Move()
    {
        
        if (isJump)
        {            
            transform.position += transform.forward * speed_move * Time.deltaTime;            
            anim.SetBool("Run", false);
        }
        else
        {            
            anim.SetBool("Run", true);
        }
    }

    int clickCount;
    float timer;
    bool isTimer;

    
    void Attack()
    {
        
        if (isTimer)
        {
            timer += Time.deltaTime;
        }

        
        if (Input.GetMouseButtonDown(0))
        {
            switch (clickCount)
            {
                
                case 0:
                    
                    anim.SetTrigger("Attack1");
                    
                    isTimer = true;
                    
                    clickCount++;
                    break;

                
                case 1:
                    
                    if (timer <= term)
                    {                        
                        anim.SetTrigger("Attack2");
                        
                        clickCount++;
                    }

                    
                    else
                    {                        
                        anim.SetTrigger("Attack1");
                        
                        clickCount = 1;
                    }

                    
                    timer = 0;
                    break;

                
                case 2:
                    
                    if (timer <= term)
                    {                        
                        anim.SetTrigger("Attack3");
                        
                        clickCount = 0;
                        
                        isTimer = false;
                    }

                    
                    else
                    {                        
                        anim.SetTrigger("Attack1");
                        
                        clickCount = 1;
                    }
                
                    timer = 0;
                    break;
            }
        }
    }

    
    void Dodge()
    {
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {            
            anim.SetTrigger("Dodge");
        }
    }

    void Block()
    {

        if (Input.GetMouseButton(1))
        {
            anim.SetBool("Block", true);
        }
        else
        {
            anim.SetBool("Block", false);
        }
    }


    void Jump()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {            
            anim.SetTrigger("Jump");

            isJump = true;
        }
    }

    
    void JumpEnd()
    {
        isJump = false;
    }
}
