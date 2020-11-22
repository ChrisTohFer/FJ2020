using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Types
    [System.Serializable]
    public struct NamedSoundEffect
    {
        public string name;
        public SFXEvent soundEffect;
    }

    //Static
    static AudioManager singleton;

    //Public
    public SFXEvent music;
    public NamedSoundEffect[] soundEffects;

    //Private
    Dictionary<string, SFXEvent> m_soundEffects;

    private void Awake()
    {
        if(singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);

            foreach(NamedSoundEffect nse in soundEffects)
            {
                m_soundEffects.Add(nse.name, nse.soundEffect);
            }
        }
        else
        {
            //We already have an audio manager
            Destroy(this);
        }
    }

    private void Start()
    {
        if (singleton == this)
            music.Play();
    }

    public static void PlaySoundEffect(string name)
    {
        singleton.m_soundEffects[name].Play();
    }
    public static void StopSoundEffect(string name)
    {
        singleton.m_soundEffects[name].Stop();
    }

}
