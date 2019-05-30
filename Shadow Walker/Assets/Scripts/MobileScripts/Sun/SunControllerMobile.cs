using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunControllerMobile : MonoBehaviour
{
    public VirtualSunJoystick sunJoystick;

    [SerializeField]
    private Transform[] points;
    [SerializeField]
    private int index;

    private int numberOfPoints = 100;

    [SerializeField]
    private Transform centerPosition;

    float right = 0;
    float left = 0;

    [Range(0f, 1f)]
    [SerializeField]
    private float sunSpeed = 0.2f;

    AudioManager audioManager;
    [SerializeField]
    bool restrictMovement = false;

    private void Start()
    {
        //lineRenderer.positionCount = numberOfPoints;
        //transform.position = points[0].position;
        float startT = index / (float)numberOfPoints * 0.5f;
        transform.position = CalculateQuadraticBezeirPoint(startT, points[0].position, points[1].position, points[2].position);
        audioManager = FindObjectOfType<AudioManager>();
        FindSunBounds();

        //DrawQuadraticCurve();
    }

    private void FixedUpdate()
    {
        if (sunJoystick.Horizontal() > 0.4f && transform.position.x < points[2].position.x)
        {
            MoveRight();
        }

        if (sunJoystick.Horizontal() < -0.4f && transform.position.x > points[0].position.x)
        {
            MoveLeft();
        }
        else
        {
            audioManager.Mute("SunMoving");
            //FindObjectOfType<AudioManager>().Stop("SunMoving");
        }
    }

    public void FindSunBounds()
    {
        right = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane)).x;
        left = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)).x;
    }

    void MoveRight()
    {
        //index++;
        //float t = index / (float)numberOfPoints * 0.15f;
        //transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
        if (restrictMovement)
        {
            if (transform.position.x < right - 0.2f)
            {
                index++;
                float t = index / (float)numberOfPoints * sunSpeed;
                transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
                audioManager.Play("SunMoving");
            }
        }
        else
        {
            index++;
            float t = index / (float)numberOfPoints * sunSpeed;
            transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
            audioManager.Play("SunMoving");
        }
    }

    void MoveLeft()
    {
        //index--;
        //float t = index / (float)numberOfPoints * 0.15f;
        //transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
        if (restrictMovement)
        {
            if (transform.position.x > left + 0.2f)
            {
                index--;
                float t = index / (float)numberOfPoints * sunSpeed;
                transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
                audioManager.Play("SunMoving");
            }
        }
        else
        {
            index--;
            float t = index / (float)numberOfPoints * sunSpeed;
            transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
            audioManager.Play("SunMoving");
        }
    }

    void Rotate()
    {
        transform.LookAt(centerPosition);
    }

    //void DrawQuadraticCurve()
    //{
    //    for (int i = 1; i < numberOfPoints + 1; i++)
    //    {
    //        float t = i / (float)numberOfPoints;
    //        positions[i - 1] = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
    //    }
    //    lineRenderer.SetPositions(positions);
    //}

    Vector3 CalculateQuadraticBezeirPoint(float t, Vector3 point1, Vector3 point2, Vector3 point3)
    {
        float i = 1 - t;
        float tSquared = t * t;
        float iSquared = i * i;

        Vector3 point = (iSquared * point1) + (2 * i * t * point2) + (tSquared * point3);

        return point;
    }

}
