using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSunBehaviorUpdatedMobile : AffectedByTheSun
{
    private GameObject startingPoint;
    [HideInInspector]
    //public Vector2 spawningPos;
    public Vector3 spawningPos;


    Animator animator;
    PlayerInputUpdatedMobile playerInput;
    PlayerUpdatedMobile player;
    //private PlayerPlatformController playerPlatformController;
    
    private float timeInSun;
    [SerializeField]
    private float timeInSunAllowed;

    [HideInInspector]
    public bool isDead = false; // Added 2019-05-19
    [HideInInspector]
    public bool isRespawning = false; // Added 2019-05-19
    [HideInInspector]
    public bool doneRespawning = true; // Added 2019-05-19

    [HideInInspector]
    public bool isSafeFromSun = false; //Added 2019-05-21

    public void Start()
    {
        AffectedByTheSunScriptStart();
        
        //playerPlatformController = GetComponent<PlayerPlatformController>();

        animator = GetComponent<Animator>();
        player = GetComponent<PlayerUpdatedMobile>();
        playerInput = GetComponent<PlayerInputUpdatedMobile>();
        startingPoint = GameObject.Find("Door");
        timeInSun = 0;
        //spawningPos = startingPoint.transform.position;
        spawningPos.x = startingPoint.transform.position.x;
        spawningPos.y = startingPoint.transform.position.y;
        spawningPos.z = -2;

    }

    public void Update()
    {
        AffectedByTheSunScriptUpdate();
        
        if (isRespawning) // Added 2019-05-19
        {
            transform.position = spawningPos;
            isRespawning = false;
        }
    }

    public override void JustGotCoveredFromSunlight()
    {
        if (timeInSun > 0)
        {
            timeInSun = 0;
        }
        //isDead = false; // Added 2019-05-19
        //Debug.Log("JustGotCoveredFromSunlight()");
    }

    public override void JustGotExposedToSunlight()
    {

        timeInSun += Time.deltaTime;
        if (timeInSun > timeInSunAllowed)
        {
            //playerPlatformController.StopAllMovement(0.8f);
            //transform.position = startingPoint.transform.position;
            //transform.position = spawningPos;
        }

        //playerController.StopAllMovement(0.8f);
        //playerPlatformController.StopAllMovement(0.8f);
        //transform.position = startingPoint.transform.position;
        //Debug.Log("JustGotExposedToSunlight()");

    }

    public override void UnderFullCover()
    {
        //Debug.Log("UnderFullCover()");
        isDead = false;
    }

    public override void UnderFullExposure()
    {
        //Debug.Log("UnderFullExposure()");
        if (isSafeFromSun)
        {
            timeInSun = 0;
            return;
        }
        timeInSun += Time.deltaTime;
        if (timeInSun > timeInSunAllowed && doneRespawning)
        {
            //playerPlatformController.StopAllMovement(0.8f);
            //transform.position = startingPoint.transform.position;
            isDead = true;
            player.velocity.x = 0;
        }
        //else
        //    isDead = false;
    }

    //public override void UnderFullExposure()
    //{
    //    //Debug.Log("UnderFullExposure()");
    //    timeInSun += Time.deltaTime;
    //    if (timeInSun > timeInSunAllowed && doneRespawning)
    //    {
    //        //playerPlatformController.StopAllMovement(0.8f);
    //        //transform.position = startingPoint.transform.position;
    //        isDead = true;
    //        player.velocity.x = 0;
    //    }
    //    //else
    //    //    isDead = false;
    //}

    public override void UnderPartialCover()
    {
        //Debug.Log("UnderPartialCover()");
        if (isSafeFromSun)
        {
            timeInSun = 0;
            return;
        }
        timeInSun += Time.deltaTime;
        if (timeInSun > timeInSunAllowed && doneRespawning)
        {
            // Added 2019-05-19
            isDead = true;
            player.velocity.x = 0;

            //playerPlatformController.StopAllMovement(0.8f);
            //transform.position = startingPoint.transform.position;
        }
        //else // Added 2019-05-19
        //{
        //    isDead = false;
        //}
    }

    //public override void UnderPartialCover()
    //{
    //    //Debug.Log("UnderPartialCover()");
    //    timeInSun += Time.deltaTime;
    //    if (timeInSun > timeInSunAllowed && doneRespawning)
    //    {
    //        // Added 2019-05-19
    //        isDead = true;
    //        player.velocity.x = 0;

    //        //playerPlatformController.StopAllMovement(0.8f);
    //        //transform.position = startingPoint.transform.position;
    //    }
    //    //else // Added 2019-05-19
    //    //{
    //    //    isDead = false;
    //    //}
    //}

    // Added 2019-05-19
    public void PlayerIsDead()
    {
        doneRespawning = false;
        isDead = true;
    }

    // Added 2019-05-19
    public void Respawning()
    {
        isRespawning = true;
        isDead = false;
        player.spawnedInSafePoint = true;
    }

    // Added 2019-05-19
    public void DoneRespawning()
    {
        doneRespawning = true;
    }

    // Added 2019-05-19
    public void ResetAnimationBools()
    {
        isDead = false;
        isRespawning = false;
        doneRespawning = true;
        player.spawnedInSafePoint = false;
    }
}
