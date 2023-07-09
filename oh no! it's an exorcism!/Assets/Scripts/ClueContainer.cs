using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClueContainer : MonoBehaviour
{
    public UnityEvent<ClueContainer> onInvestigate;

    [SerializeField] private List<Clue> clues;

    [SerializeField] private float investigateRadius = 1.0f;
    [SerializeField] private bool isInvestigated = false;

    public float InvestigateRadius { get { return investigateRadius; } }

    public Clue TakeClue()
    {
        if (isInvestigated)
        {
            return null;
        }

        if (clues.Count == 0) 
        {
            isInvestigated = true;
            onInvestigate.Invoke(this);
            return null;
        }

        Clue c = clues[Random.Range(0, clues.Count)];
        clues.Remove(c);

        return c;
    }

    private void OnDrawGizmos() 
    {
        Color color = isInvestigated ? Color.green : Color.yellow;
        color.a = 0.4f;
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, investigateRadius);
    }
}
