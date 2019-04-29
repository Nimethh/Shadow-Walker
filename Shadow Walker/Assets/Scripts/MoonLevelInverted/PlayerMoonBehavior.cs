using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoonBehavior : AffectedByTheSun
{
    private GameObject startingPoint;

    private PlayerController playerController;

    private float timeBeingInDarkness;
    [SerializeField]
    private float timeInShadowAllowed;


    public void Start()
    {
        AffectedByTheSunScriptStart();

        playerController = GetComponent<PlayerController>();
        startingPoint = GameObject.Find("PrototypeStartingPoint");
        timeBeingInDarkness = 0;
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
        //Debug.Log("JustGotExposedToSunlight()");
    }

    public override void UnderFullCover()
    {
        timeBeingInDarkness += Time.deltaTime;
        if (timeBeingInDarkness > timeInShadowAllowed)
        {
            playerController.StopAllMovement(0.8f);
            transform.position = startingPoint.transform.position;
        }
        //Debug.Log("UnderFullCover()");
    }

    public override void UnderFullExposure()
    {
        //Debug.Log("UnderFullExposure()");
        if(timeBeingInDarkness > 0)
        {
            timeBeingInDarkness = 0;
        }
    }

    public override void UnderPartialCover()
    {
        timeBeingInDarkness += Time.deltaTime;
        if(timeBeingInDarkness > timeInShadowAllowed)
        {
            playerController.StopAllMovement(0.8f);
            transform.position = startingPoint.transform.position;
        }
        //Debug.Log("UnderPartialCover()");
    }

}
