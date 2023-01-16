using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;
using MoreMountains.Tools;
using MoreMountains.Feel;
public class EffectManager : MonoBehaviour
{
    [SerializeField] 
    MMF_Player _mMFPlayer;
    MMF_MotionBlur_URP _motionBlur;
    MMF_CinemachineImpulse _cameraShakeShoot;
    MMF_CinemachineImpulse _cameraShakeGroundSlam;
    MMF_FloatingText _floatingText;
    MMF_ParticlesInstantiation _particlesInstantiation;
    public static EffectManager Instance;
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

        _cameraShakeShoot = _mMFPlayer.GetFeedbackOfType<MMF_CinemachineImpulse>("CameraShakeShoot");
        _cameraShakeGroundSlam = _mMFPlayer.GetFeedbackOfType<MMF_CinemachineImpulse>("CameraShakeGroundSlam");
        _motionBlur = _mMFPlayer.GetFeedbackOfType<MMF_MotionBlur_URP>();
        _floatingText = _mMFPlayer.GetFeedbackOfType<MMF_FloatingText>();
        _particlesInstantiation = _mMFPlayer.GetFeedbackOfType<MMF_ParticlesInstantiation>();
    }

    public void PlayCameraShakeShoot(Vector3 playPosition, float velocity)
    {
        _cameraShakeShoot.Velocity = new Vector3(velocity, velocity, velocity);
        _cameraShakeShoot.Play(playPosition);
    }

    public void PlayCameraShakeGroundSlam(Vector3 playPosition)
    {
        _cameraShakeGroundSlam.Play(playPosition);

    }

    public void PlayMotionBlur(Vector3 playPosition)
    {
        _motionBlur.Play(playPosition);
    }
    public void PlayMotionBlur(Vector3  playPosition, float intensity)
    {
        _motionBlur.Play(playPosition, intensity);
    }

    public void GenerateFloatingText(Vector3 playPosition, float value, Transform transform)
    {
        _floatingText.TargetTransform = transform;
        _floatingText.Play(playPosition, value);
    }

    public void SpawnHitEffect(Vector3 playPosition)
    {
        Debug.Log(playPosition);
        _particlesInstantiation.TargetWorldPosition = playPosition;
        _particlesInstantiation.Play(playPosition);
    }

}
