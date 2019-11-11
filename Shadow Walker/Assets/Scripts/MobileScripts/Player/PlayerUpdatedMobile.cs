using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUpdatedMobile : MonoBehaviour
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

    bool wallSliding = false;   // assinged value 2019/11/10
    int wallDirX = 0;   // assigned value 2019/11/10

    float velocityXSmoothing = 0;   // assigned value 2019/11/10
    float gravity = 0;      // assigned value 2019/11/10
    float jumpVelocity = 0; // assigned value 2019/11/10

    [HideInInspector]
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
    public bool movingIntoCheckPoint = false;
    [HideInInspector]
    public bool movingOutCheckPoint = false;
    [HideInInspector]
    public bool finishedMovingOutCheckPoint = true;
    [HideInInspector]
    public bool finishedMovingIntoCheckPoint = true;
    [HideInInspector]
    public bool spawnedInSafePoint = false;
    [HideInInspector]
    public bool hitTheGround = false;

    Controller2DUpdatedMobile controller;
    Animator animator;
    PlayerSunBehaviorUpdatedMobile playerSunBehavior;
    AudioManager audioManager;
    ClimbingCheckMobile climbingCheck;    //Added 2019/11/10

    [SerializeField]
    VirtualMovementJoystick movementJoystick;

    // Implement particles:
    
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        controller = GetComponent<Controller2DUpdatedMobile>();
        playerSunBehavior = GetComponent<PlayerSunBehaviorUpdatedMobile>();
        animator = GetComponent<Animator>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpPeak, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpPeak;
     
        if (SceneManager.GetActiveScene().name != "Level1Mobile")
        {
            spawnedInSafePoint = true;
        }

        playerSunBehavior.isSafeFromSun = true;
        hitTheGround = true;

    }

    void Update()
    {
        MovingOutOfStartingPoint();
        CalculateVelocity();
        HandleWallSliding();
        SetPlayerBools();
        controller.UpdateMovement(velocity * Time.deltaTime, directionalInput);

        if (controller.collisionInfo.above || controller.collisionInfo.below && controller.collisionInfo.climbing == false)
        {
            velocity.y = 0;
        }

        Climb();

        SetSpawnedInSafePointVariables();   // Added 2019/11/10

    }

    public void HitTheGround()
    {
        hitTheGround = true;
    }

    void SetSpawnedInSafePointVariables()   // Added 2019/11/10
    {
        if (spawnedInSafePoint)
        {
            playerSunBehavior.isDead = false;
            playerSunBehavior.isSafeFromSun = true;
            playerSunBehavior.doneRespawning = false;
            finishedMovingOutCheckPoint = false;
            onLadder = false;
        }
    }

    public void SpawningBoolManager()
    {
        animator.SetBool("Dead", false);
        spawnedInSafePoint = true;
        playerSunBehavior.isDead = false;
        playerSunBehavior.isSafeFromSun = true;
        playerSunBehavior.doneRespawning = false;
        finishedMovingOutCheckPoint = false;
    }

    public void MovingOutOFCheckPointBoolManager()
    {
        if (spawnedInSafePoint)
        {
            spawnedInSafePoint = false;
            playerSunBehavior.isSafeFromSun = false;
            playerSunBehavior.doneRespawning = true;
            finishedMovingOutCheckPoint = true;
            movingOutCheckPoint = false;
        }
        else if (finishedMovingIntoCheckPoint)
        {
            finishedMovingOutCheckPoint = true;
            movingIntoCheckPoint = false;
            movingOutCheckPoint = false;
            playerSunBehavior.isSafeFromSun = false;
            playerSunBehavior.doneRespawning = true;
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    void MovingOutOfStartingPoint()
    {
        if ((movementJoystick.Vertical() != 0 || movementJoystick.Horizontal() != 0) && spawnedInSafePoint && !playerSunBehavior.isDead)
        {
            movingOutCheckPoint = true;
            animator.SetBool("MovingOutofCheckPoint", true);
            audioManager.Play("WalkingIntoSafePoint");
        }
    }

    public void OnJumpInputDown()
    {
        if (controller.collisionInfo.below && !playerSunBehavior.isDead && !spawnedInSafePoint && finishedMovingOutCheckPoint /*!movingOutCheckPoint*/)
        {
            jumping = true;
            velocity.y = jumpVelocity;
            landing = false;
        }
    }

    public void SetPlayerBools()
    {
        // checking if the player is falling
        {
            // Put this into a function.
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
        }

        // checking landing
        {
            // Put this into a function.
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

    public void CanMoveOut()
    {
        finishedMovingIntoCheckPoint = true;
    }

    void SetMovingIntoCheckPointVariables()
    {
        movingIntoCheckPoint = true;
        movingOutCheckPoint = false;
        finishedMovingIntoCheckPoint = false;
        finishedMovingOutCheckPoint = false;
        velocity.x = 0;
        playerSunBehavior.isSafeFromSun = true;
        audioManager.Play("WalkingIntoSafePoint");
    }

    void SetMovingOutOfCheckPointVariables()
    {
        movingIntoCheckPoint = false;
        movingOutCheckPoint = true;
        velocity.x = 0;
        audioManager.Play("WalkingIntoSafePoint");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Through")) && !spawnedInSafePoint && !onLadder)
        {
            hitTheGround = true;
        }
        if (other.gameObject.tag == "MovingPlatform")
        {
            this.gameObject.transform.parent = other.gameObject.transform;
        }
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
        if (other.gameObject.CompareTag("CheckPoint") && onGround && !spawnedInSafePoint && !playerSunBehavior.isDead)
        {
            if ((movementJoystick.Vertical() > 0.4f) && !movingIntoCheckPoint && finishedMovingOutCheckPoint && !playerSunBehavior.isDead)
            {
                SetMovingIntoCheckPointVariables();   
            }
            else if ((movementJoystick.Vertical() != 0 || movementJoystick.Horizontal() != 0) && movingIntoCheckPoint && finishedMovingIntoCheckPoint && !playerSunBehavior.isDead)
            {
                SetMovingOutOfCheckPointVariables();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("LadderBottom"))    // Changed 2019/11/10
        {
            onLadder = false;
            moveOffLadder = true;
            climbingCheck = other.gameObject.GetComponent<ClimbingCheckMobile>();
            climbingCheck.startChecking = false;
        }
        if (other.gameObject.CompareTag("MovingPlatform"))
        {
            this.gameObject.transform.parent = null;
        }
        if (other.gameObject.CompareTag("CheckPoint"))
        {
            movingIntoCheckPoint = false;
            movingOutCheckPoint = false;
        }
        if (other.gameObject.CompareTag("Ground"))
        {
            hitTheGround = false;
        }
    }
}
