using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class HunterLogic : MonoBehaviour
{
    public UnityEvent<HunterState> OnEnterState;

    [SerializeField] private List<Trap> traps = null;
    [SerializeField] private Room assignedRoom = null;

    [SerializeField] private Breaker assignedBreaker = null;
    [SerializeField] private Room breakerRoom = null;

    [Header("Exploration")]
    [SerializeField] private float exploreTime = 5.0f;
    [SerializeField] private float exploreTimer = 0.0f;

    [Header("Clues")]
    [SerializeField] private ClueContainer assignedContainer = null;
    [SerializeField] private Clue examinedClue = null;

    [Header("Escort")]
    [SerializeField] private HunterLogic leader = null;

    [Header("Fear")]
    public float viewRadius = 2;
    public float viewAngle = 45;
    public float maxJumpScareDist = 0.7f;
    public LayerMask targetMask;

    [SerializeField] private float maxFear = 100;
    [SerializeField] private float currentFear = 0;
    [SerializeField] private float fearRateWhileJumpscare = 2;
    [SerializeField] private float jumpscareTime = 10.0f;
    [SerializeField] private float jumpscareTimer = 0;

    [SerializeField] private Spook spook = null;

    [Header("References")]
    [SerializeField] private HunterMovement movement = null;
    [SerializeField] private HunterState currentState = HunterState.Begin;

    private HunterDirector director = null;
    private Breaker breaker = null;

    public HunterState CurrentState { get { return currentState; } }
    public float MaxFear { get { return maxFear; } }
    public float CurrentFear { get { return currentFear; } }
    public Room AssignedRoom { get { return assignedRoom; } }

    public void SetDirector(HunterDirector director)
    {
        this.director = director;
    }

    public void Spook(Spook spook)
    {
        if (this.spook != null)
        {
            return;
        }

        this.spook = spook;
        currentFear = Mathf.Min(maxFear, currentFear + spook.ScaryPoints);
    }

    public void AssignBreakerRoom(Room breakerRoom)
    {
        this.breakerRoom = breakerRoom;
    }

    private void Awake()
    {
        Debug.Assert(movement != null, "movement is not assigned!");
    }

    // Update is called once per frame
    void Update()
    {
        // Check jumpscare.
        if (currentState != HunterState.Jumpscare && currentState != HunterState.Shock)
        {
            Transform target = director.player.transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            Vector3 forward = GetComponent<NavMeshAgent>().desiredVelocity.normalized;

            forward.z = 0;
            dirToTarget.z = 0;

            float angle = Vector3.Angle(forward, dirToTarget);

            if (angle < viewAngle / 2)
            {
                if (Vector3.Distance(transform.position, target.position) < maxJumpScareDist && director.player.isVisible)
                {
                    jumpscareTimer = jumpscareTime;
                    LogAction("just got jumpscared!");
                    return;
                }
            }
        }

        switch (currentState)
        {
            case HunterState.Idling:
                Idling();
                break;

            case HunterState.Exploring:
                Exploring();
                break;

            case HunterState.Escort:
                Escorting();
                break;

            case HunterState.InvestigatingRoom:
                InvestigatingRoom();
                break;

            case HunterState.ExaminingClue:
                ExaminingClue();
                break;

            case HunterState.ActivatingBreaker:
                ActivatingBreakers();
                break;

            case HunterState.Meeting:
                Meeting();
                break;

            case HunterState.InvestigatingSpook:
                InvestigatingSpook();
                break;
            
            case HunterState.Jumpscare:
                Jumpscare();
                break;
        }

        PostUpdateCheck();
    }

    private void Idling()
    {
        if (director.Directive == HuntDirective.Meet || breakerRoom != null)
        {
            return;
        }

        // Check for rooms to explore.
        assignedRoom = director.AssignNextRoomToExplore();

        if (assignedRoom == null)
        {
            assignedRoom = director.AssignNextRoomToInvestigate();
        }

        if (assignedRoom == null)
        {
            leader = director.FindHunterInState(HunterState.Exploring);
        }
    }

    private void Exploring()
    {
        // In room.
        if (assignedRoom.IsInRoom(this.transform))
        {
            exploreTimer += Time.deltaTime;

            // On explored.
            if (exploreTimer >= exploreTime)
            {
                foreach (var room in assignedRoom.AdjacentRooms)
                {
                    director.QueueRoomToExplore(room);
                }

                assignedRoom.MarkExplored();
                director.ReportExploredRoom(assignedRoom);

                AbandonAssignedRoom();
            }
        }
        else
        {
            if (assignedRoom.IsExplored)
            {
                AbandonAssignedRoom();
            }
            else
            {
                movement.GoToTargetPos(assignedRoom.GetDestinationPos());
            }
        }

    }

    private void Escorting()
    {
        if (leader == null) { return; }

        movement.GoToTargetPos(leader.transform.position);

        if (leader.CurrentState != HunterState.Exploring)
        {
            leader = null;
        }

    }

    private void InvestigatingRoom()
    {
        if (assignedRoom == null) { return; }

        // Not in room.
        if (!assignedRoom.IsInRoom(this.transform))
        {
            if (assignedRoom.IsInvestigated)
            {
                AbandonAssignedRoom();
            }
            else
            {
                movement.GoToTargetPos(assignedRoom.GetDestinationPos());
            }

            return;
        }

        // Get container.
        if (assignedContainer == null || assignedContainer.IsInvestigated)
        {
            assignedContainer = assignedRoom.GetRandomClueContainer();

            if (assignedContainer == null)
            {
                assignedRoom.MarkInvestigated();
                AbandonAssignedRoom();
            }

            return;
        }

        // Investigate container.
        bool withinRadius = Vector3.Distance(transform.position, assignedContainer.transform.position) < assignedContainer.InvestigateRadius;

        if (withinRadius)
        {
            movement.Stop();
            examinedClue = assignedContainer.TakeClue();

            if (examinedClue != null)
            {
                LogAction($"took {examinedClue.Id}");
            }
            else
            {
                assignedContainer = null;
            }
        }
        else
        {
            movement.GoToTargetPos(assignedContainer.transform.position);
        }
    }

    private void ExaminingClue()
    {
        if (examinedClue == null)
        {
            return;
        }

        bool examined = examinedClue.Examine(Time.deltaTime);
        if (examined)
        {
            director.SubmitClue(this, examinedClue);
            LogAction($"submitted {examinedClue.Id}.");
            examinedClue = null;
        }
    }

    private void Meeting()
    {
        if (!director.MeetingRoom.IsInRoom(transform))
        {
            movement.GoToTargetPos(director.MeetingRoom.GetDestinationPos());
            return;
        }
    }

    private void InvestigatingSpook()
    {
        if (spook == null) { return; }

        if (spook.IsInvestigated)
        {
            spook = null;
            return;
        }

        if (Vector3.Distance(spook.transform.position, this.transform.position) < spook.InvestigateRadius)
        {
            spook.Investigate();
        }
        else
        {
            movement.GoToTargetPos(spook.transform.position);
        }
    }

    private void ActivatingBreakers()
    {
        if (breakerRoom == null) { return; }

        // Not in room.
        if (!breakerRoom.IsInRoom(this.transform))
        {
            movement.GoToTargetPos(breakerRoom.GetDestinationPos());
            return;
        }

        // In room but no assigned breaker.
        if (assignedBreaker == null)
        {
            foreach (var breaker in breakerRoom.Breakers) 
            {
                if (!breaker.IsActivated) 
                {
                    assignedBreaker = breaker;
                }
            }

            if (assignedBreaker == null)
            {
                breakerRoom = null;
            }

            return;
        }

        if (Vector3.Distance(assignedBreaker.transform.position, transform.position) < assignedBreaker.ActivationRadius)
        {
            movement.GoToTargetPos(assignedBreaker.transform.position);
        }
        else 
        {
            bool activated = assignedBreaker.Activate();

            if (activated)
            {
                assignedBreaker = null;
            }
        }
    }

    private void Jumpscare()
    {
        jumpscareTimer -= Time.deltaTime;
        currentFear = Mathf.Min(maxFear, currentFear + Time.deltaTime * fearRateWhileJumpscare);
        movement.Stop();
        if (jumpscareTimer < 0)
        {
            jumpscareTimer = 0;
        }
    }

    // Handle state transitions.
    private void PostUpdateCheck()
    {

        if (currentFear == maxFear)
        {
            EnterState(HunterState.Shock);
            return;
        }

        if (currentState != HunterState.Jumpscare && jumpscareTimer > 0) 
        {
            EnterState(HunterState.Jumpscare);
            return;
        }

        if (spook != null && currentState != HunterState.InvestigatingSpook)
        {
            EnterState(HunterState.InvestigatingSpook);
            return;
        }

        if (currentState == HunterState.Begin)
        {
            EnterState(HunterState.Idling);
        }
        else if (currentState == HunterState.Idling)
        {

            if (director.Directive == HuntDirective.Meet)
            {
                EnterState(HunterState.Meeting);
            }
            // Has clue to examine.
            else if (examinedClue != null)
            {
                EnterState(HunterState.ExaminingClue);
            }
            // Investigate container.
            else if (assignedContainer != null)
            {
                EnterState(HunterState.InvestigatingRoom);
            }
            else if (assignedRoom != null)
            {
                EnterState(assignedRoom.IsExplored ? HunterState.InvestigatingRoom : HunterState.Exploring);
            }
            else if (leader != null)
            {
                EnterState(HunterState.Escort);
            }
            else if (breakerRoom != null)
            {
                EnterState(HunterState.ActivatingBreaker);
            }
        }
        else if (currentState == HunterState.Exploring)
        {
            if (assignedRoom == null || assignedRoom.IsExplored)
            {
                EnterState(HunterState.Idling);
            }

        }
        else if (currentState == HunterState.InvestigatingRoom)
        {

            if (assignedRoom == null || assignedRoom.IsInvestigated)
            {
                EnterState(HunterState.Idling);
            }
            else if (examinedClue != null)
            {
                EnterState(HunterState.ExaminingClue);
            }

        }
        else if (currentState == HunterState.ExaminingClue)
        {

            if (examinedClue == null)
            {
                EnterState(HunterState.InvestigatingRoom);
            }

        }
        else if (currentState == HunterState.Escort)
        {

            if (leader == null)
            {
                EnterState(HunterState.Idling);
            }

        }
        else if (currentState == HunterState.Meeting)
        {

            if (director.Directive != HuntDirective.Meet)
            {
                EnterState(HunterState.Idling);
            }

        }
        else if (currentState == HunterState.InvestigatingSpook)
        {
            if (spook == null || spook.IsInvestigated)
            {
                EnterState(HunterState.Idling);
            }
        }
        else if (currentState == HunterState.ActivatingBreaker)
        {
            if (breakerRoom == null)
            {
                EnterState(HunterState.Idling);
            }
        }
        else if (currentState == HunterState.Jumpscare)
        {
            if (jumpscareTimer == 0)
            {
                EnterState(HunterState.Idling);
            }
        }
    }

    private void LeaveCurrentState()
    {
        switch (currentState)
        {
            case HunterState.Idling:
                break;
            case HunterState.Exploring:
                AbandonAssignedRoom();
                break;
            case HunterState.InvestigatingRoom:
                AbandonAssignedRoom();
                break;
            case HunterState.ActivatingBreaker:
                breaker = null;
                breakerRoom = null;
                director.InformBreakerActivationFailed();
                break;
            case HunterState.Escort:
                movement.Stop();
                leader = null;
                break;
            default:
                break;
        }
    }

    private void EnterState(HunterState state)
    {

        if (currentState != state)
        {
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

        LogAction($"started {state}");
    }

    private void AbandonAssignedRoom()
    {
        if (assignedRoom != null)
        {
            if (!assignedRoom.IsExplored)
            {
                director.QueueRoomToExplore(assignedRoom);
            }
            else if (!assignedRoom.IsInvestigated)
            {
                director.QueueRoomToInvestigate(assignedRoom);
            }
        }

        assignedRoom = null;
        assignedContainer = null;
    }

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

    public void Init()
    {
    }

    private void LogAction(string str)
    {
        Debug.Log($"{this.gameObject.name} {str}");
    }
}

public enum HunterState
{
    Begin, Idling, Escort, Exploring, ExaminingClue, InvestigatingRoom, ActivatingBreaker, Meeting, InvestigatingSpook, Jumpscare, Shock
}