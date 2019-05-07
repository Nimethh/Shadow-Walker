using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private PlayerSunBehavior playerSunBehavior;
    //private SunController sunController;
    //public int sunCheckPointIndex;
    void Start()
    {
        playerSunBehavior = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSunBehavior>();
        //sunController = GameObject.FindGameObjectWithTag("Sun").GetComponent<SunController>();
        //sunCheckPointIndex = sunController.checkPointIndex;
    }

    void Update()
    {
        if (playerSunBehavior.isExposedToSunlight)
        {
            Debug.Log("Update Check");
            SpawnPlayer();

            //sunController.canMove = false;
            //MoveSunToCheckPointPos();
        }
        //else
        //{
        //    sunController.canMove = true;
        //}
    }

    void SpawnPlayer()
    {
        playerSunBehavior.gameObject.SetActive(true);
        playerSunBehavior.gameObject.transform.position = playerSunBehavior.spawningPos.transform.position;
        //sunController.canMove = true;
    }

    //public void MoveSunToCheckPointPos()
    //{
    //    if (sunController.index > sunController.checkPointIndex)
    //    {
    //        sunController.MoveLeftToCheckPointPos();
    //        if (sunController.index <= sunController.checkPointIndex)
    //        {
    //            SpawnPlayer();
    //        }
    //    }
    //    else if (sunController.index < sunController.checkPointIndex)
    //    {
    //        sunController.MoveRightToCheckPointPos();
    //        if (sunController.index >= sunController.checkPointIndex)
    //        {
    //            SpawnPlayer();
    //        }
    //    }
    //    else
    //    {
    //        SpawnPlayer();
    //    }
    //}
}
