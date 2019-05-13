using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private Vector2 directionalInput;
    private Player player;
    private PlayerSunBehaviorUpdated playerSunBehaviorUpdated;
    PlayerSunBehavior playerSunBehavior;
    Controller2D controller;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        player = GetComponent<Player>();
        playerSunBehaviorUpdated = GetComponent<PlayerSunBehaviorUpdated>();
        playerSunBehavior = GetComponent<PlayerSunBehavior>();
        controller = GetComponent<Controller2D>();
    }

    private void Update()
    {
        WalkSound();
        PlayClimbingSound();
        JumpSound();
        DeathSound();
        LandingSound();
    }

    public void WalkSound()
    {
        if ((directionalInput.x > 0 || directionalInput.x < 0) && player.onGround)
        {
            FindObjectOfType<AudioManager>().Play("Walk");
        }
        else
        {
            FindObjectOfType<AudioManager>().Stop("Walk");
        }
    }

    public void DeathSound()
    {
        //if (playerSunBehaviorUpdated.isExposedToSunlight)
        //{
        //    FindObjectOfType<AudioManager>().Play("Death");
        //}
        if (playerSunBehavior.isExposedToSunlight)
        {
            FindObjectOfType<AudioManager>().Play("Death");
        }
    }

    public void LandingSound()
    {
        if (player.landed)
        {
            FindObjectOfType<AudioManager>().Play("Land");
            player.landed = false;
        }
    }

    void JumpSound()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            FindObjectOfType<AudioManager>().Play("Jump");
        }
    }

    public void PlayClimbingSound()
    {
        if((directionalInput.y > 0 || directionalInput.y < 0) && controller.collisionInfo.climbing)
        {
            FindObjectOfType<AudioManager>().Play("Climb");
        }
        else
        {
            FindObjectOfType<AudioManager>().Stop("Climb");
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }
}
