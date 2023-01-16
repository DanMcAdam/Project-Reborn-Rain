using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlayerAudio : MonoBehaviour
{
    private FMOD.Studio.EventInstance _instance;
    
    public EventReference ReloadEvent;
    public EventReference Dash;
    public EventReference Footsteps;

    private List<AudioEventInstanceHolder> _instanceLists = new List<AudioEventInstanceHolder>();

    [ParamRef]
    public string MyParam1;
    private void Start()
    {
        _instance = RuntimeManager.CreateInstance(ReloadEvent);
    }

    public void PlayGunShot(EventReference gunShot)
    {
        RuntimeManager.PlayOneShot(gunShot, transform.position);
    }

    public void PlayReload(int sequence)
    {
        _instance.setParameterByName("ReloadSequence", sequence);
        _instance.start();
    }

    public void PlayDash()
    {
        RuntimeManager.PlayOneShot(Dash, transform.position);
    }

    public void PlayFootsteps()
    {
        RuntimeManager.PlayOneShot(Footsteps, transform.position);
    }

}

public class AudioEventInstanceHolder
{
    public FMOD.Studio.EventInstance AudioEventInstance;
    public EventReference AudioEventReference;
    public bool Discarded;

    public AudioEventInstanceHolder (FMOD.Studio.EventInstance instance, EventReference reference)
    {
        AudioEventInstance = instance;
        AudioEventReference = reference;
        Discarded = false;
    }
}
