using UnityEngine;
using System.Collections;

public class LandingCameraShake : MonoBehaviour
{
    public Camera mainCamera;
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.1f;

    public LayerMask groundLayer;       // Слой земли для проверки
    public Transform groundCheckPoint;  // Точка для проверки приземления (например, под ногами персонажа)
    public float groundCheckRadius = 0.2f;

    private bool isGrounded = false;
    private bool wasGrounded = false;

    private Vector3 originalCamPos;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        originalCamPos = mainCamera.transform.localPosition;
    }

    void Update()
    {
        // Проверка есть ли земля под персонажем
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        // Если раньше был в воздухе, а теперь на земле — значит приземлился
        if (!wasGrounded && isGrounded)
        {
            StartCoroutine(ShakeCamera());
        }

        wasGrounded = isGrounded;
    }

    IEnumerator ShakeCamera()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            mainCamera.transform.localPosition = new Vector3(originalCamPos.x + x, originalCamPos.y + y, originalCamPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalCamPos;
    }
}
