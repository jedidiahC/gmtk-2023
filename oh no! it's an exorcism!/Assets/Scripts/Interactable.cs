using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public virtual void Interact() { }
    public virtual void ShowInteractMessage(bool isShown) { }

    private void Start() 
    {
        InteractionControl.interactables.Add(this);
    }
}
