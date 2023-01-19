using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;


public class PowerShot : CharacterAbility
{
    public override Vector3 PlayerPosition { get => _moveController.transform.position; }

    public override SOCharacterAbility ScriptableObject { get; set; }

    public override List<BaseUpgradeScriptableObject> HeldUpgrades { get; set; }

    public EventInstance _instance;

    private PlayerAttack _playerAttack;


    public float TimeToFullCharge = 3f;

    private float _chargePower = 0f;
    bool _fullyCharged;

    ParticleSystem _gunCharge;
    ParticleSystem _backCharge;
    ParticleSystem _beam;

    TimerScript _timer;
    public override void InitializeAbility(ThirdPersonController thirdPersonController, PlayerAbilityController playerAbilityController)
    {
        base.InitializeAbility(thirdPersonController, playerAbilityController);
        _playerAttack = new PlayerAttack(ScriptableObject.Damage, ScriptableObject.Force, UnityEngine.Random.Range(0, 10000), true);
        _timer = new TimerScript(TimeToFullCharge);
        _instance = RuntimeManager.CreateInstance(ScriptableObject.AudioRef[0]);
        RuntimeManager.AttachInstanceToGameObject(_instance, _moveController.transform);
        _gunCharge = Object.Instantiate(ScriptableObject.ParticleSystems[1], _abilitycontroller.MuzzleEnd);
        _backCharge = Object.Instantiate(ScriptableObject.ParticleSystems[3], _abilitycontroller.MuzzleEnd);
        _beam = Object.Instantiate(ScriptableObject.ParticleSystems[2], _abilitycontroller.MuzzleEnd);

    }

    public override bool Tick(bool released)
    {
        _abilitycontroller.InterruptReload();
        _gunCharge.Play();
        _backCharge.Play();
        if (_chargePower < .01f)
        {
            _instance.start();
        }
        _timer.CountdownTime = TimeToFullCharge;

        if (!_fullyCharged)
        {
            if (_timer.Tick(Time.deltaTime))
            {
                _timer.CurrentTime = .001f;
                _fullyCharged = true;
            }
        }
        _chargePower = Mathf.InverseLerp(_timer.CountdownTime, 0, _timer.CurrentTime);
        _instance.setParameterByName("ChargeShotLevel", _chargePower);

        if (released)
        {
            DecideBeamDisplay();
            _backCharge.Emit(20);
            _gunCharge.Emit(20);
            _backCharge.Stop();
            _gunCharge.Stop();
            _beam.Play();
            _chargePower = _fullyCharged ? 2f : _chargePower + 1;
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            RaycastHit[] raycastHit = Physics.RaycastAll(ray, 999f, Physics.AllLayers);
            EffectManager.Instance.PlayCameraShakeShoot(PlayerPosition, _chargePower + 1);
            _playerAttack.Damage = Mathf.CeilToInt(_chargePower * ScriptableObject.Damage);
            _playerAttack.ID = UnityEngine.Random.Range(0, 10000);
            _instance.setParameterByName("ChargeShotLevel", _chargePower);
            foreach (RaycastHit hit in raycastHit)
            {
                if (hit.transform != null)
                {
                    if (hit.transform.TryGetComponent(out IDamageable damageable))
                    {
                        _playerAttack.HitPosition = hit.point;
                        _abilitycontroller.Effect.ApplyAttackEffects(_playerAttack, damageable);
                    }
                    Object.Instantiate(ScriptableObject.ParticleSystems[0], hit.point, Quaternion.FromToRotation(hit.point, hit.normal));
                }
            }
            Reset();

            return true;
        }
        else return false;
    }

    private void DecideBeamDisplay()
    {
        Gradient gradient = new Gradient();
        var trails = _beam.trails;
        if (_chargePower < .4)
        {
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
        }
        else if (_chargePower < .8f)
        {
            gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.yellow, 0f), new GradientColorKey(Color.yellow, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
        }
        else
        {
            gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.red, 0f), new GradientColorKey(Color.red, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
        }
        trails.colorOverLifetime = gradient;
        _beam.startSize = .5f + _chargePower;

    }

    private void Reset()
    {
        _chargePower = 0;
        _fullyCharged = false;
        _timer.Reset();
    }

    public override BaseUpgradeScriptableObject RequestUpgrade()
    {
        throw new System.NotImplementedException();
    }

    public override void ApplyUpgrade(BaseUpgradeScriptableObject upgrade)
    {
        throw new System.NotImplementedException();
    }
}
