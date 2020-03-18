using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewTest : MonoBehaviour
{
    Mesh mesh;
    [SerializeField]
    LayerMask obstacleLayer;
    Vector3[] vertices;
    [SerializeField]
    Vector3 origin;
    private float startingAngle;
    float fov = 90;

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

    vertex vert;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
        staticPlatformsCollider = new List<PolygonCollider2D>();
        movingPlatformsCollider = new List<PolygonCollider2D>();
        staticCollidersPoints = new List<Vector3>();
        movingCollidersPoints = new List<Vector3>();

        AddPlatformsCollidersToList();
        AddStaticCollidersPointsToList();
    }

    void AddPlatformsCollidersToList()
    {
        staticPlatforms = GameObject.FindGameObjectsWithTag("StaticPlatform");
        movingPlatforms = GameObject.FindGameObjectsWithTag("MovingPlatform");
        for (int i = 0; i < staticPlatforms.Length; i++)
        {
            staticPlatformsCollider.Add(staticPlatforms[i].GetComponent<PolygonCollider2D>());
        }
        for(int i = 0; i < movingPlatforms.Length; i++)
        {
            movingPlatformsCollider.Add(movingPlatforms[i].GetComponent<PolygonCollider2D>());
        }
    }

    void AddStaticCollidersPointsToList()
    {
        for (int i = 0; i < staticPlatformsCollider.Count; i++)
        {
            for (int j = 0; j < staticPlatformsCollider[i].points.Length; j++)
            {
                Vector2 colPos = staticPlatformsCollider[i].gameObject.transform.TransformPoint(staticPlatformsCollider[i].points[j]);
                if (!staticCollidersPoints.Contains(colPos))
                    staticCollidersPoints.Add(colPos);

                Debug.DrawLine(origin, colPos, Color.green, 20.0f);
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

                Debug.DrawLine(origin, colPos, Color.blue);
            }
        }
    }

    private void Update()
    {
        AddMovingCollidersPointsToList();
    }

    private void LateUpdate()
    {
        vertices = new Vector3[staticCollidersPoints.Count + movingCollidersPoints.Count];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[(staticCollidersPoints.Count + movingCollidersPoints.Count) * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
    }

    void FieldOfViewOld()
    {
        //int rayCount = 500;
        //float angle = startingAngle;
        //float angleIncrease = fov / rayCount;
        //float viewDistance = 30.0f;

        //vertices = new Vector3[rayCount + 1 + 1];
        //Vector2[] uv = new Vector2[vertices.Length];
        //int[] triangles = new int[rayCount * 3];

        //vertices[0] = origin;

        //int vertexIndex = 1;
        //int triangleIndex = 0;
        //for (int i = 0; i <= rayCount; i++)
        //{
        //    float angleRad = angle * (Mathf.PI / 180.0f);
        //    Vector3 anglePos = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        //    Vector3 vertex;

        //    RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, anglePos, viewDistance, obstacleLayer);
        //    if (raycastHit2D.collider == null)
        //    {
        //        vertex = origin + anglePos * viewDistance;
        //    }
        //    else
        //    {
        //        vertex = raycastHit2D.point;
        //    }

        //    vertices[vertexIndex] = vertex;

        //    if (i > 0)
        //    {
        //        triangles[triangleIndex + 0] = 0;
        //        triangles[triangleIndex + 1] = vertexIndex - 1;
        //        triangles[triangleIndex + 2] = vertexIndex;

        //        triangleIndex += 3;
        //    }
        //    vertexIndex++;
        //    angle -= angleIncrease;

        //    mesh.vertices = vertices;
        //    mesh.uv = uv;
        //    mesh.triangles = triangles;
        //}
    }

    public void SetOrigin(Vector3 p_origin)
    {
        this.origin = p_origin;
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        aimDirection = aimDirection.normalized;
        float n = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        startingAngle = n + fov / 2f /*- fov / 2f*/;
    }

    [System.Serializable]
    struct vertex
    {
        public Vector3 position;
        public float angle;

        void CalculateVertexAngle(Vector3 origin)
        {
            angle = Mathf.Atan2(position.y - origin.y, position.x - origin.x) * Mathf.Rad2Deg;
        }
    };
}
