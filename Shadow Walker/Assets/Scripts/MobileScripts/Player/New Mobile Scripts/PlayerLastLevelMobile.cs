using UnityEngine;

public enum PlayerStateLastLevelMobile
{
    IDLE,
    MOVING,
    MOVING_INTO_CHECK_POINT,
    MOVING_OUT_FROM_CHECK_POINT,
    INSIDE_CHECK_POINT,
    TURN,
    LEAN_AND_HOLD
}

public class PlayerLastLevelMobile : MonoBehaviour
{
    [Header("Jump Variables")]
    [SerializeField]
    float jumpHeight = 4;
    [SerializeField]
    float timeToJumpPeak = .4f;

    [Header("Movement Variables")]
    [SerializeField]
    [Range(0f, 0.5f)]
    float accelerationTimeOnGroundTurn = .1f;
    [SerializeField]
    float moveSpeed = 10;

    [HideInInspector]
    public Vector3 velocity;
    [HideInInspector]
    public Vector2 directionalInput;

    float velocityXSmoothing = 0;
    float gravity = 0;

    public float facingDirection;

    public bool isInsideCheck = true;
    public bool doneTurning = true;
    public bool lean = false;

    float top = 0;
    float bottom = 0;
    float right = 0;
    float left = 0;

    public float prevDirX;
    public float currDirX;
    public bool turnAnimRight = false;
    public bool turnAnimLeft = false;

    CollisionHandlerLastLevelMobile collisionHandler;
    Animator animator;
    AudioManager audioManager;
    public VirtualMovementJoystick movementJoystick;

    public PlayerStateLastLevelMobile playerState = PlayerStateLastLevelMobile.INSIDE_CHECK_POINT;

    void Start()
    {
        collisionHandler = GetComponent<CollisionHandlerLastLevelMobile>();
        animator = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();

        Cursor.visible = false;
        FindPlayerBounds();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpPeak, 2);

        animator.SetFloat("FacingDirection", 1.0f);
        prevDirX = 1.0f;

        lean = false;
    }

    void Update()
    {
        CheckPlayerState();
        UpdatePlayer();
    }

    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTimeOnGroundTurn);
        velocity.y += gravity * Time.deltaTime;
    }

    void UpdatePlayer()
    {
        switch (playerState)
        {
            case PlayerStateLastLevelMobile.IDLE:
                InputCheck();
                CalculateVelocity();
                audioManager.Stop("Walk");
                animator.speed = 1;
                animator.SetTrigger("Idle");
                break;

            case PlayerStateLastLevelMobile.MOVING:
                InputCheck();
                CalculateVelocity();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                CheckPlayerBounds();
                audioManager.Play("Walk");
                animator.speed = 1;
                animator.SetTrigger("Moving");
                break;

            case PlayerStateLastLevelMobile.MOVING_INTO_CHECK_POINT:
                animator.speed = 1;
                animator.SetTrigger("MovingInto");
                audioManager.Stop("Walk");
                break;

            case PlayerStateLastLevelMobile.MOVING_OUT_FROM_CHECK_POINT:
                animator.speed = 1;
                animator.SetTrigger("MovingOutFrom");
                audioManager.Stop("Walk");
                break;

            case PlayerStateLastLevelMobile.INSIDE_CHECK_POINT:
                CalculateVelocity();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                animator.speed = 1;
                animator.SetTrigger("InsideCheckPoint");
                break;

            case PlayerStateLastLevelMobile.TURN:
                InputCheck();
                CalculateVelocity();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                animator.speed = 1.2f;
                animator.SetTrigger("Turning");
                audioManager.Stop("Walk");
                break;

            case PlayerStateLastLevelMobile.LEAN_AND_HOLD:
                animator.speed = 1.0f;
                animator.SetTrigger("LeanAndHold");
                audioManager.Stop("Walk");
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
        animator.ResetTrigger("MovingOutFrom");
        animator.ResetTrigger("MovingInto");
        animator.ResetTrigger("Moving");
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("LeanAndHold");
    }

    void CheckPlayerState()
    {
        switch (playerState)
        {
            // INSIDE_CHECK_POINT                                       1
            case PlayerStateLastLevelMobile.INSIDE_CHECK_POINT:
                //if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.J) && !Input.GetKeyDown(KeyCode.L))
                if(Input.touchCount != 0)
                {
                    playerState = PlayerStateLastLevelMobile.MOVING_OUT_FROM_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                break;

            // MOVING_INTO_CHECK_POINT:                                 2
            case PlayerStateLastLevelMobile.MOVING_INTO_CHECK_POINT:
                if (isInsideCheck == true)
                {
                    playerState = PlayerStateLastLevelMobile.INSIDE_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                break;

            // MOVING_OUT_FROM_CHECK_POINT                              3
            case PlayerStateLastLevelMobile.MOVING_OUT_FROM_CHECK_POINT:
                if (isInsideCheck == false)
                {
                    playerState = PlayerStateLastLevelMobile.IDLE;
                    ResetAnimationTriggers();
                }
                break;

            // MOVING                                                   5
            case PlayerStateLastLevelMobile.MOVING:
                if (lean == true)
                {
                    playerState = PlayerStateLastLevelMobile.LEAN_AND_HOLD;
                    ResetAnimationTriggers();
                }
                else if (directionalInput.y != 0 && collisionHandler.collisionInfo.checkPointNearby == true
                    && collisionHandler.collisionInfo.below && playerState != PlayerStateLastLevelMobile.INSIDE_CHECK_POINT)
                {
                    playerState = PlayerStateLastLevelMobile.MOVING_INTO_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                else if (turnAnimLeft || turnAnimRight)
                {
                    playerState = PlayerStateLastLevelMobile.TURN;
                    ResetAnimationTriggers();
                }
                else if (directionalInput.x == 0)
                {
                    playerState = PlayerStateLastLevelMobile.IDLE;
                    ResetAnimationTriggers();
                }
                break;

            // TURN                                                     8
            case PlayerStateLastLevelMobile.TURN:
                if (doneTurning == true && directionalInput.x == 0)
                {
                    CancelInvoke();
                    turnAnimLeft = false;
                    turnAnimRight = false;
                    doneTurning = false;
                    playerState = PlayerStateLastLevelMobile.IDLE;
                    ResetAnimationTriggers();
                    animator.SetFloat("FacingDirection", facingDirection);
                }
                else if (doneTurning == true && directionalInput.x != 0)
                {
                    CancelInvoke();
                    turnAnimLeft = false;
                    turnAnimRight = false;
                    doneTurning = false;
                    playerState = PlayerStateLastLevelMobile.MOVING;
                    ResetAnimationTriggers();
                    animator.SetFloat("FacingDirection", facingDirection);
                }
                break;

            // IDLE                                                     10
            case PlayerStateLastLevelMobile.IDLE:
                if (lean == true)
                {
                    playerState = PlayerStateLastLevelMobile.LEAN_AND_HOLD;
                    ResetAnimationTriggers();
                }
                else if (directionalInput.y != 0 && collisionHandler.collisionInfo.checkPointNearby == true
                    && collisionHandler.collisionInfo.below && playerState != PlayerStateLastLevelMobile.INSIDE_CHECK_POINT)
                {
                    playerState = PlayerStateLastLevelMobile.MOVING_INTO_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                else if (turnAnimLeft || turnAnimRight)
                {
                    playerState = PlayerStateLastLevelMobile.TURN;
                    ResetAnimationTriggers();
                }
                else if (directionalInput.x != 0)
                {
                    playerState = PlayerStateLastLevelMobile.MOVING;
                    ResetAnimationTriggers();
                }

                break;
            case PlayerStateLastLevelMobile.LEAN_AND_HOLD:
                break;
            default:
                Debug.LogError("Default is called in CheckPlayerState() !");
                playerState = PlayerStateLastLevelMobile.IDLE;
                ResetAnimationTriggers();
                break;
        }
    }

    void IsInsideCheckPoint()
    {
        isInsideCheck = !isInsideCheck;
    }

    void DoneTurning()
    {
        doneTurning = true;
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
        }
    }

    void InputCheck()
    {
        directionalInput.x = (movementJoystick.Horizontal() > 0.4f || movementJoystick.Horizontal() < -0.4f) ? movementJoystick.Horizontal() : 0;
        if (directionalInput.x > 0.0f)
        {
            animator.SetFloat("MovingDirection", 1);
            currDirX = 1;
        }
        else if (directionalInput.x < 0.0f)
        {
            animator.SetFloat("MovingDirection", -1);
            currDirX = -1;
        }

        if (playerState == PlayerStateLastLevelMobile.MOVING || playerState == PlayerStateLastLevelMobile.IDLE)
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
            prevDirX = currDirX;
            animator.SetFloat("FacingDirection", facingDirection);
        }

        if (directionalInput.x != 0)
        {
            if (directionalInput.x > 0.0f)
                facingDirection = 1;
            else if (directionalInput.x < 0.0f)
                facingDirection = -1;
        }

        directionalInput.y = (movementJoystick.Vertical() > 0.4f || movementJoystick.Vertical() < -0.4f) ? movementJoystick.Vertical() : 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Girlfriend")
        {
            lean = true;
        }
    }
}
