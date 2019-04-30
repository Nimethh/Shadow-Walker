﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSunBehavior : AffectedByTheSun
{
    private GameObject startingPoint;

    //private PlayerController playerController;
    private PlayerPlatformController playerPlatformController;

    public void Start()
    {
        AffectedByTheSunScriptStart();

        //playerController = GetComponent<PlayerController>();
        playerPlatformController = GetComponent<PlayerPlatformController>();

        startingPoint = GameObject.Find("PrototypeStartingPoint");
    }

    public void Update()
    {
        AffectedByTheSunScriptUpdate();
    }

    public override void JustGotCoveredFromSunlight()
    {
        //Debug.Log("JustGotCoveredFromSunlight()");
    }

    public override void JustGotExposedToSunlight()
    {
        //playerController.StopAllMovement(0.8f);
        playerPlatformController.StopAllMovement(0.8f);
        transform.position = startingPoint.transform.position;
        //Debug.Log("JustGotExposedToSunlight()");

    }

    public override void UnderFullCover()
    {
        //Debug.Log("UnderFullCover()");
    }

    public override void UnderFullExposure()
    {
        //Debug.Log("UnderFullExposure()");
    }

    public override void UnderPartialCover()
    {
        //Debug.Log("UnderPartialCover()");
    }

}
