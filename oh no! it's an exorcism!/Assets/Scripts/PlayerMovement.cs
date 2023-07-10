using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour

{
  public bool useMouseForMovement;
  public float movementSpeed = 1.0f;
  public float squishRate = 1.0f, squishPercentage = 0.2f;
  public SpriteRenderer spriteRenderer;
  Vector2 initialScale;
  bool movedThisFrame;
  bool isDashing;
  float dashTimer;
  public float dashDuration = 0.25f;
  [SerializeField] AudioSource audioSource;
  [SerializeField] AudioClip whooshSound;

  public bool isVisible;
  bool canMove = true;

  // Start is called before the first frame update
  void Start()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
    initialScale = transform.localScale;
    canMove = true;
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Tab))
    {
      useMouseForMovement = !useMouseForMovement;
    }

    isVisible = Input.GetKey(KeyCode.Space);

    if (isDashing)
    {
      if (dashTimer < 0.0f)
      {
        isDashing = false;
      }
      dashTimer -= Time.deltaTime;
    }
    else if (Input.GetKeyDown(KeyCode.LeftShift))
    {
      isDashing = true;
      audioSource.PlayOneShot(whooshSound);
      dashTimer += dashDuration;
    }

    if (useMouseForMovement)
    {
      Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      Vector2 transformPosition = transform.position;
      spriteRenderer.flipX = mousePosition.x > transformPosition.x;
      if (Vector2.Distance(mousePosition, transformPosition) > 1.0f && canMove)
      {
        movedThisFrame = true;
        transform.position = Vector2.MoveTowards(transform.position, mousePosition, (movementSpeed + (isDashing ? 10.0f : 0.0f)) * Time.deltaTime);

      }
    }
    else
    {
      Vector3 axis = new Vector3();
      axis.x = Input.GetAxisRaw("Horizontal");
      axis.y = Input.GetAxisRaw("Vertical");
      axis = Vector3.ClampMagnitude(axis, 1.0f);
      if (axis.x != 0 || axis.y != 0)
      {
        movedThisFrame = true;
      }
      spriteRenderer.flipX = axis.x > 0;
      if (canMove) transform.position += axis * (movementSpeed + (isDashing ? 10.0f : 0.0f)) * Time.deltaTime;
    }

    SquishSquash(movedThisFrame);
    movedThisFrame = false;
  }

  void SquishSquash(bool moved)
  {
    if (!moved)
    {
      return;
    }

    Vector3 scale = transform.localScale;
    float sinModifier = (0.5f * Mathf.Sin(Mathf.PI * ((Time.time * squishRate) % 1.0f * 2.0f - 0.5f)) + 0.5f);
    scale.x = initialScale.x + squishPercentage * initialScale.x * (isDashing ? 1.0f : sinModifier);
    scale.x += isDashing ? initialScale.x * 0.2f : 0.0f;
    scale.y = initialScale.y - squishPercentage * initialScale.y * (isDashing ? 1.0f : sinModifier);
    scale.y -= isDashing ? initialScale.y * 0.2f : 0.0f;
    transform.localScale = scale;
  }

  // private void OnCollisionEnter2D(Collision2D other)
  // {
  //   if (other.gameObject.tag != "help")
  //   {
  //     return;
  //   }

  //   isDashing = false;
  // }
}
