using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HunterDirector : MonoBehaviour
{
    [SerializeField] private Level level = null;
    [SerializeField] private Ritual ritual = null;
    [SerializeField] private List<HunterLogic> hunters = null;
    [SerializeField] private List<Breaker> breaker = null;

    [SerializeField] private Queue<Room> roomsToExplore = new Queue<Room>();
    [SerializeField] private Queue<Room> roomsToInvestigate = new Queue<Room>();

    private void Awake()
    {
        Debug.Assert(level != null, "level is not assigned!");
        Debug.Assert(ritual != null, "ritual is not assigned!");
    }

    private void Start()
    {
        foreach (var hunter in hunters)
        {
            hunter.SetDirector(this);
        }

        QueueRoomToExplore(level.StartingRoom);
    }

    public HunterLogic FindHunterInState(HunterState state)
    {
        foreach (var hunter in hunters)
        {
            if (hunter.CurrentState == state)
            {
                return hunter;
            }
        }

        return null;
    }

    public void QueueRoomToExplore(Room room)
    {
        if (room == null)
        {
            return;
        }

        if (!room.IsExplored && !roomsToExplore.Contains(room))
        {
            roomsToExplore.Enqueue(room);
        }
    }

    public void QueueRoomToInvestigate(Room room)
    {
        if (room == null)
        {
            return;
        }

        if (room != null && room.IsExplored && !room.IsInvestigated && !roomsToInvestigate.Contains(room))
        {
            roomsToInvestigate.Enqueue(room);
        }
    }

    public Room AssignNextRoomToExplore()
    {
        if (roomsToExplore.Count > 0)
        {
            return roomsToExplore.Dequeue();
        }

        return null;
    }

    public Room AssignNextRoomToInvestigate()
    {
        if (roomsToInvestigate.Count > 0)
        {
            return roomsToInvestigate.Dequeue();
        }

        return null;
    }

    public void SubmitClue(HunterLogic hunter, Clue clue)
    {
        ritual.SubmitClue(hunter, clue);
    }
}
