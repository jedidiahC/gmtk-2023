using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spook : Interactable
{
    [SerializeField] private List<HunterLogic> huntersWithinRange;
    [SerializeField] private float investigateRadius;
    [SerializeField] private float scaryPoints;
    [SerializeField] private GameObject interactMessage = null;

    private bool isInvestigated = false;

    public bool IsInvestigated { get { return isInvestigated; } }
    public float InvestigateRadius { get { return investigateRadius; } }
    public float ScaryPoints { get { return scaryPoints; } }

    public void Investigate()
    {
        isInvestigated = true;
    }

    [ContextMenu("Activate")]
    public override void Interact()
    {
        base.Interact();

        foreach (var h in huntersWithinRange) 
        {
            h.Spook(this);
        }
    }

    public override void ShowInteractMessage(bool isShown)
    {
        if (interactMessage != null)
        {
            interactMessage.SetActive(isShown);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        HunterLogic h = other.GetComponent<HunterLogic>();

        if (h != null) 
        {
            huntersWithinRange.Add(h);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        HunterLogic h = other.GetComponent<HunterLogic>();

        if (h != null) 
        {
            huntersWithinRange.Remove(h);
        }       
    }

    private void OnDrawGizmos()
    {
        if (huntersWithinRange == null) { return; }

        Gizmos.color = Color.cyan;
        foreach(var h in huntersWithinRange)
        {
            Gizmos.DrawLine(transform.position, h.transform.position);
        }
    }
}
