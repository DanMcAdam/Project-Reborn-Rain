using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager Instance;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    public void PlayAudioInstance (EventReference reference, string paramName, Transform transform, int seq)
    {
        EventInstance instance = RuntimeManager.CreateInstance(reference);
        RuntimeManager.AttachInstanceToGameObject(instance, transform);
        instance.setParameterByName(paramName, seq);
        instance.start();
    }

}
