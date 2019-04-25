using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampMoonLightBehaviour : AffectedByMoonRay
{

    public MovingPlatformWithEasingAndShake platform;

    public override void JustGotHitByMoonLight()
    {
            platform.activated = true;
    }

    public override void JustStoppedBeingHitByMoonLight()
    {
        platform.activated = false;
    }

    public override void ObjectHitByMoonLight()
    {
        Debug.Log("ObjectHitByMoonLight");
    }

    public override void ObjectNotHitByMoonLight()
    {
        Debug.Log("ObjectNotHitByMoonLight");
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
