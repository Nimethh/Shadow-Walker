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

    // Layers for the raycast.
    [SerializeField]
    private LayerMask ground;
    [SerializeField]
    private LayerMask mirror;

    // List of hit points position.
    private List<Vector3> hitPoints;

    // CenterPosition for the raycast direction.
    [SerializeField]
    private Transform centerPosition = null;

    private RaycastHit2D firstHit;
    private RaycastHit2D newHit;

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
        lineRenderer = transform.GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.15f;
        lineRenderer.numCornerVertices = lineRendererNumOfCornerVertices;
        lineRenderer.numCapVertices = lineRendererNumOfCapVertices;
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



    //////////////
    void FixedUpdate()
    {
        startPoint = this.transform.position;
        direction.x = centerPosition.position.x;
        ClearLists();
        DrawFirstRay();
        UpdateLineRenderer();
        NotifyNoLongerHitObjects();
        CopyCurrentlyHitToPreviouslyHit();
    }

    void DrawFirstRay()
    {
        hitPoints.Add(startPoint);

        firstHit = Physics2D.Raycast(startPoint, (direction - startPoint).normalized, maxRayCastLength, ground | mirror);

        if(firstHit)
        {
            hitPoints.Add(firstHit.point);
            NotifyObjectHit(firstHit);
            AddGameObjectToObjectHitList(firstHit);
            if(CheckIfRayShouldBeReflected(firstHit))
            {
                Debug.Log("Hitting a mirror, should be reflecting");
                //Change the position of the mirror's light and activate it. 
                //ReflectRay(startPoint, firstHit);
                ReflectRay(firstHit.point, firstHit);

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
           
        }
        else if (objectHit.transform.CompareTag("Platform"))
        {

        }
        else if(objectHit.transform.CompareTag("Mirror"))
        {

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

        // Calculating the directions.
        prevDirection = (hit.point - origin).normalized;
        newDirection = Vector2.Reflect(prevDirection, hit.normal);

        newHit = Physics2D.Raycast(hit.point + newDirection, newDirection, maxRayCastLength, ground | mirror);

        if (newHit)
        {
            hitPoints.Add(newHit.point);
            NotifyObjectHit(newHit);
            AddGameObjectToObjectHitList(newHit);
            if (CheckIfRayShouldBeReflected(newHit))
            {
                Debug.Log("Hitting a mirror, should be reflecting");
                //Change the position of the mirror's light and activate it. 
                ReflectRay(hit.point, newHit);
            }
        }
        else
        {
            hitPoints.Add(newHit.point);
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

    void AddGameObjectToObjectHitList(RaycastHit2D firstHit)
    {
        tempGameObject = firstHit.collider.gameObject;

        if (!objectsHitByRay.Contains(tempGameObject))
        {
            currentReflection++;
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
    //////////////

    void Ray()
    {
        hitPoints.Add(startPoint);

        firstHit = Physics2D.Raycast(startPoint, (direction - startPoint).normalized, maxRayCastLength, ground | mirror);

        if (firstHit)
        {
            if (firstHit.transform.tag == "Lamp")
            {
                hitPoints.Add(firstHit.point);
                // Move the movable object.
            }
            else if (firstHit.transform.tag == "Platform")
            {
                hitPoints.Add(firstHit.point);

                if (lineRenderer.positionCount <= 2)
                {
                    // move it back to the original position.
                }

                if (lightingObject != null)
                {
                    lightingObject.LightUpObject(firstHit);
                }
            }
            else
            {
                Reflect(startPoint, firstHit);
                if (lineRenderer.positionCount <= 2)
                {
                    // move it back to the original position.
                }
            }
        }
        else
        {
            hitPoints.Add(startPoint + (direction - startPoint).normalized * maxRayCastLength);
            if (lineRenderer.positionCount <= 2)
            {
                // move it back to the original position.
            }
        }

        lineRenderer.positionCount = hitPoints.Count;
        lineRenderer.SetPositions(hitPoints.ToArray());
    }


    void Reflect(Vector2 origin, RaycastHit2D hit)
    {
        if (currentReflection > maxNumOfReflection)
        {
            return;
        }

        hitPoints.Add(hit.point);
   

        tempGameObject = hit.collider.gameObject;

        if (!objectsHitByRay.Contains(tempGameObject))
        {
            currentReflection++;
            objectsHitByRay.Add(tempGameObject);
        }

        // Calculating the directions.
        prevDirection = (hit.point - origin).normalized;
        newDirection = Vector2.Reflect(prevDirection, hit.normal);

        // New Raycast
        newHit = Physics2D.Raycast(hit.point + newDirection, newDirection, maxRayCastLength, ground | mirror);

        if (newHit)
        {
            if (newHit.transform.tag == "Lamp")
            {
                hitPoints.Add(newHit.point);
                // Move the movable object.
            }
            else if (newHit.transform.tag == "Platform")
            {
                hitPoints.Add(newHit.point);
                if (lineRenderer.positionCount > 2)
                {
                    // move it back to the original position.
                }
            }
            else
            {
                Reflect(hit.point, newHit);
            }
        }
        else
        {
            hitPoints.Add(hit.point + newDirection * maxRayCastLength);
            if (lineRenderer.positionCount > 2)
            {
                // move it back to the original position.
            }
        }

        //Lighting Up the object
        if (lightingObject != null)
        {
            lightingObject.LightUpObject(newHit);
        }
    }

    // Copy the list of gameObjects from the currently hit list to the previously hit list.
    void CopyList(List<GameObject> prev, List<GameObject> current)
    {
        for (int i = 0; i < current.Count; i++)
        {
            if (!prev.Contains(current[i]))
            {
                prev.Add(current[i]);
            }
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
        CopyList(prevObjectsHitByRay, objectsHitByRay);

    }

    // Resetting the variables and clearing the lists.   Might need to moving them to a better place.
    void ClearLists()
    {
        currentReflection = 0;
        objectsHitByRay.Clear();
        hitPoints.Clear();
    }
}
