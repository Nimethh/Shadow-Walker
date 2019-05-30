﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private PlayerSunBehaviorUpdated playerSunBehavior;
    //Player player;
    PlayerUpdated player;
    //private SunController sunController;
    //public int sunCheckPointIndex;
    AudioManager audioManager;

    BoxCollider2D collider;

    void Start()
    {
        playerSunBehavior = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSunBehaviorUpdated>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUpdated>();
        audioManager = FindObjectOfType<AudioManager>();
        //sunController = GameObject.FindGameObjectWithTag("Sun").GetComponent<SunController>();
        //sunCheckPointIndex = sunController.checkPointIndex;
        collider = this.gameObject.GetComponent<BoxCollider2D>();
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
            audioManager.Play("SafePoint");
            //collider.enabled = false;
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
