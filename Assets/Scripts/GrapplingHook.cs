using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GrapplingHook : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float maxGrappleDistance = 10f;
    public float swingForce = 10f;
    public LayerMask grappleMask;

    private Rigidbody2D rb;
    private DistanceJoint2D joint;
    private Camera cam;
    private Vector2 grapplePoint;
    private bool isGrappling;

    private PlayerController playerController;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        playerController = GetComponent<PlayerController>();

        // создаём DistanceJoint2D, но выключаем по умолчанию
        joint = gameObject.AddComponent<DistanceJoint2D>();
        joint.enabled = false;
        joint.autoConfigureConnectedAnchor = false;
        joint.enableCollision = true;

        if (!lineRenderer)
        {
            GameObject lrObj = new GameObject("GrappleLine");
            lrObj.transform.parent = transform;
            lineRenderer = lrObj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
        }

        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryStartGrapple();

        if (Input.GetMouseButtonUp(0))
            StopGrapple();

        if (isGrappling)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);

            ApplySwingForce();
        }
    }

    void TryStartGrapple()
    {
        Vector2 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorldPos - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir.normalized, maxGrappleDistance, grappleMask);

        if (hit.collider != null)
        {
            // Центр объекта вместо точки попадания
            grapplePoint = hit.collider.bounds.center;

            joint.connectedAnchor = grapplePoint;
            joint.distance = Vector2.Distance(transform.position, grapplePoint);
            joint.enabled = true;

            isGrappling = true;
            lineRenderer.enabled = true;

            if (playerController != null)
                playerController.enabled = false; // отключаем скрипт управления
        }
    }



    public void StopGrapple()
    {
        isGrappling = false;
        joint.enabled = false;
        lineRenderer.enabled = false;

        if (playerController != null)
            playerController.enabled = true; // включаем скрипт управления обратно
    }


void ApplySwingForce()
{
    float horizontalInput = Input.GetAxisRaw("Horizontal");

    Vector2 dirToAnchor = grapplePoint - rb.position;
    Vector2 tangent = Vector2.Perpendicular(dirToAnchor).normalized;

    // Корректируем направление тангенты под ввод
    if (horizontalInput != 0 && Mathf.Sign(horizontalInput) != Mathf.Sign(Vector2.Dot(tangent, Vector2.right)))
    {
        tangent = -tangent;
    }

    // Угол между вектором к якорю и вертикалью
    float angleFromVertical = Vector2.Angle(dirToAnchor, Vector2.down);

    // Затухание: 1 при 90°, 0 при 180°
    float damping = 1f;
    if (angleFromVertical > 90f)
    {
        float t = (angleFromVertical - 90f) / 90f; // от 0 до 1
        damping = Mathf.Pow(1f - t, 2); // быстрое затухание ближе к 180°
    }

    if (Mathf.Abs(horizontalInput) > 0.1f)
    {
        rb.AddForce(tangent * swingForce * damping);
    }
    else
    {
        float swingMomentum = Vector2.Dot(rb.linearVelocity, tangent);
        if (Mathf.Abs(swingMomentum) > 0.1f)
        {
            rb.AddForce(tangent * swingMomentum * 0.1f * damping);
        }
    }
}


}