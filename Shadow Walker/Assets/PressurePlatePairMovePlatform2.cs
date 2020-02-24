using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlatePairMovePlatform2 : MonoBehaviour
{
    //public MovingPlatformWithEasingAndShakePressurePlatePair platform;
    public PressurePlatform platform;

    //[SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite plate;
    [SerializeField] private Sprite plateActivated;

    [SerializeField] private Animator anim;

    public enum PlatformDirection
    {
        start,
        end
    }

    public PlatformDirection plateDirection;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Box"))
        {
            platform.activated = true;
            //spriteRenderer.sprite = plateActivated;
            anim.SetBool("ShouldBeDown", true);
            //Added 2019-05-15
            FindObjectOfType<AudioManager>().Play("PressurePlate");


            switch (plateDirection)
            {
                case PlatformDirection.start:
                    {
                        if (!platform.movingToStart)
                        {
                            platform.SwitchDestination();
                        }
                    }
                    break;
                case PlatformDirection.end:
                    {
                        if (platform.movingToStart)
                        {
                            platform.SwitchDestination();
                        }
                    }
                    break;
                default:
                    break;
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Box"))
        {
            platform.activated = false;
            //spriteRenderer.sprite = plate;
            anim.SetBool("ShouldBeDown", false);


        }
    }
}
