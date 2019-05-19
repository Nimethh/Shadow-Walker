//using UnityEngine;
//using System.Collections;

//[RequireComponent (typeof (Player))]
//public class PlayerInput : MonoBehaviour
//{
//    Controller2D controller;
//    PlayerSoundManager playerSoundManager;
//    Animator animator;
//    Player player;
//    ParticleSystem movingPartical;

//    GameObject movingParticalObject;
    
//    Vector2 directionalInput;
//    private float xMovement;
//    private float lastMoveX;

//    [SerializeField]
//    private float moveOffLadderTimer = 0.01f;
//    private float moveOffLadderCooldown = 0.01f;
//    [SerializeField]
//    private float moveOffLadderHoldTimer = 0.05f;
//    private float moveOffLadderHoldCooldown = 0.05f;

//    void Start ()
//    {
//        movingParticalObject = transform.GetChild(0).gameObject;
//        movingPartical = movingParticalObject.GetComponent<ParticleSystem>();
//        player = GetComponent<Player> ();
//        playerSoundManager = GetComponent<PlayerSoundManager>();
//        controller = GetComponent<Controller2D>();
//        animator = GetComponent<Animator>();

//        moveOffLadderCooldown = moveOffLadderTimer;
//        moveOffLadderHoldCooldown = moveOffLadderHoldTimer;
//	}

//	void Update ()
//    {
//        MoveOffLadderCheck();
//        MovementCheck();
//        JumpCheck();
//        JumpAnimationCheck();
//        FallingAnimationCheck();
//        ClimbingAnimationCheck();

//        player.SetDirectionalInput (directionalInput);
//        playerSoundManager.SetDirectionalInput(directionalInput);
//    }

//    public void PlayMovingParticle()
//    {
//        //movingPartical.Play();
//    }

//    void MovementCheck()
//    {
//        if (!player.movingToNextLevel)
//        {
//            if (controller.collisionInfo.climbing == false || controller.collisionInfo.below == true)
//            {
//                directionalInput.x = Input.GetAxisRaw("Horizontal");
//                if (player.onGround)
//                {
//                    animator.SetBool("OnGround", player.onGround);
//                    if (directionalInput.x > 0 && player.onGround)
//                    {
//                        lastMoveX = directionalInput.x;
//                        animator.SetFloat("MovementX", directionalInput.x);
//                        animator.SetBool("Moving", true);
//                    }
//                    else if (directionalInput.x < 0 && player.onGround)
//                    {
//                        lastMoveX = directionalInput.x;
//                        animator.SetFloat("MovementX", directionalInput.x);
//                        animator.SetBool("Moving", true);
//                    }
//                    else
//                    {
//                        animator.SetFloat("MovementX", directionalInput.x);
//                        animator.SetBool("Moving", false);
//                    }
//                    animator.SetFloat("LastXValue", lastMoveX);
//                }
//            }
//            directionalInput.y = Input.GetAxisRaw("Vertical");

//        }
//    }

//    void MoveOffLadderCheck()
//    {
//        if (controller.collisionInfo.climbing == true && controller.collisionInfo.reachedTopOfTheLadder == false)
//        {
//            if (directionalInput.y > 0 || directionalInput.y < 0)
//            {
//                moveOffLadderCooldown = moveOffLadderTimer;
//                moveOffLadderHoldCooldown = moveOffLadderHoldTimer;
//            }
//            if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)))
//            {
//                if (moveOffLadderHoldCooldown <= 0)
//                {
//                    directionalInput.x = Input.GetAxisRaw("Horizontal");
//                }
//                else
//                {
//                    moveOffLadderHoldCooldown -= Time.deltaTime;
//                }
//            }

//            if (Input.GetKeyDown(KeyCode.D))
//            {
//                if (moveOffLadderCooldown <= 0)
//                {
//                    directionalInput.x = Input.GetAxisRaw("Horizontal");
//                }
//                else
//                {
//                    moveOffLadderCooldown -= Time.deltaTime;
//                }
//            }
//            else if (Input.GetKeyDown(KeyCode.A))
//            {
//                if (moveOffLadderCooldown <= 0)
//                {
//                    directionalInput.x = Input.GetAxisRaw("Horizontal");
//                }
//                else
//                {
//                    moveOffLadderCooldown -= Time.deltaTime;
//                }
//            }
//        }
//    }

//    void JumpCheck()
//    {
//        if (Input.GetKeyDown(KeyCode.Space) && !player.movingToNextLevel)
//        {
//            animator.SetBool("Jumping", true);
//            player.OnJumpInputDown();
//        }
//    }

//    void JumpAnimationCheck()
//    {
//        if (player.jumping == true)
//        {
//            if (directionalInput.x > 0)
//            {
//                lastMoveX = directionalInput.x;
//                animator.SetFloat("MovementX", directionalInput.x);
//            }
//            else if (directionalInput.x < 0)
//            {
//                lastMoveX = directionalInput.x;
//                animator.SetFloat("MovementX", directionalInput.x);
//            }
//            else
//            {
//                animator.SetFloat("MovementX", directionalInput.x);
//            }
//            animator.SetFloat("LastXValue", lastMoveX);
//        }
//        else
//            animator.SetBool("Jumping", false);
//    }

//    void FallingAnimationCheck()
//    {
//        if (player.falling == true)
//        {
//            animator.SetBool("Falling", player.falling);
//            animator.SetBool("OnGround", player.onGround);
//            if (directionalInput.x > 0)
//            {
//                lastMoveX = directionalInput.x;
//                animator.SetFloat("MovementX", directionalInput.x);
//            }
//            else if (directionalInput.x < 0)
//            {
//                lastMoveX = directionalInput.x;
//                animator.SetFloat("MovementX", directionalInput.x);
//            }
//            else
//            {
//                animator.SetFloat("MovementX", directionalInput.x);
//            }
//            animator.SetFloat("LastXValue", lastMoveX);
//        }
//        else
//            animator.SetBool("Falling", player.falling);
//    }

//    void ClimbingAnimationCheck()
//    {
//        if(player.onLadder)
//        {
//            player.onGround = false;
//            animator.SetBool("Climbing", true);
//            animator.SetBool("OnGround", false);
//            if (directionalInput.y > 0 || directionalInput.y < 0)
//            {
//                animator.speed = 1;
//            }
//            else
//            {
//                animator.speed = 0;
//            }
//        }
//        else
//        {
//            animator.SetBool("Climbing", false);
//            if(animator.name == "Jump")
//            {
//                animator.speed = 1.5f;
//            }
//            else if(animator.name == "Falling")
//            {
//                animator.speed = 1.5f;
//            }
//            else if(animator.name == "Landing")
//            {
//                animator.speed = 2.5f;
//            }
//            else
//            {
//                animator.speed = 1;
//            }
//        }
//    }
//}
