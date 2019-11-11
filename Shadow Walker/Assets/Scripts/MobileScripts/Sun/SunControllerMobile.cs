using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunControllerMobile : MonoBehaviour
{
    [SerializeField]
    private Transform[] points;

    //public int checkPointIndex = 100;
    public int index;
    private int numberOfPoints = 100;

    float startT;
    float velocity;
    float t = 0.0f;
    [Range(0f, 1f)]
    [SerializeField]
    private float sunSpeed = 0.2f;
    [SerializeField]
    private float maxVelocity = 0.7f;

    [SerializeField]
    private bool invertTiltController = false;

    AudioManager audioManager;

    //public VirtualSunJoystick sunJoystick;

    private void Start()
    {
        startT = index / (float)numberOfPoints * sunSpeed;
        transform.position = CalculateQuadraticBezeirPoint(startT, points[0].position, points[1].position, points[2].position);
        audioManager = FindObjectOfType<AudioManager>();
        velocity = 0.0f; 
    }

    private void FixedUpdate()
    {

        /*if (sunJoystick.Horizontal() > 0.4f && transform.position.x < points[2].position.x)
        {
            MoveRight();
        }
        if (sunJoystick.Horizontal() < -0.4f && transform.position.x > points[0].position.x)
        {
            MoveLeft();
        }

        if (sunJoystick.Horizontal() > 0.4f && transform.position.x < points[2].position.x)
        {
            MoveRight();
        }
        else if (sunJoystick.Horizontal() < -0.4f && transform.position.x > points[0].position.x)
        {
            MoveLeft();
        }*/

        velocity = Input.acceleration.x;

        if(invertTiltController == true)
        {
            if (velocity < -0.15f && transform.position.x > points[0].position.x)
            {
                MoveLeft();
            }
            else if(velocity > 0.15f && transform.position.x < points[2].position.x)
            {
                MoveRight();
            }
            else
            {
                audioManager.Mute("SunMoving");
            }
        }
        else if(invertTiltController == false)
        {
            if(velocity < -0.15f && transform.position.x < points[2].position.x)
            {
                MoveRight();
            }
            else if(velocity > 0.15f && transform.position.x > points[0].position.x)
            {
                MoveLeft();
            }
            else
            {
                audioManager.Mute("SunMoving");
            }
        }
    }

    void MoveRight()
    {

        velocity = Mathf.Clamp(velocity, -maxVelocity, maxVelocity);
        t = t + (velocity * velocity) * 0.02f;
        transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
        audioManager.Play("SunMoving");
    }

    void MoveLeft()
    {
        velocity = Mathf.Clamp(velocity, -maxVelocity, maxVelocity);
        t = t - (velocity * velocity) * 0.02f;
        transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
        audioManager.Play("SunMoving");
    }

    Vector3 CalculateQuadraticBezeirPoint(float t, Vector3 point1, Vector3 point2, Vector3 point3)
    {
        float i = 1 - t;
        float tSquared = t * t;
        float iSquared = i * i;

        Vector3 point = (iSquared * point1) + (2 * i * t * point2) + (tSquared * point3);

        return point;
    }
}
