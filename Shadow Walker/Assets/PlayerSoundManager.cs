using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private Vector2 directionalInput;
    private Player player;
    private PlayerSunBehaviorUpdated playerSunBehaviorUpdated;
    //PlayerSunBehavior playerSunBehavior;
    Controller2D controller;
    AudioManager audioManager;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        player = GetComponent<Player>();
        playerSunBehaviorUpdated = GetComponent<PlayerSunBehaviorUpdated>();
        //playerSunBehavior = GetComponent<PlayerSunBehavior>();
        controller = GetComponent<Controller2D>();
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
