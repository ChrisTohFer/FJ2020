using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Sound/SFXEvent")]
public class SFXEvent : ScriptableObject
{

    // The effect itself
    public AudioClip sfx;

    // If there are multiple audio input put a specific one in here. There should be an SFX channel
    public AudioMixerGroup audioOutput;

    // If it's a looping sound effect. If this is true the sound should be stopped at some point by calling this.Stop(). 
    public bool looping;

    //Pitch variation
    public float minPitch = 1f;
    public float maxPitch = 1f;

    //Determines the volume of the effect. Needs to be between 0 and 1
    public float volume = 1f;

    AudioSource source;

    DestroyAudioObjectOnComplete scriptDestroy;
    public void Play()
    {
        GameObject audioObject = new GameObject();
        source = audioObject.AddComponent<AudioSource>();
        
        //Assign and play the sfx
        source.clip = sfx;
        source.loop = looping;
        source.Play();
        source.pitch = Random.Range(minPitch, maxPitch);
        source.volume = volume;

        // attach a component to the Gameobject that kills the gameobject if the sound is done playing
        scriptDestroy = audioObject.AddComponent<DestroyAudioObjectOnComplete>();

        //Prevent sounds from being cut off if we change scenes while they are playing
        DontDestroyOnLoad(audioObject);
    }

    // If this is a looping sound this will stop it
    public void Stop()
    {
        // check if the object started playing in the first place
        if (scriptDestroy == null)
        {
            return;
        }
        
        //Just stops playing the audio. The destroyaudioscript will take care of the rest.
        source.Stop();
    }
}
