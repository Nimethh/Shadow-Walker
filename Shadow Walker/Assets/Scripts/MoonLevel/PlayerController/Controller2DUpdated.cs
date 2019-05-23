using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2DUpdated : RaycastController
{
    private float maxClimbSlopeAngle = 89;
    private float maxDescendSlopeAngle = 89;
    private float ladderRayLengthUp = 0.1f;
    private float ladderRayLengthDown = 0.3f;

    [HideInInspector]
    public Vector2 playerInput;

    public CollisionInfo collisionInfo;
    private PlayerSunBehaviorUpdated playerSunBehavior;
    PlayerUpdated player;

    public override void Start()
    {
        base.Start();
        collisionInfo.raysFacingDir = 1;
        playerSunBehavior = GetComponent<PlayerSunBehaviorUpdated>();
        player = GetComponent<PlayerUpdated>();
    }

    public Vector2 UpdateMovement(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisionInfo.Reset();
        collisionInfo.moveAmountOld = moveAmount;
        playerInput = input;

        if (moveAmount.x != 0)
        {
            collisionInfo.raysFacingDir = (int)Mathf.Sign(moveAmount.x);
        }

        if (moveAmount.y < 0 && collisionInfo.climbing == false && collisionInfo.canClimbOld == false)
        {
            DescendSlope(ref moveAmount);
        }

        HorizontalCollisions(ref moveAmount);

        if (moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }

        LadderCheck(ladderRayLengthUp, ladderRayLengthDown);
        
        transform.Translate(moveAmount);

        return moveAmount;
    }

    void HorizontalCollisions(ref Vector2 moveAmount)
    {
        Collider2D otherCollider = null;

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
                if (hit.collider.tag == "Through" && collisionInfo.canClimbOld == true)
                {
                    if (playerInput.y == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    if (collisionInfo.fallingThroughPlatform)
                    {
                        continue;
                    }
                    if (playerInput.y == -1 && collisionInfo.canClimbOld == true)
                    {
                        collisionInfo.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", .5f);
                        continue;
                    }
                }

                otherCollider = hit.collider;

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
                //if (hit.collider.tag == "MovingPlatform")
                //{
                //    this.gameObject.transform.parent = hit.collider.gameObject.transform;
                //}
                //else
                //{
                //    this.gameObject.transform.parent = null;
                //}
                if (hit.collider.tag == "Through")
                {
                    if (directionY == 1 && playerInput.x == 0 && collisionInfo.canClimbOld == true)
                    {
                        continue;
                    }
                    if (collisionInfo.fallingThroughPlatform)
                    {
                        continue;
                    }
                    if (playerInput.y == -1 && playerInput.x == 0 && collisionInfo.canClimbOld == true)
                    {
                        collisionInfo.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", .5f);
                        continue;
                    }
                }

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

    void LadderCheck(float rayLengthUp, float rayLengthDown)
    {
        Vector2 raycastLadderOriginUp = raycastOrigins.middleTop;
        Vector2 raycastLadderOriginDown = raycastOrigins.middleBottom;
        RaycastHit2D ladderHitUp = Physics2D.Raycast(raycastOrigins.middleTop, Vector2.up, rayLengthUp, ladder);
        RaycastHit2D ladderHitDown = Physics2D.Raycast(raycastOrigins.middleBottom, Vector2.down, rayLengthDown, ladder);

        Debug.DrawRay(raycastLadderOriginUp, Vector2.up);
        Debug.DrawRay(raycastLadderOriginDown, Vector2.down);

        collisionInfo.canClimbOld = false;
        if (!playerSunBehavior.isDead)
        {
            if (ladderHitUp || ladderHitDown)
            {
                collisionInfo.canClimb = true;

                if (ladderHitUp)
                {
                    rayLengthUp = ladderHitUp.distance;
                }
                else if (ladderHitDown)
                {
                    rayLengthDown = ladderHitDown.distance;
                }

                //Check Input.
                if (playerInput.y < 0 && playerInput.x == 0 && player.onGround)
                {
                    collisionInfo.climbing = true;
                    if (!ladderHitDown)
                    {
                        collisionInfo.climbing = false;
                    }
                }
                else if (playerInput.y > 0 && playerInput.x == 0 && player.onGround)
                {
                    collisionInfo.climbing = true;
                }

                // Check if we reached the UpRay is out of the ladder to stop the horizontal movement.
                if (collisionInfo.climbing && ladderHitUp == false)
                {
                    collisionInfo.reachedTopOfTheLadder = true;
                }
                else
                    collisionInfo.reachedTopOfTheLadder = false;


                collisionInfo.canClimbOld = collisionInfo.canClimb;

                if (ladderHitUp)
                    Debug.DrawRay(raycastLadderOriginUp, Vector2.up, Color.green);
                if (ladderHitDown)
                    Debug.DrawRay(raycastLadderOriginDown, Vector2.down, Color.cyan);
            }
            else
            {
                collisionInfo.canClimb = false;
                collisionInfo.climbing = false;
            }
        }
        else
        {
            collisionInfo.canClimb = false;
            collisionInfo.canClimbOld = false;
            collisionInfo.climbing = false;
        }

    }

    void ResetFallingThroughPlatform()
    {
        collisionInfo.fallingThroughPlatform = false;
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

        //Ladder
        public bool canClimb;
        public bool reachedTopOfTheLadder;
        public bool climbing;
        public bool canClimbOld;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            canClimb = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
