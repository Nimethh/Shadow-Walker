using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerMoon : MonoBehaviour
{

    public float speed = 10f;
    [SerializeField]
    private float jumpForce = 600f;
    [SerializeField]
    private float groundCheckRadius = 0.2f;
    public bool onGround = false;
    [SerializeField]
    private Transform groundChecker = null;
    [SerializeField]
    private LayerMask ground = 0;
    [SerializeField]
    private LayerMask ladder = 0;

    private float maxRayDistance = 0.5f;

    private Rigidbody2D rb;
    private float movement = 0f;
    private float climbingMovement = 0f;
    private bool facingRight = true;
    private bool jumping = false;
    public bool canMove = true;
    [SerializeField]
    private bool climbing = false;

    private Vector3 velocity = Vector3.zero;
    [Range(0f, 0.5f)]
    [SerializeField]
    private float m_movementSmoothing = 0.25f;

    private Vector2 playerPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Jump();
        if (canMove == false)
            transform.position = playerPos;
    }

    void FixedUpdate()
    {
        if (canMove == true)
        {
            movement = Input.GetAxisRaw("Horizontal");
            Vector3 playerVelocity = new Vector2(movement * speed, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, playerVelocity, ref velocity, m_movementSmoothing);
            //rb.velocity = new Vector2(movement * speed, rb.velocity.y);
            if (facingRight == false && movement > 0)
            {
                Flip();
            }
            else if (facingRight == true && movement < 0)
            {
                Flip();
            }
        }

        RaycastHit2D hitUpwards = Physics2D.Raycast(transform.position, Vector2.up, maxRayDistance, ladder);
        RaycastHit2D hitDownwards = Physics2D.Raycast(transform.position, Vector2.down, maxRayDistance, ladder);
        if (hitUpwards || hitDownwards)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
            {
                climbing = true;
            }
        }
        else
        {
            climbing = false;
        }

        if (climbing == true && hitUpwards != false || hitDownwards != false)
        {
            Climb();
        }
        else
            rb.gravityScale = 3f;

        onGround = Physics2D.OverlapCircle(groundChecker.position, groundCheckRadius, ground);

    }

    void Jump()
    {
        if (onGround == true && Input.GetKeyDown(KeyCode.Space))
        {
            onGround = false;
            jumping = true;
            rb.AddForce(new Vector2(0f, jumpForce));
            //rb.velocity = Vector2.up * jumpForce;
        }
    }

    void Climb()
    {
        climbingMovement = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector2(rb.velocity.x, climbingMovement * speed);
        rb.gravityScale = 0f;
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
            playerPos = this.transform.position;
            canMove = !canMove;
        }
    }
}
