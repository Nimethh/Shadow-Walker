using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMoonLightBehaviour : AffectedByMoonRay
{
    private Vector3 lightPosActivated = new Vector3(0,0,-1);
    private Vector3 lightPosDeactivated = new Vector3(0, 0, 100);
    Light objectLight;

    public override void JustGotHitByMoonLight()
    {
        //Debug.Log("Mirror just got hit by moon light");
        objectLight.transform.position = lightPosActivated;
    }

    public override void JustStoppedBeingHitByMoonLight()
    {
        //Debug.Log("Mirror just Stopped being hit by moon light");
        objectLight.transform.position = lightPosDeactivated;
    }

    public override void ObjectHitByMoonLight()
    {
        //Debug.Log("Mirror is being hit by moon light");
        objectLight.transform.position = lightPosActivated;
    }

    public override void ObjectNotHitByMoonLight()
    {
        //Debug.Log("Mirror is not being hit by the moon light");
        objectLight.transform.position = lightPosDeactivated;
    }

    void Start()
    {
        AffectedByMoonRayStart();
        setUpLightComponent();
    }

    void Update()
    {
        AffectedByMoonRayUpdate();
    }
    
    void setUpLightComponent()
    {
        objectLight = this.gameObject.GetComponentInChildren<Light>();
        lightPosDeactivated = this.transform.position;
        lightPosActivated = this.transform.position;
        lightPosActivated.z = 4;
        lightPosDeactivated.z = 105;
        objectLight.transform.position = lightPosDeactivated;
    }
}
