using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateShadowMesh : MonoBehaviour
{
    private GameObject sun;
    private float shadowLength = 20;

    //From the object that casts the shadow
    private PolygonCollider2D col;
    private Vector2[] colPoints;
    private Vector3[] shadowEdges;
    private Vector3[] fadeEdges;

    //For the shadow object
    public MeshFilter shadowMeshFilter;
    Mesh shadowMesh;

    public float colliderDistance;
    public float fadeEdgeDistance;
    public Color shadowColor;
    public float shadowInnerAlpha;
    public float shadowEdgesAlpha;

    void Start()
    {
        sun = GameObject.Find("Sun");
        col = GetComponent<PolygonCollider2D>();
        colPoints = col.points;
        //Does this shit work?
        shadowEdges = new Vector3[colPoints.Length];
        fadeEdges = new Vector3[4];

        shadowMesh = new Mesh();
        shadowMesh.name = "Shadow Mesh";
        shadowMeshFilter.mesh = shadowMesh;
    }

    void LateUpdate()
    {
        DrawLineFromCorners();
        UpdateShadowEdges();
        OrderEdgesBasedOnAngles();
        DrawLineFromCornerToShadowEdge();
        DrawHighestAndLowestAngle();
        //CreateFadeEdges();
        CreateFadeEdgesV2();
        UpdateMesh();
    }

    void UpdateShadowEdges()
    {
        for (int i = 0; i < colPoints.Length; i++)
        {
            Vector3 polygonPoint = transform.TransformPoint(colPoints[i]);
            Vector2 DirFromSun = (polygonPoint - sun.transform.position).normalized;

            shadowEdges[i] = polygonPoint + new Vector3(DirFromSun.x * shadowLength, DirFromSun.y * shadowLength, 0);
            //shadowEdges[i] = new Vector3(DirFromSun.x * shadowLength, DirFromSun.y * shadowLength, 0);

        }
    }

    void DrawLineFromCorners()
    {
        //Debug.Log(sun.transform.up + sun.transform.localPosition);

        for (int i = 0; i < colPoints.Length; i++)
        {
            Vector3 polygonPoint = transform.TransformPoint(colPoints[i]);
            //Vector2 DirToSun = (sun.transform.position - polygonPoint).normalized;
            Vector2 DirFromSun = (polygonPoint - sun.transform.position).normalized;


            //Debug.DrawRay(polygonPoint, DirFromSun * 30, Color.red);
            //Debug.DrawRay(sun.transform.position, DirFromSun * 1, Color.red);
            //Debug.DrawLine(sun.transform.position, sun.transform.up + sun.transform.localPosition, Color.green);

            //Debug.Log(Vector3.Angle(sun.transform.up + sun.transform.localPosition, DirFromSun));
            //Debug.Log(Vector3.Angle(sun.transform.up, DirFromSun));

            //Debug.Log(" col " + i + " - " +DirFromSun);
        }
    }

    void DrawLineFromCornerToShadowEdge()
    {
        for (int i = 0; i < colPoints.Length; i++)
        {
            if (i == 0)
                Debug.DrawLine(transform.TransformPoint(colPoints[i]), shadowEdges[i], Color.red);
            if (i == 1)
                Debug.DrawLine(transform.TransformPoint(colPoints[i]), shadowEdges[i], Color.green);
            if (i == 2)
                Debug.DrawLine(transform.TransformPoint(colPoints[i]), shadowEdges[i], Color.blue);
            if (i == 3)
                Debug.DrawLine(transform.TransformPoint(colPoints[i]), shadowEdges[i], Color.white);
        }
    }

    void UpdateMesh()
    {
        //Vector3[] vertices = new Vector3[(colPoints.Length*2)];
        //int[] triangles = new int[6];
        Vector3[] vertices = new Vector3[(colPoints.Length * 2) + 4];
        int[] triangles = new int[18];

        Color[] colors = new Color[(colPoints.Length * 2) + 4];//colPoints.Length * 2];


        for (int i = 0; i < vertices.Length; i++)
        {
            //colors[i] = new Color(0, 0, 3f, 0.4f);
            colors[i] = new Color(shadowColor.r, shadowColor.g, shadowColor.b, shadowInnerAlpha);
        }

        //colors[0].a = shadowEdgesAlpha; //Right side, sun left
        //colors[3].a = shadowEdgesAlpha; //Left side, sun Left
        //colors[4].a = shadowEdgesAlpha; //Right side, sun left
        //colors[7].a = shadowEdgesAlpha; //Left side, sun left

        colors[8].a = shadowEdgesAlpha;
        colors[9].a = shadowEdgesAlpha;
        colors[10].a = shadowEdgesAlpha;
        colors[11].a = shadowEdgesAlpha;

        //colors[colPoints.Length * 2 - 2].a = 1f;
        //colors[colPoints.Length * 2 - 1].a = 1f;

        for (int i = 0; i < colPoints.Length; i++)
        {
            vertices[i] = colPoints[i];
            //vertices[i] = transform.InverseTransformPoint(colPoints[i]);
        }

        for (int i = 0; i < colPoints.Length; i++)
        {
            vertices[i + colPoints.Length] = transform.InverseTransformPoint(shadowEdges[i]);
        }

        for (int i = 0; i < fadeEdges.Length; i++)
        {
            //vertices[i + (colPoints.Length * 2)] = fadeEdges[i];
            vertices[i + (colPoints.Length * 2)] = transform.InverseTransformPoint(fadeEdges[i]);

        }

        if (sun.transform.position.x <= transform.position.x)
        {
            Debug.Log("1 " + " " + sun.transform.position.x + " " + transform.position.x);
            //Inner Left. Seems to be working as intended.
            triangles[0] = 8;
            triangles[1] = 11; //6
            triangles[2] = 9;

            //Left side, blurred edge. Seems to be working as intended.
            triangles[3] = 3; //
            triangles[4] = 11; //6
            triangles[5] = 7;

            //Right side, blurred edge. Seems to be working as intended.
            triangles[6] = 0;
            triangles[7] = 4;
            triangles[8] = 10; //5

            //Left side, inner blurred edge. Seems to be working as intended.
            triangles[9] = 9;
            triangles[10] = 11; //6
            triangles[11] = 3;

            //Right side, inner blurred edge. 
            triangles[12] = 0;
            triangles[13] = 10; //5
            triangles[14] = 8;

            //Inner right,
            triangles[15] = 8;
            triangles[16] = 10; //10
            triangles[17] = 11; //6

        }
        else
        {
            Debug.Log("2");
            //Inner Left. Seems to be working as intended.
            triangles[0] = 9;
            triangles[1] = 11; //6
            triangles[2] = 8;

            //Left side, blurred edge. Seems to be working as intended.
            triangles[3] = 7; //
            triangles[4] = 11; //6
            triangles[5] = 3;

            //Right side, blurred edge. Seems to be working as intended.
            triangles[6] = 10; //5
            triangles[7] = 4;
            triangles[8] = 0;

            //Left side, inner blurred edge. Seems to be working as intended.
            triangles[9] = 3;
            triangles[10] = 11; //6
            triangles[11] = 9;

            //Right side, inner blurred edge. 
            triangles[12] = 8;
            triangles[13] = 10; //5
            triangles[14] = 0;

            //Inner right,
            triangles[15] = 11; //6
            triangles[16] = 10; //5
            triangles[17] = 8;
        }

        shadowMesh.Clear();

        shadowMesh.vertices = vertices;
        shadowMesh.colors = colors;
        shadowMesh.triangles = triangles;
        shadowMesh.RecalculateNormals();
    }

    void CreateFadeEdges()
    {
        //Vector3 differenceVector = ((colPoints[0] - colPoints[colPoints.Length - 1]).normalized * 0.1f);// + colPoints[0];
        Vector3 differenceVector = ((colPoints[0] - colPoints[colPoints.Length - 1]).normalized * colliderDistance);// + colPoints[0];

        //Debug.DrawLine(transform.TransformPoint(colPoints[0]), transform.TransformPoint(colPoints[0]) - differenceVector, Color.red);
        //Debug.DrawLine(transform.TransformPoint(colPoints[3]), transform.TransformPoint(colPoints[3]) + differenceVector, Color.blue);

        fadeEdges[0] = transform.TransformPoint(colPoints[0]) - differenceVector;
        fadeEdges[1] = transform.TransformPoint(colPoints[3]) + differenceVector;

        differenceVector = ((shadowEdges[0] - shadowEdges[3]).normalized * fadeEdgeDistance);// + colPoints[0];

        Debug.DrawLine(transform.TransformPoint(shadowEdges[0]), transform.TransformPoint(shadowEdges[3]), Color.black);

        //Debug.DrawLine(fadeEdges[0],fadeEdges[0] - differenceVector, Color.red);
        //Debug.DrawLine(transform.TransformPoint(fadeEdges[3]), transform.TransformPoint(fadeEdges[3]) + differenceVector, Color.red);

        //fadeEdges[2] = transform.TransformPoint(shadowEdges[0]) - differenceVector;
        //fadeEdges[3] = transform.TransformPoint(shadowEdges[3]) + differenceVector;

        fadeEdges[2] = shadowEdges[0] - differenceVector;
        fadeEdges[3] = shadowEdges[3] + differenceVector;

        //if(sun.transform.position.x <= transform.position.x)
        //{
        //    Vector2 DirFromSun = (fadeEdges[0] - sun.transform.position).normalized;
        //    fadeEdges[2] = Quaternion.Euler(0,0,-0.1f) * new Vector3(DirFromSun.x * shadowLength, DirFromSun.y * shadowLength, 0);

        //    DirFromSun = (fadeEdges[1] - sun.transform.position).normalized;
        //    fadeEdges[3] = Quaternion.Euler(0, 0, 0.1f) * new Vector3(DirFromSun.x * shadowLength, DirFromSun.y * shadowLength, 0);
        //}
        //else
        //{
        //    Vector2 DirFromSun = (fadeEdges[0] - sun.transform.position).normalized;
        //    fadeEdges[2] = Quaternion.Euler(0, 0, 0.1f) * new Vector3(DirFromSun.x * shadowLength, DirFromSun.y * shadowLength, 0);

        //    DirFromSun = (fadeEdges[1] - sun.transform.position).normalized;
        //    fadeEdges[3] = Quaternion.Euler(0, 0, -0.1f) * new Vector3(DirFromSun.x * shadowLength, DirFromSun.y * shadowLength, 0);
        //}

        Debug.DrawLine(shadowEdges[0], fadeEdges[2], Color.green);
        Debug.DrawLine(shadowEdges[3], fadeEdges[3], Color.green);
    }

    void CreateFadeEdgesV2()
    {
        //Vector3 differenceVector = ((colPoints[0] - colPoints[colPoints.Length - 1]).normalized * 0.1f);// + colPoints[0];
        Vector3 differenceVector = ((transform.TransformPoint(colPoints[0]) - transform.TransformPoint(colPoints[colPoints.Length - 1])).normalized * colliderDistance);// + colPoints[0];

        Debug.DrawLine(transform.TransformPoint(colPoints[0]), transform.TransformPoint(colPoints[0]) - differenceVector, Color.red);
        Debug.DrawLine(transform.TransformPoint(colPoints[3]), transform.TransformPoint(colPoints[3]) + differenceVector, Color.red);

        fadeEdges[0] = transform.TransformPoint(colPoints[0]) - differenceVector;
        fadeEdges[1] = transform.TransformPoint(colPoints[3]) + differenceVector;

        differenceVector = ((shadowEdges[0] - shadowEdges[3]).normalized * fadeEdgeDistance);// + colPoints[0];

        Debug.DrawLine(shadowEdges[0], shadowEdges[3], Color.blue);

        //Debug.DrawLine(fadeEdges[0],fadeEdges[0] - differenceVector, Color.red);
        //Debug.DrawLine(transform.TransformPoint(fadeEdges[3]), transform.TransformPoint(fadeEdges[3]) + differenceVector, Color.red);

        //fadeEdges[2] = transform.TransformPoint(shadowEdges[0]) - differenceVector;
        //fadeEdges[3] = transform.TransformPoint(shadowEdges[3]) + differenceVector;

        fadeEdges[2] = shadowEdges[0] - differenceVector;
        fadeEdges[3] = shadowEdges[3] + differenceVector;

        //if(sun.transform.position.x <= transform.position.x)
        //{
        //    Vector2 DirFromSun = (fadeEdges[0] - sun.transform.position).normalized;
        //    fadeEdges[2] = Quaternion.Euler(0,0,-0.1f) * new Vector3(DirFromSun.x * shadowLength, DirFromSun.y * shadowLength, 0);

        //    DirFromSun = (fadeEdges[1] - sun.transform.position).normalized;
        //    fadeEdges[3] = Quaternion.Euler(0, 0, 0.1f) * new Vector3(DirFromSun.x * shadowLength, DirFromSun.y * shadowLength, 0);
        //}
        //else
        //{
        //    Vector2 DirFromSun = (fadeEdges[0] - sun.transform.position).normalized;
        //    fadeEdges[2] = Quaternion.Euler(0, 0, 0.1f) * new Vector3(DirFromSun.x * shadowLength, DirFromSun.y * shadowLength, 0);

        //    DirFromSun = (fadeEdges[1] - sun.transform.position).normalized;
        //    fadeEdges[3] = Quaternion.Euler(0, 0, -0.1f) * new Vector3(DirFromSun.x * shadowLength, DirFromSun.y * shadowLength, 0);
        //}

        Debug.DrawLine(shadowEdges[0], fadeEdges[2], Color.green);
        Debug.DrawLine(shadowEdges[3], fadeEdges[3], Color.green);
    }

    void OrderEdgesBasedOnAngles()
    {
        //for (int i = 0; i < colPoints.Length; i++)
        //{
        //    float angle = Vector3.Angle(sun.transform.right + sun.transform.localPosition, shadowEdges[i]);
        //}

        /////
        for (int i = 0; i < colPoints.Length; i++)//bubblesort, because the code is easier than insertion-sort
        {
            for (int y = 0; y < colPoints.Length - i - 1; y++)
            {
                //if (Vector3.Angle((sun.transform.right) + sun.transform.localPosition, shadowEdges[y]) >= Vector3.Angle(sun.transform.right + sun.transform.localPosition, shadowEdges[y+1]))
                if (Vector3.Angle(sun.transform.up, shadowEdges[y]) >= Vector3.Angle(sun.transform.up, shadowEdges[y + 1]))
                {
                    Vector3 temp = shadowEdges[y + 1];
                    shadowEdges[y + 1] = shadowEdges[y];
                    shadowEdges[y] = temp;

                    temp = colPoints[y + 1];
                    colPoints[y + 1] = colPoints[y];
                    colPoints[y] = temp;
                }
            }
        }
        ///
    }

    void DrawHighestAndLowestAngle()
    {
        //Debug.DrawLine(transform.TransformPoint(colPoints[0]), shadowEdges[0],Color.cyan);
        //Debug.DrawLine(transform.TransformPoint(colPoints[colPoints.Length-1]), shadowEdges[colPoints.Length-1], Color.cyan);
        //Debug.DrawLine(transform.TransformPoint(colPoints[3]), shadowEdges[3], Color.cyan);
    }
}
