using ModelShark;
using System.Collections.Generic;
using UnityEngine;

public class LootPoint : MonoBehaviour, IInteractable
{
    public float InteractionDistance = 4f;

    public List<Gun> GunList = new List<Gun>();
    public BaseItem HeldItem;
    public bool Upgrades;
    public bool Item;

    private InteractionUI _uI { get => UIManager.Instance.InteractionUI; }
    private PlayerAbilityController _player { get => PlayerManager.Instance.Player; }
    public ParticleSystem ParticleSystem;

    private bool _playerIsWithinRange;
    private bool _playerIsStillLooking;
    private bool _shutDownInteraction;
    private bool _HasPlayedOnce;
    private bool _showPrompt { get => _playerIsWithinRange && _playerIsStillLooking; }
    public bool IsInteractable = true;
    public bool DestroyOnDisable = false;

    public TooltipTrigger TooltipTrigger;

    void Start()
    {
        IsInteractable = true;
        _playerIsStillLooking = false;
    }

    private void Awake()
    {
        if (TooltipTrigger != null) TooltipTrigger.SetText("BodyText", HeldItem.Description);
    }

    void Update()
    {
        if (IsInteractable)
        {
            if (_showPrompt)
            {
                if (!_HasPlayedOnce)
                {
                    _shutDownInteraction = false;
                    if (ParticleSystem != null)
                    { ParticleSystem.Play(); }

                    _uI.ShowInteractionPrompt();
                    if (Vector3.Distance(_player.transform.position, transform.position) > InteractionDistance)
                    {
                        UpdateInteractionBools(false);
                    }
                    if (TooltipTrigger != null) TooltipTrigger.StartHover();
                    _HasPlayedOnce = true;
                }
            }
            else if (!_shutDownInteraction)
            {
                if (TooltipTrigger != null) TooltipTrigger.StopHover();
                _shutDownInteraction = true;
                _uI.HideInteractionPrompt();
                _uI.EndInteraction();
                if (ParticleSystem != null)
                { ParticleSystem.Stop(); }
                _HasPlayedOnce = false;
            }
        }
        else if (ParticleSystem != null)
        { ParticleSystem.Stop(); }
    }


    public void Interact()
    {
        if (IsInteractable)
        {
            if (Item)
            {
                _player.Inventory.PickupItem(HeldItem);
                DisableThisLootPoint();
            }
            else if (Upgrades == false)
                _uI.StartInteraction(this, GunList);
            else _uI.StartInteraction(this, PlayerManager.Instance.Player.GetUpgrades());
        }
    }

    public float ReturnInteractionDistance()
    {
        return InteractionDistance;
    }

    public void PlayerStoppedLooking()
    {
        _playerIsStillLooking = false;
    }

    public void ShowInteractionPrompt()
    {
        UpdateInteractionBools(true);
    }

    private void UpdateInteractionBools(bool showInteraction)
    {
        _playerIsWithinRange = showInteraction;
        _playerIsStillLooking = showInteraction;
    }

    public void DisableThisLootPoint()
    {
        IsInteractable = false;
        if (TooltipTrigger != null) TooltipTrigger.StopHover();
        _uI.EndInteraction();
        if (DestroyOnDisable)
        {
            Destroy(this.gameObject);
        }
    }

    public bool ThisIsInteractable()
    {
        return IsInteractable;
    }
}
