using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MoonLight : MonoBehaviour
{
    
    // RayCast Variables
    private int maxRayCastLength = 500;
    private int maxNumOfReflection = 50;
    private int currentReflection = 0;

    private Vector2 startPoint;
    private Vector2 direction;
    private Vector2 prevDirection;
    private Vector2 newDirection;
    private RaycastHit2D firstHit;
    private RaycastHit2D newHit;

    // CenterPosition for the raycast direction.
    [SerializeField]
    private Transform centerPosition = null;

    // Layers for the raycast.
    [SerializeField]
    private LayerMask layersToCheck;

    // List of hit points position.
    private List<Vector3> hitPoints;

    // LineRenderer Variables
    private LineRenderer lineRenderer;
    private int lineRendererNumOfCornerVertices = 10;
    private int lineRendererNumOfCapVertices = 10;

    // Temporary gameObject to store the info in it and adding it to the list of object that got hit by moon-light
    private GameObject tempGameObject;

    // List of objects that got hit by the moon-light.
    [SerializeField]
    List<GameObject> objectsHitByRay;
    [SerializeField]
    List<GameObject> prevObjectsHitByRay;

    // Script to light up the objects.
    private LightingObject lightingObject;
    
    void Start()
    {
        hitPoints = new List<Vector3>();
        SetUpLineRenderer();
        startPoint = this.transform.position;
    }

    //void Update() //Can probably be moved to fixedUpdate
    //{
    //    ListCheck(prevObjectsHitByRay, objectsHitByRay);
    //}

    //void FixedUpdate()
    //{
    //    startPoint = this.transform.position;
    //    direction.x = centerPosition.position.x;
    //    ClearLists();
    //    Ray();
    //}



    ////////////
    void FixedUpdate()
    {
        startPoint = this.transform.position;
        direction.x = centerPosition.position.x;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        ClearLists();
        DrawFirstRay();
        UpdateLineRenderer();
        NotifyNoLongerHitObjects();
        CopyCurrentlyHitToPreviouslyHit();
    }

    void DrawFirstRay()
    {
        firstHit = Physics2D.Raycast(startPoint, (direction - startPoint).normalized, maxRayCastLength, layersToCheck);

        hitPoints.Add(startPoint);

        if (firstHit)
        {
            hitPoints.Add(firstHit.point);
            NotifyObjectHit(firstHit);
            AddGameObjectToObjectHitList(firstHit);
            if(CheckIfRayShouldBeReflected(firstHit))
            {
                //Change the position of the mirror's light and activate it.
                ReflectRay(startPoint, firstHit);
            }
        }
        else
        {
            hitPoints.Add(startPoint + (direction - startPoint).normalized * maxRayCastLength);
            //Move the moon's own light to firstHit.point
        }

    }

    void NotifyObjectHit(RaycastHit2D objectHit)
    {
        //Tell the object that it has been hit by calling the abstract affectedByMoonRay-script. 
        //If it is just the platform or ground that has been hit, update the moon's own light to that position.

        if (objectHit.transform.CompareTag("Lamp"))
        {
            //objectHit.transform.GetComponent<AffectedByMoonRay>().isHitByMoonLight = true;
        }
        else if (objectHit.transform.CompareTag("Platform"))
        {

        }
        else if(objectHit.transform.CompareTag("Mirror"))
        {
            //objectHit.transform.GetComponent<AffectedByMoonRay>().isHitByMoonLight = true;
            objectHit.transform.parent.GetComponent<AffectedByMoonRay>().isHitByMoonLight = true;
        }
        else if(objectHit.transform.CompareTag("MirrorBody") || objectHit.transform.CompareTag("MirrorHandle"))
        {
            objectHit.transform.GetComponent<AffectedByMoonRay>().isHitByMoonLight = true;
        }
        else
        {

        }
    }

    void NotifyNoLongerHitObjects()
    {
        //We want to check all previouslyHit objects to see if they still exist in the currently hit list.
        for(int i = 0; i < prevObjectsHitByRay.Count; i++)
        {
            if(!objectsHitByRay.Contains(prevObjectsHitByRay[i]))
            {

                if (prevObjectsHitByRay[i].transform.CompareTag("Mirror"))
                {
                    //prevObjectsHitByRay[i].transform.GetComponent<AffectedByMoonRay>().isHitByMoonLight = false;
                    prevObjectsHitByRay[i].transform.parent.GetComponent<AffectedByMoonRay>().isHitByMoonLight = false;
                }

                if (prevObjectsHitByRay[i].transform.CompareTag("MirrorBody") || prevObjectsHitByRay[i].transform.CompareTag("MirrorHandle"))
                {
                    prevObjectsHitByRay[i].transform.GetComponent<AffectedByMoonRay>().isHitByMoonLight = false;
                }

                if (prevObjectsHitByRay[i].transform.CompareTag("Lamp"))
                {
                    //prevObjectsHitByRay[i].transform.GetComponent<AffectedByMoonRay>().isHitByMoonLight = false;
                }
                //if(prevObjectsHitByRay[i].CompareTag("Ground") /* OR ANY OTHER TAG THAT SHOULD NOT BE NOTFIED/AFFECTED BY THE MOON RAY */) 
                //{
                //    return;
                //}
                //else
                //{
                ////Notify the object that it is no longer being hit by the moonray. This should be done through the abstract affectedByMoonRay-Script.
                //}
            }
        }
    }


    void ReflectRay(Vector2 origin, RaycastHit2D hit)
    {
        if (currentReflection > maxNumOfReflection)
        {
            return;
        }

        //hitPoints.Add(hit.point);

        currentReflection++;

        // Calculating the directions.
        prevDirection = (hit.point - origin).normalized;
        newDirection = Vector2.Reflect(prevDirection, hit.normal);

        // New Raycast
        newHit = Physics2D.Raycast(hit.point + newDirection, newDirection, maxRayCastLength, layersToCheck);

        if (newHit)
        {
            hitPoints.Add(newHit.point);
            NotifyObjectHit(newHit);
            AddGameObjectToObjectHitList(newHit);
            if (CheckIfRayShouldBeReflected(newHit))
            {
                //Debug.Log("Hitting a mirror, should be reflecting");
                //Change the position of the mirror's light and activate it. 
                ReflectRay(hit.point, newHit);

            }
        }
        else
        {
            hitPoints.Add(hit.point + newDirection * maxRayCastLength);
            //Move the moon's own light to firstHit.point
        }

    }

    bool CheckIfRayShouldBeReflected(RaycastHit2D objectHit)
    {
        if(objectHit.transform.CompareTag("Mirror"))
        {
            return true;
        }

        return false;
    }

    void AddGameObjectToObjectHitList(RaycastHit2D hit)
    {
        tempGameObject = hit.collider.gameObject;

        if (!objectsHitByRay.Contains(tempGameObject))
        {
            objectsHitByRay.Add(tempGameObject);
        }
    }

    void UpdateLineRenderer()
    {
        lineRenderer.positionCount = hitPoints.Count;
        lineRenderer.SetPositions(hitPoints.ToArray());
    }

    void CopyCurrentlyHitToPreviouslyHit()
    {
        prevObjectsHitByRay.Clear();

        for (int i = 0; i < objectsHitByRay.Count; i++)
        {
            prevObjectsHitByRay.Add(objectsHitByRay[i]);
        }
    }

    void ListCheck(List<GameObject> prev, List<GameObject> current)
    {
        for (int i = 0; i < prev.Count; i++)
        {
            for (int j = 0; j < current.Count; j++)
            {
                if (current[j].gameObject.name == prev[i].gameObject.name)
                {
                    lightingObject = current[j].GetComponent<LightingObject>();
                    lightingObject.canBeLit = true;
                }
                else
                {
                    lightingObject = prev[i].gameObject.GetComponent<LightingObject>();
                    lightingObject.canBeLit = false;

                }
            }
        }

        // Clear the previously hit list.
        prevObjectsHitByRay.Clear();
        // Copy the currently hit objects to the previously hit list.

    }

    // Resetting the variables and clearing the lists.   Might need to moving them to a better place.
    void ClearLists()
    {
        currentReflection = 0;
        objectsHitByRay.Clear();
        hitPoints.Clear();
    }

    void SetUpLineRenderer()
    {
        lineRenderer = transform.GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.numCornerVertices = lineRendererNumOfCornerVertices;
        lineRenderer.numCapVertices = lineRendererNumOfCapVertices;
    }
}
