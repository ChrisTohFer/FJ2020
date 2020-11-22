using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Vector3 respawnPoint;
    public ChecpointController checkpointRef;
    public GameObject playerRef;
    // Start is called before the first frame update
    void Start()
    {
        checkpointRef = GameObject.FindObjectOfType<ChecpointController>();
        playerRef = GameObject.FindGameObjectWithTag("Player");                       
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            // if the player enters respawn at last checkpoint
         playerRef.transform.parent.position = checkpointRef.currentrespawnPoint;

        }
    }

}
