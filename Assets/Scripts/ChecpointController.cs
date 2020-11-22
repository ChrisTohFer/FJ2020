using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChecpointController : MonoBehaviour
{
    public Sprite redFlag;
    public Sprite greenFlag;
    private SpriteRenderer checkpointSpriteRenderer;
    public bool checkpointReached;
    public Vector3 currentrespawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        checkpointSpriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag =="Player")
        {
            checkpointSpriteRenderer.sprite = greenFlag;
            checkpointReached = true;
            currentrespawnPoint = other.transform.position;
        }
    }
}
