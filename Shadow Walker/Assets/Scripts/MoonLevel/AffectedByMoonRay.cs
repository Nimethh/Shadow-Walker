using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AffectedByMoonRay : MonoBehaviour
{
    public bool isHitByMoonLight;
    public bool wasPreviouslyHitByMoonLight;

    public bool justGotHitByMoonLight;
    public bool justStoppedBeingHitByMoonLight;


    public void AffectedByMoonRayStart()
    {
        isHitByMoonLight = false;
        wasPreviouslyHitByMoonLight = false;
        justGotHitByMoonLight = false;
        justStoppedBeingHitByMoonLight = false;
    }

    public void AffectedByMoonRayUpdate()
    {
        UpdateObjectBehaviour();
    }

    
    public void UpdateObjectBehaviour()
    {
        if(isHitByMoonLight)
        {
            if(wasPreviouslyHitByMoonLight)
            {
                ObjectHitByMoonLight();
            }
            else
            {
                JustGotHitByMoonLight();
            }

            wasPreviouslyHitByMoonLight = true;
        }
        else
        {
            if(wasPreviouslyHitByMoonLight)
            {
                JustStoppedBeingHitByMoonLight();
            }
            else
            {
                ObjectNotHitByMoonLight();
            }

            wasPreviouslyHitByMoonLight = false;
        }


    }

    public abstract void JustGotHitByMoonLight();
    public abstract void JustStoppedBeingHitByMoonLight();
    public abstract void ObjectHitByMoonLight();
    public abstract void ObjectNotHitByMoonLight();
}
