﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpdated : MonoBehaviour
{
    [Header("Jump Variables")]
    [SerializeField]
    float jumpHeight = 4;
    [SerializeField]
    float timeToJumpPeak = .4f;

    [Header("Movement Variables")]
    [SerializeField]
    [Range(0f, 0.5f)]
    float accelerationTimeInAirTurn = .2f;
    [SerializeField]
    [Range(0f, 0.5f)]
    float accelerationTimeOnGroundTurn = .1f;
    [SerializeField]
    float moveSpeed = 10;
    [SerializeField]
    float climbingSpeed = 5f;

    [SerializeField]
    float wallSlideSpeed = 3;
    bool wallSliding;
    int wallDirX;

    float velocityXSmoothing;
    float gravity;
    float jumpVelocity;

    //[HideInInspector]
    public Vector3 velocity;
    private Vector2 directionalInput;

    [HideInInspector]
    public bool jumping = false;
    [HideInInspector]
    public bool onGround = false;
    [HideInInspector]
    public bool falling = false;
    [HideInInspector]
    public bool landing = false;
    [HideInInspector]
    public bool landed = false;
    [HideInInspector]
    public bool onLadder = false;
    [HideInInspector]
    public bool moveOffLadder = false;
    [HideInInspector]
    public bool movingToNextLevel = false;

    Controller2DUpdated controller;
    Animator animator;
    GameObject endOfTheScene;
    PlayerSunBehaviorUpdated playerSunBehavior;

    //Instantiate them instead.
    //ParticleSystem jumpParticle;
    //GameObject jumpParticleObject;
    //ParticleSystem landParticle;
    //GameObject landParticleObject;

    void Start()
    {
        controller = GetComponent<Controller2DUpdated>();
        playerSunBehavior = GetComponent<PlayerSunBehaviorUpdated>();
        animator = GetComponent<Animator>();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpPeak, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpPeak;
        endOfTheScene = GameObject.Find("SceneManager");
    }

    void Update()
    {
        //if (!playerSunBehavior.isDead && playerSunBehavior.doneRespawning)
        //{
            CalculateVelocity();
            HandleWallSliding();
            SetBools();
            MovingToEndOfTheSceneCheck();
            controller.UpdateMovement(velocity * Time.deltaTime, directionalInput);

            if (controller.collisionInfo.above || controller.collisionInfo.below && controller.collisionInfo.climbing == false)
            {
                velocity.y = 0;
            }
            Climb();
        //}
        //else if (playerSunBehavior.isDead)
        //{
        //    //gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpPeak, 2);
        //    velocity.y += gravity * Time.deltaTime;
        //}
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (controller.collisionInfo.below && !playerSunBehavior.isDead)
        {
            jumping = true;
            velocity.y = jumpVelocity;
            landing = false;
            //jumpParticle.Play(); // Instantiate.
        }
    }

    public void SetBools()
    {
        if (jumping == true && velocity.y < 0)
        {
            falling = true;
            jumping = false;
        }
        if (velocity.y < 0 && !controller.collisionInfo.below && !controller.collisionInfo.climbing && moveOffLadder)
        {
            falling = true;
            onGround = false;
            jumping = false;
        }
        if (velocity.y < 0 && controller.collisionInfo.below)
        {
            if (landing == false)
            {
                landing = true;
                animator.SetTrigger("Landing");
                landed = true;
            }
            falling = false;
            onGround = true;
        }
        else if (!controller.collisionInfo.below)
        {
            landing = false;
            onGround = false;
        }
        else
            onGround = false;
    }

    public void Climb()
    {
        if (controller.collisionInfo.climbing && directionalInput.x == 0)
        {
            gravity = 0f;
            if (directionalInput.y > 0)
            {
                velocity.y = climbingSpeed;
            }
            else if (directionalInput.y < 0)
            {
                velocity.y = -climbingSpeed;
            }
            else
            {
                velocity.y = 0f;
            }
        }
        else if (controller.collisionInfo.canClimbOld == false)
        {
            gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpPeak, 2);
            jumpVelocity = Mathf.Abs(gravity) * timeToJumpPeak;
        }
    }

    void HandleWallSliding()
    {
        if (controller.collisionInfo.canClimbOld == false)
        {
            wallDirX = (controller.collisionInfo.left) ? -1 : 1;
            wallSliding = false;
            if ((controller.collisionInfo.left || controller.collisionInfo.right) && !controller.collisionInfo.below && velocity.y < 0)
            {
                wallSliding = true;

                if (velocity.y < -wallSlideSpeed)
                {
                    velocity.y = -wallSlideSpeed;
                }
            }
        }

    }

    void CalculateVelocity()
    {
        if (!playerSunBehavior.isDead && playerSunBehavior.doneRespawning)
        {
            float targetVelocityX = directionalInput.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisionInfo.below) ? accelerationTimeOnGroundTurn : accelerationTimeInAirTurn);
        }
            if (controller.collisionInfo.climbing == false)
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    void MovingToEndOfTheSceneCheck()
    {
        if (movingToNextLevel)
        {
            transform.position = Vector2.MoveTowards(transform.position, endOfTheScene.transform.position, 0.3f * Time.deltaTime);
        }
    }

    void PlayJumpParticle()
    {
        // Instantiate
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //if(other.gameObject.CompareTag("LevelEndPoint"))
        //{
        //    movingToNextLevel = true;
        //    //animator.SetTrigger("MoveToNextLevel");
        //}
        if (other.gameObject.CompareTag("Ground"))
        {
            //landParticle.Play(); // Instantiate
        }
        if (other.gameObject.tag == "MovingPlatform")
        {
            this.gameObject.transform.parent = other.gameObject.transform;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder") && !onGround)
        {
            onLadder = true;
            moveOffLadder = false;
        }
        else
        {
            onLadder = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            onLadder = false;
            moveOffLadder = true;
            animator.SetBool("Climbing", false);
        }
        if(other.gameObject.CompareTag("MovingPlatform"))
        {
            this.gameObject.transform.parent = null;
        }
    }
}
