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

    Vector2 directionalInput;

    float moveOffLadderTimer = 0.01f;
    float moveOffLadderCooldown = 0.01f;
    float moveOffLadderHoldTimer = 0.4f;
    float moveOffLadderHoldCooldown = 0.4f;
    float turnAnimationTimer = 0.5f;
    [SerializeField]
    float prevDirX = 0.0f;
    [SerializeField]
    float currDirX = 0.0f;

    public bool turnAnimRight = false;
    public bool turnAnimLeft = false;

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
    }

    void Update()
    {
        if (!playerSunBehavior.isDead && playerSunBehavior.doneRespawning && player.finishedMovingOutCheckPoint)
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

    void MovementCheck()
    {
        if (controller.collisionInfo.climbing == false || controller.collisionInfo.below == true)
        {
            directionalInput.x = (movementJoystick.Horizontal() > 0.4f || movementJoystick.Horizontal() < -0.4f) ? movementJoystick.Horizontal() : 0;
            currDirX = directionalInput.x;
        }

        TurnCheck();

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

    void TurnCheck()
    {
        if (player.hitTheGround)
        {
            turnAnimationTimer -= Time.deltaTime;

            if (turnAnimationTimer >= 0)
            {
                Debug.Log("turn timer >= 0");
                prevDirX = currDirX;
            }
            else
            {
                if (currDirX > 0f && prevDirX < 0f && !turnAnimRight && player.onGround)
                {
                    Debug.Log("TurnCheck() in the player input triggered");
                    prevDirX = currDirX;
                    turnAnimRight = true;
                    turnAnimLeft = false;
                }
                else if (currDirX < 0f && prevDirX > 0 && !turnAnimLeft && player.onGround)
                {
                    Debug.Log("TurnCheck() in the player input triggered");
                    prevDirX = currDirX;
                    turnAnimLeft = true;
                    turnAnimRight = false;
                }
            }
        }
        else if (!player.hitTheGround && !player.spawnedInSafePoint)
        {
            Debug.Log("else if triggered");
            turnAnimationTimer = .5f;
            prevDirX = currDirX;
        }
    }

    void MoveOffLadderCheck()
    {
        if (controller.collisionInfo.climbing == true && controller.collisionInfo.reachedTopOfTheLadder == false)
        {
            // Reset the timer if we move vertically
            if (directionalInput.y > 0 || directionalInput.y < 0)
            {
                moveOffLadderCooldown = moveOffLadderTimer;
                moveOffLadderHoldCooldown = moveOffLadderHoldTimer;
            }
            //  Start holding the key timer and check if the timer < 0 when we move horizontally
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

            // Start double press the key timer and check if we press the key twice before the timer resets.
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
        if (movementJoystick.jump && SceneManager.GetActiveScene().name != "FinalScene" && SceneManager.GetActiveScene().name != "FinalSceneMobile")
        {
            player.OnJumpInputDown();
            movementJoystick.jump = false;
        }
    }
}
