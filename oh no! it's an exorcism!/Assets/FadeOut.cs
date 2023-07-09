using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
  float timer = 1.0f;
  [SerializeField] Image image;

  void Start()
  {
    image.CrossFadeAlpha(0.0f, 1.0f, false);
  }

  void Update()
  {
    // if (timer < 0)
    // {
    //   gameObject.SetActive(false);
    // }

    // if (timer > 0)
    // {
    //   timer -= Time.deltaTime;
    // }

    // image.CrossFadeAlpha(0.0f, 1.0f, false);
  }
}
