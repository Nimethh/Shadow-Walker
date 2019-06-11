using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    float moveOffLadderHoldTimer = 0.2f;
    float moveOffLadderHoldCooldown = 0.2f;

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

        //movingParticalObject = transform.GetChild(2).gameObject;
        //movingPartical = movingParticalObject.GetComponent<ParticleSystem>();
        //movingLeftParticleObject = transform.GetChild(3).gameObject;
        //movingLeftParticle = movingLeftParticleObject.GetComponent<ParticleSystem>();
    }

    //void Update()
    //{
    //    if (!playerSunBehavior.isDead && playerSunBehavior.doneRespawning && !player.movingIntoCheckPoint && !player.movingOutCheckPoint)
    //    {
    //        MoveOffLadderCheck();
    //        MovementCheck();
    //        JumpCheck();

    //        player.SetDirectionalInput(directionalInput);
    //        playerSoundManager.SetDirectionalInput(directionalInput);
    //        playerAnimationManager.SetDirectionalInput(directionalInput);
    //    }

    //    CheckPlayerBounds();

    //}
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
            playerSunBehavior.isDead = true;
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
            //directionalInput.x = Input.GetAxisRaw("Horizontal");
            directionalInput.x = (movementJoystick.Horizontal() > 0.4f || movementJoystick.Horizontal() < -0.4f) ? movementJoystick.Horizontal() : 0;
        }
        //directionalInput.y = Input.GetAxisRaw("Vertical");
        directionalInput.y = (movementJoystick.Vertical() > 0.4f || movementJoystick.Vertical() < -0.4f) ? movementJoystick.Vertical() : 0;
        //Debug.Log(directionalInput.y);
        //Debug.Log(directionalInput.x + " " + directionalInput.y);
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
        if (movementJoystick.jump && !player.movingToNextLevel)
        {
            player.OnJumpInputDown();
            //Checking something
            movementJoystick.jump = false;
        }
    }
}
