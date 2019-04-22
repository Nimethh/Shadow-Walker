using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Code from www.youtube.com/watch?v=9A9yj8KnM8c
//Channel: Brackeys
//Title of video: CAMERA SHAKE in Unity, released 25 Feb. 2018

    public enum ShakeEasing
    {
        LinearEase,
        EaseInOutSine,
        EaseInOutCubic,
        EaseInOutQuint,
        EaseInOutQuart,
        EaseInOutExpo
    }

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude, ShakeEasing camEaseType)
    {
        Debug.Log("Shake!");


        Vector3 originalPosition = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            if (Time.timeScale == 0)
            {
                yield break;
            }

            float scale = 0.0f;

            switch (camEaseType)
            {
                case ShakeEasing.LinearEase:
                    {
                        scale = Mathf.Lerp(magnitude, 0, LinearEase(elapsed, 0, magnitude, duration));
                    }
                    break;
                case ShakeEasing.EaseInOutSine:
                    {
                        scale = Mathf.Lerp(magnitude, 0, SineEaseInOut(elapsed, 0, magnitude, duration));
                    }
                    break;
                case ShakeEasing.EaseInOutCubic:
                    {
                        scale = Mathf.Lerp(magnitude, 0, CubicEaseInOut(elapsed, 0, magnitude, duration));
                    }
                    break;
                case ShakeEasing.EaseInOutQuint:
                    {
                        scale = Mathf.Lerp(magnitude, 0, QuintEaseInOut(elapsed, 0, magnitude, duration));
                    }
                    break;
                case ShakeEasing.EaseInOutQuart:
                    {
                        scale = Mathf.Lerp(magnitude, 0, QuartEaseInOut(elapsed, 0, magnitude, duration));
                    }
                    break;
                case ShakeEasing.EaseInOutExpo:
                    {
                        scale = Mathf.Lerp(magnitude, 0, ExpoEaseInOut(elapsed, 0, magnitude, duration));
                    }
                    break;
                default:
                    Debug.Log("Default case CameraShake");
                    break;
            }

            float x = Random.Range(-1, 1) * scale;
            float y = Random.Range(-1, 1) * scale;

            transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed = elapsed + Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    float LinearEase(float t, float b, float c, float d)
    {
        float ease = t / d;

        return ease;
    }

    float SineEaseInOut(float t, float b, float c, float d)
    {
        return -c / 2 * (Mathf.Cos(Mathf.PI * t / d) - 1) + b;
    }

    float CubicEaseInOut(float t, float b, float c, float d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
        return c / 2 * ((t -= 2) * t * t + 2) + b;
    }

    float QuintEaseInOut(float t, float b, float c, float d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
        return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
    }

    float QuartEaseInOut(float t, float b, float c, float d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
        return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
    }

    float ExpoEaseInOut(float t, float b, float c, float d)
    {
        if (t == 0) return b;
        if (t == d) return b + c;
        if ((t /= d / 2) < 1) return c / 2 * Mathf.Pow(2, 10 * (t - 1)) + b;
        return c / 2 * (-Mathf.Pow(2, -10 * --t) + 2) + b;
    }
}
