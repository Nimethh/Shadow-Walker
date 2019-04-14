using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingSunRays : MonoBehaviour
{

    private GameObject startingPoint;
    private GameObject sun;

    [SerializeField]
    private LayerMask obstacleLayer;

    private PolygonCollider2D col;
    private Vector2[] colPoints;

    public bool isExposedToSunlight;

    private Rigidbody2D rigid;
    private PlayerController playerController;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<PolygonCollider2D>();
        playerController = GetComponent<PlayerController>();
        colPoints = col.points;
        sun = GameObject.Find("Sun");
        startingPoint = GameObject.Find("StartingPoint");
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
                //Debug.DrawLine(polygonPoint, sun.transform.position, Color.red);
                playerController.StopAllMovement(0.8f);
                transform.position = startingPoint.transform.position;
            }
            else
            {
                isExposedToSunlight = false;
                Debug.DrawLine(polygonPoint, sun.transform.position, Color.blue);
            }
        }
    }
}
