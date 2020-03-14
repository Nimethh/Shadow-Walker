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
        AffectedByTheSunScriptUpdate();
    }

    public override void JustGotCoveredFromSunlight()
    {
        audioManager.Stop("Death");
        if (timeInSun > 0)
        {
            timeInSun = 0.0f;
        }
    }

    public override void JustGotExposedToSunlight()
    {
        audioManager.Play("Death");
        // play burning particle.
    }

    public override void UnderFullCover()
    {
        audioManager.Stop("Death");
        timeInSun = 0.0f;
    }

    public override void UnderFullExposure()
    {
        audioManager.Play("Death");
        timeInSun += Time.deltaTime;
        if (timeInSun > timeInSunAllowed)
        {
            isDead = true;
            timeInSun = 0;
        }
    }

    public override void UnderPartialCover()
    {
        audioManager.Play("Death");
        timeInSun += Time.deltaTime;
        if (timeInSun > timeInSunAllowed)
        {
            isDead = true;
            timeInSun = 0;
        }
    }
}
