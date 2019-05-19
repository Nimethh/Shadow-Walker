using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteReplace : MonoBehaviour
{
    public Material material;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            material.SetFloat("_Value", 1);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            material.SetFloat("_Value", 0);
        }
    }
}
