using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Breaker : MonoBehaviour
{
    [SerializeField] private float activation = 0;
    [SerializeField] private float activateRate = 20.0f;
    [SerializeField] private GameObject connected = null;
    [SerializeField] private float activationRadius = 1.0f;

    public bool IsActivated { get { return activation == 100; }}
    public float ActivationRadius { get { return activationRadius; }}

    public bool Activate()
    {
        activation = Mathf.Min(100, activation + Time.deltaTime * activateRate);

        if (activation == 100)
        {
            connected.SetActive(true);
        }

        return activation == 100;
    } 

    public void Deactivate()
    {
        activation = 0;
        connected.SetActive(false);
    }
}
