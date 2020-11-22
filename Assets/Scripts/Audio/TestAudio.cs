using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudio : MonoBehaviour
{
    public SFXEvent testSFX;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            testSFX.Play();
        }
        if (Input.GetKeyDown("s"))
        {
            testSFX.Stop();
        }

    }
}
