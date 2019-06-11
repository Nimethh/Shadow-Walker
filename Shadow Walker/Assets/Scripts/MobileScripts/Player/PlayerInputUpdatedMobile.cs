using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerUpdatedMobile))]
public class PlayerInputUpdatedMobile : MonoBehaviour
{
    Controller2DUpdatedMobile controller;
    PlayerSoundManagerMobile playerSoundManager;
    PlayerUpdatedMobile player;
    PlayerSunBehaviorUpdatedMobile playerSunBehavior;
    PlayerAnimationManagerMobile playerAnimationManager;
    // GameObject movingParticle;  Instantiate the particle instead.

    Vector2 directionalInput;

    float moveOffLadderTimer = 0.01f;
    float moveOffLadderCooldown = 0.01f;
    float moveOffLadderHoldTimer = 0.4f;
    float moveOffLadderHoldCooldown = 0.4f;
    float turnAnimationTimer = 0.5f;

    float prevDirX;
    float currDirX;
    public bool turnAnimRight = false;
    public bool turnAnimLeft = false;
    //ParticleSystem movingPartical;
    //GameObject movingParticalObject;
    //ParticleSystem movingLeftParticle;
    //GameObject movingLeftParticleObject;

    float top = 0;
    float bottom = 0;
    float right = 0;
    float left = 0;

    public VirtualMovementJoystick movementJoystick;

    void Start()
    {
        player = GetComponent<PlayerUpdatedMobile>();
        playerSoundManager = GetComponent<PlayerSoundManagerMobile>();
        controller = GetComponent<Controller2DUpdatedMobile>();
        playerSunBehavior = GetComponent<PlayerSunBehaviorUpdatedMobile>();
        playerAnimationManager = GetComponent<PlayerAnimationManagerMobile>();
        moveOffLadderCooldown = moveOffLadderTimer;
        moveOffLadderHoldCooldown = moveOffLadderHoldTimer;

        //Cursor.visible = false;
        FindPlayerBounds();
        turnAnimationTimer = 0;
        //movingParticalObject = transform.GetChild(2).gameObject;
        //movingPartical = movingParticalObject.GetComponent<ParticleSystem>();
        //movingLeftParticleObject = transform.GetChild(3).gameObject;
        //movingLeftParticle = movingLeftParticleObject.GetComponent<ParticleSystem>();


    }

    void Update()
    {
        if (!playerSunBehavior.isDead && playerSunBehavior.doneRespawning && player.finishedMovingOutCheckPoint/*&& playerSunBehavior.doneRespawning*/ /*&& !player.movingIntoCheckPoint && !player.movingOutCheckPoint*/)
        {
            MoveOffLadderCheck();
            MovementCheck();
            JumpCheck();

            player.SetDirectionalInput(directionalInput);
            playerSoundManager.SetDirectionalInput(directionalInput);
            playerAnimationManager.SetDirectionalInput(directionalInput);
        }

        CheckPlayerBounds();
    }

    public void FindPlayerBounds()
    {
        top = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane)).y;
        right = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane)).x;
        left = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)).x;
        bottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)).y;
    }

    public void CheckPlayerBounds()
    {
        //Right
        if (transform.position.x > right - 0.2f)// && transform.position.x > left)
        {
            transform.position = new Vector3(right - 0.2f, transform.position.y, transform.position.z);
        }

        //Left
        if (transform.position.x < left + 0.2f)
        {
            transform.position = new Vector3(left + 0.2f, transform.position.y, transform.position.z);
        }

        //Bottom
        if (transform.position.y < (bottom - 10.0f))
        {
            player.velocity.y = 0;
            //playerSunBehavior.isDead = true;
            transform.position = playerSunBehavior.spawningPos;
        }

    }

    //public void PlayMovingRightParticle()
    //{
    //    movingPartical.Play();
    //    // Instantiate(movingParticle);
    //}

    //public void PlayMovingLeftParticle()
    //{
    //    movingLeftParticle.Play();
    //}

    void MovementCheck()
    {
        if (controller.collisionInfo.climbing == false || controller.collisionInfo.below == true)
        {
            directionalInput.x = (movementJoystick.Horizontal() > 0.4f || movementJoystick.Horizontal() < -0.4f) ? movementJoystick.Horizontal() : 0;
            //if (player.onGround)
            //{
            currDirX = directionalInput.x;
            //}
        }
        if (player.hitTheGround)
        {
            //prevDirX = currDirX;
            //player.hitTheGround = false;
            turnAnimationTimer -= Time.deltaTime;

            if (turnAnimationTimer >= 0)
            {
                prevDirX = currDirX;
            }
            else
            {
                if (currDirX == 1 && prevDirX == -1 && !turnAnimRight && player.onGround)
                {
                    //Debug.Log("Changed Right");
                    prevDirX = currDirX;
                    turnAnimRight = true;
                    turnAnimLeft = false;
                }
                else if (currDirX == -1 && prevDirX == 1 && !turnAnimLeft && player.onGround)
                {
                    //Debug.Log("Changed Left");
                    prevDirX = currDirX;
                    turnAnimLeft = true;
                    turnAnimRight = false;
                }
            }
        }
        else if (!player.hitTheGround && !player.spawnedInSafePoint)
        {
            turnAnimationTimer = .5f;
            prevDirX = currDirX;
        }
        //if (!player.onGround)
        //{
        //    prevDirX = currDirX;
        //}
        //else
        //{
        //    turnAnimLeft = false;
        //    turnAnimRight = false;
        //}

        directionalInput.y = (movementJoystick.Vertical() > 0.4f || movementJoystick.Vertical() < -0.4f) ? movementJoystick.Vertical() : 0;

        //Check player bounds
        if (directionalInput.x > 0 && transform.position.x > right - 0.2f)
        {
            directionalInput.x = 0;
        }
        if (directionalInput.x < 0 && transform.position.x < left + 0.2f)
        {
            directionalInput.x = 0;
        }

        if (prevDirX == 0)
        {
            prevDirX = directionalInput.x;

        }

    }

    void MoveOffLadderCheck()
    {
        if (controller.collisionInfo.climbing == true && controller.collisionInfo.reachedTopOfTheLadder == false)
        {
            if (directionalInput.y > 0 || directionalInput.y < 0)
            {
                moveOffLadderCooldown = moveOffLadderTimer;
                moveOffLadderHoldCooldown = moveOffLadderHoldTimer;
            }
            if ((movementJoystick.Horizontal() > 0.4f || movementJoystick.Horizontal() < -0.4f))
            {
                if (moveOffLadderHoldCooldown <= 0)
                {
                    directionalInput.x = (movementJoystick.Horizontal() > 0.4f || movementJoystick.Horizontal() < -0.4f) ? movementJoystick.Horizontal() : 0;
                }
                else
                {
                    moveOffLadderHoldCooldown -= Time.deltaTime;
                }
            }

            if (movementJoystick.Horizontal() > 0.4f)
            {
                if (moveOffLadderCooldown <= 0)
                {
                    directionalInput.x = (movementJoystick.Horizontal() > 0.4f || movementJoystick.Horizontal() < -0.4f) ? movementJoystick.Horizontal() : 0;
                }
                else
                {
                    moveOffLadderCooldown -= Time.deltaTime;
                }
            }
            else if (movementJoystick.Horizontal() < -0.4f)
            {
                if (moveOffLadderCooldown <= 0)
                {
                    directionalInput.x = (movementJoystick.Horizontal() > 0.4f || movementJoystick.Horizontal() < -0.4f) ? movementJoystick.Horizontal() : 0;
                }
                else
                {
                    moveOffLadderCooldown -= Time.deltaTime;
                }
            }
        }
    }

    void JumpCheck()
    {
        if (movementJoystick.jump && !player.movingToNextLevel && SceneManager.GetActiveScene().name != "FinalScene" && SceneManager.GetActiveScene().name != "FinalSceneMobile")
        {
            player.OnJumpInputDown();
            movementJoystick.jump = false;
        }
    }
}
