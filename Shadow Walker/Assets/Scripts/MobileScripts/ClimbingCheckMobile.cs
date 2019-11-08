using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingCheckMobile : MonoBehaviour
{
    float endOfLadderDetection;
    public bool startChecking;
    GameObject player;
    PlayerUpdatedMobile playerScript;
    GameObject ladderTop;

    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerUpdatedMobile>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.y > this.transform.position.y && this.gameObject.name == "LadderTop" && startChecking)
        {
            startChecking = false;
            playerScript.onLadder = false;
            playerScript.moveOffLadder = true;
            //Debug.Log("Finished Climbing");
        }
        if (startChecking)
        {
            Debug.Log("Started checking");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && this.gameObject.name == "LadderTop" && playerScript.onLadder)
        {
            startChecking = true;
        }
        else
        {
            startChecking = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && this.gameObject.name == "LadderBottom")
        {
            Debug.Log("ExitLadder");
            playerScript.onLadder = false;
            playerScript.moveOffLadder = true;
            startChecking = false;
        }
    }
}
