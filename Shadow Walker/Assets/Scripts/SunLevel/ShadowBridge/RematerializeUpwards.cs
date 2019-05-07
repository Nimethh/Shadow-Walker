using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RematerializeUpwards : MonoBehaviour
{
    public Material dissolveMaterial;
    private float max;

    public float rematerializeDuration;
    private float rematerializeTime;

    public SpriteRenderer spriteRenderer;

    [SerializeField] public float currentY;

    private void Start()
    {
        rematerializeTime = 0;
        //rematerializeDuration = 1.5f;
        StartRematerializing();
        max = spriteRenderer.bounds.max.y - spriteRenderer.bounds.min.y;
        dissolveMaterial.SetFloat("_StartingY", spriteRenderer.bounds.min.y);
    }

    public void Update()
    {

        rematerializeTime += Time.deltaTime;
        if (rematerializeTime > rematerializeDuration)
            rematerializeTime = rematerializeDuration;

        if (currentY < max)
        {
            dissolveMaterial.SetFloat("_RematerializeY", currentY);
            currentY = LinearEase() * max;
        }
    }

    float LinearEase()
    {
        float ease = rematerializeTime / rematerializeDuration;

        return ease;
    }

    public void StartRematerializing()
    {
        currentY = 0;
        dissolveMaterial.SetFloat("_RematerializeY", currentY);
        rematerializeTime = 0;
    }

}
