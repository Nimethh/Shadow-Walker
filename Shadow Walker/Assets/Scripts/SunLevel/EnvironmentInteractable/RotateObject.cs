using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Your Class
public class RotateObject : MonoBehaviour
{

    public bool activated;
    public bool rotateClockwise;
    private float rotation;

    public enum RotationType { FullCircle, FixedRotationAngle };
    public RotationType moveType;

    //Looping variables
    public float rotationSpeed;


    //FixedRotationAngle varialbes
    public int newAngle;
    public float tweenDuration;
    public bool backAndForward;
    private bool doneWithRotation;
    private bool hasReachedEnd;
    public float waitTime;
    private float waitTimeCounter;
    public bool repeating;
    public float sleepTime;
    private float sleepTimeCounter;
    private float tweenTime;


    private Quaternion originalSpot;

    void Start()
    {
        hasReachedEnd = false;
        originalSpot = transform.rotation;

        switch (moveType)
        {
            case RotationType.FullCircle:
                break;
            case RotationType.FixedRotationAngle:
                {
                    if (!rotateClockwise)
                    {
                        newAngle = newAngle*-1;
                    }
                }
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if (!activated || doneWithRotation)
        {
            return;
        }


        //switch statement for different variables
        switch (moveType)
        {

            //AutoMove
            case RotationType.FullCircle:
                {
                if (rotateClockwise)
                {
                    rotation -= rotationSpeed;

                    if (rotation < -360)
                        rotation += 360;
                }
                else
                {
                    rotation += rotationSpeed;

                    if (rotation > 360)
                        rotation -= 360;
                }

                Quaternion rot = Quaternion.Euler(0, 0, rotation);
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
                }break;

            //waypoint
            case RotationType.FixedRotationAngle:
                {
                    if(hasReachedEnd == false)
                    {
                        if(sleepTimeCounter > 0)
                        {
                            sleepTimeCounter -= Time.deltaTime;
                            if (sleepTimeCounter <= 0)
                            {
                                sleepTimeCounter = 0;
                            }
                            return;
                        }

                        Quaternion rot = Quaternion.Euler(0, 0, newAngle);
                        transform.rotation = Quaternion.Lerp(originalSpot, rot, BounceEaseOut(tweenTime,tweenDuration));
                        UpdateHasReachedEnd();
                    }
                    else
                    {

                        if(waitTimeCounter > 0)
                        {
                            waitTimeCounter -= Time.deltaTime;
                            if(waitTimeCounter <= 0)
                            {
                                waitTimeCounter = 0;
                            }
                            return;
                        }
                        else
                        {
                            Quaternion rot = Quaternion.Euler(0, 0, newAngle);
                            transform.rotation = Quaternion.Lerp(rot, originalSpot, easeTest());
                            UpdateHasReachedStartAgain();
                        }
                    }


                    IncreaseTweenTime();
                }
                break;
            default:
                Debug.Log("RotationType case not found, called from default");
                break;
        }//end switch

    }

    void IncreaseTweenTime()
    {
        tweenTime += Time.deltaTime;
        if (tweenTime > tweenDuration)
            tweenTime = tweenDuration;
    }

    void UpdateHasReachedEnd()
    {
        if(tweenTime >= tweenDuration)
        {
            if (!backAndForward)
            {
                doneWithRotation = true;
            }

            hasReachedEnd = true;
            waitTimeCounter = waitTime;
            tweenTime = 0f;
        }
    }

    void UpdateHasReachedStartAgain()
    {
        if (tweenTime >= tweenDuration)
        {
            if(!repeating)
            {
                doneWithRotation = true;
            }

            hasReachedEnd = false;
            sleepTimeCounter = sleepTime;
            tweenTime = 0f;
        }
    }

    float easeTest()
    {
        float ease = tweenTime / tweenDuration;

        return ease;
    }

    float CubicEaseIn(float t, float d)
    {
        return 1 * (t /= d) * t * t + 0;
    }

    float BounceEaseOut(float t, float d)
    {
        if ((t /= d) < (1 / 2.75f))
        {
            return 1 * (7.5625f * t * t) + 0;
        }
        else if (t < (2 / 2.75f))
        {
            float postFix = t -= (1.5f / 2.75f);
            //return 1 * (7.5625f * (postFix) * t + .75f) + 0;
            return 1 * (8.2f * (postFix) * t + .78f) + 0;
        }
        else if (t < (2.5 / 2.75))
        {
            float postFix = t -= (2.25f / 2.75f);
            return 1 * (7.5625f * (postFix) * t + .9375f) + 0;
        }
        else
        {
            float postFix = t -= (2.625f / 2.75f);
            return 1 * (7.5625f * (postFix) * t + .984375f) + 0;
        }
    }

} //end of class


//Custom inspector starts here
#if UNITY_EDITOR

[CustomEditor(typeof(RotateObject))]
[CanEditMultipleObjects]
public class RotateObjectInspectorEditor : Editor
{

    public override void OnInspectorGUI()
    {

        //cast target
        var enumScript = target as RotateObject;

        enumScript.activated = EditorGUILayout.Toggle("Activated", enumScript.activated);//bool example
        enumScript.rotateClockwise = EditorGUILayout.Toggle("RotateClockwise", enumScript.rotateClockwise);//bool example



        //Enum drop down
        enumScript.moveType = (RotateObject.RotationType)EditorGUILayout.EnumPopup(enumScript.moveType);
        EditorGUI.BeginChangeCheck();


        //switch statement for different variables
        switch (enumScript.moveType)
        {

            //AutoMove
            case RotateObject.RotationType.FullCircle:
                enumScript.rotationSpeed = EditorGUILayout.FloatField("RotationSpeed", enumScript.rotationSpeed); //float example
                break;

            //waypoint
            case RotateObject.RotationType.FixedRotationAngle:
                enumScript.newAngle = EditorGUILayout.IntSlider("NewAngle",enumScript.newAngle,0,360);
                enumScript.backAndForward = EditorGUILayout.Toggle("BackAndForward", enumScript.backAndForward);//bool example
                enumScript.waitTime = EditorGUILayout.FloatField("WaitTimeUntilBackwardsRotation", enumScript.waitTime); //float example
                enumScript.tweenDuration = EditorGUILayout.FloatField("TweenDuration", enumScript.tweenDuration); //float example
                enumScript.repeating = EditorGUILayout.Toggle("Repeating", enumScript.repeating);//bool example
                enumScript.sleepTime = EditorGUILayout.FloatField("SleepTime", enumScript.sleepTime); //float example




                //EditorGUI.BeginChangeCheck();
                //EditorGUILayout.PropertyField(rotationAngle, true); //array example (works with any Serialized property)
                //if (EditorGUI.EndChangeCheck()) //End Array inspector dropped down
                //serializedObject.ApplyModifiedProperties();
                break;
        }//end switch

        if (EditorGUI.EndChangeCheck()) //End Array inspector dropped down
            serializedObject.ApplyModifiedProperties();
    }
}//end inspectorclass

#endif