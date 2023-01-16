using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public float ReturnInteractionDistance();
    public void ShowInteractionPrompt();
    public void PlayerStoppedLooking();
    public void Interact();

    public bool ThisIsInteractable();
}
