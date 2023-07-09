using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spook : MonoBehaviour
{
    [SerializeField] private List<HunterLogic> huntersWithinRange;
    [SerializeField] private float investigateRadius;
    [SerializeField] private float scaryPoints;

    private bool isInvestigated = false;

    public bool IsInvestigated { get { return isInvestigated; } }
    public float InvestigateRadius { get { return investigateRadius; } }
    public float ScaryPoints { get { return scaryPoints; } }

    public void Investigate()
    {
        isInvestigated = true;
    }

    [ContextMenu("Activate")]
    public void Activate()
    {
        foreach (var h in huntersWithinRange) 
        {
            h.Spook(this);
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
