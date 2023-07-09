using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallLightMasterController : MonoBehaviour
{
  public WallLight[] wallLights;
  float countdown = 0.05f;
  int index;

  void Start()
  {
    wallLights = FindObjectsOfType<WallLight>();
    index = wallLights.Length - 1;
  }

  void Update()
  {
    if (index > 0)
    {
      if (countdown < 0.0f)
      {
        --index;
        countdown += Random.Range(0.0f, 0.05f);
      }

      wallLights[index].ToggleLights(false);

      countdown -= Time.deltaTime;
    }
  }
}
