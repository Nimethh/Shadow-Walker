using UnityEngine;

public enum PlayerStateLevel1
{
    BLINKING_IN_BED,
    OUT_OF_BED,
    MOVE_TOWARDS_NOTE,
    PICKING_UP_NOTE,
    IDLE,
    MOVING,
    JUMPING,
    FALLING,
    LANDING,
    CLIMBING,
    MOVING_INTO_CHECK_POINT,
    MOVING_OUT_FROM_CHECK_POINT,
    INSIDE_CHECK_POINT,
    TURN,
    TELEPORT,
    DEAD
}

public class PlayerLevel1 : MonoBehaviour
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

    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public Vector2 directionalInput;

    float velocityXSmoothing = 0;
    float gravity = 0;
    float jumpVelocity = 0;

    bool wallSliding = false;
    int wallDirX = 0;

    public float facingDirection;

    /// Newly Added starts.
    public bool isInsideCheck = false;
    public bool doneTurning = true;
    public bool doneLanding = false;

    float moveOffLadderTimer = 0.01f;
    float moveOffLadderCooldown = 0.01f;
    float moveOffLadderHoldTimer = 0.15f;
    float moveOffLadderHoldCooldown = 0.15f;

    public float fallingTimer = 0.0f;

    float top = 0;
    float bottom = 0;
    float right = 0;
    float left = 0;

    public float prevDirX;
    public float currDirX;
    public bool turnAnimRight = false;
    public bool turnAnimLeft = false;

    CollisionHandlerLevel1 collisionHandler;
    PlayerSunBehavior playerSunBehavior;
    Animator animator;
    AudioManager audioManager;

    GameObject note;
    Transform destination;

    public PlayerStateLevel1 playerState = PlayerStateLevel1.BLINKING_IN_BED;

    void Start()
    {
        collisionHandler = GetComponent<CollisionHandlerLevel1>();
        playerSunBehavior = GetComponent<PlayerSunBehavior>();
        animator = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();

        destination = GameObject.Find("Door").transform;
        note = GameObject.Find("Note");

        Cursor.visible = false;
        FindPlayerBounds();
        moveOffLadderCooldown = moveOffLadderTimer;
        moveOffLadderHoldCooldown = moveOffLadderHoldTimer;

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpPeak, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpPeak;

        animator.SetFloat("FacingDirection", 1.0f);
        prevDirX = 1.0f;
        isInsideCheck = false;
    }

    void Update()
    {
        CheckPlayerState();
        UpdatePlayer();
        if (!collisionHandler.collisionInfo.below)
        {
            fallingTimer += Time.deltaTime;
        }
        else
        {
            fallingTimer = 0.0f;
        }
    }

    void CalculateVelocity()
    {
        if (playerState != PlayerStateLevel1.INSIDE_CHECK_POINT)
        {
            float targetVelocityX = directionalInput.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (collisionHandler.collisionInfo.below) ? accelerationTimeOnGroundTurn : accelerationTimeInAirTurn);
        }
        velocity.y += gravity * Time.deltaTime;
    }

    public void Climb()
    {
        if (directionalInput.y > 0)
        {
            velocity.y = climbingSpeed;
            audioManager.Play("Climb");
            animator.speed = 1;
        }
        else if (directionalInput.y < 0)
        {
            velocity.y = -climbingSpeed;
            audioManager.Play("Climb");
            animator.speed = 1;
        }
        else
        {
            velocity.y = 0f;
            audioManager.Stop("Climb");
            animator.speed = 0;
        }
    }

    void HandleWallSliding()
    {
        wallDirX = (collisionHandler.collisionInfo.left) ? -1 : 1;
        wallSliding = false;
        if ((collisionHandler.collisionInfo.left || collisionHandler.collisionInfo.right) && !collisionHandler.collisionInfo.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeed)
            {
                velocity.y = -wallSlideSpeed;
            }
        }
    }

    void UpdatePlayer()
    {
        switch (playerState)
        {
            case PlayerStateLevel1.BLINKING_IN_BED:
                break;

            case PlayerStateLevel1.OUT_OF_BED:
                CalculateVelocity();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                break;

            case PlayerStateLevel1.MOVE_TOWARDS_NOTE:
                Vector3 notePosition = new Vector3(note.transform.position.x, transform.position.y, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, notePosition, Time.deltaTime);
                if (Vector3.Distance(transform.position, notePosition) <= 0.27f)
                {
                    Invoke("MoveToNextState", 0.1f);
                }
                break;

            case PlayerStateLevel1.PICKING_UP_NOTE:
                DeactivateNote();
                break;

            case PlayerStateLevel1.IDLE:
                InputCheck();
                CalculateVelocity();
                audioManager.Stop("Walk");
                animator.speed = 1;
                animator.SetTrigger("Idle");
                break;

            case PlayerStateLevel1.MOVING:
                InputCheck();
                CalculateVelocity();
                HandleWallSliding();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                CheckPlayerBounds();
                audioManager.Play("Walk");
                animator.speed = 1;
                animator.SetTrigger("Moving");
                break;

            case PlayerStateLevel1.JUMPING:
                InputCheck();
                CalculateVelocity();
                HandleWallSliding();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                CheckPlayerBounds();
                audioManager.Stop("Walk");
                animator.speed = 1.5f;
                animator.SetTrigger("Jumping");
                break;

            case PlayerStateLevel1.FALLING:
                InputCheck();
                CalculateVelocity();
                HandleWallSliding();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                CheckPlayerBounds();
                animator.speed = 1.5f;
                animator.SetTrigger("Falling");
                audioManager.Stop("Walk");
                break;

            case PlayerStateLevel1.LANDING:
                InputCheck();
                CalculateVelocity();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                animator.speed = 1.0f;
                animator.SetTrigger("Landing");
                break;

            case PlayerStateLevel1.CLIMBING:
                InputCheck();
                velocity.x = 0.0f;
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                Climb();
                animator.SetTrigger("Climbing");
                break;

            case PlayerStateLevel1.MOVING_INTO_CHECK_POINT:
                playerSunBehavior.isSafeFromSun = true;
                animator.speed = 1;
                animator.SetTrigger("MovingInto");
                audioManager.Stop("Walk");
                break;

            case PlayerStateLevel1.MOVING_OUT_FROM_CHECK_POINT:
                playerSunBehavior.isSafeFromSun = false;
                animator.speed = 1;
                animator.SetTrigger("MovingOutFrom");
                audioManager.Stop("Walk");
                break;

            case PlayerStateLevel1.INSIDE_CHECK_POINT:
                CalculateVelocity();
                velocity.x = 0.0f;
                directionalInput.x = 0.0f;
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                playerSunBehavior.isSafeFromSun = true;
                animator.speed = 1;
                animator.SetTrigger("InsideCheckPoint");
                break;

            case PlayerStateLevel1.TURN:
                InputCheck();
                CalculateVelocity();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                animator.speed = 1.2f;
                animator.SetTrigger("Turning");
                audioManager.Stop("Walk");
                break;

            case PlayerStateLevel1.TELEPORT:
                if (collisionHandler.teleport == true)
                {
                    transform.position = destination.position;
                    playerState = PlayerStateLevel1.INSIDE_CHECK_POINT;
                }
                break;

            case PlayerStateLevel1.DEAD:
                audioManager.Stop("Walk");
                audioManager.Play("Death");
                animator.speed = 1;
                animator.SetTrigger("Death");
                break;

            default:
                break;
        }
        if (collisionHandler.collisionInfo.below)
        {
            velocity.y = 0.0f;
        }
    }

    void ResetAnimationTriggers()
    {
        animator.ResetTrigger("Turning");
        animator.ResetTrigger("InsideCheckPoint");
        animator.ResetTrigger("Death");
        animator.ResetTrigger("MovingOutFrom");
        animator.ResetTrigger("MovingInto");
        animator.ResetTrigger("Climbing");
        animator.ResetTrigger("Landing");
        animator.ResetTrigger("Falling");
        animator.ResetTrigger("Jumping");
        animator.ResetTrigger("Moving");
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("OutOfBed");
        animator.ResetTrigger("MoveTowardsNote");
        animator.ResetTrigger("PickUpNote");
    }

    void CheckPlayerState()
    {
        switch (playerState)
        {
            case PlayerStateLevel1.BLINKING_IN_BED:
                Invoke("MoveToNextState", 2.0f);
                break;

            case PlayerStateLevel1.OUT_OF_BED:
                animator.SetTrigger("OutOfBed");
                Invoke("MoveToNextState", 1.0f);
                break;

            case PlayerStateLevel1.MOVE_TOWARDS_NOTE:
                animator.SetTrigger("MoveTowardsNote");
                break;

            case PlayerStateLevel1.PICKING_UP_NOTE:
                animator.SetTrigger("PickUpNote");
                Invoke("MoveToNextState", 2.5f);
                break;

            // INSIDE_CHECK_POINT                                       1
            case PlayerStateLevel1.INSIDE_CHECK_POINT:
                if (collisionHandler.teleport == true)
                {
                    playerState = PlayerStateLevel1.TELEPORT;
                    ResetAnimationTriggers();
                }
                if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.J) && !Input.GetKeyDown(KeyCode.L))
                {
                    playerState = PlayerStateLevel1.MOVING_OUT_FROM_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                break;

            // MOVING_INTO_CHECK_POINT:                                 2
            case PlayerStateLevel1.MOVING_INTO_CHECK_POINT:
                if (isInsideCheck == true)
                {
                    playerState = PlayerStateLevel1.INSIDE_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                break;

            // MOVING_OUT_FROM_CHECK_POINT                              3
            case PlayerStateLevel1.MOVING_OUT_FROM_CHECK_POINT:
                if (isInsideCheck == false)
                {
                    playerState = PlayerStateLevel1.IDLE;
                    ResetAnimationTriggers();
                }
                break;

            // DEAD                                                     4
            case PlayerStateLevel1.DEAD:
                Invoke("RespawnAterDeath", 1.5f);
                break;

            // MOVING                                                   5
            case PlayerStateLevel1.MOVING:
                if (playerSunBehavior.isDead == true)
                {
                    playerState = PlayerStateLevel1.DEAD;
                    ResetAnimationTriggers();
                }
                else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && collisionHandler.collisionInfo.checkPointNearby == true
                    && collisionHandler.collisionInfo.below && playerState != PlayerStateLevel1.INSIDE_CHECK_POINT)
                {
                    playerState = PlayerStateLevel1.MOVING_INTO_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && collisionHandler.collisionInfo.ladderNearby == true)
                {
                    collisionHandler.collisionInfo.moveOffLadder = false;
                    collisionHandler.collisionInfo.climbingLadder = true;
                    playerState = PlayerStateLevel1.CLIMBING;
                    ResetAnimationTriggers();
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    velocity.y = jumpVelocity;
                    playerState = PlayerStateLevel1.JUMPING;
                    audioManager.Play("Jump");
                    ResetAnimationTriggers();
                }
                else if (velocity.y < 0 && !collisionHandler.collisionInfo.below && fallingTimer >= 0.2f /*&& !collisionHandler.collisionInfo.ladderNearby*/)
                {
                    playerState = PlayerStateLevel1.FALLING;
                    ResetAnimationTriggers();
                }
                else if (turnAnimLeft || turnAnimRight)
                {
                    playerState = PlayerStateLevel1.TURN;
                    ResetAnimationTriggers();
                }
                else if (Input.GetAxisRaw("Horizontal") == 0)
                {
                    playerState = PlayerStateLevel1.IDLE;
                    ResetAnimationTriggers();
                }
                break;

            // FALLING                                                  6
            case PlayerStateLevel1.FALLING:
                doneLanding = false;
                if (collisionHandler.collisionInfo.below && !collisionHandler.collisionInfo.ladderNearby)
                {
                    playerState = PlayerStateLevel1.LANDING;
                    audioManager.Play("Land");
                    ResetAnimationTriggers();
                }
                else if (collisionHandler.collisionInfo.below && collisionHandler.collisionInfo.ladderNearby)
                {
                    playerState = PlayerStateLevel1.IDLE;
                    ResetAnimationTriggers();
                }
                break;

            // LANDING                                                  7
            case PlayerStateLevel1.LANDING:
                Invoke("DoneLanding", 0.2f);
                if (playerSunBehavior.isDead == true)
                {
                    playerState = PlayerStateLevel1.DEAD;
                    ResetAnimationTriggers();
                }

                else if (Input.GetAxisRaw("Horizontal") != 0.0f && doneLanding)
                {
                    CancelInvoke();
                    playerState = PlayerStateLevel1.MOVING;
                    ResetAnimationTriggers();
                }
                else if (Input.GetAxisRaw("Horizontal") == 0.0f && doneLanding)
                {
                    CancelInvoke();
                    playerState = PlayerStateLevel1.IDLE;
                    ResetAnimationTriggers();
                }
                break;

            // TURN                                                     8
            case PlayerStateLevel1.TURN:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    CancelInvoke();
                    turnAnimLeft = false;
                    turnAnimRight = false;
                    doneTurning = false;
                    velocity.y = jumpVelocity;
                    playerState = PlayerStateLevel1.JUMPING;
                    audioManager.Play("Jump");
                    ResetAnimationTriggers();
                }
                else if (doneTurning == true && Input.GetAxisRaw("Horizontal") == 0.0f)
                {
                    CancelInvoke();
                    turnAnimLeft = false;
                    turnAnimRight = false;
                    doneTurning = false;
                    playerState = PlayerStateLevel1.IDLE;
                    ResetAnimationTriggers();
                }
                else if (doneTurning == true && Input.GetAxisRaw("Horizontal") != 0.0f)
                {
                    CancelInvoke();
                    turnAnimLeft = false;
                    turnAnimRight = false;
                    doneTurning = false;
                    playerState = PlayerStateLevel1.MOVING;
                    ResetAnimationTriggers();
                }
                break;

            // JUMP                                                     9
            case PlayerStateLevel1.JUMPING:
                if (collisionHandler.collisionInfo.above)
                {
                    velocity.y = 0;
                }
                if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && collisionHandler.collisionInfo.ladderNearby == true)
                {
                    collisionHandler.collisionInfo.moveOffLadder = false;
                    collisionHandler.collisionInfo.climbingLadder = true;
                    playerState = PlayerStateLevel1.CLIMBING;
                    ResetAnimationTriggers();
                }
                else if (velocity.y < 0.0f && collisionHandler.collisionInfo.below)
                {
                    playerState = PlayerStateLevel1.LANDING;
                    ResetAnimationTriggers();
                }
                else if (velocity.y < 0.0f && fallingTimer >= 0.2f)
                {
                    playerState = PlayerStateLevel1.FALLING;
                    ResetAnimationTriggers();
                }
                break;

            // IDLE                                                     10
            case PlayerStateLevel1.IDLE:
                if (playerSunBehavior.isDead == true)
                {
                    playerState = PlayerStateLevel1.DEAD;
                    ResetAnimationTriggers();
                }
                else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && collisionHandler.collisionInfo.checkPointNearby == true
                    && collisionHandler.collisionInfo.below && playerState != PlayerStateLevel1.INSIDE_CHECK_POINT)
                {
                    playerState = PlayerStateLevel1.MOVING_INTO_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && collisionHandler.collisionInfo.ladderNearby == true)
                {
                    collisionHandler.collisionInfo.moveOffLadder = false;
                    collisionHandler.collisionInfo.climbingLadder = true;
                    playerState = PlayerStateLevel1.CLIMBING;
                    ResetAnimationTriggers();
                }
                else if (turnAnimLeft || turnAnimRight)
                {
                    playerState = PlayerStateLevel1.TURN;
                    ResetAnimationTriggers();
                }
                else if (directionalInput.x != 0)
                {
                    playerState = PlayerStateLevel1.MOVING;
                    ResetAnimationTriggers();
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    velocity.y = jumpVelocity;
                    playerState = PlayerStateLevel1.JUMPING;
                    audioManager.Play("Jump");
                    ResetAnimationTriggers();
                }
                else if (velocity.y < 0 && !collisionHandler.collisionInfo.below && fallingTimer >= 0.2f /* && !collisionHandler.collisionInfo.ladderNearby*/)
                {
                    playerState = PlayerStateLevel1.FALLING;
                    ResetAnimationTriggers();
                }
                break;

            // CLIMB                                                    11
            case PlayerStateLevel1.CLIMBING:
                if (playerSunBehavior.isDead == true)
                {
                    playerState = PlayerStateLevel1.FALLING;
                    ResetAnimationTriggers();
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    playerState = PlayerStateLevel1.JUMPING;
                    audioManager.Play("Jump");
                    ResetAnimationTriggers();
                }
                else if (collisionHandler.collisionInfo.climbingLadder == false)
                {
                    playerState = PlayerStateLevel1.IDLE;
                    ResetAnimationTriggers();
                }
                if (directionalInput.y > 0 || directionalInput.y < 0)   //  Need to be changed. moving off the ladder.
                {
                    moveOffLadderCooldown = moveOffLadderTimer;
                    moveOffLadderHoldCooldown = moveOffLadderHoldTimer;
                }
                else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow)))
                {
                    if (moveOffLadderHoldCooldown <= 0)
                    {
                        collisionHandler.collisionInfo.moveOffLadder = true;
                        playerState = PlayerStateLevel1.FALLING;
                        ResetAnimationTriggers();
                    }
                    else
                    {
                        moveOffLadderHoldCooldown -= Time.deltaTime;
                    }
                }
                if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A) ||
                     Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow)))
                {
                    if (moveOffLadderCooldown <= 0)
                    {
                        collisionHandler.collisionInfo.moveOffLadder = true;
                        collisionHandler.checkingCollisionCooldown = collisionHandler.checkingCollisionTimer;
                        playerState = PlayerStateLevel1.FALLING;
                        ResetAnimationTriggers();
                    }
                    else
                    {
                        moveOffLadderCooldown -= Time.deltaTime;
                    }
                }
                break;
            default:
                Debug.LogError("Default is called in CheckPlayerState() !");
                playerState = PlayerStateLevel1.IDLE;
                ResetAnimationTriggers();
                break;
        }
    }

    void RespawnAterDeath()
    {
        playerSunBehavior.isDead = false;
        isInsideCheck = true;
        playerState = PlayerStateLevel1.INSIDE_CHECK_POINT;
        ResetAnimationTriggers();
        animator.SetTrigger("InsideCheckPoint");
        transform.position = collisionHandler.spawningPosition;
        directionalInput = Vector2.zero;
        velocity.x = 0.0f;
        CancelInvoke();
    }

    void IsInsideCheckPoint()
    {
        isInsideCheck = !isInsideCheck;
    }

    void DoneTurning()
    {
        doneTurning = true;
    }

    void MoveToNextState()
    {
        ResetAnimationTriggers();
        CancelInvoke();
        if (playerState == PlayerStateLevel1.PICKING_UP_NOTE)
        {
            ActivateNote();
        }
        playerState++;
    }

    void DoneLanding()
    {
        doneLanding = true;
    }

    void DeactivateNote()
    {
        note.SetActive(false);
    }

    void ActivateNote()
    {
        note.SetActive(true);
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
        if (transform.position.x > right - 0.2f)
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
            velocity.y = 0;
            playerSunBehavior.isDead = true;
        }

    }

    void InputCheck()
    {
        if (playerState != PlayerStateLevel1.CLIMBING)
        {
            directionalInput.x = Input.GetAxisRaw("Horizontal");
            animator.SetFloat("MovingDirection", directionalInput.x);
            currDirX = directionalInput.x;
        }

        if (playerState == PlayerStateLevel1.MOVING || playerState == PlayerStateLevel1.IDLE)
        {
            if (currDirX > 0 && prevDirX < 0)
            {
                doneTurning = false;
                turnAnimRight = true;
                prevDirX = currDirX;
                animator.SetFloat("TurnDirection", 1f);
                Invoke("DoneTurning", 0.2f);
            }
            else if (currDirX < 0 && prevDirX > 0)
            {
                doneTurning = false;
                turnAnimLeft = true;
                prevDirX = currDirX;
                animator.SetFloat("TurnDirection", -1f);
                Invoke("DoneTurning", 0.2f);
            }
        }
        else
        {
            if (directionalInput.x != 0)
            {
                prevDirX = currDirX;
            }
        }

        if (directionalInput.x != 0)
        {
            facingDirection = directionalInput.x;
            animator.SetFloat("FacingDirection", facingDirection);
        }

        directionalInput.y = Input.GetAxisRaw("Vertical");
    }
}
