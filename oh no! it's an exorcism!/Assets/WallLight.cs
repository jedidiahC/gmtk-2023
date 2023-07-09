using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WallLight : MonoBehaviour
{
  [SerializeField] Light2D mainLight, areaLight;

  public void ToggleLights(bool isOn)
  {
    mainLight.enabled = isOn;
    areaLight.enabled = isOn;
  }
}
