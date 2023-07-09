using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public class HunterRender : MonoBehaviour
{
    private SpriteRenderer sr;
    private NavMeshAgent agent;
    private Animator animator;

    private void Awake() 
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update() 
    {
        Vector3 vel = agent.velocity;

        sr.flipX = vel.x < 0;
        animator.SetFloat("speed", vel.magnitude);
    }
}
