using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    Vector2 directionalInput;

    PlayerUpdated player;
    PlayerSunBehaviorUpdated playerSunBehavior;
    Controller2DUpdated controller;

    AudioManager audioManager;

    private void Start()
    {
        player = GetComponent<PlayerUpdated>();
        playerSunBehavior = GetComponent<PlayerSunBehaviorUpdated>();
        controller = GetComponent<Controller2DUpdated>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        WalkSound();
        PlayClimbingSound();
        JumpSound();
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
        audioManager.Play("Death");
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
        if(Input.GetKeyDown(KeyCode.Space) && !player.spawnedInSafePoint && !playerSunBehavior.isDead)
        {
            audioManager.Play("Jump");
        }
    }

    public void PlayClimbingSound()
    {
        if( (directionalInput.y > 0 || directionalInput.y < 0) 
            && controller.collisionInfo.climbing && !player.spawnedInSafePoint && !playerSunBehavior.isDead)
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
