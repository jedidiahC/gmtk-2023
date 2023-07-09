using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ritual : MonoBehaviour
{

    [SerializeField] List<Clue> requiredClues = null;

    private void Awake() 
    {
        Debug.Assert(requiredClues != null && requiredClues.Count > 0, "requiredClues are not assigned!");     
    }

    private void OnClueInvestigated(HunterLogic hunter, Clue clue) 
    {
        
    }
}
