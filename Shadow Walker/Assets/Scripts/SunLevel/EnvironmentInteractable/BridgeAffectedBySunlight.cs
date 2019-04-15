using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeAffectedBySunlight : AffectedByTheSun
{
    public GameObject bridgeObject;
    public bool bridgeActive;

    void Start()
    {
        AffectedByTheSunScriptStart();
    }

    void Update()
    {
        AffectedByTheSunScriptUpdate();
    }


    public override void JustGotCoveredFromSunlight()
    {
        bridgeActive = true;
        bridgeObject.SetActive(true);
        Debug.Log("Button JustGotCoveredFromSunlight()");
    }

    public override void JustGotExposedToSunlight()
    {
        bridgeActive = false;
        bridgeObject.SetActive(false);
        Debug.Log("Button JustGotExposedToSunlight()");
    }

    public override void UnderFullCover()
    {
        Debug.Log("Button UnderFullCover()");
    }

    public override void UnderFullExposure()
    {
        Debug.Log("Button UnderFullExposure()");
    }

    public override void UnderPartialCover()
    {
        Debug.Log("Button UnderPartialCover()");
    }

}
