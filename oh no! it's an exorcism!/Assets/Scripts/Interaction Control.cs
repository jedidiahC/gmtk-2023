using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class InteractionControl : MonoBehaviour
{
    public static List<Interactable> interactables = new List<Interactable>();
    [SerializeField] private float interactableRange = 0.5f;

    private void Update()
    {
        foreach (var i in interactables)
        {
            float distance = Vector3.Distance(i.transform.position, this.transform.position);
            bool canInteract = distance < interactableRange;
            i.ShowInteractMessage(canInteract);
            if (Input.GetKeyDown(KeyCode.E))
            {
                i.Interact();
            }
        }
    }
}
