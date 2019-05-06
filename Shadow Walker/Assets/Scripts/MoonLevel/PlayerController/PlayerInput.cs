using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour
{

	Player player;
    Vector2 directionalInput;
    Controller2D controller;

    void Start ()
    {
		player = GetComponent<Player> ();
        controller = GetComponent<Controller2D>();
	}

	void Update ()
    {
        //Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (controller.collisionInfo.climbing == false || controller.collisionInfo.below == true)
        {
            directionalInput.x = Input.GetAxisRaw("Horizontal");
        }
        directionalInput.y = Input.GetAxisRaw("Vertical");
        player.SetDirectionalInput (directionalInput);

		if (Input.GetKeyDown (KeyCode.Space))
        {
			player.OnJumpInputDown ();
            FindObjectOfType<AudioManager>().Play("Jump");
        }
	}
}
