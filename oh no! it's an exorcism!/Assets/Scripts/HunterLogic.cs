using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class HunterLogic : MonoBehaviour
{
    public UnityEvent<HunterState> OnEnterState;

    [SerializeField] private List<Trap> traps = null;
    [SerializeField] private Room assignedRoom = null;
    [SerializeField] private float exploreTime = 5.0f;
    [SerializeField] private float exploreTimer = 0.0f;
    [SerializeField] private int maxFear = 100;
    [SerializeField] private int currentFear = 0;
    [SerializeField] private HunterMovement movement = null;
    [SerializeField] private HunterState currentState = HunterState.Begin;

    private HunterDirector director = null;
    private Breaker breaker = null;

    public HunterState CurrentState { get { return currentState; } }
    public Room AssignedRoom { get { return assignedRoom; } }

    public void SetDirector(HunterDirector director) {
        this.director = director;
    }

    private void Awake() 
    {
        Debug.Assert(movement != null, "movement is not assigned!");
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState) {
            case HunterState.Idling:
                Idling();
                break;

            case HunterState.Exploring:

                Explore();
                break;

            case HunterState.Escort:
                Escort();
                break;

            case HunterState.Investigating:
                Investigate();
                break;

            case HunterState.ActivatingBreaker:
                ActivatingBreaker();
                break;
        }

        PostUpdateCheck();
    }

    private void Idling()
    {
        // Check for rooms to explore.
        assignedRoom = director.AssignNextRoomToExplore();
        if (assignedRoom != null) {
            movement.GoToTargetPos(assignedRoom.GetDestinationPos());
        }

        LogHunter("is idling");
    }

    private void Explore() 
    {
        // In room.
        if (assignedRoom.IsInRoom(this.transform)) {
            exploreTimer += Time.deltaTime;

            if (exploreTimer >= exploreTime) {
                foreach (var room in assignedRoom.AdjacentRooms) {
                    director.QueueRoom(room);
                }

                assignedRoom.MarkExplored();
                assignedRoom = null;
            }
        } else {
            movement.GoToTargetPos(assignedRoom.GetDestinationPos());
        }

        LogHunter("is exploring");
    }

    private void Escort() 
    {

    }

    private void Investigate() 
    {
        if (assignedRoom == null) {
            EnterState(HunterState.Idling);
        }

        LogHunter("is investigating");
    }

    private void ActivatingBreaker() 
    {
        LogHunter("is activating breaker");
    }

    private void PostUpdateCheck() 
    {
        if (currentState == HunterState.Begin) {
            EnterState(HunterState.Idling);
        } else if (currentState == HunterState.Idling) {

            if (assignedRoom != null) {
                if (!assignedRoom.IsExplored) {
                    EnterState(HunterState.Exploring);
                } else {
                    EnterState(HunterState.Investigating);
                }
            }

        } else if (currentState == HunterState.Exploring) {

            if (assignedRoom == null) {
                EnterState(HunterState.Idling);
            }

        }
    }

    private void LeaveCurrentState()
    {
        switch(currentState) {
            case HunterState.ActivatingBreaker:
                breaker = null;
                break;
            default:
                break;
        } 
    }

    private void EnterState(HunterState state) 
    {
        if (currentState != state) {
            LeaveCurrentState();
        }

        currentState = state;

        switch (state)
        {
            case HunterState.Exploring:
                exploreTimer = 0;
                break;
            default:
                break;
        }

        OnEnterState.Invoke(state);

        LogHunter($"entering {state}");
    }

    public void Init() 
    {

    }

    private void LogHunter(string str) 
    {
        // Debug.Log($"{this.gameObject.name} {str}");
    }
}

public enum HunterState {
    Begin, Idling, Escort, Exploring, ExaminingClue, Investigating, ActivatingBreaker
}