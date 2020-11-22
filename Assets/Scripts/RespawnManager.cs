using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public ChecpointController checkpointRef;
    public Rigidbody2D playerRb; 
    public Vector3 currentRespawnPoint;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            checkpointRef = GameObject.FindObjectOfType<ChecpointController>();
            currentRespawnPoint = ChecpointController.lastCheckPointReached;
            // if the player enters respawn at last checkpoint
            playerRb.velocity = Vector3.zero;
            Movement.playerTransform.position = currentRespawnPoint;

            
            

        }
    }

}
