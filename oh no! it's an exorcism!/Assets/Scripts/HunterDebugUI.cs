using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class HunterDebugUI : MonoBehaviour
{
    [SerializeField] private float verticalOffset = 1f;
    private HunterLogic hunter = null;

    private void Awake() 
    {
        hunter = GetComponent<HunterLogic>();
    }

    private void OnDrawGizmos() {
        // if (hunter != null) {
        //     Handles.Label(transform.position + Vector3.up * verticalOffset, $"{hunter.name} : {hunter.CurrentState} \n Fear {hunter.CurrentFear} / {hunter.MaxFear}");
        // }     
    }
}
