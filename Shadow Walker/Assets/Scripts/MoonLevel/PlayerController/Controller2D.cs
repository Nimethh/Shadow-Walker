using UnityEngine;
using System.Collections;

public class Controller2D : RaycastController
{
    [Header("Slope Handling")]
	[SerializeField][Range(0f,100f)]
	private float maxClimbSlopeAngle = 80;
    [SerializeField][Range(0f,100f)]
	private float maxDescendSlopeAngle = 80;
	
	public CollisionInfo collisionInfo;
	[HideInInspector]
	public Vector2 playerInput;
	
	public override void Start()
    {
		base.Start ();
        collisionInfo.faceDir = 1;
	}
    
    public Vector2 Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
    {
		UpdateRaycastOrigins ();
        collisionInfo.Reset ();
        collisionInfo.moveAmountOld = moveAmount;
		playerInput = input;

		if (moveAmount.x != 0)
        {
            collisionInfo.faceDir = (int)Mathf.Sign(moveAmount.x);
		}

		if (moveAmount.y < 0 && collisionInfo.climbing == false && collisionInfo.canClimbOld == false)
        {
			DescendSlope(ref moveAmount);
		}

		HorizontalCollisions (ref moveAmount);

		if (moveAmount.y != 0)
        {
			VerticalCollisions (ref moveAmount);
		}

		transform.Translate (moveAmount);

		if (standingOnPlatform)
        {
            collisionInfo.below = true;
            collisionInfo.canClimbOld = false;
            collisionInfo.climbing = false;
            Debug.Log("Standing on platform " + collisionInfo.climbing);

        }

        LadderCheck(0.1f,0.1f);

		return moveAmount;
	}

	void HorizontalCollisions(ref Vector2 moveAmount)
    {
        
		float originalMoveAmountX = moveAmount.x;
		Collider2D otherCollider = null;

		float directionX = collisionInfo.faceDir;
		float rayLength = Mathf.Abs (moveAmount.x) + skinWidth;

		if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
			rayLength = 2*skinWidth;
		}

		
		for (int i = 0; i < numberOfHorizontalRays; i ++)
        {
            //if (collisionInfo.canClimbOld == true)
            //{
            //    continue;
            //}
            //if (collisionInfo.climbing == true)
            //{
            //    continue;
            //}
            Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, ground);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX,Color.red);

			if (hit)
            {
                //This was causing problem in collision
                //if (hit.distance == 0)
                //{
                //    Debug.Log("return");
                //	continue;
                //}

                if (hit.collider.tag == "Through" && collisionInfo.canClimbOld == true)
                {
                    if (playerInput.y == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    if (collisionInfo.fallingThroughPlatform /*&& collisionInfo.canClimbOld == true*/)
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
			
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				if (i == 0 && slopeAngle <= maxClimbSlopeAngle)
                {
					if (collisionInfo.descendingSlope)
                    {
                        collisionInfo.descendingSlope = false;
						moveAmount = collisionInfo.moveAmountOld;
					}
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisionInfo.slopeAngleOld)
                    {
						distanceToSlopeStart = hit.distance-skinWidth;
						moveAmount.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref moveAmount, slopeAngle);
					moveAmount.x += distanceToSlopeStart * directionX;
				}

				if (!collisionInfo.climbingSlope || slopeAngle > maxClimbSlopeAngle)
                {
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisionInfo.climbingSlope)
                    {
						moveAmount.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
					}

                    collisionInfo.left = directionX == -1;
                    collisionInfo.right = directionX == 1;
				}
			}
		}
    }
	
	void VerticalCollisions(ref Vector2 moveAmount)
    {
		float directionY = Mathf.Sign (moveAmount.y);
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;
        
        for (int i = 0; i < numberOfVerticalRays; i ++)
        {
            //if (collisionInfo.canClimbOld == true)
            //{
            //    continue;
            //}
            //if (collisionInfo.climbing == true)
            //{
            //    continue;
            //}
            Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, ground);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY,Color.red);

			if (hit)
            {
                if(hit.collider.tag == "MovingPlatform")
                {
                    this.gameObject.transform.parent = hit.collider.gameObject.transform;
                }
                if(hit.collider.tag == "LadderTop" && playerInput.y == 1)
                {
                    collisionInfo.canClimbOld = false;
                }
                else if (hit.collider.tag == "LadderTop" && playerInput.y == -1)
                {
                    continue;
                }
                if (hit.collider.tag == "LadderBottom" && playerInput.y == -1)
                {
                    collisionInfo.canClimbOld = false;
                }
                else if (hit.collider.tag == "LadderBottom" && playerInput.y == 1)
                {
                    continue;
                }
                if (hit.collider.tag == "Through")
                {
                    if (directionY == 1 && collisionInfo.canClimbOld == true/*|| hit.distance == 0*/)
                    {
                        continue;
                    }
                    if (collisionInfo.fallingThroughPlatform/* && collisionInfo.canClimbOld == true*/)
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

		if (collisionInfo.climbingSlope)
        {
			float directionX = Mathf.Sign(moveAmount.x);
			rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.right * directionX,rayLength,ground);

			if (hit && collisionInfo.canClimbOld == true)
            {
				float slopeAngle = Vector2.Angle(hit.normal,Vector2.up);
				if (slopeAngle != collisionInfo.slopeAngle)
                {
					moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisionInfo.slopeAngle = slopeAngle;
				}
			}
		}
	}

	void ClimbSlope(ref Vector2 moveAmount, float slopeAngle)
    {
		float moveDistance = Mathf.Abs (moveAmount.x);
		float climbmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (moveAmount.y <= climbmoveAmountY)
        {
			moveAmount.y = climbmoveAmountY;
			moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
            collisionInfo.below = true;
            collisionInfo.climbingSlope = true;
            collisionInfo.slopeAngle = slopeAngle;
		}
	}

	void DescendSlope(ref Vector2 moveAmount)
    {

        float directionX = Mathf.Sign (moveAmount.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, ground);

		if (hit && collisionInfo.canClimbOld == true)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendSlopeAngle)
            {
				if (Mathf.Sign(hit.normal.x) == directionX)
                {
					if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                    {
						float moveDistance = Mathf.Abs(moveAmount.x);
						float descendmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
						moveAmount.y -= descendmoveAmountY;

                        collisionInfo.slopeAngle = slopeAngle;
                        collisionInfo.descendingSlope = true;
                        collisionInfo.below = true;
					}
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

        if (ladderHitUp || ladderHitDown)
        {
            if(ladderHitUp)
            {
                rayLengthUp = ladderHitUp.distance;
            }
            else if(ladderHitDown)
            {
                rayLengthDown = ladderHitDown.distance;
            }
            collisionInfo.canClimb = true;
            if (playerInput.y < 0)
            {
                collisionInfo.climbing = true;
            }
            else if (playerInput.y > 0)
            {
                collisionInfo.climbing = true;
            }
            collisionInfo.canClimbOld = collisionInfo.canClimb;
            Debug.DrawRay(raycastLadderOriginUp, Vector2.up,Color.green);
            Debug.DrawRay(raycastLadderOriginDown, Vector2.down,Color.green);
        }
        else if(collisionInfo.canClimbOld == false)
        {
            collisionInfo.canClimb = false;
            collisionInfo.climbing = false;
        }
    }

    void ResetFallingThroughPlatform()
    {
        collisionInfo.fallingThroughPlatform = false;
	}

    public struct CollisionInfo
    {
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector2 moveAmountOld;
		public int faceDir;
		public bool fallingThroughPlatform;
        public bool canClimb;
        public bool reachedTopOfTheLadder;
        public bool reachedBottomOfTheLadder;
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
