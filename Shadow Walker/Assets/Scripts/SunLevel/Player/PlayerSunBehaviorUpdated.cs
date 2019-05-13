using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSunBehaviorUpdated : AffectedByTheSun
{
    private GameObject startingPoint;
    public Transform spawningPos;
    //private PlayerController playerController;
    private PlayerPlatformController playerPlatformController;
    private float timeInSun;
    [SerializeField]
    private float timeInSunAllowed;

    public void Start()
    {
        AffectedByTheSunScriptStart();

        //playerController = GetComponent<PlayerController>();
        playerPlatformController = GetComponent<PlayerPlatformController>();

        startingPoint = GameObject.Find("PrototypeStartingPoint");
        timeInSun = 0;
        spawningPos = startingPoint.transform;

    }

    public void Update()
    {
        AffectedByTheSunScriptUpdate();
    }

    public override void JustGotCoveredFromSunlight()
    {
        if (timeInSun > 0)
        {
            timeInSun = 0;
        }
        //Debug.Log("JustGotCoveredFromSunlight()");
    }

    public override void JustGotExposedToSunlight()
    {

        timeInSun += Time.deltaTime;
        if (timeInSun > timeInSunAllowed)
        {
            //playerPlatformController.StopAllMovement(0.8f);
            transform.position = spawningPos.transform.position;
        }

        //playerController.StopAllMovement(0.8f);
        //playerPlatformController.StopAllMovement(0.8f);
        //transform.position = startingPoint.transform.position;
        //Debug.Log("JustGotExposedToSunlight()");

    }

    public override void UnderFullCover()
    {
        //Debug.Log("UnderFullCover()");
    }

    public override void UnderFullExposure()
    {
        //Debug.Log("UnderFullExposure()");
        timeInSun += Time.deltaTime;
        if (timeInSun > timeInSunAllowed)
        {
            //playerPlatformController.StopAllMovement(0.8f);
            transform.position = spawningPos.transform.position;
        }
    }

    public override void UnderPartialCover()
    {
        //Debug.Log("UnderPartialCover()");
        timeInSun += Time.deltaTime;
        if (timeInSun > timeInSunAllowed)
        {
            //playerPlatformController.StopAllMovement(0.8f);
            transform.position = spawningPos.transform.position;
        }
    }

}
