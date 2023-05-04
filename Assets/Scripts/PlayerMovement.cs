using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D Rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private TrailRenderer trailRenderer;

    public bool isOnGround;
    private float dirX = 0f;
    public float moveSpeed = 7f;
    public float jumpForce = 10f;

    private int maxJumps = 2;
    private int jumpsLeft;

    public float dashVel;
    private float dashTime = 0.1f;
    private Vector2 dashingDir;
    private bool isDashing;
    private bool canDash = true;

    // Movement States
    private enum MovementState
    {
        idle, running, jumping, falling
    }


    // Start is called before the first frame update
    private void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        jumpsLeft = maxJumps;
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        var dashInput = Input.GetButtonDown("Dash");
        dirX = Input.GetAxis("Horizontal");

        // Dash
        if (dashInput && canDash)
        {
            isDashing = true;
            canDash = false;
            trailRenderer.emitting = true;
            dashingDir = new Vector2(dirX, 0);
            
            if (jumpsLeft == 0)
            {
                ++jumpsLeft;
            }

            StartCoroutine(StopDashing());
        }

        if (isDashing)
        {
            GetComponent<Rigidbody2D>().velocity = dashingDir.normalized * dashVel;
            return;
        }

        if (isOnGround)
        {
            canDash = true;
        }

        // Move left or right
        Rb.velocity = new Vector2(dirX * moveSpeed, Rb.velocity.y);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && jumpsLeft > 0)
        {
            Rb.velocity = new Vector2(Rb.velocity.x, jumpForce);
            isOnGround = false;
            jumpsLeft -= 1;
        }
        
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {

        MovementState state;
        // Running animation
        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (Rb.velocity.y > .1f) 
        {
            state = MovementState.jumping;
        }
        else if (Rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }
        anim.SetInteger("state", (int)state);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            jumpsLeft = maxJumps;
        }
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashTime);
        trailRenderer.emitting = false;
        isDashing = false;
    }
}
