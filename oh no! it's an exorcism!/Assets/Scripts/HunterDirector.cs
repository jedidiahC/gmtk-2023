using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class HunterDirector : MonoBehaviour
{
    [SerializeField] private Level level = null;
    [SerializeField] private Ritual ritual = null;
    [SerializeField] private List<HunterLogic> hunters = null;
    [SerializeField] private List<Breaker> breaker = null;
    [SerializeField] private int minCluesBeforeMeet = 6;
    [SerializeField] private int maxCluesBeforeMeet = 8;
    [SerializeField] private int minMeetingTime = 2;
    [SerializeField] private int maxMeetingTime = 5;

    [SerializeField] private Queue<Room> roomsToExplore = new Queue<Room>();
    [SerializeField] private Queue<Room> roomsToInvestigate = new Queue<Room>();

    [SerializeField] private List<Room> roomsExplored;
    [SerializeField] private List<Room> breakerRoomsExplored;
    [SerializeField] private HuntDirective currentDirective = HuntDirective.Normal;
    [SerializeField] private Room meetingRoom;

    [SerializeField] private int cluesBeforeNextMeet = 0;
    [SerializeField] private float meetingTimeRequired = 0;

    public HuntDirective Directive { get { return currentDirective; }}
    public Room MeetingRoom { get { return meetingRoom; } }

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

        cluesBeforeNextMeet = Random.Range(minCluesBeforeMeet, maxCluesBeforeMeet);
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
            Debug.Log($"Queued {room.gameObject.name} for exploration.");
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
            Debug.Log($"Queued {room.gameObject.name} for investigation.");
            roomsToInvestigate.Enqueue(room);
        }
    }

    public void ReportExploredRoom(Room room)
    {
        if (!roomsExplored.Contains(room))
        {
            roomsExplored.Add(room);
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

    public void TriggerMeet() 
    {
        currentDirective = HuntDirective.Meet;
        meetingRoom = roomsExplored[Random.Range(0, roomsExplored.Count)];
        meetingTimeRequired = Random.Range(minMeetingTime, maxMeetingTime);
        Debug.Log("Meeting triggered");
    }

    public void SubmitClue(HunterLogic hunter, Clue clue)
    {
        ritual.SubmitClue(hunter, clue);
        cluesBeforeNextMeet--;

        if (cluesBeforeNextMeet == 0) 
        {
            TriggerMeet();
        }

        cluesBeforeNextMeet = Mathf.Min(Random.Range(minCluesBeforeMeet, maxCluesBeforeMeet), ritual.NumCluesRemaining);
    }

    private void Update() {
        if (currentDirective == HuntDirective.Meet) {

            foreach (var h in hunters) {
                if (!meetingRoom.IsInRoom(h.transform)) {
                    return;
                }
            }

            meetingTimeRequired -= Time.deltaTime;

            if (meetingTimeRequired <= 0)
            {
                if (ritual.NumRitualCluesRemaining == 0) 
                {
                    Debug.Log("Ready for ritual");
                    currentDirective = HuntDirective.CompleteRitual;
                } else 
                {
                    meetingTimeRequired = Random.Range(minMeetingTime, maxMeetingTime);
                    currentDirective = HuntDirective.Normal;
                }
            }
        }    
    }
}

public enum HuntDirective
{
    Normal, Meet, CompleteRitual
}