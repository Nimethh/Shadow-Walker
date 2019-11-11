using UnityEngine;
using UnityEngine.SceneManagement;

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

    bool wallSliding = false;   // assigned value 2019/11/10
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

    Controller2DUpdated controller;
    Animator animator;
    PlayerSunBehaviorUpdated playerSunBehavior;
    AudioManager audioManager;
    ClimbingCheck climbingCheck;    //Added 19/06/05

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        controller = GetComponent<Controller2DUpdated>();
        playerSunBehavior = GetComponent<PlayerSunBehaviorUpdated>();
        animator = GetComponent<Animator>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpPeak, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpPeak;

        if (SceneManager.GetActiveScene().name != "Level1")
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

        if(spawnedInSafePoint)
        {
            playerSunBehavior.isDead = false;
            playerSunBehavior.isSafeFromSun = true;
            playerSunBehavior.doneRespawning = false;
            finishedMovingOutCheckPoint = false;
            onLadder = false;
        }
    }

    public void HitTheGround()
    {
        hitTheGround = true;
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
            playerSunBehavior.doneRespawning = true;
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    void MovingOutOfStartingPoint()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) ||
             Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || 
             Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) && spawnedInSafePoint && !playerSunBehavior.isDead)
        {
            movingOutCheckPoint = true;
            animator.SetBool("MovingOutofCheckPoint", true);
            audioManager.Play("WalkingIntoSafePoint");
        }
    }

    public void OnJumpInputDown()
    {
        if (controller.collisionInfo.below && !playerSunBehavior.isDead && !spawnedInSafePoint && finishedMovingOutCheckPoint)
        {
            jumping = true;
            velocity.y = jumpVelocity;
            landing = false;
        }
    }

    public void SetPlayerBools()
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

    public void Climb()
    {
        if (controller.collisionInfo.climbing && directionalInput.x == 0 )
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
        if(other.gameObject.CompareTag("CheckPoint") && onGround && !spawnedInSafePoint && !playerSunBehavior.isDead)
        {
            // Moving into the check point.
            if((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && !movingIntoCheckPoint && finishedMovingOutCheckPoint && !playerSunBehavior.isDead)
            {
                SetMovingIntoCheckPointVariables();
            }
            // Moving out of the check point.
            else if( (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)
                   || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) 
                   || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Space) ) 
                && movingIntoCheckPoint && finishedMovingIntoCheckPoint && !playerSunBehavior.isDead )
            {
                SetMovingOutOfCheckPointVariables();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("LadderBottom")) // Added 19/06/05
        {
            onLadder = false;
            moveOffLadder = true;
            climbingCheck = other.gameObject.GetComponent<ClimbingCheck>();
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
        if(other.gameObject.CompareTag("Ground"))
        {
            hitTheGround = false;
        }
    }
}
