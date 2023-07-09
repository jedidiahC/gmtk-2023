using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Clue : MonoBehaviour
{
    [SerializeField] private string id = "Clue";
    [SerializeField] private float timeToExamineInSec = 50.0f;
    [SerializeField] private bool isExamined = false;

    public string Id { get { return id; } }
    public float TimeToExamineInSec { get { return timeToExamineInSec; } }

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
