using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingCheck : MonoBehaviour
{
    float endOfLadderDetection;
    public bool startChecking;
    GameObject player;
    PlayerUpdated playerScript;
    GameObject ladderTop;

    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerUpdated>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.transform.position.y > this.transform.position.y && this.gameObject.name == "LadderTop" && startChecking)
        {
            startChecking = false;
            playerScript.onLadder = false;
            playerScript.moveOffLadder = true;
            Debug.Log("Finished Climbing");
        }
        if (startChecking)
        {
            Debug.Log("Started checking");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
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
