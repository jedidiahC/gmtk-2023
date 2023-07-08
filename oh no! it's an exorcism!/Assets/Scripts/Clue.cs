using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clue : MonoBehaviour
{
    [SerializeField] private string id = "Clue";
    [SerializeField] private float investigateTimeInSec = 50.0f;
    [SerializeField] private List<Clue> clues = null;

    public float InvestigateTime { get { return investigateTimeInSec; } }
}
