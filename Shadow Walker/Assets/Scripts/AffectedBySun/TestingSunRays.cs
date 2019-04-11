using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingSunRays : MonoBehaviour
{

    private GameObject sun;
    [SerializeField]
    private LayerMask obstacleLayer;

    private PolygonCollider2D col;
    private Vector2[] colPoints;
    public bool isExposedToSunlight;

    void Start()
    {
        col = GetComponent<PolygonCollider2D>();
        colPoints = col.points;
        sun = GameObject.Find("Sun");
        isExposedToSunlight = false;
    }

    void Update()
    {

        for(int i = 0; i < colPoints.Length; i++ )
        {
            //Vector3 polygonPoint = new Vector3(transform.position.x + colPoints[i].x, transform.position.y + colPoints[i].y);
            Vector3 polygonPoint = transform.TransformPoint(colPoints[i]);
            Vector2 DirToSun = (sun.transform.position - polygonPoint).normalized;
            float DistanceToSun = Vector2.Distance(transform.position, sun.transform.position);

            if(!Physics2D.Raycast(polygonPoint, DirToSun,DistanceToSun,obstacleLayer))
            {
                isExposedToSunlight = true;
                Debug.DrawLine(polygonPoint, sun.transform.position, Color.red);
            }
            else
            {
                isExposedToSunlight = false;
                Debug.DrawLine(polygonPoint, sun.transform.position, Color.blue);
            }
        }
    }
}
