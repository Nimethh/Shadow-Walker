using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateShadowMesh : MonoBehaviour
{
    [SerializeField]
    GameObject[] staticPlatforms;
    [SerializeField]
    GameObject[] movingPlatforms;

    [SerializeField]
    List<PolygonCollider2D> staticPlatformsCollider;
    [SerializeField]
    List<PolygonCollider2D> movingPlatformsCollider;
    [SerializeField]
    List<Vector3> staticCollidersPoints;
    [SerializeField]
    List<Vector3> movingCollidersPoints;

    [SerializeField]
    List<Vector3> meshVerticesPositions;

    [SerializeField]
    LayerMask obstacles;

    public Vector3 topLeft;
    public Vector3 topRight;
    public Vector3 bottomLeft;
    public Vector3 bottomRight;

    float raycastDistance = 30.0f;

    Mesh mesh;

    public Vector3 origin;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        FindWorldBounds();
        meshVerticesPositions = new List<Vector3>();

        AddPlatformsCollidersToList();
        AddStaticCollidersPointsToList();
    }

    private void LateUpdate()
    {
        //AddStaticCollidersPointsToList();
        AddMovingCollidersPointsToList();
        RayCastTowardsCollidersPoints();
    }

    void AddPlatformsCollidersToList()
    {
        staticPlatforms = GameObject.FindGameObjectsWithTag("StaticPlatform");
        movingPlatforms = GameObject.FindGameObjectsWithTag("MovingPlatform");
        for (int i = 0; i < staticPlatforms.Length; i++)
        {
            staticPlatformsCollider.Add(staticPlatforms[i].GetComponent<PolygonCollider2D>());
        }
        for (int i = 0; i < movingPlatforms.Length; i++)
        {
            movingPlatformsCollider.Add(movingPlatforms[i].GetComponent<PolygonCollider2D>());
        }
    }

    void AddStaticCollidersPointsToList()
    {
        staticCollidersPoints.Clear();
        for (int i = 0; i < staticPlatformsCollider.Count; i++)
        {
            for (int j = 0; j < staticPlatformsCollider[i].points.Length; j++)
            {
                Vector2 colPos = staticPlatformsCollider[i].gameObject.transform.TransformPoint(staticPlatformsCollider[i].points[j]);
                if (!staticCollidersPoints.Contains(colPos))
                    staticCollidersPoints.Add(colPos);
            }
        }
    }

    void AddMovingCollidersPointsToList()
    {
        movingCollidersPoints.Clear();
        for (int i = 0; i < movingPlatformsCollider.Count; i++)
        {
            for (int j = 0; j < movingPlatformsCollider[i].points.Length; j++)
            {
                Vector2 colPos = movingPlatformsCollider[i].gameObject.transform.TransformPoint(movingPlatformsCollider[i].points[j]);
                movingCollidersPoints.Add(colPos);
            }
        }
    }

    void RayCastTowardsCollidersPoints()
    {
        float angleOffset = 0.001f;
        Vector3 raycastDirection = Vector3.zero;
        meshVerticesPositions.Clear();

        for (int i = 0; i < staticCollidersPoints.Count; i++)
        {
            float angle = Mathf.Atan2(staticCollidersPoints[i].y - transform.position.y, staticCollidersPoints[i].x - transform.position.x);
            raycastDirection = staticCollidersPoints[i] - transform.position;
            RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, raycastDirection, raycastDirection.magnitude, obstacles);

            if (raycastHit)
            {
                meshVerticesPositions.Add(raycastHit.point);
            }
            else
            {
                meshVerticesPositions.Add(staticCollidersPoints[i]);
            }

            CreateAndAddNeighborPoints(angleOffset, raycastDistance, staticCollidersPoints[i]);
            CreateAndAddNeighborPoints(-angleOffset, raycastDistance, staticCollidersPoints[i]);
        }

        for (int i = 0; i < movingCollidersPoints.Count; i++)
        {
            float angle = Mathf.Atan2(movingCollidersPoints[i].y - transform.position.y, movingCollidersPoints[i].x - transform.position.x);
            RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, movingCollidersPoints[i] - transform.position, (movingCollidersPoints[i] - transform.position).magnitude, obstacles);

            if (raycastHit)
            {
                meshVerticesPositions.Add(raycastHit.point);
                //Debug.DrawRay(transform.position, new Vector3(raycastHit.point.x, raycastHit.point.y , transform.position.z) - transform.position, Color.red);
            }
            else
            {
                meshVerticesPositions.Add(movingCollidersPoints[i]);
                //Debug.DrawRay(transform.position, movingCollidersPoints[i] - transform.position, Color.blue);
            }

            CreateAndAddNeighborPoints( angleOffset, raycastDistance, movingCollidersPoints[i]);
            CreateAndAddNeighborPoints(-angleOffset, raycastDistance, movingCollidersPoints[i]);
        }

        CreateAndAddWorldCornersToVertexList();
        meshVerticesPositions.Sort(SortByAngle);

        //for (int i = 0; i < meshVerticesPositions.Count; i++)
        //{
        //    float angle = Mathf.Atan2(meshVerticesPositions[i].y - transform.position.y, meshVerticesPositions[i].x - transform.position.x) * Mathf.Rad2Deg;
        //    if (angle < 0.0f)
        //    {
        //        angle += 360.0f;
        //    }
        //}

        //for(int i = 0; i < meshVerticesPositions.Count; i++)
        //{
        //    Color color = Color.black + (Color.red * i / meshVerticesPositions.Count);
        //    Debug.DrawLine(transform.position, meshVerticesPositions[i], color);
        //}

        meshVerticesPositions.Insert(0, transform.localPosition);
        meshVerticesPositions.Add(meshVerticesPositions[1]);

        int vertexCount = meshVerticesPositions.Count;
        int[] triangles = new int[(vertexCount - 1) * 3];

        Debug.Log("VertexCount " + vertexCount);
        Debug.Log("trainglesCount " + triangles.Length);
        for (int i = 0; i < vertexCount - 1; i++)
        {
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 2;
                triangles[i * 3 + 2] = i + 1;
            }
            //else
            //{
            //    Debug.Log("else happens" + vertexCount + "triangles" + i * 3);
            //    triangles[i * 3] = 0;
            //    triangles[i * 3 + 1] = i + 2;
            //    triangles[i * 3 + 2] = 1;
            //}
        }

        mesh.Clear();
        //Debug.Log(meshVerticesPositions[0]);
        mesh.vertices = meshVerticesPositions.ToArray();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        //Debug.Log(mesh.vertices.Length);
    }

    void CreateAndAddNeighborPoints(float angleOffset, float raycastDistance, Vector3 colliderPoint)
    {
        Vector3 raycastOffset = new Vector3(Mathf.Cos(angleOffset) * colliderPoint.x - Mathf.Sin(angleOffset) * colliderPoint.y,
                                             Mathf.Sin(angleOffset) * colliderPoint.x + Mathf.Cos(angleOffset) * colliderPoint.y,
                                             colliderPoint.z);
        
        RaycastHit2D raycastHitOffset = Physics2D.Raycast(transform.position, raycastOffset - transform.position, raycastDistance, obstacles);

        if (raycastHitOffset)
        {
            meshVerticesPositions.Add(raycastHitOffset.point);
            //Debug.DrawRay(transform.position, new Vector3(raycastHitOffset.point.x, raycastHitOffset.point.y, transform.position.z) - transform.position, Color.black);
        }
        else
        {
            meshVerticesPositions.Add((raycastOffset - transform.position).normalized * raycastDistance);
            //Debug.DrawRay(transform.position, (raycastOffset - transform.position).normalized * raycastDistance, Color.yellow);
        }
    }

    void CreateAndAddWorldCornersToVertexList()
    {
        RaycastToPosition(topLeft);
        RaycastToPosition(topRight);
        RaycastToPosition(bottomLeft);
        RaycastToPosition(bottomRight);
        //meshVerticesPositions.Add(topLeft);
        //meshVerticesPositions.Add(topRight);
        //meshVerticesPositions.Add(bottomLeft);
        //meshVerticesPositions.Add(bottomRight);
    }

    void RaycastToPosition(Vector3 raycastPosition)
    {
        Vector3 raycastDirection = raycastPosition - transform.position;
        RaycastHit2D raycastHitOffset = Physics2D.Raycast(transform.position, raycastDirection, raycastDistance, obstacles);

        if (raycastHitOffset)
        {
            meshVerticesPositions.Add(raycastHitOffset.point);
            //Debug.DrawRay(transform.position, new Vector3(raycastHitOffset.point.x, raycastHitOffset.point.y, transform.position.z) - transform.position, Color.white);
        }
        else
        {
            meshVerticesPositions.Add(raycastDirection.normalized * raycastDistance);
            //Debug.DrawRay(transform.position, raycastDirection.normalized * raycastDistance, Color.magenta);
        }
    }

    void FindWorldBounds()
    {
        topLeft     = Camera.main.ViewportToWorldPoint(new Vector3( 0, 1, Camera.main.nearClipPlane));
        topLeft.z = 0.0f;
        topRight    = Camera.main.ViewportToWorldPoint(new Vector3( 1, 1, Camera.main.nearClipPlane));
        topRight.z = 0.0f;
        bottomLeft  = Camera.main.ViewportToWorldPoint(new Vector3( 0, 0, Camera.main.nearClipPlane));
        bottomLeft.z = 0.0f;
        bottomRight = Camera.main.ViewportToWorldPoint(new Vector3( 1, 0, Camera.main.nearClipPlane));
        bottomRight.z = 0.0f;
    }

    int SortByAngle(Vector3 vector1, Vector3 vector2)
    {
        float angle1 = Mathf.Atan2(vector1.y - transform.position.y, vector1.x - transform.position.x) * Mathf.Rad2Deg;
        float angle2 = Mathf.Atan2(vector2.y - transform.position.y, vector2.x - transform.position.x) * Mathf.Rad2Deg;
        if (angle1 < 0.0f)
        {
            angle1 += 360.0f;
        }
        if(angle2 < 0.0f)
        {
            angle2 += 360.0f;
        }

        if (angle1 < angle2)
        {
            return -1;
        }
        else if(angle1 > angle2)
        {
            return 1;
        }
        return 0;
    }
}
