using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManagerMobileOld : MonoBehaviour
{
    //private PlayerInput playerInput;
    PlayerInputUpdatedMobile playerInput;
    Vector2 directionalInput;
    //private Player player;
    PlayerUpdatedMobile player;
    PlayerSunBehaviorUpdatedMobile playerSunBehaviorUpdated;
    //PlayerSunBehavior playerSunBehavior;
    //Controller2D controller;
    Controller2DUpdatedMobile controller;
    AudioManager audioManager;

    public VirtualMovementJoystick movementJoystick;

    private void Start()
    {
        //playerInput = GetComponent<PlayerInput>();
        playerInput = GetComponent<PlayerInputUpdatedMobile>();
        //player = GetComponent<Player>();
        player = GetComponent<PlayerUpdatedMobile>();
        playerSunBehaviorUpdated = GetComponent<PlayerSunBehaviorUpdatedMobile>();
        //playerSunBehavior = GetComponent<PlayerSunBehavior>();
        //controller = GetComponent<Controller2D>();
        controller = GetComponent<Controller2DUpdatedMobile>();
        audioManager = FindObjectOfType<AudioManager>();
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
            audioManager.Play("Walk");
        }
        else
        {
            audioManager.Stop("Walk");
        }
    }

    public void DeathSound()
    {
        //if (playerSunBehaviorUpdated.isExposedToSunlight)
        //{
        //    FindObjectOfType<AudioManager>().Play("Death");
        //}
        if (playerSunBehaviorUpdated.isExposedToSunlight)
        {
            audioManager.Play("Death");
        }
    }

    public void LandingSound()
    {
        if (player.landed)
        {
            audioManager.Play("Land");
            player.landed = false;
        }
    }

    void JumpSound()
    {
        if(movementJoystick.jump)
        {
            audioManager.Play("Jump");
        }
    }

    public void PlayClimbingSound()
    {
        if((directionalInput.y > 0 || directionalInput.y < 0) && controller.collisionInfo.climbing)
        {
            audioManager.Play("Climb");
        }
        else
        {
            audioManager.Stop("Climb");
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }
}
