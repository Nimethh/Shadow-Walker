using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlfriendAnmationManager : MonoBehaviour
{
    PlayerUpdated player;
    PlayerInputUpdated playerInput;
    GameObject logo;
    SceneTransition sceneTransition;
    
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerUpdated>();
        playerInput = GameObject.Find("Player").GetComponent<PlayerInputUpdated>();
        logo = GameObject.Find("Logo");
        logo.SetActive(false);
        sceneTransition = GameObject.Find("SceneManager").GetComponent<SceneTransition>();
    }

    void PlayLogoAnimation()
    {
        logo.SetActive(true);
    }

    void GoToNextLevel()
    {
        sceneTransition.goToNextScene = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(gameObject.name == "PlayerDetection" && other.gameObject.CompareTag("Player"))
        {
            Animator anim = GetComponentInParent<Animator>();
            anim.SetTrigger("Turn");
        }
        else if(gameObject.name == "Girlfriend" && other.gameObject.CompareTag("Player"))
        {
            player.enabled = false;
            playerInput.enabled = false;
            Animator anim = GetComponent<Animator>();
            anim.SetTrigger("Turn");
            Invoke("PlayLogoAnimation", 3);
            Invoke("GoToNextLevel", 12);
        }
    }
}
