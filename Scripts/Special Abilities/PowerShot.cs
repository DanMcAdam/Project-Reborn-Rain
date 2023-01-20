using FMOD.Studio;
using FMODUnity;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;


public class PowerShot : CharacterAbility
{
    public override Vector3 PlayerPosition { get => _moveController.transform.position; }

    public override SOCharacterAbility ScriptableObject { get; set; }

    public override List<BaseUpgradeScriptableObject> HeldUpgrades { get; set; }

    public EventInstance _chargeSoundInstance;

    private PlayerAttack _playerAttack;

    private int _baseDamage;
    public int Damage => _baseDamage + Mathf.CeilToInt(_baseDamage * DamageModifier);
    public float DamageModifier => _fastChargeDamage;

    public float _chargeSpeedModifier => _fastChargeSpeed;
    #region Fast Charge
    private float _fastChargeSpeed;
    private float _fastChargeDamage;
    #endregion


    public float TimeToFullCharge => _baseTimeToFullCharge - (_baseTimeToFullCharge * _chargeSpeedModifier);
    private float _baseTimeToFullCharge = 3f;
    private float _chargePower = 0f;
    bool _fullyCharged;

    ParticleSystem _gunCharge;
    ParticleSystem _backCharge;
    ParticleSystem _beam;

    TimerScript _timer;
    public override void InitializeAbility(ThirdPersonController thirdPersonController, PlayerAbilityController playerAbilityController)
    {
        HeldUpgrades = new List<BaseUpgradeScriptableObject>();
        base.InitializeAbility(thirdPersonController, playerAbilityController);
        _playerAttack = new PlayerAttack(ScriptableObject.Damage, ScriptableObject.Force, UnityEngine.Random.Range(0, 10000), true);
        _timer = new TimerScript(TimeToFullCharge);
        _chargeSoundInstance = RuntimeManager.CreateInstance(ScriptableObject.AudioRef[0]);
        RuntimeManager.AttachInstanceToGameObject(_chargeSoundInstance, _moveController.transform);
        _gunCharge = Object.Instantiate(ScriptableObject.ParticleSystems[1], _abilitycontroller.MuzzleEnd);
        _backCharge = Object.Instantiate(ScriptableObject.ParticleSystems[3], _abilitycontroller.MuzzleEnd);
        _beam = Object.Instantiate(ScriptableObject.ParticleSystems[2], _abilitycontroller.MuzzleEnd);
        _baseDamage = ScriptableObject.Damage;
    }

    public override bool Tick(bool released)
    {
        _abilitycontroller.InterruptReload();
        _gunCharge.Play();
        _backCharge.Play();
        if (_chargePower < .01f)
        {
            _chargeSoundInstance.start();
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
        _chargeSoundInstance.setParameterByName("ChargeShotLevel", _chargePower);

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
            _playerAttack.Damage = Mathf.CeilToInt(_chargePower * Damage);
            _playerAttack.ID = UnityEngine.Random.Range(0, 10000);
            _chargeSoundInstance.setParameterByName("ChargeShotLevel", _chargePower);
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
        BaseUpgradeScriptableObject upgrade = AvailableUpgrades[Random.Range(0, AvailableUpgrades.Count)];
        Debug.Log(upgrade.Name);
        upgrade.ability = this;
        return upgrade;
    }

    public override void ApplyUpgrade(BaseUpgradeScriptableObject upgrade)
    {
        BasePowerShotScriptableObject switchedUpgrade = upgrade as BasePowerShotScriptableObject;
        Debug.Log("applying upgrade " + switchedUpgrade.Name);

        AvailableUpgrades.Remove(switchedUpgrade);
        if (!switchedUpgrade.IsLastUpgrade) AvailableUpgrades.Add(switchedUpgrade.NextLevel);
        HeldUpgrades.Add(switchedUpgrade);

        switch (switchedUpgrade.ID)
        {
            case 0:
                {
                    switch (switchedUpgrade.UpgradeLevel)
                    {
                        case 1:
                            _fastChargeDamage = switchedUpgrade.DamageMultiplier;
                            _fastChargeSpeed = switchedUpgrade.ChargeSpeed;
                            break;
                        case 2:
                            _fastChargeSpeed = switchedUpgrade.ChargeSpeed;
                            _fastChargeDamage = switchedUpgrade.DamageMultiplier;
                            break;
                        case 3:
                            _fastChargeDamage = switchedUpgrade.DamageMultiplier;
                            _fastChargeSpeed = switchedUpgrade.ChargeSpeed;
                            break;
                    }
                }
                break;
        }
    }
}
