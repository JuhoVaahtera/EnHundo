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

    private SwordCollider swordCollider; // Reference to the SwordCollider script

    // Animation States
    private enum State { Idle, Running, Jumping, Falling, Attacking, Dying, Hit }
    private State currentState = State.Idle;

    private bool isAttacking = false;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        swordCollider = GetComponentInChildren<SwordCollider>(); // Get the SwordCollider component from the child
    }

    void Update()
    {
        if (currentState != State.Dying && currentState != State.Hit)
        {
            moveInput = Input.GetAxis("Horizontal");
            

            HandleJump();
            HandleAttack();
            UpdateAnimationState();
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (!isAttacking)
        {
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
        else if (rb.velocity.y == 0) // Only change direction if player is not jumping
        {
            rb.velocity = new Vector2(0f, rb.velocity.y); // Stop horizontal movement during attack
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
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        currentState = State.Attacking;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.2f); // Adjust this delay according to your attack animation duration
        isAttacking = false;
    }

    void UpdateAnimationState()
    {
        if (isAttacking)
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
            // Stop player movement temporarily
            rb.velocity = Vector2.zero;
            isAttacking = false; // Stop attacking if the player was attacking

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
        animator.SetInteger("State", (int)currentState);
    }

    // Methods to enable and disable the sword collider
    public void EnableSwordCollider()
    {
        if (swordCollider != null)
        {
            swordCollider.EnableSwordCollider();
        }
    }

    public void DisableSwordCollider()
    {
        if (swordCollider != null)
        {
            swordCollider.DisableSwordCollider();
        }
    }
}
