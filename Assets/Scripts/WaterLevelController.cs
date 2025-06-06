using UnityEngine;

public enum CharacterType { Light, Heavy }

public class WaterLevelController : MonoBehaviour
{
    [Header("Общие настройки")]
    public CharacterType characterType;
    public float maxSwimSpeed = 4f;

    [Header("Настройки легкого персонажа")]
    public float swimForce = 5f;
    public float swimStopDistance = 0.3f;
    
    [Header("Настройки тяжелого персонажа")]
    public float heavyUpwardForce = 15f;    // Сила рывка вверх
    public float heavyHorizontalSpeed = 3f; // Скорость движения вбок
    public float heavySinkSpeed = 2f;      // Скорость погружения
    public float movementDrag = 3f;        // Сопротивление в воде
    
    private Rigidbody2D rb;
    private bool isInWater = false;
    private Vector2 swimTarget;
    private float horizontalInput;
    private bool isHeavyJumping;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isInWater) return;

        if (characterType == CharacterType.Light)
        {
            // Управление для легкого персонажа (как было)
            if (Input.GetMouseButton(0))
            {
                swimTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Debug.DrawLine(transform.position, swimTarget, Color.green);
            }
        }
        else
        {
            // Управление для тяжелого персонажа
            horizontalInput = Input.GetAxisRaw("Horizontal");
            
            // Рывок вверх по пробелу
            if (Input.GetKeyDown(KeyCode.Space))
            {
                HeavyUpwardBoost();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isInWater) return;

        if (characterType == CharacterType.Light)
        {
            // Физика для легкого персонажа
            rb.gravityScale = 0.3f;
            rb.linearDamping = 1f;

            if (Input.GetMouseButton(0))
            {
                Vector2 direction = (swimTarget - (Vector2)transform.position).normalized;
                float distance = Vector2.Distance(transform.position, swimTarget);
                
                if (distance > swimStopDistance)
                {
                    float verticalBoost = Mathf.Abs(direction.y) > 0.7f ? 1.5f : 1f;
                    Vector2 force = direction * swimForce * verticalBoost;
                    rb.AddForce(force);
                }
            }
        }
        else
        {
            // Физика для тяжелого персонажа
            rb.linearDamping = movementDrag;
            
            // Движение вбок (независимо от вертикали)
            float targetXVelocity = horizontalInput * heavyHorizontalSpeed;
            float newX = Mathf.Lerp(rb.linearVelocity.x, targetXVelocity, 0.1f);
            
            // Постоянное погружение (если не было прыжка)
            float sinkVelocity = isHeavyJumping ? rb.linearVelocity.y : -heavySinkSpeed;
            
            rb.linearVelocity = new Vector2(newX, sinkVelocity);
            
            // Ограничение скорости
            if (Mathf.Abs(rb.linearVelocity.x) > heavyHorizontalSpeed)
            {
                rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * heavyHorizontalSpeed, rb.linearVelocity.y);
            }
        }
    }

    private void HeavyUpwardBoost()
    {
        // Резкий рывок вверх
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * heavyUpwardForce, ForceMode2D.Impulse);
        isHeavyJumping = true;
        Invoke("ResetHeavyJump", 0.5f);
    }

    private void ResetHeavyJump()
    {
        isHeavyJumping = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = true;
            rb.gravityScale = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = false;
            rb.gravityScale = 1f;
            isHeavyJumping = false;
        }
    }
}