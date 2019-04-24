using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingObject : MonoBehaviour
{
    public bool canBeLit;
    public Vector3 lightPos;
    Light objectLight;

    private void Start()
    {
        objectLight = this.gameObject.GetComponentInChildren<Light>();
        objectLight.transform.position = this.transform.position;
    }
    public void LightUpObject(RaycastHit2D hitPoint)
    {
        if (canBeLit == true)
        {
            lightPos = hitPoint.point;
            lightPos.z = -1;
            objectLight.transform.position = lightPos;
            objectLight.gameObject.SetActive(true);
        }
        else
        {
            objectLight.gameObject.SetActive(false);
        }
    }
}
