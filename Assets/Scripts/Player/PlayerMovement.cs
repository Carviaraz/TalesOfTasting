using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float speed;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.3f;
    [SerializeField] private float transparency = 0.3f;

    private Rigidbody2D rb2d;
    private PlayerAction actions;
    private SpriteRenderer spriteRenderer;

    private Vector2 moveDirection;
    private float currentSpeed;
    private bool isDashing;
    private Animator animator;
    private Vector2 lastMoveDirection;

    private Vector2 lastNonZeroDirection;
    private float idleTransitionBufferTime = 0.1f;  // Buffer time to prevent idle animation
    private float idleTransitionTimer;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        actions = new PlayerAction();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        currentSpeed = speed;
        actions.Movement.Dash.performed += context => Dash();
    }

    void Update()
    {
        CaptureInput();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        rb2d.MovePosition(rb2d.position + moveDirection * (currentSpeed * Time.fixedDeltaTime));
    }

    private void Dash()
    {
        if (isDashing)
        {
            return;
        }

        isDashing = true;
        StartCoroutine(IEDash());
    }

    private IEnumerator IEDash()
    {
        currentSpeed = dashSpeed;
        ModifySpriteRenderer(transparency);
        yield return new WaitForSeconds(dashTime);
        currentSpeed = speed;
        ModifySpriteRenderer(1f);
        isDashing = false;
    }

    private void ModifySpriteRenderer(float alpha)
    {
        Color color = spriteRenderer.color;
        color = new Color(color.r, color.g, color.b, alpha);
        spriteRenderer.color = color;
    }

    private void UpdateAnimation()
    {
        if (moveDirection.magnitude > 0.1f)
        {
            animator.SetFloat("moveX", moveDirection.x);
            animator.SetFloat("moveY", moveDirection.y);
            animator.SetBool("isMoving", true);

            lastNonZeroDirection = moveDirection;

            idleTransitionTimer = 0f;
        }
        else
        {
            idleTransitionTimer += Time.deltaTime;

            if (idleTransitionTimer < idleTransitionBufferTime)
            {
                animator.SetFloat("moveX", lastNonZeroDirection.x);
                animator.SetFloat("moveY", lastNonZeroDirection.y);
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
                animator.SetFloat("moveX", lastNonZeroDirection.x);
                animator.SetFloat("moveY", lastNonZeroDirection.y);
            }
        }

        animator.SetBool("isDashing", isDashing);
    }

    private void CaptureInput()
    {
        moveDirection = actions.Movement.Move.ReadValue<Vector2>().normalized;
    }

    private void OnEnable()
    {
        actions.Enable();
    }

    private void OnDisable()
    {
        actions.Disable();
    }
}
