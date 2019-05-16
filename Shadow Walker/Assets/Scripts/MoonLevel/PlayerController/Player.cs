using UnityEngine;
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
    public bool movingToNextLevel = false;
    public bool onLadder = false;
    public bool moveOffLadder = false;

    [SerializeField]
    public float wallSlideSpeed = 3;
    private bool wallSliding;
    private int wallDirX;

    private Controller2D controller;
    Animator animator;
    private GameObject endOfTheScene;

    ParticleSystem jumpParticle;
    GameObject jumpParticleObject;
    ParticleSystem landParticle;
    GameObject landParticleObject;
    void Start()
    {
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpPeak, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpPeak;
        endOfTheScene = GameObject.Find("SceneManager");
        jumpParticleObject = transform.GetChild(2).gameObject;
        jumpParticle = jumpParticleObject.GetComponent<ParticleSystem>();
        landParticleObject = transform.GetChild(1).gameObject;
        landParticle = landParticleObject.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        CalculateVelocity();
        HandleWallSliding();
        GroundCheck();
        MovingToEndOfTheSceneCheck();
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
            jumpParticle.Play();
        }
    }

    public void GroundCheck()
    {
        if(jumping == true && velocity.y < 0)
        {
            falling = true;
            jumping = false;
        }
        if(velocity.y < 0 && !controller.collisionInfo.below && !controller.collisionInfo.climbing && moveOffLadder)
        {
            falling = true;
            onGround = false;
            jumping = false;
        }
        if (velocity.y < 0 && controller.collisionInfo.below)
        {
            if(landing == false)
            {
                landing = true;
                animator.SetTrigger("Landing");
                landed = true;
            }
            falling = false;
            onGround = true;
        }
        else if(!controller.collisionInfo.below)
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

    void MovingToEndOfTheSceneCheck()
    {
        if(movingToNextLevel)
        {
            transform.position = Vector2.MoveTowards(transform.position, endOfTheScene.transform.position, 0.3f * Time.deltaTime);
        }
    }

    void PlayJumpParticle()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //if(other.gameObject.CompareTag("LevelEndPoint"))
        //{
        //    movingToNextLevel = true;
        //    //animator.SetTrigger("MoveToNextLevel");
        //}
        if(other.gameObject.CompareTag("Ground"))
        {
            landParticle.Play();
        }
       
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder") && !onGround)
        {
            onLadder = true;
            moveOffLadder = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Ladder"))
        {
            onLadder = false;
            moveOffLadder = true;
            animator.SetBool("Climbing", false);
        }
    }
}

