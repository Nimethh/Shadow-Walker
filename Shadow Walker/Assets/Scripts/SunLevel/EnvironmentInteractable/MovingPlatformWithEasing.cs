using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformWithEasing : MonoBehaviour
{
    private enum MovementPattern
    {
        StartToEnd,
        BackAndForth,
        Repeating
    }

    private enum MovementEasing
    {
        EaseInOutSine,
        EaseInOutCubic,
        EaseInOutQuint,
        EaseInOutQuart,
        EaseInOutExpo
    }

    [SerializeField]
    public bool activated;
    [SerializeField]
    private Transform movingPlatform;
    private Rigidbody2D movingPlatformRigid;
    private Vector3 direction;
    public Transform movingToLocation;
    public Transform movingFromLocation;

    [SerializeField]
    private float movingSpeed;
    private bool finishedMoving;
    private bool hasReachedStartTransformation;
    private bool hasReachedEndTransformation;


    [SerializeField]
    private float tweenTime;
    [SerializeField]
    private float tweenDuration;
    private float b = 0; //Required for easing
    private float c = 1; //Required for easing

    [SerializeField]
    private MovementPattern movementPattern;
    [SerializeField]
    private MovementEasing movementEasing;

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
        movingToLocation = startTransform;
        movingFromLocation = endTransform;
    }

    float easeTest()
    {
        float ease = tweenTime / tweenDuration;

        return ease;
    }

    void FixedUpdate()
    {

        if(finishedMoving || !activated)
        {
            return;
        }

        UpdateEasingPosition(tweenTime, tweenDuration);
        UpdateTweenTime();
        UpdateDestination();
    }


    void UpdateEasingPosition(float t, float d)
    {
        switch (movementEasing)
        {
            case MovementEasing.EaseInOutSine:
                {
                    movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, SineEaseInOut(tweenTime, tweenDuration));
                }
                break;
            case MovementEasing.EaseInOutCubic:
                {
                    movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, CubicEaseInOut(tweenTime, tweenDuration));
                }
                break;
            case MovementEasing.EaseInOutQuint:
                {
                    movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, QuintEaseInOut(tweenTime, tweenDuration));
                }
                break;
            case MovementEasing.EaseInOutQuart:
                {
                    movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, QuartEaseInOut(tweenTime, tweenDuration));
                }
                break;
            case MovementEasing.EaseInOutExpo:
                {
                    movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, ExpoEaseInOut(tweenTime, tweenDuration));

                }break;
            default:
                Debug.Log("Default case UpdateEasingPosition");
                break;
        }
    }

    void UpdateDestination()
    {
        if (tweenTime + Time.deltaTime >= tweenDuration)
        {
            if (movingToLocation == startTransform)
            {
                hasReachedStartTransformation = true;
                if (movementPattern == MovementPattern.StartToEnd)
                {
                    finishedMoving = true;
                }
            }
            else
            {
                hasReachedEndTransformation = true;
                if (movementPattern == MovementPattern.BackAndForth)
                {
                    finishedMoving = true;
                }
            }

            SetDestination(movingToLocation == startTransform ? endTransform : startTransform);
        }
    }

    void SetDestination(Transform destination)
    {
        if(movingToLocation == startTransform)
        {
            movingToLocation = destination;
            movingFromLocation = startTransform;
        }
        else
        {
            movingToLocation = destination;
            movingFromLocation = endTransform;
        }

        direction = (movingToLocation.position - movingPlatform.position).normalized;
        tweenTime = 0;
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

    void UpdateTweenTime()
    {
        tweenTime += Time.deltaTime;
        if (tweenTime > tweenDuration)
            tweenTime = tweenDuration;
    }

    float SineEaseInOut(float t, float d)
    {
        return -c / 2 * (Mathf.Cos(Mathf.PI * t / d) - 1) + b;
    }

    float CubicEaseInOut(float t, float d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
        return c / 2 * ((t -= 2) * t * t + 2) + b;
    }

    float QuintEaseInOut(float t, float d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
        return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
    }

    float QuartEaseInOut(float t, float d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
        return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
    }

    float ExpoEaseInOut(float t, float d)
    {
        if (t == 0) return b;
        if (t == d) return b + c;
        if ((t /= d / 2) < 1) return c / 2 * Mathf.Pow(2, 10 * (t - 1)) + b;
        return c / 2 * (-Mathf.Pow(2, -10 * --t) + 2) + b;
    }
}
