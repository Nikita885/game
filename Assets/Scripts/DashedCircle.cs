using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DashedCircle : MonoBehaviour
{
    public float radius = 5f;
    public int segments = 100;
    public float dashLength = 0.2f;
    public float gapLength = 0.2f;

    void Start()
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;

        float totalLength = dashLength + gapLength;
        float angleStep = 360f / segments;
        float angle = 0f;

        var points = new System.Collections.Generic.List<Vector3>();

        for (int i = 0; i < segments; i++)
        {
            float t = (i * totalLength) / (2 * Mathf.PI * radius);
            angle = t * 360f;

            float rad = Mathf.Deg2Rad * angle;
            Vector3 start = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;

            rad = Mathf.Deg2Rad * (angle + (dashLength / (2 * Mathf.PI * radius)) * 360f);
            Vector3 end = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;

            points.Add(start);
            points.Add(end);
        }

        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
    }
}
