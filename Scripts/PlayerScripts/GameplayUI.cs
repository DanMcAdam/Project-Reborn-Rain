using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayUI : MonoBehaviour
{
    #region Dash UI
    [SerializeField]
    private TextMeshProUGUI _dashCountUI;
    [SerializeField]
    private Slider _dashCooldownSlider;
    [SerializeField]
    private GameObject _dashAvailablePanel;
    #endregion

    #region Ammo UI
    [SerializeField]
    private TextMeshProUGUI _ammoCountUI;
    [SerializeField]
    private TextMeshProUGUI _maxAmmoCountUI;
    #endregion

    [SerializeField]
    private GameObject _jumpImage;

    #region Primary Special UI
    [SerializeField]
    private TextMeshProUGUI _primarySpecialCountUI;
    [SerializeField]
    private Slider _primarySpecialCooldownSlider;
    [SerializeField]
    private Image _primarySpecialAvailableImage;
    #endregion

    #region Secondary Special UI
    [SerializeField]
    private TextMeshProUGUI _secondarySpecialCountUI;
    [SerializeField]
    private Slider _secondarySpecialCooldownSlider;
    [SerializeField]
    private Image _secondarySpecialAvailableImage; 
    #endregion

    PlayerManager _playerManager => PlayerManager.Instance;
    PlayerAbilityController _abilityController => _playerManager.Player;

    #region Dash
    private int _dashCount => _abilityController.CurrentDashCount;
    private float _dashCooldownTimer => _abilityController.DashTimer.CurrentTime;
    private float _maxDashCooldown => _abilityController.DashTimer.CountdownTime;
    private bool _canDash => _abilityController.CanDash;
    #endregion

    #region Primary Special
    private int _primarySpecialCount => _abilityController.CurrentPrimarySpecialCount;
    private float _primarySpecialCooldownTimer => _abilityController.PrimarySpecialTimer.CurrentTime;
    private float _maxPrimarySpecialCooldown => _abilityController.PrimarySpecialTimer.CountdownTime;
    private bool _canPrimarySpecial => _abilityController.CanPrimarySpecial;

    private bool resetPrimaryIcon = false;
    #endregion

    #region Secondary Special
    private int _secondarySpecialCount => _abilityController.CurrentSecondarySpecialCount;
    private float _secondarySpecialCooldownTimer => _abilityController.SecondarySpecialTimer.CurrentTime;
    private float _maxSecondarySpecialCooldown => _abilityController.SecondarySpecialTimer.CountdownTime;
    private bool _canSecondarySpecial => _abilityController.CanSecondarySpecial;

    private bool resetSecondaryIcon = false;
    #endregion

    #region Primary Attack
    private int _ammoCount => _abilityController.AmmoCount;
    private int _maxAmmoCount => _abilityController.MaxAmmoCount;
    private bool _canAttack => _abilityController.CanAttack;
    #endregion

    private bool _initSetup = true;

    [SerializeField]
    private Slider _healthSlider;
    [SerializeField]
    private Slider _barrierSlider;
    [SerializeField]
    private Slider _shieldSlider;

    private int _playerHealth => _playerManager.Player.PlayerHealth;
    private int _playerMaxHealth => _playerManager.Player.PlayerMaxHealth;

    private int _playerBarrier => _playerManager.Player.PlayerStats.PlayerBarrier;
    private int _playerShield => _playerManager.Player.PlayerStats.PlayerShield;
    private int _healthSliderValue => Mathf.CeilToInt(Mathf.InverseLerp(0, (float)_playerMaxHealth, (float)_playerHealth) * 1000) ;

    //Player shield and barrier may break if shield/barrier value > max health
    private int _shieldSliderValue => Mathf.CeilToInt(Mathf.InverseLerp(0, (float)_playerMaxHealth, (float)_playerShield) * 1000);
    private int _barrierSliderValue => Mathf.CeilToInt(Mathf.InverseLerp(0, (float)_playerMaxHealth, (float)_playerBarrier) * 1000);

    private void Start()
    {

    }
    private void Update()
    {
        if (_initSetup)
        {
            _dashCooldownSlider.maxValue = _maxDashCooldown;

            _primarySpecialCooldownSlider.maxValue = _maxPrimarySpecialCooldown;

            _secondarySpecialCooldownSlider.maxValue = _maxSecondarySpecialCooldown; 


            _initSetup = false;
        }
        _healthSlider.value = _healthSliderValue;

        _jumpImage.SetActive(_abilityController.CanJump);

        if (!resetPrimaryIcon)
        {
            resetPrimaryIcon = true;
            _primarySpecialAvailableImage.sprite = _abilityController.PrimarySpecial.ScriptableObject.Icon;
        }
        if (!resetSecondaryIcon)
        {
            resetSecondaryIcon = true;
            _secondarySpecialAvailableImage.sprite = _abilityController.SecondarySpecial.ScriptableObject.Icon;
        }

        HandleDashUI();
        HandlePrimarySpecialUI();
        HandleSecondarySpecialUI();
        HandleHealthUI();
        _ammoCountUI.text = UIManager.Instance.ReturnString(_ammoCount);
        _maxAmmoCountUI.text = UIManager.Instance.ReturnString(_maxAmmoCount);

    }

    private void HandleDashUI()
    {
        if (_dashCooldownTimer == _maxDashCooldown)
        {
            _dashCooldownSlider.gameObject.SetActive(false);
        }
        else
        {
            _dashCooldownSlider.gameObject.SetActive(true);
            _dashCooldownSlider.value = _dashCooldownTimer;
        }
        if (_dashCount <= 1)
        {
            _dashCountUI.gameObject.SetActive(false);
        }
        else
        {
            _dashCountUI.gameObject.SetActive(true);
            _dashCountUI.text = UIManager.Instance.ReturnString(_dashCount);
        }

        if (!_canDash)
        {
            _dashAvailablePanel.SetActive(false);
        }
        else _dashAvailablePanel.SetActive(true);
    }

    private void HandlePrimarySpecialUI()
    {
        if (_primarySpecialCooldownTimer == _maxPrimarySpecialCooldown)
        {
            _primarySpecialCooldownSlider.gameObject.SetActive(false);
        }
        else
        {
            _primarySpecialCooldownSlider.gameObject.SetActive(true);
            _primarySpecialCooldownSlider.value = _primarySpecialCooldownTimer;
        }

        if (_primarySpecialCount <= 1)
        {
            _primarySpecialCountUI.gameObject.SetActive(false);
        }
        else
        {
            _primarySpecialCountUI.gameObject.SetActive(true);
            _primarySpecialCountUI.text = UIManager.Instance.ReturnString(_primarySpecialCount);
        }
        if (!_canPrimarySpecial)
        {
            _primarySpecialAvailableImage.gameObject.SetActive(false);
        }
        else _primarySpecialAvailableImage.gameObject.SetActive(true);
    }

    private void HandleSecondarySpecialUI()
    {
        if (_secondarySpecialCooldownTimer == _maxSecondarySpecialCooldown)
        {
            _secondarySpecialCooldownSlider.gameObject.SetActive(false);
        }
        else
        {
            _secondarySpecialCooldownSlider.gameObject.SetActive(true);
            _secondarySpecialCooldownSlider.value = _secondarySpecialCooldownTimer;
        }

        if (_secondarySpecialCount <= 1)
        {
            _secondarySpecialCountUI.gameObject.SetActive(false);
        }
        else
        {
            _secondarySpecialCountUI.gameObject.SetActive(true);
            _secondarySpecialCountUI.text = UIManager.Instance.ReturnString(_secondarySpecialCount);
        }

        if (!_canSecondarySpecial)
        {
            _secondarySpecialAvailableImage.gameObject.SetActive(false);
        }
        else _secondarySpecialAvailableImage.gameObject.SetActive(true);
    }

    private void HandleHealthUI()
    {
        _healthSlider.value = _healthSliderValue;
        _shieldSlider.value = _shieldSliderValue;
        _barrierSlider.value = _barrierSliderValue;
    }


}
