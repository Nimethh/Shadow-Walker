using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBridgeAnimated : AffectedByTheSun
{
    public GameObject bridgeObject;
    public bool bridgeActive;

    private Animator anim;



    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("ShouldBeOut", true);
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
        //jumpCollider.enabled = true;

        //Debug.Log("Bridge JustGotCoveredFromSunlight()");
        //anim.SetTrigger("StartToAppear");
        anim.SetBool("ShouldBeOut", true);

    }

    public override void JustGotExposedToSunlight()
    {
        bridgeActive = false;
        bridgeObject.SetActive(false);
        //jumpCollider.enabled = false;


        //Debug.Log("Bridge JustGotExposedToSunlight()");
        //anim.SetTrigger("StartToDisappear");
        anim.SetBool("ShouldBeOut", false);

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