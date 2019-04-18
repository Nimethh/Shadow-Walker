using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformUpdated : MonoBehaviour
{
    private enum MovementPattern
    {
        StartToEnd,
        BackAndForth,
        Repeating
    }

    [SerializeField]
    public bool activated;
    [SerializeField]
    private Transform movingPlatform;
    private Rigidbody2D movingPlatformRigid;
    private Vector3 direction;
    private Transform movingDestination;

    [SerializeField]
    private float movingSpeed;
    private bool finishedMoving;
    private bool hasReachedStartTransformation;
    private bool hasReachedEndTransformation;
  
    [SerializeField]
    private MovementPattern movementPattern;

    [Header("Visual Aid")] //Visualisation Aid ( Only for the scene window )
    [SerializeField]
    private GameObject visualAid;
    [SerializeField]
    private Transform startTransform;
    [SerializeField]
    private Transform endTransform;

    void Start()
    {
        finishedMoving = false;
        movingPlatformRigid = movingPlatform.GetComponent<Rigidbody2D>();
        SetDestination(startTransform);
    }

    void FixedUpdate()
    {

        if(finishedMoving || !activated)
        {
            return;
        }

        movingPlatform.position += direction * movingSpeed * Time.fixedDeltaTime;

        if (Vector3.Distance(movingPlatform.position, movingDestination.position) < movingSpeed * Time.fixedDeltaTime)
        {
            if(movingDestination == startTransform)
            {
                hasReachedStartTransformation = true;
                if(movementPattern == MovementPattern.StartToEnd)
                {
                    finishedMoving = true;
                }
            }
            else
            {
                hasReachedEndTransformation = true;
                if(movementPattern == MovementPattern.BackAndForth)
                {
                    finishedMoving = true;
                }
            }

            SetDestination(movingDestination == startTransform ? endTransform : startTransform);
        }
    }

    void SetDestination(Transform destination)
    {
        movingDestination = destination;
        direction = (movingDestination.position - movingPlatform.position).normalized;
    }

    private void OnDrawGizmos() //Visualisation Aid
    {
        Gizmos.color = Color.cyan;
        //Gizmos.DrawCube(startTransform.position, new Vector3(0.2f,0.2f,0.1f));
        Gizmos.DrawWireCube(startTransform.position, visualAid.transform.localScale);
        Gizmos.color = Color.cyan;
        //Gizmos.DrawCube(endTransform.position, new Vector3(0.2f, 0.2f, 0.1f));
        Gizmos.DrawWireCube(endTransform.position, visualAid.transform.localScale);
    }
}
