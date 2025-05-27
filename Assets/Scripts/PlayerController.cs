using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Controls { mobile, pc }

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool useLargeScale = true;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float doubleJumpForce = 8f;
    public LayerMask groundLayer;
    public Transform groundCheck;

    private Rigidbody2D rb;
    private bool isGroundedBool = false;
    private bool canDoubleJump = false;

    public Controls controlmode;

    private float moveX;
    public bool isPaused = false;

    public ParticleSystem footsteps;
    private ParticleSystem.EmissionModule footEmissions;

    public ParticleSystem ImpactEffect;
    private bool wasonGround;

    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    private float lastDirection = 1f;

    [Header("Key Bindings")]
    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode shootKey = KeyCode.Mouse0;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        footEmissions = footsteps.emission;

        if (controlmode == Controls.mobile)
        {
            UIManager.instance.EnableMobileControls();
        }
    }

    private void Update()
    {
        isGroundedBool = IsGrounded();

        if (controlmode == Controls.pc)
        {
            moveX = 0f;
            if (Input.GetKey(moveLeftKey)) moveX = -1f;
            if (Input.GetKey(moveRightKey)) moveX = 1f;
        }

        if (isGroundedBool)
        {
            canDoubleJump = true;
            if (Input.GetKeyDown(jumpKey))
            {
                Jump(jumpForce);
            }
        }
        else
        {
            if (canDoubleJump && Input.GetKeyDown(jumpKey))
            {
                Jump(doubleJumpForce);
                canDoubleJump = false;
            }
        }

        if (!isPaused)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 lookDirection = mousePosition - transform.position;

            if (controlmode == Controls.pc && Input.GetKeyDown(shootKey) && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }

        // Обработка шагов
        if (moveX != 0 && isGroundedBool)
        {
            footEmissions.rateOverTime = 35f;
        }
        else
        {
            footEmissions.rateOverTime = 0f;
        }

        if (!wasonGround && isGroundedBool)
        {
            ImpactEffect.gameObject.SetActive(true);
            ImpactEffect.Stop();
            ImpactEffect.transform.position = new Vector2(footsteps.transform.position.x, footsteps.transform.position.y - 0.2f);
            ImpactEffect.Play();
        }

        wasonGround = isGroundedBool;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);

        if (moveX != 0 && moveX != lastDirection)
        {
            FlipSprite(moveX);
            lastDirection = moveX;
        }
    }

    private void Jump(float jumpForce)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private bool IsGrounded()
    {
        float rayLength = 0.25f;
        Vector2 rayOrigin = new Vector2(groundCheck.transform.position.x, groundCheck.transform.position.y - 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, groundLayer);
        return hit.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "killzone")
        {
            GameManager.instance.Death();
        }
    }

    // --- Mobile Controls ---
    public void MobileMove(float value)
    {
        moveX = value;
    }

    public void MobileJump()
    {
        if (isGroundedBool)
        {
            Jump(jumpForce);
        }
        else
        {
            if (canDoubleJump)
            {
                Jump(doubleJumpForce);
                canDoubleJump = false;
            }
        }
    }

    public void Shoot()
    {
        // Раскомментируй и настрой при необходимости
        // GameObject fireBall = Instantiate(projectile, firePoint.position, Quaternion.identity);
        // fireBall.GetComponent<Rigidbody2D>().AddForce(firePoint.right * 500f);
    }

    public void MobileShoot()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void FlipSprite(float direction)
    {
        if (direction > 0)
        {
            transform.localScale = useLargeScale ? new Vector3(3, 2, 1) : new Vector3(1, 1, 1);
        }
        else if (direction < 0)
        {
            transform.localScale = useLargeScale ? new Vector3(-3, 2, 1) : new Vector3(-1, 1, 1);
        }
    }
}
