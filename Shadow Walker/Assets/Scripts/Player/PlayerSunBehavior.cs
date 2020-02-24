using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSunBehavior : AffectedByTheSun
{
    [SerializeField]
    private float timeInSun;
    [SerializeField]
    private float timeInSunAllowed = 0.5f;

    [HideInInspector]
    public bool isDead = false;
    [HideInInspector]
    public bool isSafeFromSun = true;

    AudioManager audioManager;

    public void Start()
    {
        AffectedByTheSunScriptStart();
        timeInSun = 0;
        isSafeFromSun = true;
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void Update()
    {
        if(!isSafeFromSun)
            AffectedByTheSunScriptUpdate();
    }

    public override void JustGotCoveredFromSunlight()
    {
        audioManager.Stop("Death");
        if (timeInSun > 0)
        {
            timeInSun = 0;
        }
    }

    public override void JustGotExposedToSunlight()
    {
        audioManager.Play("Death");
        // play burning particle.
    }

    public override void UnderFullCover()
    {
        if (!isDead)
        {
            isDead = false;
        }
    }

    public override void UnderFullExposure()
    {
        if (isSafeFromSun)
        {
            timeInSun = 0;
            return;
        }
        timeInSun += Time.deltaTime;
        if (timeInSun > timeInSunAllowed)
        {
            isDead = true;
        }
    }

    public override void UnderPartialCover()
    {
        if (isSafeFromSun)
        {
            timeInSun = 0;
            return;
        }
        timeInSun += Time.deltaTime;
        if (timeInSun > timeInSunAllowed)
        {
            isDead = true;
        }
    }
}
