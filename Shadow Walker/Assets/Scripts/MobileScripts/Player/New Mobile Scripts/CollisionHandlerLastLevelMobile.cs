using UnityEngine;

public class CollisionHandlerLastLevelMobile : RaycastController
{
    [HideInInspector]
    public Vector2 playerInput = Vector2.zero;

    public Vector3 spawningPosition = Vector3.zero;

    public CollisionInfo collisionInfo;

    private float maxClimbSlopeAngle = 89.0f;
    private float maxDescendSlopeAngle = 89.0f;

    GameObject startingPosition = null;

    public override void Start()
    {
        base.Start();
        collisionInfo.raysFacingDir = 1;
        SetUpSpawningPosition();
    }

    public Vector2 UpdateMovement(Vector2 moveAmount, Vector2 input)
    {
        UpdateRaycastOrigins();
        collisionInfo.Reset();
        collisionInfo.moveAmountOld = moveAmount;
        playerInput = input;

        if (moveAmount.x != 0)
        {
            collisionInfo.raysFacingDir = (int)Mathf.Sign(moveAmount.x);
        }

        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }

        HorizontalCollisions(ref moveAmount);
        VerticalCollisions(ref moveAmount);

        transform.Translate(moveAmount);

        return moveAmount;
    }

    void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisionInfo.raysFacingDir;
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < numberOfHorizontalRays; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, ground);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {
                SlopeCheckHorizontally(hit, i, ref moveAmount, directionX, rayLength);
            }
        }
    }

    void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < numberOfVerticalRays; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, ground);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)
            {
                moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisionInfo.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                collisionInfo.below = directionY == -1;
                collisionInfo.above = directionY == 1;

            }
        }

        SlopeCheckVertically(ref moveAmount, rayLength);
    }

    void ClimbSlope(ref Vector2 moveAmount, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (moveAmount.y <= climbmoveAmountY)
        {
            moveAmount.y = climbmoveAmountY;
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            collisionInfo.below = true;
            collisionInfo.climbingSlope = true;
            collisionInfo.slopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector2 moveAmount)
    {
        float directionX = Mathf.Sign(moveAmount.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, ground);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendSlopeAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                    {
                        float moveDistance = Mathf.Abs(moveAmount.x);
                        float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                        moveAmount.y -= descendmoveAmountY;

                        collisionInfo.slopeAngle = slopeAngle;
                        collisionInfo.descendingSlope = true;
                        collisionInfo.below = true;
                    }
                }
            }
        }
    }

    void SlopeCheckHorizontally(RaycastHit2D hit, int rayCastIterationCount, ref Vector2 moveAmount, float raysDirection, float rayLength)
    {
        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

        if (rayCastIterationCount == 0 && slopeAngle <= maxClimbSlopeAngle)
        {
            if (collisionInfo.descendingSlope)
            {
                collisionInfo.descendingSlope = false;
                moveAmount = collisionInfo.moveAmountOld;
            }
            float distanceToSlopeStart = 0;
            if (slopeAngle != collisionInfo.slopeAngleOld)
            {
                distanceToSlopeStart = hit.distance - skinWidth;
                moveAmount.x -= distanceToSlopeStart * raysDirection;
            }
            ClimbSlope(ref moveAmount, slopeAngle);
            moveAmount.x += distanceToSlopeStart * raysDirection;
        }

        if (!collisionInfo.climbingSlope || slopeAngle > maxClimbSlopeAngle)
        {
            moveAmount.x = (hit.distance - skinWidth) * raysDirection;
            rayLength = hit.distance;

            if (collisionInfo.climbingSlope)
            {
                moveAmount.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
            }

            collisionInfo.left = raysDirection == -1;
            collisionInfo.right = raysDirection == 1;
        }
    }

    void SlopeCheckVertically(ref Vector2 moveAmount, float rayLength)
    {
        if (collisionInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, ground);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisionInfo.slopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisionInfo.slopeAngle = slopeAngle;
                }
            }
        }
    }

    void SetUpSpawningPosition()
    {
        startingPosition = GameObject.Find("Door");
        spawningPosition.x = startingPosition.transform.position.x;
        spawningPosition.y = startingPosition.transform.position.y;
        spawningPosition.z = -3;
        transform.position = spawningPosition;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "MovingPlatform")
        {
            this.gameObject.transform.parent = other.gameObject.transform;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("CheckPoint"))
        {
            collisionInfo.checkPointNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("CheckPoint"))
        {
            collisionInfo.checkPointNearby = false;
        }
    }

    public struct CollisionInfo
    {
        //Movement
        public bool above, below;
        public bool left, right;
        public Vector2 moveAmountOld;

        public int raysFacingDir;
        public bool fallingThroughPlatform;

        //Slopes
        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle, slopeAngleOld;

        //CheckPoint.
        public bool checkPointNearby;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            checkPointNearby = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
