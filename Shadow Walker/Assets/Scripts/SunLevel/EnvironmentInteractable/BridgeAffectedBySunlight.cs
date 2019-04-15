using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeAffectedBySunlight : AffectedByTheSun
{
    public GameObject bridgeObject;
    public bool bridgeActive;

    private PolygonCollider2D jumpCollider;
    private PolygonCollider2D shadowCastingCollider;

    void Start()
    {
        shadowCastingCollider = GetComponent<PolygonCollider2D>();
        jumpCollider = transform.parent.GetComponent<PolygonCollider2D>();
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
        shadowCastingCollider.enabled = true;
        jumpCollider.enabled = true;
        Debug.Log("Bridge JustGotCoveredFromSunlight()");
    }

    public override void JustGotExposedToSunlight()
    {
        bridgeActive = false;
        bridgeObject.SetActive(false);
        shadowCastingCollider.enabled = false;
        jumpCollider.enabled = false;
        Debug.Log("Bridge JustGotExposedToSunlight()");
    }

    public override void UnderFullCover()
    {
        Debug.Log("Bridge UnderFullCover()");
    }

    public override void UnderFullExposure()
    {
        Debug.Log("Bridge UnderFullExposure()");
    }

    public override void UnderPartialCover()
    {
        Debug.Log("Bridge UnderPartialCover()");
    }

}
