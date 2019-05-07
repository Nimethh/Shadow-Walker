using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBridge : AffectedByTheSun
{
    public GameObject bridgeObject;
    public bool bridgeActive;

    private PolygonCollider2D jumpCollider;
    private PolygonCollider2D shadowCastingCollider;


    public GameObject dematerialize;
    private DematerializeDown dematerializeScript;
    public GameObject rematerialize;
    private RematerializeUpwards rematerializeScript;


    void Start()
    {
        shadowCastingCollider = GetComponent<PolygonCollider2D>();
        jumpCollider = transform.parent.GetComponent<PolygonCollider2D>();
        rematerializeScript = GetComponent<RematerializeUpwards>();
        dematerializeScript = GetComponent<DematerializeDown>();

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
        dematerialize.SetActive(false);
        rematerializeScript.StartRematerializing();
        rematerialize.SetActive(true);

        //Debug.Log("Bridge JustGotCoveredFromSunlight()");
    }

    public override void JustGotExposedToSunlight()
    {
        bridgeActive = false;
        bridgeObject.SetActive(false);
        shadowCastingCollider.enabled = false;
        jumpCollider.enabled = false;

        rematerialize.SetActive(false);
        dematerializeScript.StartDissolving();
        dematerialize.SetActive(true);


        //Debug.Log("Bridge JustGotExposedToSunlight()");
    }

    public override void UnderFullCover()
    {
        //Debug.Log("Bridge UnderFullCover()");
    }

    public override void UnderFullExposure()
    {
        //Debug.Log("Bridge UnderFullExposure()");
    }

    public override void UnderPartialCover()
    {
        //Debug.Log("Bridge UnderPartialCover()");
    }

}
