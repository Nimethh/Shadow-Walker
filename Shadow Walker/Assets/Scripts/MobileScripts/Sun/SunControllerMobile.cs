using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunControllerMobile : MonoBehaviour
{
    [SerializeField]
    private Transform[] points;
    [SerializeField]
    private Transform centerPosition;
    public int index;

    public bool canMove = true;
    public int checkPointIndex = 100;
    private int numberOfPoints = 100;
    float startT;

    AudioManager audioManager;

    float velocity;

    [Range(0f, 1f)]
    [SerializeField]
    private float sunSpeed = 0.2f;

    [SerializeField]
    private float maxVelocity = 0.7f;

    float right = 0;
    float left = 0;

    float t = 0.0f;

    bool restrictMovement = false;
    //public VirtualSunJoystick sunJoystick;

    [SerializeField]
    enum SunMovementType
    {
        DRAG,
        TILT
    }
    
    [SerializeField]
    SunMovementType movementType;

    private void Start()
    {
        //transform.position = points[0].position;
        startT = index / (float)numberOfPoints * sunSpeed;
        transform.position = CalculateQuadraticBezeirPoint(startT, points[0].position, points[1].position, points[2].position);
        audioManager = FindObjectOfType<AudioManager>();
        FindSunBounds();
        velocity = 0.0f;
    }

    private void FixedUpdate()
    {
        /*if (sunJoystick.Horizontal() > 0.4f && transform.position.x < points[2].position.x)
        {
            MoveRight();
        }
        if (sunJoystick.Horizontal() < -0.4f && transform.position.x > points[0].position.x)
        {
            MoveLeft();
        }

        if (sunJoystick.Horizontal() > 0.4f && transform.position.x < points[2].position.x)
        {
            MoveRight();
        }
        else if (sunJoystick.Horizontal() < -0.4f && transform.position.x > points[0].position.x)
        {
            MoveLeft();
        }*/

        if (Input.acceleration.x > 0.15f && transform.position.x < points[2].position.x)
        {
            velocity = Input.acceleration.x;
            MoveRight();
        }
        else if (Input.acceleration.x < -0.15f && transform.position.x > points[0].position.x)
        {
            velocity = Input.acceleration.x;
            MoveLeft();
        }
        else
        {
            audioManager.Mute("SunMoving");
            //FindObjectOfType<AudioManager>().Stop("SunMoving");
        }

    }

    public void FindSunBounds()
    {
        right = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane)).x;
        left = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)).x;
    }

    public void CheckSunBounds()
    {
        //Right
        if (transform.position.x > right - 0.2f)// && transform.position.x > left)
        {
            transform.position = new Vector3(right - 0.2f, transform.position.y, transform.position.z);
        }

        //Left
        if (transform.position.x < left + 0.2f)
        {
            transform.position = new Vector3(left + 0.2f, transform.position.y, transform.position.z);
        }

    }

    void MoveRight()
    {
        if (restrictMovement)
        {
            if (transform.position.x < right - 0.2f)
            {
                //index++;
                //float t = index / (float)numberOfPoints * sunSpeed;
                //if (velocity <= 0.75f)
                //    velocity = 0.75f;
                velocity = Mathf.Clamp(velocity, -maxVelocity, maxVelocity);
                t = t + (velocity * velocity) * 0.02f;
                //Debug.Log(t);
                transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
                audioManager.Play("SunMoving");
            }
        }
        else
        {
            //index++;
            //float t = index / (float)numberOfPoints * sunSpeed;
            velocity = Mathf.Clamp(velocity, -maxVelocity, maxVelocity);
            t = t + (velocity * velocity) * 0.02f;
            transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
            audioManager.Play("SunMoving");
        }
    }

    void MoveLeft()
    {
        if (restrictMovement)
        {
            if (transform.position.x > left + 0.2f)
            {
                //index--;
                //float t = index / (float)numberOfPoints * sunSpeed;
                velocity = Mathf.Clamp(velocity, -maxVelocity, maxVelocity);
                t = t - (velocity * velocity) * 0.02f;
                transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
                audioManager.Play("SunMoving");
            }
        }
        else
        {
            //index--;
            //float t = index / (float)numberOfPoints * sunSpeed;
            velocity = Mathf.Clamp(velocity, -maxVelocity, maxVelocity);
            t = t - (velocity* velocity) * 0.02f;
            transform.position = CalculateQuadraticBezeirPoint(t, points[0].position, points[1].position, points[2].position);
            audioManager.Play("SunMoving");
        }
    }

    void Rotate()
    {
        transform.LookAt(centerPosition);
    }

    Vector3 CalculateQuadraticBezeirPoint(float t, Vector3 point1, Vector3 point2, Vector3 point3)
    {
        float i = 1 - t;
        float tSquared = t * t;
        float iSquared = i * i;

        Vector3 point = (iSquared * point1) + (2 * i * t * point2) + (tSquared * point3);

        return point;
    }
}
