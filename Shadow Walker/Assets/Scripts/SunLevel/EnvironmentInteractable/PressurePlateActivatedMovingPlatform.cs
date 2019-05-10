using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateActivatedMovingPlatform : MonoBehaviour
{
    public MovingPlatformWithEasingAndShake platform;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite plate;
    [SerializeField] private Sprite plateActivated;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Box"))
        {
            platform.activated = true;

            spriteRenderer.sprite = plateActivated;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Box"))
        {
            platform.activated = false;
            spriteRenderer.sprite = plate;

        }
    }
}
