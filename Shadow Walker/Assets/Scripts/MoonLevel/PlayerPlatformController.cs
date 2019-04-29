using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPlatformController : PlayerPhysics
{
    public float maxSpeed = 2f;
    public float jumpForce = 6;
    Vector2 playerPos = Vector2.zero;
    private bool canMove = true;
    private bool facingRight = true;

    private SpriteRenderer spriteRenderer;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Movement()
    {
        //if (canMove == true)
        //{
            // resetting the vector so we don't use the old one.
            Vector2 movement = Vector2.zero;
            playerPos = Vector2.zero;
            movement.x = Input.GetAxis("Horizontal");

            Jump();

            playerVelocity = movement * maxSpeed;
            if (facingRight == false && movement.x > 0)
            {
                Flip();
            }
            else if (facingRight == true && movement.x < 0)
            {
                Flip();
            }
        //}
        //else
        //{
        //    playerVelocity = Vector2.zero;
        //    transform.position = playerPos;
        //}
        //Debug.Log(playerPos);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && onGround)
        {
            velocity.y = jumpForce;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector2 characterScale = transform.localScale;
        characterScale.x *= -1;
        transform.localScale = characterScale;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Mirror" && Input.GetKeyDown(KeyCode.Space) && onGround == true)
        {
            //if (playerPos == Vector2.zero)
            //{
                playerPos = this.transform.position;
            //}
            //canMove = !canMove;
        }
    }

}