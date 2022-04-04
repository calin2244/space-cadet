using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    private bool shiftIsHeld;

    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            shiftIsHeld = true;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            shiftIsHeld = false;
        }    

        AudioSlowMotion();
    }

    private void Start()
    {
        Play("Theme");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s==null)
            return;
        s.source.Play();
    }

    private void AudioSlowMotion()
    {
        GameObject cameraMain = GameObject.Find("Main Camera");
        
        if(shiftIsHeld)
        {
            cameraMain.GetComponent<BlitCRT>().enabled=true;
            foreach(Sound s in sounds)
            {
                s.source.pitch = Time.timeScale;
            }
        }
        else
        {
            cameraMain.GetComponent<BlitCRT>().enabled=false;
            foreach (Sound s in sounds)
            {
                s.source.pitch = Time.timeScale;
            }
        }
    }

}
