using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMoonLightBehaviour : AffectedByMoonRay
{
    public override void JustGotHitByMoonLight()
    {
        //Debug.Log("Mirror just got hit by moon light");
    }

    public override void JustStoppedBeingHitByMoonLight()
    {
        //Debug.Log("Mirror just Stopped being hit by moon light");
    }

    public override void ObjectHitByMoonLight()
    {
        //Debug.Log("Mirror is being hit by moon light");
    }

    public override void ObjectNotHitByMoonLight()
    {
        //Debug.Log("Mirror is not being hit by the moon light");
    }

    void Start()
    {
        AffectedByMoonRayStart();
    }

    void Update()
    {
        AffectedByMoonRayUpdate();
    }
}
