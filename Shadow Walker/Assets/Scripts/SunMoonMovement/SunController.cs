using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    [SerializeField]
    private Transform[] points;
    [SerializeField]
    private int index;

    private int numberOfPoints = 100;


    [SerializeField]
    private Transform centerPosition;

    private void Start()
    {
        //lineRenderer.positionCount = numberOfPoints;
        //transform.position = points[0].position;
        float startT = index / (float)numberOfPoints * 0.5f;
        transform.position = CalculateQuadraticBezeirPoint(startT, points[0].position, points[1].position, points[2].position);
        //DrawQuadraticCurve();
    }

    private void FixedUpdate()
    {
        //if ( Input.GetAxis("Mouse X") > 0 && transform.position.x < points[2].position.x)
        //{
        //    MoveRight();
        //}
        //if (Input.GetAxis("Mouse X") < 0 && transform.position.x > points[0].position.x)
        //{
        //    MoveLeft();
        //}

        if (Input.GetKey(KeyCode.L) && transform.position.x < points[2].position.x)
        {
            MoveRight();
        }
        if (Input.GetKey(KeyCode.J) && transform.position.x > points[0].position.x)
        {
            MoveLeft();
        }

        //Rotate();

    }

    void MoveRight()
    {
        index++;
        float t = index / (float)numberOfPoints * 0.5f;
        transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
    }

    void MoveLeft()
    {
        index--;
        float t = index / (float)numberOfPoints * 0.5f;
        transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
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
