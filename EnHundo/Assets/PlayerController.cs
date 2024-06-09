using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private bool isGrounded;
    private bool isFacingRight = true;

    // Animation States
    private enum State { Idle, Running, Jumping, Falling, Attacking, Dying, Hit }
    private State currentState = State.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (currentState != State.Dying && currentState != State.Hit)
        {
            HandleMovement();
            HandleJump();
            HandleAttack();
            UpdateAnimationState();
        }
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (moveInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            currentState = State.Jumping;
        }
    }

    void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            currentState = State.Attacking;
            animator.SetTrigger("Attack");
        }
    }

    void UpdateAnimationState()
    {
        if (currentState == State.Attacking)
            return;

        if (rb.velocity.y > 0.1f)
        {
            currentState = State.Jumping;
        }
        else if (rb.velocity.y < -0.1f)
        {
            currentState = State.Falling;
        }
        else if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            currentState = State.Running;
        }
        else
        {
            currentState = State.Idle;
        }

        animator.SetInteger("State", (int)currentState);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            currentState = State.Hit;
            animator.SetTrigger("Hit");
        }
    }

    public void Die()
    {
        currentState = State.Dying;
        animator.SetTrigger("Death");
    }

    private void ResetState()
    {
        currentState = State.Idle;
    }
}

