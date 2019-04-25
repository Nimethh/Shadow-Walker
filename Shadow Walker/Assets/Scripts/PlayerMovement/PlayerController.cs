using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float jumpForce = 5f;
    [SerializeField]
    private float groundCheckRadius = 0.2f;
    [SerializeField]
    private float jumpingTimer = 0.4f;
    [SerializeField]
    private bool onGround = false;
    [SerializeField]
    private Transform groundChecker = null;
    [SerializeField]
    private LayerMask ground = 0;

    float unableToMoveTimer = 0f;

    private Rigidbody2D rb;
    [SerializeField]
    private float movement = 0f;
    private float jumpingCounter = 0f;
    private bool facingRight = true;
    private bool jumping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        facingRight = true;
    }

    void Update()
    {
    }

    void Jump()
    {
        if (onGround == true && (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Space)))
        {
            onGround = false;
            jumping = true;
            jumpingCounter = jumpingTimer;
            //rb.AddForce(new Vector2(0f, jumpForce));
            rb.velocity = Vector2.up * jumpForce;
        }
        if (Input.GetKey(KeyCode.Space) && jumping == true)
        {
            if (jumpingCounter >= 0)
            {
                //rb.AddForce(new Vector2(0f, jumpForce));
                rb.velocity = Vector2.up * jumpForce;
                jumpingCounter -= Time.deltaTime;
            }
            else
            {
                jumping = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumping = false;
        }
    }

    void FixedUpdate()
    {
        if(unableToMoveTimer > 0)
        {
            unableToMoveTimer -= Time.deltaTime;
            if(unableToMoveTimer > 0)
            {
                return;
            }
            else
            {
                unableToMoveTimer = 0;
            }
        }

        Jump();
        movement = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(movement * speed, rb.velocity.y);

        if (facingRight == false && movement > 0)
        {
            Flip();
        }
        else if (facingRight == true && movement < 0)
        {
            Flip();
        }

        onGround = Physics2D.OverlapCircle(groundChecker.position, groundCheckRadius, ground);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector2 characterScale = transform.localScale;
        characterScale.x *= -1;
        transform.localScale = characterScale;
    }

    public void StopAllMovement(float amountOfSecondsToStopMovement)
    {
        rb.Sleep();
        amountOfSecondsToStopMovement = Mathf.Abs(amountOfSecondsToStopMovement);
        unableToMoveTimer = amountOfSecondsToStopMovement;
    }
}