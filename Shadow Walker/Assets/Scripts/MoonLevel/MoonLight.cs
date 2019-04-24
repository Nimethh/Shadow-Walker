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

    void Update()
    {
        ListCheck(prevObjectsHitByRay, objectsHitByRay);
    }

    void FixedUpdate()
    {
        startPoint = this.transform.position;
        direction.x = centerPosition.position.x;
        Reset();
        Ray();
    }

    void Ray()
    {
        firstHit = Physics2D.Raycast(startPoint, (direction - startPoint).normalized, maxRayCastLength, ground | mirror);

        hitPoints.Add(startPoint);

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
    void Reset()
    {
        currentReflection = 0;
        objectsHitByRay.Clear();
        hitPoints.Clear();
    }
}
