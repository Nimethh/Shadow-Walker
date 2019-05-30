using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointMobile : MonoBehaviour
{
    private PlayerSunBehaviorUpdatedMobile playerSunBehavior;
    //Player player;
    PlayerUpdatedMobile player;
    //private SunController sunController;
    //public int sunCheckPointIndex;
    AudioManager audioManager;

    bool safePointSoundPlayer = false;

    void Start()
    {
        playerSunBehavior = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSunBehaviorUpdatedMobile>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUpdatedMobile>();
        audioManager = FindObjectOfType<AudioManager>();
        //sunController = GameObject.FindGameObjectWithTag("Sun").GetComponent<SunController>();
        //sunCheckPointIndex = sunController.checkPointIndex;
    }

    void Update()
    {
        //if (playerSunBehavior.isExposedToSunlight)
        //{
        //    SpawnPlayer();

        //    //sunController.canMove = false;
        //    //MoveSunToCheckPointPos();
        //}
        //else
        //{
        //    sunController.canMove = true;
        //}
    }

    void SpawnPlayer()
    {
        playerSunBehavior.gameObject.SetActive(true);
        playerSunBehavior.gameObject.transform.position = playerSunBehavior.spawningPos;
        player.landed = false;
        player.velocity.y = 0;
        //sunController.canMove = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Vector3 position = this.gameObject.transform.position;
            position.z = -3;
            playerSunBehavior.spawningPos = position;
            if (!safePointSoundPlayer)
            {
                audioManager.Play("SafePoint");
                safePointSoundPlayer = true;
            }
        }
    }

    //public void MoveSunToCheckPointPos()
    //{
    //    if (sunController.index > sunController.checkPointIndex)
    //    {
    //        sunController.MoveLeftToCheckPointPos();
    //        if (sunController.index <= sunController.checkPointIndex)
    //        {
    //            SpawnPlayer();
    //        }
    //    }
    //    else if (sunController.index < sunController.checkPointIndex)
    //    {
    //        sunController.MoveRightToCheckPointPos();
    //        if (sunController.index >= sunController.checkPointIndex)
    //        {
    //            SpawnPlayer();
    //        }
    //    }
    //    else
    //    {
    //        SpawnPlayer();
    //    }
    //}
}
