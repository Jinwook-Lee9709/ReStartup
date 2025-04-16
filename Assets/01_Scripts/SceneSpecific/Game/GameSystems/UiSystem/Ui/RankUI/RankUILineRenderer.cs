using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RankUILineRenderer : Graphic
{
    public List<RectTransform> controlPoints = new List<RectTransform>();
    [Range(2, 100)]
    public int segmentsPerCurve = 20;

    public float thickness = 5f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (controlPoints.Count < 2)
            return;

        List<Vector2> worldPoints = new List<Vector2>();
        foreach (var pt in controlPoints)
        {
            worldPoints.Add(WorldToLocal(pt.position));
        }

        List<Vector2> curvePoints = GenerateCatmullRomPoints(worldPoints, segmentsPerCurve);

        for (int i = 0; i < curvePoints.Count - 1; i++)
        {
            AddLineSegment(vh, curvePoints[i], curvePoints[i + 1], thickness);
        }
    }

    private Vector2 WorldToLocal(Vector3 worldPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, worldPos, canvas.worldCamera, out Vector2 localPos);
        return localPos;
    }

    private void AddLineSegment(VertexHelper vh, Vector2 start, Vector2 end, float width)
    {
        Vector2 direction = (end - start).normalized;
        Vector2 normal = new Vector2(-direction.y, direction.x) * (width / 2f);

        int idx = vh.currentVertCount;

        vh.AddVert(start - normal, color, Vector2.zero);
        vh.AddVert(start + normal, color, Vector2.zero);
        vh.AddVert(end + normal, color, Vector2.zero);
        vh.AddVert(end - normal, color, Vector2.zero);

        vh.AddTriangle(idx, idx + 1, idx + 2);
        vh.AddTriangle(idx + 2, idx + 3, idx);
    }

    private List<Vector2> GenerateCatmullRomPoints(List<Vector2> points, int segments)
    {
        List<Vector2> result = new List<Vector2>();

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 p0 = i == 0 ? points[i] : points[i - 1];
            Vector2 p1 = points[i];
            Vector2 p2 = points[i + 1];
            Vector2 p3 = (i + 2 < points.Count) ? points[i + 2] : p2;

            for (int j = 0; j < segments; j++)
            {
                float t = j / (float)segments;
                result.Add(CatmullRom(p0, p1, p2, p3, t));
            }
        }

        result.Add(points[points.Count - 1]); // 마지막 점 추가
        return result;
    }

    private Vector2 CatmullRom(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }
}