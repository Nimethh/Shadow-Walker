using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour
{

	Player player;
    Vector2 directionalInput;
    Controller2D controller;
    Animator animator;
    private bool facingRight = true;
    private SpriteRenderer spriteRenderer;

    void Start ()
    {
		player = GetComponent<Player> ();
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
	}

	void Update ()
    {
        //Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (controller.collisionInfo.climbing == false || controller.collisionInfo.below == true)
        {
            directionalInput.x = Input.GetAxisRaw("Horizontal");
            Debug.Log(directionalInput.x);
            if (directionalInput.x > 0)
            {
                animator.SetBool("WalkRight", true);
                animator.SetBool("WalkLeft", false);
                animator.SetBool("Idle", false);
            }
            else if(directionalInput.x < 0)
            {
                animator.SetBool("WalkLeft", true);
                animator.SetBool("WalkRight", false);
                animator.SetBool("Idle", false);
            }
            else
            {
                animator.SetBool("Idle", true);
                animator.SetBool("WalkRight", false);
                animator.SetBool("WalkLeft", false);
            }
        }
        directionalInput.y = Input.GetAxisRaw("Vertical");
        player.SetDirectionalInput (directionalInput);

        if (facingRight == false && directionalInput.x > 0)
        {
            Flip();
        }
        else if (facingRight == true && directionalInput.x < 0)
        {
            Flip();
        }

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
}
