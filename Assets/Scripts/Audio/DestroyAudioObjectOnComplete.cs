using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAudioObjectOnComplete : MonoBehaviour
{
    // This object can be instantiated on an object with an AudioSource to destroy itself once the audio is finished playing.
    
    AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        //Get the audiosource that's playing the SFC 
        source = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!source.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}
