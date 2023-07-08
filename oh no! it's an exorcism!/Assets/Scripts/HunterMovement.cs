using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public class HunterMovement : MonoBehaviour
{
    [SerializeField] private Vector3 targetPos = Vector3.zero;
    [SerializeField] private Transform target = null;
    [SerializeField] private bool hasTarget = false;
    [SerializeField] private bool hasTargetPos = false;
    [SerializeField] private NavMeshAgent agent = null;

    public void GoToTarget(Transform target) 
    {
        if (target == null) {
            Debug.LogError("Can't move to target! Target is null!");
            return;
        }

        this.target = target;
        hasTarget = true;
        hasTargetPos = false;
    }

    public void GoToTargetPos(Vector3 targetPos) 
    {
        hasTarget = true;
        hasTargetPos = false;

        // Inform navmesh agent.
        agent.SetDestination(targetPos);
    }

    public void Stop() 
    {
        hasTarget = false;
        hasTargetPos = false;
        target = null;

        // Inform navmesh agent.
        agent.SetDestination(this.transform.position);
    }
}
