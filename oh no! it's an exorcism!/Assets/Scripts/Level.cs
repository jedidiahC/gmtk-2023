using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private Room startingRoom = null;

    public Room StartingRoom { get { return startingRoom; } }

    private void Awake() {
        Debug.Assert(startingRoom != null, "startingRoom is not assigned!"); 
    }
}
