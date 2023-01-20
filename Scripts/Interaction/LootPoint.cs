using UnityEngine;
using System;
using System.Collections.Generic;

public class LootPoint : MonoBehaviour, IInteractable
{
    public float InteractionDistance = 4f;

    public List<Gun> GunList = new List<Gun>();

    public bool Upgrades;

    private InteractionUI _uI { get => UIManager.Instance.InteractionUI; }
    private PlayerAbilityController _player { get => PlayerManager.Instance.Player; }
    public ParticleSystem ParticleSystem;

    private bool _playerIsWithinRange;
    private bool _playerIsStillLooking;
    private bool _shutDownInteraction;
    private bool _showPrompt { get => _playerIsWithinRange && _playerIsStillLooking; }
    public bool IsInteractable = true;
    public bool DestroyOnDisable = false;
    // Start is called before the first frame update
    void Start()
    {
        IsInteractable = true;
        _playerIsStillLooking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInteractable)
        {
            if (_showPrompt)
            {
                _shutDownInteraction = false;
                if (ParticleSystem != null)
                { ParticleSystem.Play(); }

                _uI.ShowInteractionPrompt();
                if (Vector3.Distance(_player.transform.position, transform.position) > InteractionDistance)
                {
                    UpdateInteractionBools(false);
                }
            }
            else if(!_shutDownInteraction)
            {
                _shutDownInteraction = true;
                _uI.HideInteractionPrompt();
                _uI.EndInteraction();
                if (ParticleSystem != null)
                { ParticleSystem.Stop(); }
            }
        }
        else if (ParticleSystem != null)
        { ParticleSystem.Stop(); }
    }


    public void Interact()
    {
        if (IsInteractable)
        { 
            if (Upgrades == false)
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
        //IsInteractable = false;
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
