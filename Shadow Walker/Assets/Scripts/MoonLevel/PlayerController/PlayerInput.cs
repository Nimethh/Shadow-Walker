using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour
{
    ParticleSystem movingPartical;
    private GameObject movingParticalObject;
	Player player;
    Vector2 directionalInput;
    Controller2D controller;
    Animator animator;
    private bool facingRight = true;
    private SpriteRenderer spriteRenderer;

    private float lastMoveX;

    void Start ()
    {
		player = GetComponent<Player> ();
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
        movingParticalObject = transform.GetChild(0).gameObject;
        movingPartical = movingParticalObject.GetComponent<ParticleSystem>();
	}

	void Update ()
    {
        //Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (controller.collisionInfo.climbing == false || controller.collisionInfo.below == true)
        {
            directionalInput.x = Input.GetAxisRaw("Horizontal");
            if (directionalInput.x > 0)
            {
                lastMoveX = directionalInput.x;
                animator.SetFloat("MovementX", directionalInput.x);
                animator.SetBool("Moving", true);
            }
            else if (directionalInput.x < 0)
            {
                lastMoveX = directionalInput.x;
                animator.SetFloat("MovementX", directionalInput.x);
                animator.SetBool("Moving", true);
            }
            else
            {
                animator.SetFloat("MovementX", directionalInput.x);
                animator.SetBool("Moving", false);
            }

            animator.SetFloat("LastXValue", lastMoveX);
            //if (directionalInput.x > 0)
            //{
            //    animator.SetBool("WalkRight", true);
            //    animator.SetBool("WalkLeft", false);
            //    animator.SetBool("Idle", false);
            //}
            //else if(directionalInput.x < 0)
            //{
            //    animator.SetBool("WalkLeft", true);
            //    animator.SetBool("WalkRight", false);
            //    animator.SetBool("Idle", false);
            //}
            //else
            //{
            //    animator.SetBool("Idle", true);
            //    animator.SetBool("WalkRight", false);
            //    animator.SetBool("WalkLeft", false);
            //}
        }
        directionalInput.y = Input.GetAxisRaw("Vertical");
        player.SetDirectionalInput (directionalInput);

        //if (facingRight == false && directionalInput.x > 0)
        //{
        //    Flip();
        //}
        //else if (facingRight == true && directionalInput.x < 0)
        //{
        //    Flip();
        //}

        if (Input.GetKeyDown (KeyCode.Space))
        {
			player.OnJumpInputDown ();
            FindObjectOfType<AudioManager>().Play("Jump");
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector2 characterScale = transform.localScale;
        characterScale.x *= -1;
        transform.localScale = characterScale;
    }

    public void PlayMovingParticle()
    {
        movingPartical.Play();
    }
}
