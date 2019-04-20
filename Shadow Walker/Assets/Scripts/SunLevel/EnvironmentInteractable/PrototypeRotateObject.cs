using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeRotateObject : MonoBehaviour
{

    public bool activated;

    private float rotation;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private bool rotateClockwise;

    void Update()
    {
        if (!activated)
        {
            return;
        }

        if(rotateClockwise)
        {
            rotation -= rotationSpeed;

            if (rotation < -360)
                rotation += 360;
        }
        else
        {
        rotation += rotationSpeed;

            if (rotation > 360)
                rotation -= 360;
        }

        Quaternion rot = Quaternion.Euler(0, 0, rotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
    }
}
