using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    //private PlayerInput playerInput;
    PlayerInputUpdated playerInput;
    Vector2 directionalInput;
    //private Player player;
    PlayerUpdated player;
    PlayerSunBehaviorUpdated playerSunBehavior;
    //PlayerSunBehavior playerSunBehavior;
    //Controller2D controller;
    Controller2DUpdated controller;
    AudioManager audioManager;

    private void Start()
    {
        //playerInput = GetComponent<PlayerInput>();
        playerInput = GetComponent<PlayerInputUpdated>();
        //player = GetComponent<Player>();
        player = GetComponent<PlayerUpdated>();
        playerSunBehavior = GetComponent<PlayerSunBehaviorUpdated>();
        //playerSunBehavior = GetComponent<PlayerSunBehavior>();
        //controller = GetComponent<Controller2D>();
        controller = GetComponent<Controller2DUpdated>();
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
        if (playerSunBehavior.isDead)
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
        if(Input.GetKeyDown(KeyCode.Space))
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
