﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{

    [Header("Jump Variables")]
    [SerializeField]
    private float jumpHeight = 4;
    [SerializeField]
    private float timeToJumpPeak = .4f;

    [Header("Movement Variables")]
    [SerializeField]
    [Range(0f, 0.5f)]
    private float accelerationTimeInAirTurn = .2f;
    [SerializeField]
    [Range(0f, 0.5f)]
    private float accelerationTimeOnGroundTurn = .1f;
    [SerializeField]
    private float moveSpeed = 10;

    [SerializeField]
    private float climbingSpeed = 5f;
    public bool jumping = false;
    private float velocityXSmoothing;
    private float gravity;
    private float jumpVelocity;
    public Vector3 velocity;
    private Vector2 directionalInput;
    public bool onGround;
    public bool falling;
    public bool landing;
    public bool landed;

    [SerializeField]
    public float wallSlideSpeed = 3;
    private bool wallSliding;
    private int wallDirX;

    private Controller2D controller;

    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpPeak, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpPeak;
    }

    void Update()
    {
        CalculateVelocity();
        HandleWallSliding();
        GroundCheck();
        
        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisionInfo.above || controller.collisionInfo.below && controller.collisionInfo.climbing == false)
        {
            velocity.y = 0;
        }
        Climb();
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (controller.collisionInfo.below)
        {
            jumping = true;
            velocity.y = jumpVelocity;
            landing = false;
        }
    }

    public void GroundCheck()
    {
        if(jumping == true && velocity.y < 0)
        {
            falling = true;
            jumping = false;
        }
        if (velocity.y < 0 && controller.collisionInfo.below)
        {
            if(landing == false)
            {
                landing = true;
                landed = true;
            }
            falling = false;
            onGround = true;
        }
        else if(!controller.collisionInfo.below)
        {
            landing = false;
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
        else if(controller.collisionInfo.canClimbOld == false)
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
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisionInfo.below) ? accelerationTimeOnGroundTurn : accelerationTimeInAirTurn);
        if (controller.collisionInfo.climbing == false)
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    
}
