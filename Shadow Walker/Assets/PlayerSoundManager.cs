using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private Vector2 directionalInput;


    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void JumpSound()
    {
        FindObjectOfType<AudioManager>().Play("Jump");
    }

    public void WalkSound()
    {
        if (directionalInput.x > 0 || directionalInput.x < 0)
        {
            FindObjectOfType<AudioManager>().Play("Walk");
        }
    }

    public void DeathSound()
    {
        FindObjectOfType<AudioManager>().Play("Death");
    }

    public void LandingSound()
    {
        FindObjectOfType<AudioManager>().Play("Land");
    }

    public void PlayClimbingSound()
    {
        if(directionalInput.y > 0 || directionalInput.y < 0)
        {
            FindObjectOfType<AudioManager>().Play("Climb");
        }
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }
}
