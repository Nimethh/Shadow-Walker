using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSunBehaviorUpdated : AffectedByTheSun
{
    private GameObject startingPoint;
    [HideInInspector]
    public Vector3 spawningPos;

    private Animator animator;
    private PlayerUpdated player;
   
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
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerUpdated>();
        timeInSun = 0;
        SetUpSpawningPosition();
    }

    public void Update()
    {
        AffectedByTheSunScriptUpdate();

        if (isRespawning) // Added 2019-05-19
        {
            transform.position = spawningPos;
            isRespawning = false;
            animator.speed = 1;
            animator.SetBool("Climbing", false);
        }
        if (isDead)
        {
            animator.SetBool("Climbing", false);
            animator.speed = 1;
        }
    }

    public override void JustGotCoveredFromSunlight()
    {
        if (timeInSun > 0)
        {
            timeInSun = 0;
        }
        //Debug.Log("JustGotCoveredFromSunlight()");
    }

    public override void JustGotExposedToSunlight()
    {

        timeInSun += Time.deltaTime;

        // play burning sound.        

        //Debug.Log("JustGotExposedToSunlight()");

    }

    public override void UnderFullCover()
    {
        if(!isDead)
        { 
            isDead = false;
        }
        //Debug.Log("UnderFullCover()");
    }

    public override void UnderFullExposure()
    {
        if (isSafeFromSun)
        {
            timeInSun = 0;
            return;
        }
        timeInSun += Time.deltaTime;
        if (timeInSun > timeInSunAllowed && doneRespawning)
        {
            isDead = true;
            player.velocity.x = 0;
        }
        //Debug.Log("UnderFullExposure()");
    }

    public override void UnderPartialCover()
    {
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
        }
        //Debug.Log("UnderPartialCover()");
    }

    // Added 2019-05-19
    public void PlayerIsDead()
    {
        doneRespawning = false;
        animator.SetBool("Climbing", false);
        animator.speed = 1;
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

    // Added 2019-11-09
    void SetUpSpawningPosition()
    {
        startingPoint = GameObject.Find("Door");
        spawningPos.x = startingPoint.transform.position.x;
        spawningPos.y = startingPoint.transform.position.y;
        spawningPos.z = -3;
        if (SceneManager.GetActiveScene().name != "Level1") //Added 30/5/2019
        {
            transform.position = spawningPos;
        }
    }
}
