using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DematerializeDown : MonoBehaviour
{
    public Material dissolveMaterial;
    private float max;

    public float dematerializeDuration;
    private float dematerializeTime;

    public SpriteRenderer spriteRenderer;


    [SerializeField] public float currentY;

    private void Start()
    {
        dematerializeTime = 0;
        //dematerializeDuration = 1.5f;
        StartDissolving();
        //max = spriteRenderer.bounds.max.y - spriteRenderer.bounds.min.y +0.1f;
        max = spriteRenderer.bounds.max.y - spriteRenderer.bounds.min.y +0.01f;
        dissolveMaterial.SetFloat("_StartingY", spriteRenderer.bounds.max.y);
    }

    public void Update()
    {

        dematerializeTime += Time.deltaTime;
        if (dematerializeTime > dematerializeDuration)
            dematerializeTime = dematerializeDuration;

        if (currentY < max)
        {
            currentY = LinearEase() * max;//Swapped places
            dissolveMaterial.SetFloat("_DissolveY", currentY);
        }

    }

    float LinearEase()
    {
        float ease = dematerializeTime / dematerializeDuration;

        return ease;
    }

    public void StartDissolving()
    {
        currentY = 0;
        dematerializeTime = 0;
    }

}
