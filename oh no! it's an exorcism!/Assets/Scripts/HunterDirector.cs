using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HunterDirector : MonoBehaviour
{
    [SerializeField] private Level level = null;
    [SerializeField] private List<HunterLogic> hunters = null;
    [SerializeField] private List<Breaker> breaker = null;

    [SerializeField] private Queue<Room> roomsToExplore = new Queue<Room>();

    private void Awake() 
    {
        Debug.Assert(level != null, "level is not assigned!");     
    }

    private void Start() 
    {
        foreach (var hunter in hunters) {
            hunter.SetDirector(this);
        }

        QueueRoom(level.StartingRoom);
    }

    public void QueueRoom(Room room) {
        if (!room.IsExplored && !roomsToExplore.Contains(room)) {
            roomsToExplore.Enqueue(room);
        }
    }

    public Room AssignNextRoomToExplore() {
        if (roomsToExplore.Count > 0) {
            return roomsToExplore.Dequeue();
        }

        return null;
    }
}
