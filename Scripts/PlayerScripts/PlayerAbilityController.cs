using Sirenix.OdinInspector;
using StarterAssets;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAbilityController : MonoBehaviour
{
    #region Dash
    private float _dashSpeed => PlayerStats.PlayerDashSpeed;
    private float _dashTime => PlayerStats.PlayerDashTime;
    private float _dashCooldown => PlayerStats.PlayerDashCooldown;
    private int _maxDashCount => PlayerStats.PlayerMaxDashCount;

    public TimerScript DashTimer;

    public bool CanDash => CurrentDashCount > 0 ? true : false;
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

    public List<BaseUpgradeScriptableObject> AvailableUpgrades;
    public List<BaseUpgradeScriptableObject> HeldUpgrades { get; set; }


    public bool HasSlowfallAiming;
    private float _slowfallAimingTime { set { _slowfallTimer.CountdownTime = value; } }
    private float _slowfallAimingShield;
    private float _slowfallAimingCrit;
    private TimerScript _slowfallTimer;
    private bool _slowfallTimeLeft;

    public bool Grounded { get => _grounded; set { _grounded = value; _slowfallTimer.Reset(); _slowfallTimeLeft = true;} }
    private bool _grounded;
    public bool IsAiming { get => _isAiming; set { _isAiming = value; } }
    private bool _isAiming;

    public bool CanJump { get => _moveController.CanJump; }


    #region References
    [FoldoutGroup("References")]
    private ThirdPersonAim _aim;
    [FoldoutGroup("References")]
    private ThirdPersonController _moveController;
    [FoldoutGroup("References")]
    public PlayerStats PlayerStats;
    [FoldoutGroup("References")]
    public PlayerInventory Inventory;
    [FoldoutGroup("References")]
    public StarterAssetsInputs Inputs;
    [FoldoutGroup("References")]
    public LineRenderer LineRenderer;
    [FoldoutGroup("References")]
    public EffectApplier Effect;
    [FoldoutGroup("References")]
    public PlayerAudio PlayerAudio;
    [FoldoutGroup("References")]
    public Transform PlayerTarget;
    [FoldoutGroup("References")]
    public Transform MuzzleEnd => _aim.MuzzleEnd;
    [FoldoutGroup("References")]

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
        LineRenderer = GetComponentInChildren<LineRenderer>();
        Effect = GetComponent<EffectApplier>();
        Inventory = GetComponent<PlayerInventory>();
        Inventory.SetPlayer(this);

        HeldUpgrades = new List<BaseUpgradeScriptableObject>();

        PrimarySpecial = GetComponent<PowerShot>();
        PrimarySpecial.InitializeAbility(_moveController, this);
        SecondarySpecial = GetComponent<JumpSlam>();
        SecondarySpecial.InitializeAbility(_moveController, this);
        DashTimer = new TimerScript(_dashCooldown);
        PrimarySpecialTimer = new TimerScript(_primarySpecialCooldown);
        SecondarySpecialTimer = new TimerScript(_secondarySpecialCooldown);
        _slowfallTimer = new TimerScript();

        _moveController.IsGrounded += ChangeGroundState;
        _aim.IsAiming += ChangeAimState;
    }

    private void OnDisable()
    {
        _moveController.IsGrounded -= ChangeGroundState;
        _aim.IsAiming -= ChangeAimState;
    }

    private void Start()
    {
        _aim.SetWeaponStats(Inventory.Weapon);
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

        HandleSlowfallUpgrade();

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
    }

    private bool _startedSlowFall;
    private void HandleSlowfallUpgrade()
    {
        if (HasSlowfallAiming)
        {
            if (!Grounded && IsAiming && _slowfallTimeLeft)
            {
                if (!_startedSlowFall)
                {
                    _startedSlowFall = true;
                    _moveController.TurnOffVerticalMovement(true);
                    PlayerStats.PlayerCritChance = _slowfallAimingCrit;
                }
                if (_slowfallTimer.Tick(Time.deltaTime))
                {
                    Debug.Log("no slowfall left");
                    _slowfallTimeLeft = false;
                    _moveController.TurnOffVerticalMovement(false);
                }
            }
            else
            {
                if (_startedSlowFall)
                {
                    _startedSlowFall = false;
                    PlayerStats.PlayerCritChance = -_slowfallAimingCrit;
                }
                _moveController.TurnOffVerticalMovement(false);
            }
        }
    }

    public void SetWeaponStats(Gun newGun)
    {
        _aim.SetWeaponStats(newGun);
    }

    public void ChooseUpgrade(BaseUpgradeScriptableObject upgrade)
    {
        ApplyUpgrade(upgrade);
    }

    public List<BaseUpgradeScriptableObject> GetUpgrades()
    {
        List<BaseUpgradeScriptableObject> upgrades = new List<BaseUpgradeScriptableObject>();
        int i = 0;
        while (upgrades.Count < 3)
        {
            if (AvailableUpgrades.Count > 0) upgrades.Add(AvailableUpgrades[Random.Range(0, AvailableUpgrades.Count)]);
            if (PrimarySpecial.AvailableUpgrades.Count > 0 && upgrades.Count < 3) upgrades.Add(PrimarySpecial.RequestUpgrade());
            if (SecondarySpecial.AvailableUpgrades.Count > 0 && upgrades.Count < 3) upgrades.Add(SecondarySpecial.RequestUpgrade());
            i++;
            if (i == 3) break;
        }

        return upgrades;
    }

    public void ApplyUpgrade(BaseUpgradeScriptableObject upgrade)
    {
        BaseShootBoiUpgradeScriptableObject switchedUpgrade = upgrade as BaseShootBoiUpgradeScriptableObject;
        Debug.Log("applying upgrade " + switchedUpgrade.Name);

        AvailableUpgrades.Remove(switchedUpgrade);
        if (!switchedUpgrade.IsLastUpgrade) AvailableUpgrades.Add(switchedUpgrade.NextLevel);
        HeldUpgrades.Add(switchedUpgrade);

        switch (switchedUpgrade.ID)
        {
            case 0:
                switch (switchedUpgrade.UpgradeLevel)
                {

                    case 1:
                        {
                            HasSlowfallAiming = true;
                            _slowfallAimingTime = switchedUpgrade.Time;
                        }
                        break;
                    case 2:
                        {
                            _slowfallAimingShield = switchedUpgrade.Shield;
                            _slowfallAimingTime = switchedUpgrade.Time;
                        }
                        break;
                    case 3:
                        {
                            _slowfallAimingShield = switchedUpgrade.Shield;
                            _slowfallAimingTime = switchedUpgrade.Time;
                            _slowfallAimingCrit = switchedUpgrade.CritChance;
                        }
                        break;
                }
                break;
        }
    }

    private void ChangeGroundState(bool grounded)
    {
        Grounded = grounded;
    }

    private void ChangeAimState(bool aim)
    {
        IsAiming = aim;
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
