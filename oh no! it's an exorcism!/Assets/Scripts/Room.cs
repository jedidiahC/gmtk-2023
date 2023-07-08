using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] bool isExplored = false;
    [SerializeField] bool allCluesFound = false;
    [SerializeField] List<Room> adjacentRooms;
    [SerializeField] Transform destination = null;

    private Collider2D col = null;

    public bool IsExplored { get { return isExplored; } }

    public List<Room> AdjacentRooms { get { return adjacentRooms; } }

    private void Awake() {
        this.col = GetComponent<Collider2D>();
    }

    public void MarkExplored() 
    {
        isExplored = true;
    }    

    public Vector3 GetDestinationPos() {
        return destination != null ? destination.position : GetComponent<Collider2D>().bounds.center;
    }

    public bool IsInRoom(Transform other) {
        Vector3 pos = other.position;
        pos.z = transform.position.z;
        return this.col.bounds.Contains(pos);
    }

    void OnDrawGizmos() {
        if (adjacentRooms == null) {
            return;
        }

        Gizmos.color = isExplored ? Color.green : Color.red;
        foreach (var room in adjacentRooms) {
            if (room != null) {
                Gizmos.DrawLine(GetDestinationPos(), room.GetDestinationPos());
            }
        }
    }
}
