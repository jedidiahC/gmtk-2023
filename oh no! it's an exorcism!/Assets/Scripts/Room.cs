using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] bool isExplored = false;
    [SerializeField] bool isInvestigated = false;

    [SerializeField] List<ClueContainer> clueContainers;
    [SerializeField] List<Room> adjacentRooms;
    [SerializeField] Transform destination = null;

    private Collider2D col = null;
    private List<Clue> clues;
    
    [Header("Debug")]
    [SerializeField] List<ClueContainer> uninvestigatedClueContainers;
    [SerializeField] private List<ClueContainer> investigatedClueContainers;

    public bool IsExplored { get { return isExplored; } }
    public bool IsInvestigated { get { return isInvestigated; } }
    public List<ClueContainer> ClueContainers { get { return clueContainers; } }
    public List<ClueContainer> UninvestigatedClueContainers { get { return uninvestigatedClueContainers; } }
    public List<ClueContainer> InvestigatedClueContainers { get { return investigatedClueContainers; } }

    public List<Room> AdjacentRooms { get { return adjacentRooms; } }

    public ClueContainer GetRandomClueContainer() {
        if (uninvestigatedClueContainers.Count < 1) {
            return null;
        }

        return uninvestigatedClueContainers[Random.Range(0, uninvestigatedClueContainers.Count)];
    }

    private void Awake() {
        this.col = GetComponent<Collider2D>();

        SetupBidirectionalEdges();

        uninvestigatedClueContainers.AddRange(clueContainers);
        investigatedClueContainers = new List<ClueContainer>();
    }

    private void Start() {
        foreach (var cc in clueContainers) 
        {
            cc.onInvestigate.AddListener(OnContainerInvestigated);
        } 
    }

    private void OnContainerInvestigated(ClueContainer container) 
    {
        uninvestigatedClueContainers.Remove(container);
        investigatedClueContainers.Add(container);
    }

    private void SetupBidirectionalEdges() {
        foreach (var room in adjacentRooms) {
            
            // If room reference is missing, ignore.
            if (room == null) {
                continue;
            }

            if (!room.AdjacentRooms.Contains(this)) {
                room.AdjacentRooms.Add(this);
            }
        }
    }

    public void MarkExplored() 
    {
        isExplored = true;
        Debug.Log($"{gameObject.name} explored.");
    }    

    public void MarkInvestigated() 
    {
        isInvestigated = true;
        Debug.Log($"{gameObject.name} investigated. Investigated {investigatedClueContainers.Count} clue containers!");
    }    

    public Vector3 GetDestinationPos() {
        return destination != null ? destination.position : GetComponent<Collider2D>().bounds.center;
    }

    public bool IsInRoom(Transform other) {
        Vector3 pos = other.position;
        pos.z = transform.position.z;
        return this.col.bounds.Contains(pos);
    }

}
