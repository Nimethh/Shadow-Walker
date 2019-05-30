using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlfriendAnmationManager : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(gameObject.name == "PlayerDetection" && other.gameObject.CompareTag("Player"))
        {
            Animator anim = GetComponentInParent<Animator>();
            anim.SetTrigger("Turn");
        }
        else if(gameObject.name == "Girlfriend" && other.gameObject.CompareTag("Player"))
        {
            Animator anim = GetComponent<Animator>();
            anim.SetTrigger("Turn");
        }
    }
}
