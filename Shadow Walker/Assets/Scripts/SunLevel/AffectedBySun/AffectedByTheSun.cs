using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AffectedByTheSun : MonoBehaviour
{
    public bool isExposedToSunlight;
    public bool isPartiallyExposed;
    public bool isFullyExposed;
    public bool isFullyCovered;
    public bool wasPreviouslyExposedToSun;
    public bool justGotExposedToSunlight;
    public bool justGotCoveredFromSunlight;

    private PolygonCollider2D col;
    private Vector2[] colPoints;
    private int halfColPoints;
    private int numberOfExposedColliderPoints;

    private GameObject sun;
    private LayerMask obstacleLayer;


    public void AffectedByTheSunScriptStart()
    {
        sun = GameObject.Find("Sun");
        obstacleLayer = LayerMask.GetMask("Ground");
        if (obstacleLayer.value == 0)
        {
            Debug.Log("Make sure you've spelled the LayerMask.GetMask correctly, It appears to be the default one");
        }
        col = GetComponent<PolygonCollider2D>();

        colPoints = col.points;
        halfColPoints = colPoints.Length / 2;
        numberOfExposedColliderPoints = 0;

        isExposedToSunlight = false;
        isPartiallyExposed = false;
        isFullyExposed = false;
        wasPreviouslyExposedToSun = false;
    }

    public void AffectedByTheSunScriptUpdate()
    {
        UpdateAffectedBySunStatus();
        UpdateObjectBehaviour();
        justGotExposedToSunlight = false;
        justGotCoveredFromSunlight = false;
    }

    public void UpdateAffectedBySunStatus()
    {
        if (isExposedToSunlight)
        {
            wasPreviouslyExposedToSun = true;
        }
        else
        {
            wasPreviouslyExposedToSun = false;
        }

        isPartiallyExposed = false;
        isFullyExposed = false;
        isExposedToSunlight = false;
        isFullyCovered = true;
        numberOfExposedColliderPoints = 0;

        for (int i = 0; i < colPoints.Length; i++)
        {
            Vector3 polygonPoint = transform.TransformPoint(colPoints[i]);
            Vector2 DirToSun = (sun.transform.position - polygonPoint).normalized;
            float DistanceToSun = Vector2.Distance(transform.position, sun.transform.position);

            if (!Physics2D.Raycast(polygonPoint, DirToSun, DistanceToSun, obstacleLayer))
            {
                if (isExposedToSunlight == false && wasPreviouslyExposedToSun == false)
                {
                    justGotExposedToSunlight = true;
                }

                numberOfExposedColliderPoints++;
                isExposedToSunlight = true;
                isFullyCovered = false;
                Debug.DrawLine(polygonPoint, sun.transform.position, Color.red);
            }
            else
            {
                Debug.DrawLine(polygonPoint, sun.transform.position, Color.blue);
            }
        }

        if (numberOfExposedColliderPoints >= halfColPoints && numberOfExposedColliderPoints != colPoints.Length)
        {
            isPartiallyExposed = true;
        }
        else if (numberOfExposedColliderPoints == colPoints.Length)
        {
            isFullyExposed = true;
        }
        else if(numberOfExposedColliderPoints == 0)
        {
            isFullyCovered = true;
        }
        else
        {
            isPartiallyExposed = false;
            isFullyExposed = false;
        }

        if (wasPreviouslyExposedToSun && !isExposedToSunlight)
        {
            justGotCoveredFromSunlight = true;
        }

    }

    public bool ObjectJustGotCoveredOrExposedToSunlight()
    {
        if (justGotCoveredFromSunlight)
        {
            JustGotCoveredFromSunlight();
            return true;
        }

        if (justGotExposedToSunlight)
        {
            JustGotExposedToSunlight();
            return true;
        }

        return false;
    }

    public void UpdateObjectBehaviour()
    {
        if (!ObjectJustGotCoveredOrExposedToSunlight())
        {
            if (isExposedToSunlight)
            {
                if (isPartiallyExposed)
                {
                    UnderPartialCover();
                }
                else if (isFullyExposed)
                {
                    UnderFullExposure();
                }
            }
            else
            {
                if (isPartiallyExposed)
                {
                    UnderPartialCover();
                }
                else if(isFullyCovered)
                {
                    UnderFullCover();
                }
            }
        }
    }

    public abstract void JustGotExposedToSunlight();
    public abstract void UnderFullExposure();
    public abstract void JustGotCoveredFromSunlight();
    public abstract void UnderFullCover();
    public abstract void UnderPartialCover();
}
