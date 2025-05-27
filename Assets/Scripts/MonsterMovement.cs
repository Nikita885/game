using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    public float speed = 2f;
    public Transform wallCheck;
    public Transform groundCheck;
    public float checkRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool movingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rb.linearVelocity = new Vector2((movingRight ? 1 : -1) * speed, rb.linearVelocity.y);

        // Проверка стены
        bool hittingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);

        // Проверка пола
        bool groundAhead = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (hittingWall || !groundAhead)
        {
            Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // отражаем по X
        transform.localScale = localScale;
    }

    private void OnDrawGizmosSelected()
    {
        // Визуализация точек проверки
        if (wallCheck != null)
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}
