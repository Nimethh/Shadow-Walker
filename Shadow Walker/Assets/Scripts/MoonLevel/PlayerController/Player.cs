using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

    [Header("Jump Variables")]
    [SerializeField]
	private float jumpHeight = 4;
    [SerializeField]
    private float timeToJumpPeak = .4f;

    [Header("Movement Variables")]
    [SerializeField][Range(0f,0.5f)]
    private float accelerationTimeInAirTurn = .2f;
    [SerializeField][Range(0f,0.5f)]
	private float accelerationTimeOnGroundTurn = .1f;
    [SerializeField]
	private float moveSpeed = 10;

    private float velocityXSmoothing;
    private float gravity;
    private float jumpVelocity;
    private Vector3 velocity;
    private Vector2 directionalInput;

    [SerializeField]
	public float wallSlideSpeed = 3;
    private bool wallSliding;
    private Controller2D controller;
    private int wallDirX;
    
    void Start()
    {
		controller = GetComponent<Controller2D> ();
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpPeak, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpPeak;
	}

	void Update()
    {
		CalculateVelocity ();
		HandleWallSliding ();

		controller.Move (velocity * Time.deltaTime, directionalInput);

		if (controller.collisions.above || controller.collisions.below)
        {
			velocity.y = 0;
		}
	}

	public void SetDirectionalInput (Vector2 input)
    {
		directionalInput = input;
	}

	public void OnJumpInputDown()
    {
		if (controller.collisions.below)
        {
			velocity.y = jumpVelocity;
		}
	}
    
	void HandleWallSliding()
    {
		wallDirX = (controller.collisions.left) ? -1 : 1;
		wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
			wallSliding = true;

			if (velocity.y < -wallSlideSpeed)
            {
				velocity.y = -wallSlideSpeed;
			}

		}

	}

	void CalculateVelocity()
    {
		float targetVelocityX = directionalInput.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)? accelerationTimeOnGroundTurn : accelerationTimeInAirTurn);
		velocity.y += gravity * Time.deltaTime;
	}
}
