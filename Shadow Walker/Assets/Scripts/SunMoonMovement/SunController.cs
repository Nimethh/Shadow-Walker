using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    [SerializeField]
    private Transform[] points;
    [SerializeField]
    private Transform centerPosition;
    public int index;

    public bool canMove = true;
    public int checkPointIndex = 100;
    private int numberOfPoints = 100;
    float startT;
   

    [Range(0f, 1f)][SerializeField]
    private float sunSpeed = 0.2f;
    private void Start()
    {
        //lineRenderer.positionCount = numberOfPoints;
        //transform.position = points[0].position;
        startT = index / (float)numberOfPoints * sunSpeed;
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



        if (canMove == true)
        {
            if (Input.GetKey(KeyCode.L) && transform.position.x < points[2].position.x)
            {
                MoveRight();
                FindObjectOfType<AudioManager>().Play("SunMoving");
            }
            if (Input.GetKey(KeyCode.J) && transform.position.x > points[0].position.x)
            {
                MoveLeft();
                FindObjectOfType<AudioManager>().Play("SunMoving");
            }
        }
        //else
        //{
        //    if (index > checkPointIndex)
        //    {
        //        Debug.Log("Move Left");
        //        MoveLeftToCheckPointPos();
        //        //if (sunController.index <= sunCheckPointIndex)
        //        //{
        //        //    Debug.Log("still didn't reach the pos");
        //        //    SpawnPlayer();
        //        //    sunController.canMove = true;

        //        //}
        //    }
        //    else if (index < checkPointIndex)
        //    {
        //        Debug.Log("Move right");
        //        MoveRightToCheckPointPos();
        //        //if (sunController.index >= sunCheckPointIndex)
        //        //{
        //        //    Debug.Log("still didn't reach the pos");

        //        //    sunController.canMove = true;
        //        //    SpawnPlayer();
        //        //}
        //    }
        //}
        //Rotate();

    }
    
    void MoveRight()
    {
        index++;
        float t = index / (float)numberOfPoints * sunSpeed;
        transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
    }

    void MoveLeft()
    {
        index--;
        float t = index / (float)numberOfPoints * sunSpeed;
        transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
    }

    public void MoveRightToCheckPointPos()
    {
        index++;
        float t = index / (float)numberOfPoints * sunSpeed /** Time.smoothDeltaTime*/;
        transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
    }

    public void MoveLeftToCheckPointPos()
    {
        index--;
        float t = index / (float)numberOfPoints * sunSpeed /** Time.smoothDeltaTime*/;
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
