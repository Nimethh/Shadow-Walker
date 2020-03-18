using UnityEngine;

public enum PlayerStateMobile
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

public class PlayerMobile : MonoBehaviour
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

    public PlayerStateMobile playerState = PlayerStateMobile.INSIDE_CHECK_POINT;

    [SerializeField]
    LayerMask jumpLayer;
    Camera camera;

    void Start()
    {
        camera = Camera.main;

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
        if (playerState != PlayerStateMobile.INSIDE_CHECK_POINT)
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
            case PlayerStateMobile.IDLE:
                InputCheck();
                CalculateVelocity();
                audioManager.Stop("Walk");
                animator.speed = 1;
                animator.SetTrigger("Idle");
                break;

            case PlayerStateMobile.MOVING:
                InputCheck();
                CalculateVelocity();
                HandleWallSliding();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                CheckPlayerBounds();
                audioManager.Play("Walk");
                animator.speed = 1;
                animator.SetTrigger("Moving");
                break;

            case PlayerStateMobile.JUMPING:
                InputCheck();
                CalculateVelocity();
                HandleWallSliding();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                CheckPlayerBounds();
                audioManager.Stop("Walk");
                animator.speed = 1.5f;
                animator.SetTrigger("Jumping");
                break;

            case PlayerStateMobile.FALLING:
                InputCheck();
                CalculateVelocity();
                HandleWallSliding();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                CheckPlayerBounds();
                animator.speed = 1.5f;
                animator.SetTrigger("Falling");
                audioManager.Stop("Walk");
                break;

            case PlayerStateMobile.LANDING:
                InputCheck();
                CalculateVelocity();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                animator.speed = 1.0f;
                animator.SetTrigger("Landing");
                break;

            case PlayerStateMobile.CLIMBING:
                InputCheck();
                velocity.x = 0.0f;
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                Climb();
                animator.SetTrigger("Climbing");
                break;

            case PlayerStateMobile.MOVING_INTO_CHECK_POINT:
                playerSunBehavior.isSafeFromSun = true;
                animator.speed = 1;
                animator.SetTrigger("MovingInto");
                audioManager.Stop("Walk");
                break;

            case PlayerStateMobile.MOVING_OUT_FROM_CHECK_POINT:
                playerSunBehavior.isSafeFromSun = false;
                animator.speed = 1;
                animator.SetTrigger("MovingOutFrom");
                audioManager.Stop("Walk");
                break;

            case PlayerStateMobile.INSIDE_CHECK_POINT:
                InputCheck();
                CalculateVelocity();
                velocity.x = 0.0f;
                directionalInput.x = 0.0f;
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                playerSunBehavior.isSafeFromSun = true;
                animator.speed = 1;
                animator.SetTrigger("InsideCheckPoint");
                break;

            case PlayerStateMobile.TURN:
                InputCheck();
                CalculateVelocity();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                animator.speed = 1.2f;
                animator.SetTrigger("Turning");
                audioManager.Stop("Walk");
                break;

            case PlayerStateMobile.DEAD:
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
    }

    void CheckPlayerState()
    {
        switch (playerState)
        {
            // INSIDE_CHECK_POINT                                       1
            case PlayerStateMobile.INSIDE_CHECK_POINT:
                if(directionalInput.x != 0 || directionalInput.y != 0 || jumpCheck())
                {
                    playerState = PlayerStateMobile.MOVING_OUT_FROM_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                break;

            // MOVING_INTO_CHECK_POINT:                                 2
            case PlayerStateMobile.MOVING_INTO_CHECK_POINT:
                if (isInsideCheck == true)
                {
                    playerState = PlayerStateMobile.INSIDE_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                break;

            // MOVING_OUT_FROM_CHECK_POINT                              3
            case PlayerStateMobile.MOVING_OUT_FROM_CHECK_POINT:
                if (isInsideCheck == false)
                {
                    playerState = PlayerStateMobile.IDLE;
                    ResetAnimationTriggers();
                }
                break;

            // DEAD                                                     4
            case PlayerStateMobile.DEAD:
                Invoke("RespawnAterDeath", 1.5f);
                break;

            // MOVING                                                   5
            case PlayerStateMobile.MOVING:
                if (playerSunBehavior.isDead == true)
                {
                    playerState = PlayerStateMobile.DEAD;
                    ResetAnimationTriggers();
                }
                else if (directionalInput.y != 0 && collisionHandler.collisionInfo.checkPointNearby == true
                    && collisionHandler.collisionInfo.below && playerState != PlayerStateMobile.INSIDE_CHECK_POINT)
                {
                    playerState = PlayerStateMobile.MOVING_INTO_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                else if (directionalInput.y != 0 && collisionHandler.collisionInfo.ladderNearby == true)
                {
                    //collisionHandler.collisionInfo.climbingLadder = true;
                    playerState = PlayerStateMobile.CLIMBING;
                    ResetAnimationTriggers();
                }
                else if (jumpCheck() && collisionHandler.collisionInfo.below)
                {
                    velocity.y = jumpVelocity;
                    playerState = PlayerStateMobile.JUMPING;
                    audioManager.Play("Jump");
                    ResetAnimationTriggers();
                }
                else if (velocity.y < 0 && !collisionHandler.collisionInfo.below && fallingTimer >= 0.2f)
                {
                    playerState = PlayerStateMobile.FALLING;
                    ResetAnimationTriggers();
                }
                else if (turnAnimLeft || turnAnimRight)
                {
                    playerState = PlayerStateMobile.TURN;
                    ResetAnimationTriggers();
                }
                else if (directionalInput.x == 0)
                {
                    playerState = PlayerStateMobile.IDLE;
                    ResetAnimationTriggers();
                }
                break;

            // FALLING                                                  6
            case PlayerStateMobile.FALLING:
                doneLanding = false;
                if (collisionHandler.collisionInfo.below && !collisionHandler.collisionInfo.ladderNearby)
                {
                    playerState = PlayerStateMobile.LANDING;
                    audioManager.Play("Land");
                    ResetAnimationTriggers();
                }
                else if (collisionHandler.collisionInfo.below && collisionHandler.collisionInfo.ladderNearby)
                {
                    playerState = PlayerStateMobile.IDLE;
                    ResetAnimationTriggers();
                }
                break;

            // LANDING                                                  7
            case PlayerStateMobile.LANDING:
                Invoke("DoneLanding", 0.2f);
                if (playerSunBehavior.isDead == true)
                {
                    playerState = PlayerStateMobile.DEAD;
                    ResetAnimationTriggers();
                }
                else if (jumpCheck() && collisionHandler.collisionInfo.below)
                {
                    velocity.y = jumpVelocity;
                    playerState = PlayerStateMobile.JUMPING;
                    audioManager.Play("Jump");
                    ResetAnimationTriggers();
                }
                else if (directionalInput.x != 0.0f && doneLanding)
                {
                    CancelInvoke();
                    playerState = PlayerStateMobile.MOVING;
                    ResetAnimationTriggers();
                }
                else if (directionalInput.x == 0.0f && doneLanding)
                {
                    CancelInvoke();
                    playerState = PlayerStateMobile.IDLE;
                    ResetAnimationTriggers();
                }
                break;

            // TURN                                                     8
            case PlayerStateMobile.TURN:
                if (jumpCheck() && collisionHandler.collisionInfo.below)
                {
                    CancelInvoke();

                    turnAnimLeft = false;
                    turnAnimRight = false;
                    doneTurning = false;
                    velocity.y = jumpVelocity;
                    playerState = PlayerStateMobile.JUMPING;
                    audioManager.Play("Jump");
                    ResetAnimationTriggers();
                }
                else if (doneTurning == true && directionalInput.x == 0.0f)
                {
                    CancelInvoke();
                    turnAnimLeft = false;
                    turnAnimRight = false;
                    doneTurning = false;
                    playerState = PlayerStateMobile.IDLE;
                    ResetAnimationTriggers();
                    animator.SetFloat("FacingDirection", facingDirection);
                }
                else if (doneTurning == true && directionalInput.x != 0)
                {
                    CancelInvoke();
                    turnAnimLeft = false;
                    turnAnimRight = false;
                    doneTurning = false;
                    playerState = PlayerStateMobile.MOVING;
                    ResetAnimationTriggers();
                    animator.SetFloat("FacingDirection", facingDirection);
                }
                break;

            // JUMP                                                     9
            case PlayerStateMobile.JUMPING:
                if (collisionHandler.collisionInfo.above)
                {
                    velocity.y = 0;
                }
                if (directionalInput.y != 0 && collisionHandler.collisionInfo.ladderNearby == true)
                {
                    //collisionHandler.collisionInfo.climbingLadder = true;
                    playerState = PlayerStateMobile.CLIMBING;
                    ResetAnimationTriggers();
                }
                else if (collisionHandler.collisionInfo.below)
                {
                    playerState = PlayerStateMobile.IDLE;
                    ResetAnimationTriggers();
                }
                else if (velocity.y < 0.0f && fallingTimer >= 0.2f)
                {
                    playerState = PlayerStateMobile.FALLING;
                    ResetAnimationTriggers();
                }
                break;

            // IDLE                                                     10
            case PlayerStateMobile.IDLE:
                if (playerSunBehavior.isDead == true)
                {
                    playerState = PlayerStateMobile.DEAD;
                    ResetAnimationTriggers();
                }
                else if (directionalInput.y != 0 && collisionHandler.collisionInfo.checkPointNearby == true
                    && collisionHandler.collisionInfo.below && playerState != PlayerStateMobile.INSIDE_CHECK_POINT)
                {
                    playerState = PlayerStateMobile.MOVING_INTO_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                else if (directionalInput.y != 0 && collisionHandler.collisionInfo.ladderNearby == true)
                {
                    //collisionHandler.collisionInfo.climbingLadder = true;
                    playerState = PlayerStateMobile.CLIMBING;
                    ResetAnimationTriggers();
                }
                else if (turnAnimLeft || turnAnimRight)
                {
                    playerState = PlayerStateMobile.TURN;
                    ResetAnimationTriggers();
                }
                else if (directionalInput.x != 0)
                {
                    playerState = PlayerStateMobile.MOVING;
                    ResetAnimationTriggers();
                }
                else if (jumpCheck() && collisionHandler.collisionInfo.below)
                {
                    velocity.y = jumpVelocity;
                    playerState = PlayerStateMobile.JUMPING;
                    audioManager.Play("Jump");
                    ResetAnimationTriggers();
                }
                else if (velocity.y < 0 && !collisionHandler.collisionInfo.below && fallingTimer >= 0.2f)
                {
                    playerState = PlayerStateMobile.FALLING;
                    ResetAnimationTriggers();
                }
                break;

            // CLIMB                                                    11
            case PlayerStateMobile.CLIMBING:
                if (playerSunBehavior.isDead == true)
                {
                    playerState = PlayerStateMobile.FALLING;
                    ResetAnimationTriggers();
                }
                else if (jumpCheck())
                {
                    playerState = PlayerStateMobile.JUMPING;
                    audioManager.Play("Jump");
                    ResetAnimationTriggers();
                }
                //else if (collisionHandler.collisionInfo.climbingLadder == false)
                {
                    playerState = PlayerStateMobile.IDLE;
                    ResetAnimationTriggers();
                }
                if (directionalInput.y > 0 || directionalInput.y < 0)   //  Need to be changed. moving off the ladder.
                {
                    moveOffLadderCooldown = moveOffLadderTimer;
                    moveOffLadderHoldCooldown = moveOffLadderHoldTimer;
                }

                else if (directionalInput.x != 0)
                {
                    if (moveOffLadderHoldCooldown <= 0)
                    {
                        collisionHandler.collisionInfo.moveOffLadder = true;
                        playerState = PlayerStateMobile.FALLING;
                        ResetAnimationTriggers();
                    }
                    else
                    {
                        moveOffLadderHoldCooldown -= Time.deltaTime;
                    }
                }
                if (directionalInput.x != 0)
                {
                    if (moveOffLadderCooldown <= 0)
                    {
                        collisionHandler.collisionInfo.moveOffLadder = true;
                        collisionHandler.checkingCollisionCooldown = collisionHandler.checkingCollisionTimer;
                        playerState = PlayerStateMobile.FALLING;
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
                playerState = PlayerStateMobile.IDLE;
                ResetAnimationTriggers();
                break;
        }
    }

    void RespawnAterDeath()
    {
        playerSunBehavior.isDead = false;
        isInsideCheck = true;
        playerState = PlayerStateMobile.INSIDE_CHECK_POINT;
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

    bool jumpCheck()
    {
        for (int i = 0; i < Input.touchCount; ++i)
        {
            Touch currentTouch = Input.GetTouch(i);
            if (currentTouch.phase == TouchPhase.Began)
            {
                Ray ray = camera.ScreenPointToRay(Input.GetTouch(i).position);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector3.forward, 10.0f, jumpLayer);
                if(hit)
                {
                    Debug.Log("True");
                    return true;
                }
            }
        }
        return false;
    }

    public void SetHorizontalInput(float p_x)
    {
        directionalInput.x = p_x;
    }

    public void SetVerticalInput(float p_y)
    {
        directionalInput.y = p_y;
    }

    public void ResetInput()
    {
        directionalInput = Vector2.zero;
    }

    void InputCheck()
    {
        if (playerState != PlayerStateMobile.CLIMBING)
        {
            //directionalInput.x = (movementJoystick.Horizontal() > 0.4f || movementJoystick.Horizontal() < -0.4f) ? movementJoystick.Horizontal() : 0;
            //if (directionalInput.x > 0.0f)
            //{
            animator.SetFloat("MovingDirection", directionalInput.x);
            currDirX = directionalInput.x;
            //}
            //else if (directionalInput.x < 0.0f)
            //{
            //    animator.SetFloat("MovingDirection", -1);
            //    currDirX = -1;
            //}
        }

        if (playerState == PlayerStateMobile.MOVING || playerState == PlayerStateMobile.IDLE || playerState == PlayerStateMobile.TURN)
        {
            if (currDirX > 0 && prevDirX < 0)
            {
                doneTurning = false;
                turnAnimRight = true;
                prevDirX = currDirX;
                animator.SetFloat("TurnDirection", prevDirX);
                Invoke("DoneTurning", 0.2f);
            }
            else if (currDirX < 0 && prevDirX > 0)
            {
                doneTurning = false;
                turnAnimLeft = true;
                prevDirX = currDirX;
                animator.SetFloat("TurnDirection", prevDirX);
                Invoke("DoneTurning", 0.2f);
            }
        }
        else
        {
            if (currDirX != 0)
            {
                prevDirX = currDirX;
                animator.SetFloat("FacingDirection", facingDirection);
            }
        }

        if (directionalInput.x != 0)
        {
            facingDirection = directionalInput.x;
            //if (directionalInput.x > 0.0f)
            //    facingDirection = 1;
            //else if (directionalInput.x < 0.0f)
            //    facingDirection = -1;
        }

        //directionalInput.y = (movementJoystick.Vertical() > 0.4f || movementJoystick.Vertical() < -0.4f) ? movementJoystick.Vertical() : 0;
    }
}
