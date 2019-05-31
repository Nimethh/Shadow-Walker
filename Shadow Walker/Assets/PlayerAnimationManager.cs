using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    GameObject note;
    Vector3 notePosition;
    GameObject bedCollider;
    GameObject character;
    bool moveTowardsTheNote = false;
    float speed = 0.75f;
    bool teleport = false;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerUpdated>();
        playerInput = GetComponent<PlayerInputUpdated>();
        playerSunBehavior = GetComponent<PlayerSunBehaviorUpdated>();
        controller = GetComponent<Controller2DUpdated>();
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            bedCollider = GameObject.Find("BedCollider");
            bedCollider.SetActive(false);
            note = GameObject.Find("Note");
        }
    }
    
    void Update()
    {
        if (player.finishedMovingIntoCheckPoint && player.finishedMovingOutCheckPoint)
        {
            MovementAnimationCheck();
            JumpAnimationCheck();
            FallingAnimationCheck();
            ClimbingAnimationCheck();
            DeathAnimationCheck();
            MovementTurnCheck();
            //RespawningAnimationCheck();
        }
        CheckPointAnimationCheck();
        if(moveTowardsTheNote)
        {
            transform.position = Vector3.MoveTowards(transform.position, notePosition, Time.deltaTime * speed);
            if(Vector2.Distance(transform.position,notePosition) <= 0.2f)
            {
                moveTowardsTheNote = false;
                note.SetActive(false);
            }
        }
    }

    public void ActivateBedCollider()
    {
        bedCollider.SetActive(true);
    }

    public void TutorialAnimation()
    {
        playerSunBehavior.doneRespawning = false;
        player.finishedMovingOutCheckPoint = false;
    }

    public void MoveTowardsTheNote()
    {
        notePosition.x = note.transform.position.x;
        notePosition.y = transform.position.y;
        notePosition.z = -3;
        moveTowardsTheNote = true;
    }

    public void FinishedTurning()
    {
        if(playerInput.turnAnimLeft)
        {
            playerInput.turnAnimLeft = false;
        }
        else if(playerInput.turnAnimRight)
        {
            playerInput.turnAnimRight = false;
        }
    }

    void MovementTurnCheck()
    {
        if (playerInput.turnAnimLeft)
        {
            animator.SetBool("Turn", true);
            animator.SetFloat("CurrentDirX", -1f);
        }
        else if (playerInput.turnAnimRight)
        {
            animator.SetBool("Turn", true);
            animator.SetFloat("CurrentDirX", 1);
        }
        else
        {
            animator.SetBool("Turn", false);
        }

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

        if (player.movingIntoCheckPoint)
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

    public void Teleport()
    {
        if (teleport)
        {
            GameObject spawnPos = GameObject.Find("Door1");
            Vector3 spawnPosition = spawnPos.transform.position;
            spawnPosition.z = -3;
            transform.position = spawnPosition;

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
            Debug.Log("IsDead = false");
            animator.SetBool("Dead", false);
        }
    }

    void RespawningAnimationCheck()
    {
        if(playerSunBehavior.isRespawning)
        {
            animator.SetBool("Respawning", true);
        }
        if(playerSunBehavior.doneRespawning)
        {
            animator.SetBool("Respawning", false);
        }
    }

    void CheckPointAnimationCheck()
    {
        if(player.movingIntoCheckPoint /*&& !controller.collisionInfo.climbing*/)
        {
            animator.SetBool("MovingIntoCheckPoint",true);
            animator.SetBool("Movement", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Climbing", false);
        }
        else if(player.movingOutCheckPoint)
        {
            animator.SetBool("MovingOutofCheckPoint", true);
            animator.SetBool("MovingIntoCheckPoint", false);
            animator.SetBool("Movement", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Climbing", false);
        }
        else
        {
            animator.SetBool("MovingOutofCheckPoint", false);
            animator.SetBool("MovingIntoCheckPoint", false);
        }
    }

    public void StopCheckPointAnimation()
    {
        player.movingOutCheckPoint = false;
        player.movingIntoCheckPoint = false;
        playerSunBehavior.isSafeFromSun = false;
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (SceneManager.GetActiveScene().name == "FinalScene")
        {
            if (other.gameObject.CompareTag("Girlfriend"))
            {
                animator.SetTrigger("Lean");
                PlayerUpdated playerUpdated = GetComponent<PlayerUpdated>();
                playerUpdated.enabled = false;
            }
        }
        
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == "Teleport" && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            teleport = true;
            other.gameObject.SetActive(false);
        }
    }
}
