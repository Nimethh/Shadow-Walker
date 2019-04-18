using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeButtonMovingPlatformUpdated : MonoBehaviour
{
    public MovingPlatformUpdated platform;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            platform.activated = true;
        }
    }
}
