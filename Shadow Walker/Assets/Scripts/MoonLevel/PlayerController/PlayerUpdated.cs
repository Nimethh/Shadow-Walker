﻿using UnityEngine;

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

    [HideInInspector]
    public Vector3 velocity;
    private Vector2 directionalInput;

    [SerializeField]
    GameObject LandParticle;
    [SerializeField]
    GameObject jumpParticle;
    [SerializeField]
    GameObject walkParticleLeft;
    [SerializeField]
    GameObject walkParticleRight;
    GameObject particlesSpawnPos;


    [HideInInspector]
    public bool jumping = false;
    //[HideInInspector]
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
    [HideInInspector]
    public bool movingIntoCheckPoint = false;
    [HideInInspector]
    public bool movingOutCheckPoint = false;
    [HideInInspector]
    public bool finishedMovingOutCheckPoint = true;
    [HideInInspector]
    public bool finishedMovingIntoCheckPoint = true;
    [HideInInspector]
    public bool spawnedInSafePoint = false;

    Controller2DUpdated controller;
    Animator animator;
    GameObject endOfTheScene;
    PlayerSunBehaviorUpdated playerSunBehavior;
    
    AudioSource audioSource;    // Added 28/5/2019

    //Instantiate them instead.
    //ParticleSystem jumpParticle;
    //GameObject jumpParticleObject;
    //ParticleSystem landParticle;
    //GameObject landParticleObject;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();  // Added 28/5/2019

        controller = GetComponent<Controller2DUpdated>();
        playerSunBehavior = GetComponent<PlayerSunBehaviorUpdated>();
        animator = GetComponent<Animator>();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpPeak, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpPeak;
        endOfTheScene = GameObject.Find("SceneManager");
        particlesSpawnPos = transform.GetChild(0).gameObject;
        spawnedInSafePoint = true;
        playerSunBehavior.isSafeFromSun = true;
    }

    void Update()
    {

        CheckSpawnMovingOut();
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
    }


    public void IsInSafePointBoolManager()
    {
        spawnedInSafePoint = true;
        playerSunBehavior.isSafeFromSun = true;
        playerSunBehavior.doneRespawning = false;
        finishedMovingOutCheckPoint = false;
    }

    public void MovingOutOFCheckPointBoolManager()
    {
        if(spawnedInSafePoint)
        {
            spawnedInSafePoint = false;
            playerSunBehavior.isSafeFromSun = false;
            playerSunBehavior.doneRespawning = true;
            finishedMovingOutCheckPoint = true;
            movingOutCheckPoint = false;
        }
        else if(finishedMovingIntoCheckPoint)
        {
            finishedMovingOutCheckPoint = true;
            movingIntoCheckPoint = false;
            movingOutCheckPoint = false;
            playerSunBehavior.isSafeFromSun = false;
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    void CheckSpawnMovingOut()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) ||
                Input.GetKeyDown(KeyCode.Space)) && spawnedInSafePoint && !playerSunBehavior.isDead)
        {
            animator.SetBool("MovingOutofCheckPoint", true);
        }
    }

    public void OnJumpInputDown()
    {
        if (controller.collisionInfo.below && !playerSunBehavior.isDead && !spawnedInSafePoint && finishedMovingOutCheckPoint /*!movingOutCheckPoint*/)
        {
            jumping = true;
            Instantiate(jumpParticle, particlesSpawnPos.transform);
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
        if (velocity.y < 0 && !controller.collisionInfo.below && !controller.collisionInfo.climbing && moveOffLadder && !controller.collisionInfo.climbingSlope
            && !controller.collisionInfo.descendingSlope)
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

    public void RightWalkingParticle()
    {
        Instantiate(walkParticleRight, particlesSpawnPos.transform);
    }

    public void LeftWalkingParticle()
    {
        Instantiate(walkParticleLeft, particlesSpawnPos.transform);
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
            if (finishedMovingOutCheckPoint)
            {
                float targetVelocityX = directionalInput.x * moveSpeed;
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisionInfo.below) ? accelerationTimeOnGroundTurn : accelerationTimeInAirTurn);
            }
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
        if (other.gameObject.CompareTag("Ground") && !spawnedInSafePoint)
        {
            Instantiate(LandParticle, particlesSpawnPos.transform);
        }
        if (other.gameObject.tag == "MovingPlatform")
        {
            this.gameObject.transform.parent = other.gameObject.transform;
        }
        if(other.gameObject.tag == "DeadlyDoor")
        {
            playerSunBehavior.isDead = true;
        }
    }

    public void CanMoveOut()
    {
        finishedMovingIntoCheckPoint = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (controller.collisionInfo.climbing && !onLadder)
        {
            if (other.gameObject.CompareTag("Ladder") && !onGround)
            {
                onLadder = true;
                moveOffLadder = false;
            }
        }
        if(other.gameObject.CompareTag("CheckPoint") && onGround && !spawnedInSafePoint && !playerSunBehavior.isDead)
        {
            if((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && !movingIntoCheckPoint && finishedMovingOutCheckPoint && !playerSunBehavior.isDead/*!movingOutCheckPoint*/ /*&& directionalInput.x == 0*/)
            {
                movingIntoCheckPoint = true;
                movingOutCheckPoint = false;
                finishedMovingIntoCheckPoint = false;
                finishedMovingOutCheckPoint = false;
                velocity.x = 0;
                playerSunBehavior.isSafeFromSun = true;
                audioSource.Play();
            }
            else if((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) ||
                Input.GetKeyDown(KeyCode.Space)) && movingIntoCheckPoint && finishedMovingIntoCheckPoint && !playerSunBehavior.isDead)
            {
                movingIntoCheckPoint = false;
                movingOutCheckPoint = true;
                velocity.x = 0;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            onLadder = false;
            moveOffLadder = true;
        }
        if(other.gameObject.CompareTag("MovingPlatform"))
        {
            this.gameObject.transform.parent = null;
        }
        if (other.gameObject.CompareTag("CheckPoint"))
        {
            movingIntoCheckPoint = false;
            movingOutCheckPoint = false;
        }
    }
}
