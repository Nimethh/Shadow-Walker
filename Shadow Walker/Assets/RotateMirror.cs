using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMirror : MonoBehaviour
{
    [Range(0f, 0.5f)]
    public float maxRotation = 0.2f;
    public bool canBeRotated = false;
    [SerializeField]
    private PlayerControllerMoon playerController;

    private Transform rotatingSurface;

    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerControllerMoon>();
        rotatingSurface = this.transform.GetChild(0);
    }

    void Update()
    {
        if (canBeRotated == true && Input.GetKey(KeyCode.A))
        {
            if (rotatingSurface.transform.rotation.z < maxRotation)
            {
                rotatingSurface.transform.Rotate(0f, 0f, 0.1f);
            }
        }
        if (canBeRotated == true && Input.GetKey(KeyCode.D))
        {
            if (rotatingSurface.transform.rotation.z > maxRotation * -1f)
            {
                rotatingSurface.transform.Rotate(0f, 0f, -0.1f);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == "Player" && Input.GetKeyDown(KeyCode.Space))
        {
            if (playerController == null)
                playerController = GameObject.Find("Player").GetComponent<PlayerControllerMoon>();

            if (playerController.onGround == true)
                canBeRotated = !canBeRotated;
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
