using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ritual : MonoBehaviour
{
    public UnityEvent onAllRequiredCluesFound;

    [SerializeField] List<Clue> allClues;
    [SerializeField] List<Clue> ritualClues;
    [SerializeField] List<Clue> remainingRitualClues;
    [SerializeField] List<Clue> cluesExamined;

    public int NumClues { get { return allClues.Count; } }
    public int NumRitualClues { get { return ritualClues.Count; } }
    public int NumCluesExamined { get { return cluesExamined.Count; } }
    public int NumCluesRemaining { get { return NumClues - NumCluesExamined; } }
    public int NumRitualCluesRemaining { get { return remainingRitualClues.Count; } }

    private void Awake() 
    {
        allClues.AddRange(GetComponentsInChildren<Clue>());
        ritualClues.AddRange(allClues.FindAll(c => c.IsRitualClue));
    }

    private void Start() 
    {
        remainingRitualClues.AddRange(ritualClues);
    }

    public void SubmitClue(HunterLogic hunter, Clue clue) 
    {
        cluesExamined.Add(clue);

        remainingRitualClues.Remove(clue);

        if (remainingRitualClues.Count == 0)
        {
            Debug.Log("All ritual clues found!");
            onAllRequiredCluesFound.Invoke();
        }

        LogCluesFound();
    }

    private void LogCluesFound()
    {
        string clueStr = "";
        foreach (var c in cluesExamined) {
            clueStr += $" | {c.Id} | ";
        }

        Debug.Log($"Clues found so far: {clueStr}");
    }
}
