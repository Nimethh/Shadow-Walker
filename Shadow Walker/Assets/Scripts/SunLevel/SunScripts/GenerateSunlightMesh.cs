using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSunlightMesh : MonoBehaviour
{
    public List<Vector2> colliderEdges;
    public List<ObstacleAngles> meshPoints;

    public Collider2D[] obstaclesVisibileOnTheScreen;
    public LayerMask obstacleLayer;

    void Start()
    {
        obstacleLayer = LayerMask.GetMask("Obstacle");
        if (obstacleLayer.value == 0)
        {
            Debug.Log("Make sure you've spelled the LayerMask.GetMask correctly, It appears to be the default one");
        }
    }

    void LateUpdate() //Want it to be late update, so that everything has moved when we create the mesh
    {
        Vector3 tempTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
        Vector3 tempBotLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));

        Vector2 topRight = new Vector2(tempTopRight.x +5, tempTopRight.y +5);
        Vector2 topLeft = new Vector2(tempBotLeft.x -5, tempTopRight.y +5);
        Vector2 bottomRight = new Vector2(tempTopRight.x +5, tempBotLeft.y -5);
        Vector2 bottomLeft = new Vector2(tempBotLeft.x -5, tempBotLeft.y -5);

        Debug.DrawLine(transform.position, topRight, Color.cyan);
        Debug.DrawLine(transform.position, topLeft, Color.cyan);
        Debug.DrawLine(transform.position, bottomRight, Color.cyan);
        Debug.DrawLine(transform.position, bottomLeft, Color.cyan);

        FindAllObstaclesOnTheScreen();
        FindAndStoreAllObstacleEdgesInWorldCoordinates();

        ConvertObstacleEdgesToObstacleAngles();
    }

    public void FindAllObstaclesOnTheScreen()
    {
        Vector3 tempTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
        Vector3 tempBotLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));

        Vector2 topRight = new Vector2(tempTopRight.x + 5, tempTopRight.y + 5);
        Vector2 bottomLeft = new Vector2(tempBotLeft.x - 5, tempBotLeft.y - 5);

        obstaclesVisibileOnTheScreen = Physics2D.OverlapAreaAll(topRight, bottomLeft, obstacleLayer); //Could make this more efficient by using NonAlloc?
    }

    public void FindAndStoreAllObstacleEdgesInWorldCoordinates()
    {
        //TODO: Clear the list of all the obstacle edges
        colliderEdges.Clear();
        //Add all the persistent points (sun, world edges)

        PolygonCollider2D tempCollider;
        Transform tempTransform;

        for(int i = 0; i < obstaclesVisibileOnTheScreen.Length; i++)
        {
            tempCollider = obstaclesVisibileOnTheScreen[i].GetComponent<PolygonCollider2D>();
            tempTransform = obstaclesVisibileOnTheScreen[i].GetComponent<Transform>();

            if (tempCollider == null)
            {
                Debug.Log("Could not find PolygonCollider2D on an obstacle. Check if all obstacles has one or if another object is using the Obstacle-LayerMask.");
                return;
            }

            for(int j = 0; j < tempCollider.points.Length; j++)
            {
                colliderEdges.Add(tempTransform.TransformPoint(tempCollider.points[j]));
            }

            ////Debugging all the corners
            //for (int k = 0; k < colliderEdges.Count; k++)
            //{
            //    Debug.DrawLine(transform.position, colliderEdges[k], Color.green);
            //}
        }
    }

    public void ConvertObstacleEdgesToObstacleAngles()
    {

        float angle = 0;

        for(int i = 0; i < colliderEdges.Count; i++)
        {
            angle = Vector2.Angle(transform.position,colliderEdges[i]);
            Vector3 crossProduct = Vector3.Cross(transform.position, colliderEdges[i]);
            if(crossProduct.z > 0)
            {
                angle = 360 - angle;
            }


            Debug.Log("Angle: " + angle);

            Debug.DrawLine(transform.position, new Vector2(DirectionFromAngle(angle).x + transform.position.x, DirectionFromAngle(angle).y + transform.position.y));

            //ObstacleAngles newObstacleAngle = new ObstacleAngles(angle, 15);
            //meshPoints.Add(newObstacleAngle);
        }
    }

    public Vector2 DirectionFromAngle(float angleInDegrees)
    {
        return new Vector2(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ObstacleAngles
    {
        public ObstacleAngles(float _angleInDegrees, float _distance)
        {
        angle = _angleInDegrees;
        distance = _distance;
        }

        public float angle;
        public float distance;
    }


}
