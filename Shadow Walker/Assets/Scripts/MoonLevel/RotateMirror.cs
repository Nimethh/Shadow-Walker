using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMirror : MonoBehaviour
{
    [Range(0f, 0.5f)]
    public float maxRotation = 0.2f;
    public bool canBeRotated = false;
    private PlayerPlatformController playerPlatformController;

    private Transform rotatingSurface;

    void Start()
    {
        playerPlatformController = GameObject.Find("Player").GetComponent<PlayerPlatformController>();

        rotatingSurface = this.transform.GetChild(0);
    }

    void Update()
    {
        EnableAndDisablePlayerController();
        if (canBeRotated == true && Input.GetKey(KeyCode.A))
        {
            if (rotatingSurface.transform.rotation.z < maxRotation)
            {
                rotatingSurface.transform.Rotate(0f, 0f, 0.15f);
            }
        }
        if (canBeRotated == true && Input.GetKey(KeyCode.D))
        {
            if (rotatingSurface.transform.rotation.z > maxRotation * -1f)
            {
                rotatingSurface.transform.Rotate(0f, 0f, -0.15f);
            }
        }
    }

    void EnableAndDisablePlayerController()
    {
        if (canBeRotated == true)
        {
            playerPlatformController.enabled = false;
        }
        else
            playerPlatformController.enabled = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == "Player" && Input.GetKeyDown(KeyCode.Space))
        {
            if (playerPlatformController == null)
                playerPlatformController = GameObject.Find("Player").GetComponent<PlayerPlatformController>();
            
            if (playerPlatformController.onGround == true)
            {
                canBeRotated = !canBeRotated;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            canBeRotated = false;
        }
    }
}
