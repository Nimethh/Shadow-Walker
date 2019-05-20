using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    Animator animator;
    PlayerUpdated player;
    PlayerInputUpdated playerInput;
    PlayerSunBehaviorUpdated playerSunBehavior;
    Controller2DUpdated controller;

    float xMovement;
    float lastMoveX;
    Vector2 directionalInput;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerUpdated>();
        playerInput = GetComponent<PlayerInputUpdated>();
        playerSunBehavior = GetComponent<PlayerSunBehaviorUpdated>();
        controller = GetComponent<Controller2DUpdated>();
    }
    
    void Update()
    {
        MovementAnimationCheck();
        JumpAnimationCheck();
        FallingAnimationCheck();
        ClimbingAnimationCheck();
        DeathAnimationCheck();
        RespawningAnimationCheck();
    }

    void MovementAnimationCheck()
    {
        if (player.onGround)
        {
            animator.SetBool("OnGround", true);
            if (directionalInput.x > 0 && player.onGround)
            {
                lastMoveX = directionalInput.x;
                animator.SetFloat("MovementX", directionalInput.x);
                animator.SetBool("Moving", true);
            }
            else if (directionalInput.x < 0 && player.onGround)
            {
                lastMoveX = directionalInput.x;
                animator.SetFloat("MovementX", directionalInput.x);
                animator.SetBool("Moving", true);
            }
            else
            {
                animator.SetFloat("MovementX", directionalInput.x);
                animator.SetBool("Moving", false);
            }
            animator.SetFloat("LastXValue", lastMoveX);
        }
        else
            animator.SetBool("OnGround", false);
    }

    void JumpAnimationCheck()
    {
        if (player.jumping == true)
        {
            animator.SetBool("Jumping", true);
            if (directionalInput.x > 0)
            {
                lastMoveX = directionalInput.x;
                animator.SetFloat("MovementX", directionalInput.x);
            }
            else if (directionalInput.x < 0)
            {
                lastMoveX = directionalInput.x;
                animator.SetFloat("MovementX", directionalInput.x);
            }
            else
            {
                animator.SetFloat("MovementX", directionalInput.x);
            }
            animator.SetFloat("LastXValue", lastMoveX);
        }
        else
            animator.SetBool("Jumping", false);
    }

    void FallingAnimationCheck()
    {
        if (player.falling == true && (!playerSunBehavior.isRespawning || !playerSunBehavior.isDead))
        {
            animator.SetBool("Falling", player.falling);
            if (directionalInput.x > 0)
            {
                lastMoveX = directionalInput.x;
                animator.SetFloat("MovementX", directionalInput.x);
            }
            else if (directionalInput.x < 0)
            {
                lastMoveX = directionalInput.x;
                animator.SetFloat("MovementX", directionalInput.x);
            }
            else
            {
                animator.SetFloat("MovementX", directionalInput.x);
            }
            animator.SetFloat("LastXValue", lastMoveX);
        }
        else
            animator.SetBool("Falling", player.falling);
    }

    void ClimbingAnimationCheck()
    {
        if (player.onLadder && !playerSunBehavior.isDead)
        {
            if (directionalInput.y > 0 || directionalInput.y < 0)
            {
                animator.SetBool("Climbing", true);
                player.onGround = false;
                animator.speed = 1;
            }
            else
            {
                animator.speed = 0;
            }
        }
        else
        {
            animator.SetBool("Climbing", false);
            if (animator.name == "Jump")
            {
                animator.speed = 1.5f;
            }
            else if (animator.name == "Falling")
            {
                animator.speed = 1.5f;
            }
            else if (animator.name == "Landing")
            {
                animator.speed = 2.5f;
            }
            else
            {
                animator.speed = 1;
            }
        }

        if(playerSunBehavior.isDead)
        {
            animator.SetBool("Climbing", false);
            if (animator.name == "Jump")
            {
                animator.speed = 1.5f;
            }
            else if (animator.name == "Falling")
            {
                animator.speed = 1.5f;
            }
            else if (animator.name == "Landing")
            {
                animator.speed = 2.5f;
            }
            else
            {
                animator.speed = 1;
            }
        }
    }

    void DeathAnimationCheck()
    {
        if (playerSunBehavior.isDead)
        {
            animator.SetBool("Dead", true);
            if (animator.GetBool("Jumping") == true)
            {
                animator.SetBool("Jumping", false);
            }
            if(animator.GetBool("Climbing") == true)
            {
                animator.SetBool("Climbing", false);
            }
            if(animator.GetBool("Falling") == true)
            {
                animator.SetBool("Falling", false);
            }

            if (player.onGround)
            {
                animator.SetBool("OnGround", true);
            }
        }
        else
        {
            animator.SetBool("Dead", false);
        }
    }

    void RespawningAnimationCheck()
    {
        if(playerSunBehavior.isRespawning)
        {
            Debug.Log("RespaningAnimationCheck");
            animator.SetBool("Respawning", true);
        }
        if(playerSunBehavior.doneRespawning)
        {
            animator.SetBool("Respawning", false);
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }
}
