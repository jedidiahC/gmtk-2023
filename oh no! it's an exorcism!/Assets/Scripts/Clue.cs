using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Clue : MonoBehaviour
{
    [SerializeField] private float timeToExamineInSec = 50.0f;
    [SerializeField] private bool isExamined = false;
    [SerializeField] private bool isRitualClue = false;

    public string Id { get { return gameObject.name; } }
    public float TimeToExamineInSec { get { return timeToExamineInSec; } }
    public bool IsRitualClue { get { return isRitualClue; } }

    public bool Examine(float timeSpentExamining)
    {
        timeToExamineInSec -= timeSpentExamining;

        if (timeToExamineInSec <= 0)
        {
            isExamined = true;
        }

        return isExamined;
    }
}
