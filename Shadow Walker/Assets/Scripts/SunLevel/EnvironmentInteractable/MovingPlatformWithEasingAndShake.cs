using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformWithEasingAndShake : MonoBehaviour
{
    private enum MovementPattern
    {
        StartToEnd,
        BackAndForth,
        Repeating
    }

    private enum MovementEasing
    {
        LinearEase,
        EaseInOutSine,
        EaseInSine,
        EaseOutSine,
        EaseInOutCubic,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutQuint,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuart,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutExpo
    }

    [SerializeField]
    public bool activated;

    [Header("Required Objects")]
    [SerializeField]
    private Transform movingPlatform;
    [SerializeField]
    private Transform startTransform;
    [SerializeField]
    private Transform endTransform;
    [SerializeField]
    private GameObject visualAid;

    [Header("Optional Objects")]
    [SerializeField]
    private ParticleSystem dustParticleSystem;

    [Header("MovementControl")]
    [SerializeField]
    private MovementPattern movementPattern;
    //[SerializeField]
    //private MovementEasing movementEasing;
    [SerializeField]
    private MovementEasing ToStartEasing;
    [SerializeField]
    private MovementEasing ToEndEasing;
    [SerializeField]
    private float tweenDuration;
    [SerializeField]
    private float waitTime;

    [Header("Shake Controls")]
    [SerializeField]
    private bool StartDestShake;
    [SerializeField]
    private bool EndDestShake;
    [SerializeField]
    private float shakeDuration;
    private float shakeTimer;
    [SerializeField]
    private float shakeIntensity;

    private Rigidbody2D movingPlatformRigid;
    private Vector3 direction;
    private Transform movingToLocation;
    private Transform movingFromLocation;

    private float waitTimeCounter;
    private bool finishedMoving;
    private bool hasReachedStartTransformation;
    private bool hasReachedEndTransformation;

    private float tweenTime;
    private float b = 0; //Required for easing
    private float c = 1; //Required for easing

    void Start()
    {
        finishedMoving = false;
        movingPlatformRigid = movingPlatform.GetComponent<Rigidbody2D>();
        //SetDestination(startTransform);
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

        if(finishedMoving || !activated || waitTimeCounter > 0 || shakeTimer > 0)
        {
            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;
                return;
            }

            if(waitTimeCounter > 0)
            {
                waitTimeCounter -= Time.deltaTime;
            }


            return;
        }

        UpdateEasingPosition(tweenTime, tweenDuration);
        UpdateTweenTime();
        UpdateDestination();
    }


    void UpdateEasingPosition(float t, float d)
    {
        if(movingToLocation == startTransform)
        {
            switch (ToStartEasing)
            {
                case MovementEasing.LinearEase:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, LinearEase());
                    }
                    break;
                case MovementEasing.EaseInOutSine:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, SineEaseInOut(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseInSine:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, SineEaseIn(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseOutSine:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, SineEaseOut(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseInOutCubic:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, CubicEaseInOut(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseInCubic:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, CubicEaseIn(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseOutCubic:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, CubicEaseOut(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseInOutQuint:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, QuintEaseInOut(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseInQuint:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, QuintEaseIn(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseOutQuint:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, QuintEaseOut(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseInOutQuart:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, QuartEaseInOut(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseInQuart:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, QuartEaseIn(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseOutQuart:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, QuartEaseOut(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseInOutExpo:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, ExpoEaseInOut(tweenTime, tweenDuration));

                    }
                    break;
                default:
                    Debug.Log("Default case UpdateEasingPosition");
                    break;
            }
        }
        else
        {
            switch (ToEndEasing)
            {
                case MovementEasing.LinearEase:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, LinearEase());
                    }
                    break;
                case MovementEasing.EaseInOutSine:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, SineEaseInOut(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseInSine:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, SineEaseIn(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseOutSine:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, SineEaseOut(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseInOutCubic:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, CubicEaseInOut(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseInCubic:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, CubicEaseIn(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseOutCubic:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, CubicEaseOut(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseInOutQuint:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, QuintEaseInOut(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseInQuint:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, QuintEaseIn(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseOutQuint:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, QuintEaseOut(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseInOutQuart:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, QuartEaseInOut(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseInQuart:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, QuartEaseIn(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseOutQuart:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, QuartEaseOut(tweenTime, tweenDuration));
                    }
                    break;
                case MovementEasing.EaseInOutExpo:
                    {
                        movingPlatform.position = Vector3.Lerp(movingFromLocation.position, movingToLocation.position, ExpoEaseInOut(tweenTime, tweenDuration));

                    }
                    break;
                default:
                    Debug.Log("Default case UpdateEasingPosition");
                    break;
            }
        }

    }

    void UpdateDestination()
    {
        if (tweenTime + Time.fixedDeltaTime >= tweenDuration)
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

            SetWaitTime();
            SetDestination(movingToLocation == startTransform ? endTransform : startTransform);
        }
    }

    void SetWaitTime()
    {
        waitTimeCounter = waitTime;
    }

    void SetDestination(Transform destination)
    {
        if(movingToLocation == startTransform)
        {
            if (StartDestShake)
            {
                StartCoroutine(Shake(shakeDuration, shakeIntensity));
                shakeTimer = shakeDuration;
            }
            movingToLocation = destination;
            movingFromLocation = startTransform;
        }
        else
        {
            if (EndDestShake)
            {
                StartCoroutine(Shake(shakeDuration, shakeIntensity));
                shakeTimer = shakeDuration;
            }
            movingToLocation = destination;
            movingFromLocation = endTransform;
        }

        direction = (movingToLocation.position - movingPlatform.position).normalized;
        tweenTime = 0;
    }

    private void OnDrawGizmos() //Visualisation Aid
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(startTransform.position, visualAid.transform.localScale);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(endTransform.position, visualAid.transform.localScale);
    }

    void UpdateTweenTime()
    {
        tweenTime += Time.deltaTime;
        if (tweenTime > tweenDuration)
            tweenTime = tweenDuration;
    }

    float LinearEase()
    {
        float ease = tweenTime / tweenDuration;

        return ease;
    }

    float SineEaseInOut(float t, float d)
    {
        return -c / 2 * (Mathf.Cos(Mathf.PI * t / d) - 1) + b;
    }

    float SineEaseIn(float t, float d)
    {
        return -c * Mathf.Cos(t / d * (Mathf.PI / 2)) + c + b;
    }

    float SineEaseOut(float t, float d)
    {
        return c * Mathf.Sin(t / d * (Mathf.PI / 2)) + b;
    }

    float CubicEaseInOut(float t, float d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
        return c / 2 * ((t -= 2) * t * t + 2) + b;
    }

    float CubicEaseIn(float t, float d)
    {
        return c * (t /= d) * t * t + b;
    }
    float CubicEaseOut(float t, float d)
    {
        return c * ((t = t / d - 1) * t * t + 1) + b;
    }

    float QuintEaseInOut(float t, float d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
        return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
    }

    float QuintEaseIn(float t, float d)
    {
        return c * (t /= d) * t * t * t * t + b;
    }
    float QuintEaseOut(float t, float d)
    {
        return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
    }

    float QuartEaseInOut(float t, float d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
        return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
    }

    float QuartEaseIn(float t, float d)
    {
        return c * (t /= d) * t * t * t + b;
    }
    float QuartEaseOut(float t, float d)
    {
        return -c * ((t = t / d - 1) * t * t * t - 1) + b;
    }

    float ExpoEaseInOut(float t, float d)
    {
        if (t == 0) return b;
        if (t == d) return b + c;
        if ((t /= d / 2) < 1) return c / 2 * Mathf.Pow(2, 10 * (t - 1)) + b;
        return c / 2 * (-Mathf.Pow(2, -10 * --t) + 2) + b;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {


        Vector3 originalPosition = movingPlatform.transform.position;

        if(dustParticleSystem != null)
        {
            dustParticleSystem.Play();
        }

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            if (Time.timeScale == 0)
            {
                yield break;
            }

            float scale = Mathf.Lerp(magnitude, 0, elapsed / duration);

            float x = Random.Range(-0.5f, 0.5f) * scale;
            float y = Random.Range(-0.5f, 0.5f) * scale;

            movingPlatform.transform.position = new Vector3(x + originalPosition.x, y + originalPosition.y, originalPosition.z);

            elapsed = elapsed + Time.deltaTime;

            yield return null;
        }

        movingPlatform.transform.position = originalPosition;
    }

}
