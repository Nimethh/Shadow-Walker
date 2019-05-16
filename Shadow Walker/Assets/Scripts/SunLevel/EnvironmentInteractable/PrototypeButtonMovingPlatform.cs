using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeButtonRotatingPlatform : MonoBehaviour
{
    public RotateObject platform;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            platform.activated = true;
        }
    }
}
