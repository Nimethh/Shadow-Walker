using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManagerMobile : MonoBehaviour
{
    //private PlayerInput playerInput;
    PlayerInputUpdatedMobile playerInput;
    Vector2 directionalInput;
    //private Player player;
    PlayerUpdatedMobile player;
    PlayerSunBehaviorUpdatedMobile playerSunBehavior;
    //PlayerSunBehavior playerSunBehavior;
    //Controller2D controller;
    Controller2DUpdatedMobile controller;
    AudioManager audioManager;
    AudioSource aS;

    private void Start()
    {
        //playerInput = GetComponent<PlayerInput>();
        playerInput = GetComponent<PlayerInputUpdatedMobile>();
        //player = GetComponent<Player>();
        player = GetComponent<PlayerUpdatedMobile>();
        playerSunBehavior = GetComponent<PlayerSunBehaviorUpdatedMobile>();
        //playerSunBehavior = GetComponent<PlayerSunBehavior>();
        //controller = GetComponent<Controller2D>();
        controller = GetComponent<Controller2DUpdatedMobile>();
        audioManager = FindObjectOfType<AudioManager>();
        aS = GetComponent<AudioSource>();
    }

    private void Update()
    {
        WalkSound();
        PlayClimbingSound();
        JumpSound();
        //DeathSound();
        LandingSound();
    }

    public void WalkSound()
    {
        if ((directionalInput.x > 0 || directionalInput.x < 0) && player.onGround && !player.spawnedInSafePoint && !playerSunBehavior.isDead)
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
        //if (playerSunBehavior.isDead)
        //{
        audioManager.Play("Death");
        //}
    }

    public void LandingSound()
    {
        if (player.landed && !playerSunBehavior.isDead && !player.spawnedInSafePoint)
        {
            audioManager.Play("Land");
            player.landed = false;
        }
    }

    void JumpSound()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !player.spawnedInSafePoint && !playerSunBehavior.isDead)
        {
            audioManager.Play("Jump");
        }
    }

    public void PlayClimbingSound()
    {
        if ((directionalInput.y > 0 || directionalInput.y < 0) && controller.collisionInfo.climbing && !player.spawnedInSafePoint && !playerSunBehavior.isDead)
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

    public void PlayWalkingOutOFSafePointSound()
    {
        aS.Play();
    }
}
