using UnityEngine;

public enum PlayerState
{
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
    DEAD
}

public class Player : MonoBehaviour
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
    public bool isInsideCheck = true;
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

    CollisionHandler collisionHandler;
    PlayerSunBehavior playerSunBehavior;
    Animator animator;
    AudioManager audioManager;

    public PlayerState playerState = PlayerState.INSIDE_CHECK_POINT;

    void Start()
    {
        collisionHandler = GetComponent<CollisionHandler>();
        playerSunBehavior = GetComponent<PlayerSunBehavior>();
        animator = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();

        Cursor.visible = false;
        FindPlayerBounds();
        moveOffLadderCooldown = moveOffLadderTimer;
        moveOffLadderHoldCooldown = moveOffLadderHoldTimer;

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpPeak, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpPeak;

        animator.SetFloat("FacingDirection", 1.0f);
        prevDirX = 1.0f;
    }

    void Update()
    {
        CheckPlayerState();
        UpdatePlayer();
        if(!collisionHandler.collisionInfo.below)
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
        if (playerState != PlayerState.INSIDE_CHECK_POINT)
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
            case PlayerState.IDLE:
                InputCheck();
                CalculateVelocity();
                audioManager.Stop("Walk");
                animator.speed = 1;
                animator.SetTrigger("Idle");
                break;

            case PlayerState.MOVING:
                InputCheck();
                CalculateVelocity();
                HandleWallSliding();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                CheckPlayerBounds();
                audioManager.Play("Walk");
                animator.speed = 1;
                animator.SetTrigger("Moving");
                break;

            case PlayerState.JUMPING:
                InputCheck();
                CalculateVelocity();
                HandleWallSliding();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                CheckPlayerBounds();
                audioManager.Stop("Walk");
                animator.speed = 1.5f;
                animator.SetTrigger("Jumping");
                break;

            case PlayerState.FALLING:
                InputCheck();
                CalculateVelocity();
                HandleWallSliding();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                CheckPlayerBounds();
                animator.speed = 1.5f;
                animator.SetTrigger("Falling");
                audioManager.Stop("Walk");
                break;

            case PlayerState.LANDING:
                InputCheck();
                CalculateVelocity();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                animator.speed = 1.0f;
                animator.SetTrigger("Landing");
                break;

            case PlayerState.CLIMBING:
                InputCheck();
                velocity.x = 0.0f;
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                Climb();
                animator.SetTrigger("Climbing");
                break;

            case PlayerState.MOVING_INTO_CHECK_POINT:
                playerSunBehavior.isSafeFromSun = true;
                animator.speed = 1;
                animator.SetTrigger("MovingInto");
                audioManager.Stop("Walk");
                break;

            case PlayerState.MOVING_OUT_FROM_CHECK_POINT:
                playerSunBehavior.isSafeFromSun = false;
                animator.speed = 1;
                animator.SetTrigger("MovingOutFrom");
                audioManager.Stop("Walk");
                break;

            case PlayerState.INSIDE_CHECK_POINT:
                CalculateVelocity();
                velocity.x = 0.0f;
                directionalInput.x = 0.0f;
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                playerSunBehavior.isSafeFromSun = true;
                animator.speed = 1;
                animator.SetTrigger("InsideCheckPoint");
                break;

            case PlayerState.TURN:
                InputCheck();
                CalculateVelocity();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                animator.speed = 1.2f;
                animator.SetTrigger("Turning");
                audioManager.Stop("Walk");
                break;

            case PlayerState.DEAD:
                audioManager.Stop("Walk");
                audioManager.Play("Death");
                animator.speed = 1;
                animator.SetTrigger("Death");
                break;
            default:
                break;
        }
        if(collisionHandler.collisionInfo.below)
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
    }

    void CheckPlayerState()
    {
        switch(playerState)
        {
            // INSIDE_CHECK_POINT                                       1
            case PlayerState.INSIDE_CHECK_POINT:
                if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.J) && !Input.GetKeyDown(KeyCode.L))
                {
                    playerState = PlayerState.MOVING_OUT_FROM_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                break;

            // MOVING_INTO_CHECK_POINT:                                 2
            case PlayerState.MOVING_INTO_CHECK_POINT:
                if (isInsideCheck == true)
                {
                    playerState = PlayerState.INSIDE_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                break;

            // MOVING_OUT_FROM_CHECK_POINT                              3
            case PlayerState.MOVING_OUT_FROM_CHECK_POINT:
                if(isInsideCheck == false)
                {
                    playerState = PlayerState.IDLE;
                    ResetAnimationTriggers();
                }
                break;

            // DEAD                                                     4
            case PlayerState.DEAD:
                Invoke("RespawnAterDeath", 1.5f);
                break;

            // MOVING                                                   5
            case PlayerState.MOVING:
                if(playerSunBehavior.isDead == true)
                {
                    playerState = PlayerState.DEAD;
                    ResetAnimationTriggers();
                }
                else if((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && collisionHandler.collisionInfo.checkPointNearby == true 
                    && collisionHandler.collisionInfo.below && playerState != PlayerState.INSIDE_CHECK_POINT)
                {
                    playerState = PlayerState.MOVING_INTO_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                else if((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && collisionHandler.collisionInfo.ladderNearby == true)
                {
                    collisionHandler.collisionInfo.climbingLadder = true;
                    playerState = PlayerState.CLIMBING;
                    ResetAnimationTriggers();
                }
                else if(Input.GetKeyDown(KeyCode.Space))
                {
                    velocity.y = jumpVelocity;
                    playerState = PlayerState.JUMPING;
                    audioManager.Play("Jump");
                    ResetAnimationTriggers();
                }
                else if(velocity.y < 0 && !collisionHandler.collisionInfo.below && fallingTimer >= 0.2f /*&& !collisionHandler.collisionInfo.ladderNearby*/)
                {
                    playerState = PlayerState.FALLING;
                    ResetAnimationTriggers();
                }
                else if (turnAnimLeft || turnAnimRight)
                {
                    playerState = PlayerState.TURN;
                    ResetAnimationTriggers();
                }
                else if (Input.GetAxisRaw("Horizontal") == 0)
                {
                    playerState = PlayerState.IDLE;
                    ResetAnimationTriggers();
                }
                break;

            // FALLING                                                  6
            case PlayerState.FALLING:
                doneLanding = false;
                if (collisionHandler.collisionInfo.below && !collisionHandler.collisionInfo.ladderNearby)
                {
                    playerState = PlayerState.LANDING;
                    audioManager.Play("Land");
                    ResetAnimationTriggers();
                }
                else if(collisionHandler.collisionInfo.below && collisionHandler.collisionInfo.ladderNearby)
                {
                    playerState = PlayerState.IDLE;
                    ResetAnimationTriggers();
                }
                break;

            // LANDING                                                  7
            case PlayerState.LANDING:
                Invoke("DoneLanding", 0.2f);
                if (playerSunBehavior.isDead == true)
                {
                    playerState = PlayerState.DEAD;
                    ResetAnimationTriggers();
                }
                else if (Input.GetAxisRaw("Horizontal") != 0.0f && doneLanding)
                {
                    CancelInvoke();
                    playerState = PlayerState.MOVING;
                    ResetAnimationTriggers();
                }
                else if(Input.GetAxisRaw("Horizontal") == 0.0f && doneLanding)
                {
                    CancelInvoke();
                    playerState = PlayerState.IDLE;
                    ResetAnimationTriggers();
                }
                break;

            // TURN                                                     8
            case PlayerState.TURN:
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    CancelInvoke();
                    
                    turnAnimLeft = false;
                    turnAnimRight = false;
                    doneTurning = false;
                    velocity.y = jumpVelocity;
                    playerState = PlayerState.JUMPING;
                    audioManager.Play("Jump");
                    ResetAnimationTriggers();
                }
                else if(doneTurning == true && Input.GetAxisRaw("Horizontal") == 0.0f)
                {
                    CancelInvoke();
                    turnAnimLeft = false;
                    turnAnimRight = false;
                    doneTurning = false;
                    playerState = PlayerState.IDLE;
                    ResetAnimationTriggers();
                }
                else if(doneTurning == true && Input.GetAxisRaw("Horizontal") != 0.0f)
                {
                    CancelInvoke();
                    turnAnimLeft = false;
                    turnAnimRight = false;
                    doneTurning = false;
                    playerState = PlayerState.MOVING;
                    ResetAnimationTriggers();
                }
                break;

            // JUMP                                                     9
            case PlayerState.JUMPING:
                if(collisionHandler.collisionInfo.above)
                {
                    velocity.y = 0;
                }
                if((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && collisionHandler.collisionInfo.ladderNearby == true)
                {
                    collisionHandler.collisionInfo.climbingLadder = true;
                    playerState = PlayerState.CLIMBING;
                    ResetAnimationTriggers();
                }
                else if(collisionHandler.collisionInfo.below)
                {
                    playerState = PlayerState.IDLE;
                    ResetAnimationTriggers();
                }
                else if(velocity.y < 0.0f && fallingTimer >= 0.2f)
                {
                    playerState = PlayerState.FALLING;
                    ResetAnimationTriggers();
                }
                break;

            // IDLE                                                     10
            case PlayerState.IDLE:
                if(playerSunBehavior.isDead == true)
                {
                    playerState = PlayerState.DEAD;
                    ResetAnimationTriggers();
                }
                else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && collisionHandler.collisionInfo.checkPointNearby == true
                    && collisionHandler.collisionInfo.below && playerState != PlayerState.INSIDE_CHECK_POINT)
                {
                    playerState = PlayerState.MOVING_INTO_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && collisionHandler.collisionInfo.ladderNearby == true)
                {
                    collisionHandler.collisionInfo.climbingLadder = true;
                    playerState = PlayerState.CLIMBING;
                    ResetAnimationTriggers();
                }
                else if (turnAnimLeft || turnAnimRight)
                {
                    playerState = PlayerState.TURN;
                    ResetAnimationTriggers();
                }
                else if(directionalInput.x != 0)
                {
                    playerState = PlayerState.MOVING;
                    ResetAnimationTriggers();
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    velocity.y = jumpVelocity;
                    playerState = PlayerState.JUMPING;
                    audioManager.Play("Jump");
                    ResetAnimationTriggers();
                }
                else if (velocity.y < 0 && !collisionHandler.collisionInfo.below && fallingTimer >= 0.2f)
                {
                    playerState = PlayerState.FALLING;
                    ResetAnimationTriggers();
                }
                break;

            // CLIMB                                                    11
            case PlayerState.CLIMBING:
                if(playerSunBehavior.isDead == true)
                {
                    playerState = PlayerState.FALLING;
                    ResetAnimationTriggers();
                }
                else if(Input.GetKeyDown(KeyCode.Space))
                {
                    playerState = PlayerState.JUMPING;
                    audioManager.Play("Jump");
                    ResetAnimationTriggers();
                }
                else if(collisionHandler.collisionInfo.climbingLadder == false)
                {
                    playerState = PlayerState.IDLE;
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
                        playerState = PlayerState.FALLING;
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
                        playerState = PlayerState.FALLING;
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
                playerState = PlayerState.IDLE;
                ResetAnimationTriggers();
                break;
        }
    }

    void RespawnAterDeath()
    {
        playerSunBehavior.isDead = false;
        isInsideCheck = true;
        playerState = PlayerState.INSIDE_CHECK_POINT;
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

    void DoneLanding()
    {
        doneLanding = true;
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
            velocity.y = 0;
            playerSunBehavior.isDead = true;
        }

    }

    void InputCheck()
    {
        if (playerState != PlayerState.CLIMBING)
        {
            directionalInput.x = Input.GetAxisRaw("Horizontal");
            animator.SetFloat("MovingDirection", directionalInput.x);
            currDirX = directionalInput.x;
        }

        if (playerState == PlayerState.MOVING || playerState == PlayerState.IDLE)
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
            if (currDirX != 0)
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
