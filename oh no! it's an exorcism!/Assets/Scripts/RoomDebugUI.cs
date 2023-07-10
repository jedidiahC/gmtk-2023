using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomDebugUI : MonoBehaviour
{
    private Room room = null;

    void OnDrawGizmos()
    {
        if (room == null) {
            room = GetComponent<Room>();
            return;
        }

        Vector3 offset = Vector3.up * 0.0f;

        // Handles.Label(transform.position + offset, $"{room.name} : Explored ({room.IsExplored})\n | Investigated ({room.IsInvestigated})\n | Clue Containers Investigated ({room.InvestigatedClueContainers.Count} / {room.ClueContainers.Count})");

        if (room.AdjacentRooms == null)
        {
            return;
        }

        foreach (var r in room.AdjacentRooms)
        {
            // Show exploration lines.
            if (r != null)
            {
                Gizmos.color = r.IsExplored || room.IsExplored ? Color.green : Color.red;
                Gizmos.DrawLine(r.GetDestinationPos(), room.GetDestinationPos());
            }
        }
    }
}
