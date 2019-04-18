using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActivatedMovingPlatform : MonoBehaviour
{
    [SerializeField]
    public bool activated;

    [SerializeField]
    private Transform movingPlatform;
    private Rigidbody2D movingPlatformRigid;

    [SerializeField]
    private GameObject visualAid;

    [SerializeField]
    private float movingSpeed;

    Vector3 direction;
    Transform movingDestination;


    //Visualisation Aid ( Only for the scene window )
    [SerializeField]
    private Transform startTransform;
    [SerializeField]
    private Transform endTransform;

    void Start()
    {
        activated = false;
        movingPlatformRigid = movingPlatform.GetComponent<Rigidbody2D>();
        SetDestination(startTransform);
    }

    void FixedUpdate()
    {
        if(!activated)
        {
            return;
        }

        //movingPlatformRigid.MovePosition(movingPlatform.position + direction * movingSpeed * Time.fixedDeltaTime);
        movingPlatform.position += direction * movingSpeed * Time.fixedDeltaTime;

        if (Vector3.Distance(movingPlatform.position, movingDestination.position) < movingSpeed * Time.fixedDeltaTime)
        {
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
