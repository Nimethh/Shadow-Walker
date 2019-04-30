using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    public float minGroundYNormalized = 0.8f;
    public float gravityScale = 3f;
    [SerializeField]
    private float gravityOnSlopes = 2f;
    [SerializeField]
    private float normalGravity = 3f;
    float movementDistance = 0f;
    private float minMoveDistance = 0.001f;
    private float collisionOffset = 0.01f;

    protected Rigidbody2D rb;
    protected RaycastHit2D[] hitPoints = new RaycastHit2D[25];
    protected List<RaycastHit2D> hitPointsList = new List<RaycastHit2D>(25);
    protected ContactFilter2D contactFilter;

    protected Vector2 groundNormalized = Vector2.zero;
    protected Vector2 velocity = Vector2.zero;
    protected Vector2 playerVelocity = Vector2.zero;

    public bool onGround = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetUpContactFilters();
    }

    void Update()
    {
        playerVelocity = Vector2.zero;
        Movement();
    }

    protected virtual void Movement()
    {

    }

    void FixedUpdate()
    {
        ApplyVelocity();
    }

    void Move(Vector2 move, bool movingDiagnoly)
    {
        movementDistance = move.magnitude;
        CheckCollision(move, movingDiagnoly);
        rb.position = rb.position + move.normalized * movementDistance;
    }

    void SetUpContactFilters()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    void ApplyVelocity()
    {
        velocity += gravityScale * Physics2D.gravity * Time.deltaTime;
        velocity.x = playerVelocity.x;

        onGround = false;

        Vector2 deltaPosition = velocity * Time.fixedDeltaTime;

        Vector2 moveAlongGround = new Vector2(groundNormalized.y, -groundNormalized.x);

        Vector2 move = moveAlongGround * deltaPosition.x;

        Move(move, false);

        move = Vector2.up * deltaPosition.y;

        Move(move, true);
    }

    void CheckCollision(Vector2 move, bool p_movingOnSlope)
    {
        if (movementDistance > minMoveDistance)
        {
            int hitPointsCount = rb.Cast(move, contactFilter, hitPoints, movementDistance + collisionOffset);
            hitPointsList.Clear();
            for (int i = 0; i < hitPointsCount; i++)
            {
                hitPointsList.Add(hitPoints[i]);
            }

            for (int i = 0; i < hitPointsList.Count; i++)
            {
                Vector2 currentNormal = hitPointsList[i].normal;
                if (currentNormal.y > minGroundYNormalized)
                {
                    onGround = true;
                    if (p_movingOnSlope)
                    {
                        if (currentNormal.y < 0.8f)
                        {
                            gravityScale = gravityOnSlopes;
                        }
                        else
                            gravityScale = normalGravity;
                        groundNormalized = currentNormal;
                        currentNormal.x = 0;

                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float newDistance = hitPointsList[i].distance - collisionOffset;
                movementDistance = newDistance < movementDistance ? newDistance : movementDistance;
            }
        }
    }
}
