using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using System;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;
using MoreMountains.Tools;
public class PlayerAbilityController : MonoBehaviour
{

    #region Dash
    [SerializeField]
    private float _dashSpeed = 15f;
    [SerializeField]
    private float _dashTime = .2f;
    [SerializeField]
    private float _dashCooldown = 1.5f;
    [SerializeField]
    private int _maxDashCount = 2;

    public TimerScript DashTimer;

    public bool CanDash { get; private set; }
    public int CurrentDashCount { get; private set; }

    public float MaxDashCooldownTime { get => _dashCooldown; }
    #endregion


    #region Primary Attack
    public int AmmoCount { get => _aim.AmmoCount; }
    public int MaxAmmoCount { get => _aim.MagazineSize; }
    public float AttackCooldown;
    public bool CanAttack;
    #endregion


    #region Primary Special
    // Lambdas are temporary, will need to change when cooldown and use modifiers come into effect.
    [SerializeField]
    private float _primarySpecialCooldown { get => PrimarySpecial.ScriptableObject.CoolDown; }
    [SerializeField]
    private int _maxPrimarySpecialCount { get => PrimarySpecial.ScriptableObject.NumberOfUses; }

    public CharacterAbility PrimarySpecial;

    public TimerScript PrimarySpecialTimer;

    public bool CanPrimarySpecial { get => CurrentPrimarySpecialCount > 0; }
    public int CurrentPrimarySpecialCount { get; private set; }

    public int MaxPrimarySpecialCount { get => _maxPrimarySpecialCount; }

    public float MaxPrimarySpecialCooldownTime { get => _primarySpecialCooldown; }
    #endregion


    #region Secondary Special
    [SerializeField]
    private float _secondarySpecialCooldown { get => SecondarySpecial.ScriptableObject.CoolDown; }
    [SerializeField]
    private int _maxSecondarySpecialCount { get => SecondarySpecial.ScriptableObject.NumberOfUses; }

    public CharacterAbility SecondarySpecial;

    public TimerScript SecondarySpecialTimer;

    public bool CanSecondarySpecial { get => CurrentSecondarySpecialCount > 0; }
    public int CurrentSecondarySpecialCount { get; private set; }

    public int MaxSecondarySpecialCount { get => _maxSecondarySpecialCount; }

    public float MaxSecondarySpecialCooldownTime { get => _secondarySpecialCooldown; }
    #endregion

    public bool HasSlowfallAiming;

    public bool CanJump { get => _moveController.CanJump; }


    #region References
    private ThirdPersonAim _aim;
    private ThirdPersonController _moveController;
    public PlayerStats PlayerStats;
    public PlayerInventory Inventory;
    public StarterAssetsInputs Inputs;
    public LineRenderer LineRenderer;
    public EffectApplier Effect;
    public PlayerAudio PlayerAudio;
    public Transform PlayerTarget;
    public Transform MuzzleEnd => _aim.MuzzleEnd;

    public GameObject ObjectPoolerObject;

    public int PlayerHealth => PlayerStats.PlayerHealth;
    public int PlayerMaxHealth => PlayerStats.PlayerMaxHealth;


    private bool _isRegistered = false;
    #endregion

    public void InterruptReload() => _aim.InterruptReload();

    private void Awake()
    {
        _aim = GetComponent<ThirdPersonAim>();
        _moveController = GetComponent<ThirdPersonController>();
        Inputs = GetComponent<StarterAssetsInputs>();
        LineRenderer = GetComponent<LineRenderer>();
        Effect = GetComponent<EffectApplier>();
        Inventory = GetComponent<PlayerInventory>();
        Inventory.SetPlayer(this);

        PrimarySpecial = GetComponent<PowerShot>();
        PrimarySpecial.InitializeAbility(_moveController, this);
        SecondarySpecial = GetComponent<JumpSlam>();
        SecondarySpecial.InitializeAbility(_moveController, this);
        DashTimer = new TimerScript(_dashCooldown);
        PrimarySpecialTimer = new TimerScript(_primarySpecialCooldown);
        SecondarySpecialTimer = new TimerScript(_secondarySpecialCooldown);
    }

    private void Start()
    {
        _aim.SetWeaponStats(Inventory.Weapon);
        _moveController.SetDashStats(_dashTime, _dashSpeed);
        LineRenderer.enabled = false;
        if (!_isRegistered) _isRegistered = PlayerManager.Instance.RegisterPlayer(this);
        CurrentDashCount = _maxDashCount;
        CurrentPrimarySpecialCount = _maxPrimarySpecialCount;
        CurrentSecondarySpecialCount = _maxSecondarySpecialCount;
    }

    private void Update()
    {
        HandleDashCooldown();
        HandlePrimarySpecialCooldown();
        HandleSecondarySpecialCooldown();

        if ((Inputs.PrimarySpecialHeld || Inputs.PrimarySpecialReleased) && CanPrimarySpecial)
        {
            HandlePrimarySpecialLogic();
        }
        if ((Inputs.SecondarySpecialHeld || Inputs.SecondarySpecialReleased) && CanSecondarySpecial)
        {
            HandleSeconarySpecialLogic();
        }
    }

    private void HandlePrimarySpecialLogic()
    {
        if (PrimarySpecial.Tick(Inputs.PrimarySpecialReleased) == false) return;
        else { Inputs.PrimarySpecialReleased = false; CurrentPrimarySpecialCount--; }
    }

    private void HandleSeconarySpecialLogic()
    {
        if (SecondarySpecial.Tick(Inputs.SecondarySpecialReleased) == false) return;
        else { Inputs.SecondarySpecialReleased = false; CurrentSecondarySpecialCount--; }
    }

    private void HandlePrimarySpecialCooldown()
    {
        if (_primarySpecialCooldown != PrimarySpecialTimer.CountdownTime) PrimarySpecialTimer.CountdownTime = _primarySpecialCooldown;
        if (CurrentPrimarySpecialCount < _maxPrimarySpecialCount)
        {
            if (PrimarySpecialTimer.Tick(Time.deltaTime))
            {
                CurrentPrimarySpecialCount++;
            }
        }
        if (!CanPrimarySpecial)
        {
            Inputs.PrimarySpecialReleased = false;
        }
    }

    private void HandleSecondarySpecialCooldown()
    {
        if (_secondarySpecialCooldown != SecondarySpecialTimer.CountdownTime) SecondarySpecialTimer.CountdownTime = _secondarySpecialCooldown;
        if (CurrentSecondarySpecialCount < _maxSecondarySpecialCount)
        {
            if (SecondarySpecialTimer.Tick(Time.deltaTime))
            {
                CurrentSecondarySpecialCount++;
            }
        }
        if (!CanSecondarySpecial)
        {
            Inputs.SecondarySpecialReleased = false;
        }
    }

    private void HandleDashCooldown()
    {
        if (_dashCooldown != DashTimer.CountdownTime) DashTimer.CountdownTime = _dashCooldown;
        if (CurrentDashCount < _maxDashCount)
        {
            if (DashTimer.Tick(Time.deltaTime))
            {
                CurrentDashCount++;
            }
        }
        CanDash = CurrentDashCount > 0 ? true : false;
    }

    public void SetWeaponStats(Gun newGun)
    {
        _aim.SetWeaponStats(newGun);
    }

    //Called by ThirdPersonController to Check Dash Availability
    public bool TryDashing()
    {
        if (CanDash)
        {
            PlayerAudio.PlayDash();
            EffectManager.Instance.PlayMotionBlur(transform.position);
            CurrentDashCount--;
            InterruptReload();
            return true;
        }
        else return false;
    }
}
