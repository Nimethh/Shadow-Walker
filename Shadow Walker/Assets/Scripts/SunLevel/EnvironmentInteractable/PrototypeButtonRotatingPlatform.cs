using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeButtonMovingPlatform : MonoBehaviour
{
    public ButtonActivatedMovingPlatform platform;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            platform.activated = true;
            //FindObjectOfType<AudioManager>().Play("Drawbridge falling");
        }
    }
}
