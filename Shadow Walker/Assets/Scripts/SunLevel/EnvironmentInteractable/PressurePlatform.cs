using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlatform : MonoBehaviour
{
    public bool activated;
    public bool movingToStart;

    [Header("Required Objects")]
    [SerializeField]
    private Transform movingPlatform;
    [SerializeField]
    private Transform startTransform;
    [SerializeField]
    private Transform endTransform;
    [SerializeField]
    private GameObject visualAid;

    private Rigidbody2D movingPlatformRigid;
    private Vector3 direction;
    private Transform movingToLocation;
    private Transform movingFromLocation;
    private bool finishedMoving;
    [SerializeField] public float travelTime;
    [SerializeField] public float movingSpeed;

    public void Start()
    {
        finishedMoving = false;
        movingPlatformRigid = movingPlatform.GetComponent<Rigidbody2D>();
        movingToLocation = startTransform;
        movingFromLocation = endTransform;
        movingToStart = true;
        direction = (movingToLocation.position - movingPlatform.position).normalized;

    }

    public void Update()
    {
        //movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, LinearEase());
    }

    public void FixedUpdate()
    {
        if(!activated)
        {
            return;
        }

        if (Vector3.Distance(movingPlatform.position, movingToLocation.position) > movingSpeed * Time.fixedDeltaTime)
        {
            movingPlatform.position += direction * movingSpeed * Time.fixedDeltaTime;
            //SetDestination(movingToLocation == startTransform ? endTransform : startTransform);
        }
    }

    public void SwitchDestination()
    {
        if (movingToLocation == startTransform)
        {
            movingToLocation = endTransform;
            movingFromLocation = movingPlatform;
            movingToStart = false;
        }
        else
        {
            movingToLocation = startTransform;
            movingFromLocation = movingPlatform;
            movingToStart = true;
        }

        direction = (movingToLocation.position - movingPlatform.position).normalized;
        finishedMoving = false;
    }

    private void OnDrawGizmos() //Visualisation Aid
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(startTransform.position, visualAid.transform.localScale);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(endTransform.position, visualAid.transform.localScale);
    }

    //void SetDestination(Transform destination)
    //{
    //    movingToLocation = destination;
    //    direction = (movingToLocation.position - movingPlatform.position).normalized;
    //}
}
