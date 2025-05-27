using UnityEngine;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;

public class CircleGrapplingSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public Camera mainCamera;
    public Transform parentObject;
    public Transform playerTransform; // Центр персонажа
    public int maxCount = 5;

    public float animationDuration = 0.3f; // Время анимации
    public Vector3 finalScale = new Vector3(0.3868f, 0.3868f, 0.3868f);

    public CinemachineTargetGroup targetGroup; // Ссылка на группу

    // Указываем объект второго игрока
    public GameObject secondPlayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (parentObject.childCount >= maxCount)
            {
                Debug.Log("Достигнут лимит CircleGrappling: " + maxCount);
                return;
            }

            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            StartCoroutine(SpawnWithAnimation(mouseWorldPos));
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.CompareTag("CircleGrapplingsTeg"))
                {
                    RemoveFromTargetGroup(hitObject);
                    Destroy(hitObject);

                    // Отключаем GrapplingHook и включаем PlayerControl у второго игрока
                    if (secondPlayer != null)
                    {
                        var grappling = secondPlayer.GetComponent<GrapplingHook>();

                        if (grappling != null)
                        {
                            grappling.StopGrapple();
                            Debug.Log("Вызван StopGrapple у второго игрока.");
                        }
                    }

                }
            }
        }
    }

    IEnumerator SpawnWithAnimation(Vector3 targetPos)
    {
        GameObject clone = Instantiate(objectToSpawn, playerTransform.position, Quaternion.identity);
        clone.transform.SetParent(parentObject);
        clone.tag = "CircleGrapplingsTeg";

        AddToTargetGroup(clone);

        Vector3 startPos = playerTransform.position;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = finalScale;

        float time = 0f;

        while (time < animationDuration)
        {
            float t = time / animationDuration;
            clone.transform.position = Vector3.Lerp(startPos, targetPos, t);
            clone.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            time += Time.deltaTime;
            yield return null;
        }

        clone.transform.position = targetPos;
        clone.transform.localScale = endScale;
    }

    void AddToTargetGroup(GameObject obj)
    {
        if (targetGroup == null)
        {
            Debug.LogError("CinemachineTargetGroup не назначен!");
            return;
        }

        var targetsList = new List<CinemachineTargetGroup.Target>(targetGroup.m_Targets);
        CinemachineTargetGroup.Target newTarget = new CinemachineTargetGroup.Target
        {
            target = obj.transform,
            weight = 1f,
            radius = 0.5f
        };
        targetsList.Add(newTarget);
        targetGroup.m_Targets = targetsList.ToArray();
    }

    void RemoveFromTargetGroup(GameObject obj)
    {
        if (targetGroup == null) return;

        var targetsList = new List<CinemachineTargetGroup.Target>(targetGroup.m_Targets);
        targetsList.RemoveAll(t => t.target == obj.transform);
        targetGroup.m_Targets = targetsList.ToArray();
    }
}
