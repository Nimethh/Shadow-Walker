using UnityEngine;

public enum PlayerStateLastLevel
{
    IDLE,
    MOVING,
    MOVING_INTO_CHECK_POINT,
    MOVING_OUT_FROM_CHECK_POINT,
    INSIDE_CHECK_POINT,
    TURN,
    LEAN_AND_HOLD
}

public class PlayerLastLevel : MonoBehaviour
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

    CollisionHandlerLastLevel collisionHandler;
    Animator animator;
    AudioManager audioManager;

    public PlayerStateLastLevel playerState = PlayerStateLastLevel.INSIDE_CHECK_POINT;

    void Start()
    {
        collisionHandler = GetComponent<CollisionHandlerLastLevel>();
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
            case PlayerStateLastLevel.IDLE:
                InputCheck();
                CalculateVelocity();
                audioManager.Stop("Walk");
                animator.speed = 1;
                animator.SetTrigger("Idle");
                break;

            case PlayerStateLastLevel.MOVING:
                InputCheck();
                CalculateVelocity();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                CheckPlayerBounds();
                audioManager.Play("Walk");
                animator.speed = 1;
                animator.SetTrigger("Moving");
                break;

            case PlayerStateLastLevel.MOVING_INTO_CHECK_POINT:
                animator.speed = 1;
                animator.SetTrigger("MovingInto");
                audioManager.Stop("Walk");
                break;

            case PlayerStateLastLevel.MOVING_OUT_FROM_CHECK_POINT:
                animator.speed = 1;
                animator.SetTrigger("MovingOutFrom");
                audioManager.Stop("Walk");
                break;

            case PlayerStateLastLevel.INSIDE_CHECK_POINT:
                CalculateVelocity();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                animator.speed = 1;
                animator.SetTrigger("InsideCheckPoint");
                break;

            case PlayerStateLastLevel.TURN:
                InputCheck();
                CalculateVelocity();
                collisionHandler.UpdateMovement(velocity * Time.deltaTime, directionalInput);
                animator.speed = 1.2f;
                animator.SetTrigger("Turning");
                audioManager.Stop("Walk");
                break;

            case PlayerStateLastLevel.LEAN_AND_HOLD:
                animator.speed = 1.0f;
                animator.SetTrigger("LeanAndHold");
                audioManager.Stop("Walk");
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
        animator.ResetTrigger("MovingOutFrom");
        animator.ResetTrigger("MovingInto");
        animator.ResetTrigger("Moving");
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("LeanAndHold");
    }

    void CheckPlayerState()
    {
        switch(playerState)
        {
            // INSIDE_CHECK_POINT                                       1
            case PlayerStateLastLevel.INSIDE_CHECK_POINT:
                if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.J) && !Input.GetKeyDown(KeyCode.L))
                {
                    playerState = PlayerStateLastLevel.MOVING_OUT_FROM_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                break;

            // MOVING_INTO_CHECK_POINT:                                 2
            case PlayerStateLastLevel.MOVING_INTO_CHECK_POINT:
                if (isInsideCheck == true)
                {
                    playerState = PlayerStateLastLevel.INSIDE_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                break;

            // MOVING_OUT_FROM_CHECK_POINT                              3
            case PlayerStateLastLevel.MOVING_OUT_FROM_CHECK_POINT:
                if(isInsideCheck == false)
                {
                    playerState = PlayerStateLastLevel.IDLE;
                    ResetAnimationTriggers();
                }
                break;

            // MOVING                                                   5
            case PlayerStateLastLevel.MOVING:
                if (lean == true)
                {
                    playerState = PlayerStateLastLevel.LEAN_AND_HOLD;
                    ResetAnimationTriggers();
                }
                else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && collisionHandler.collisionInfo.checkPointNearby == true 
                    && collisionHandler.collisionInfo.below && playerState != PlayerStateLastLevel.INSIDE_CHECK_POINT)
                {
                    playerState = PlayerStateLastLevel.MOVING_INTO_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                else if (turnAnimLeft || turnAnimRight)
                {
                    playerState = PlayerStateLastLevel.TURN;
                    ResetAnimationTriggers();
                }
                else if (Input.GetAxisRaw("Horizontal") == 0)
                {
                    playerState = PlayerStateLastLevel.IDLE;
                    ResetAnimationTriggers();
                }
                break;

            // TURN                                                     8
            case PlayerStateLastLevel.TURN:
                if(doneTurning == true && Input.GetAxisRaw("Horizontal") == 0.0f)
                {
                    CancelInvoke();
                    turnAnimLeft = false;
                    turnAnimRight = false;
                    doneTurning = false;
                    playerState = PlayerStateLastLevel.IDLE;
                    ResetAnimationTriggers();
                }
                else if(doneTurning == true && Input.GetAxisRaw("Horizontal") != 0.0f)
                {
                    CancelInvoke();
                    turnAnimLeft = false;
                    turnAnimRight = false;
                    doneTurning = false;
                    playerState = PlayerStateLastLevel.MOVING;
                    ResetAnimationTriggers();
                }
                break;

            // IDLE                                                     10
            case PlayerStateLastLevel.IDLE:
                if (lean == true)
                {
                    playerState = PlayerStateLastLevel.LEAN_AND_HOLD;
                    ResetAnimationTriggers();
                }
                else if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && collisionHandler.collisionInfo.checkPointNearby == true
                    && collisionHandler.collisionInfo.below && playerState != PlayerStateLastLevel.INSIDE_CHECK_POINT)
                {
                    playerState = PlayerStateLastLevel.MOVING_INTO_CHECK_POINT;
                    ResetAnimationTriggers();
                }
                else if (turnAnimLeft || turnAnimRight)
                {
                    playerState = PlayerStateLastLevel.TURN;
                    ResetAnimationTriggers();
                }
                else if(directionalInput.x != 0)
                {
                    playerState = PlayerStateLastLevel.MOVING;
                    ResetAnimationTriggers();
                }
                
                break;
            case PlayerStateLastLevel.LEAN_AND_HOLD:
                break;
            default:
                Debug.LogError("Default is called in CheckPlayerState() !");
                playerState = PlayerStateLastLevel.IDLE;
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
        directionalInput.x = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("MovingDirection", directionalInput.x);
        currDirX = directionalInput.x;

        if (playerState == PlayerStateLastLevel.MOVING || playerState == PlayerStateLastLevel.IDLE)
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
        }

        if (directionalInput.x != 0)
        {
            facingDirection = directionalInput.x;
            animator.SetFloat("FacingDirection", facingDirection);
        }

        directionalInput.y = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.name == "Girlfriend")
        {
            lean = true;
        }
    }
}
