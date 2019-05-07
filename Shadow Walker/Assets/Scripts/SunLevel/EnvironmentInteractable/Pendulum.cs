using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{

    [SerializeField, Range(0.0f, 360f)] private float angle = 90.0f;
    [SerializeField, Range(0.0f, 5f)] private float speed = 2.0f;

    Quaternion start;
    Quaternion end;

    private float startTime = 0.0f;

    private void Start()
    {
        start = pendulumRotation(angle);
        end = pendulumRotation(-angle);

    }

    private void FixedUpdate()
    {
        startTime += Time.fixedDeltaTime;
        transform.rotation = Quaternion.Lerp(start, end, (Mathf.Sin(startTime * speed + Mathf.PI / 2) + 1.0f) / 2.0f);
    }

    void ResetTime()
    {
        startTime = 0.0f;
    }

    Quaternion pendulumRotation(float angle)
    {
        var pendulumRotation = transform.rotation;
        var angleZ = pendulumRotation.eulerAngles.z + angle;

        if (angleZ > 180)
            angleZ -= 360;
        else if (angleZ < -180)
            angleZ += 360;

        pendulumRotation.eulerAngles = new Vector3(pendulumRotation.eulerAngles.x, pendulumRotation.eulerAngles.y, angleZ);
        return pendulumRotation;
    }
}
