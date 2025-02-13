using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableBundle : MonoBehaviour
{
    [Header("Bezier Settings")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private List<Transform> controlPoints; // Контрольные точки основной кривой

    [Header("Cable Settings")]
    [SerializeField] private int curveSegments = 20;
    [SerializeField] private int radialSegments = 8;
    [SerializeField] private float cableRadius = 0.05f;
    [SerializeField] private float bundleSpacing = 0.1f; // Расстояние между кабелями в пучке

    [Header("Material Settings")]
    [SerializeField] private Material cableMaterial;

    private List<GameObject> bundleCables = new List<GameObject>();

    public void GenerateCableBundle(List<List<Vector3>> individualCableCurves)
    {
        if (individualCableCurves.Count == 0) return;

        // 1. Создаем усредненную кривую
        Vector3[] bundleCurve = GenerateAverageCurve(individualCableCurves);

        // 2. Создаем кабели вокруг основной кривой
        int cableCount = individualCableCurves.Count;
        float angleStep = 360f / cableCount;

        for (int i = 0; i < cableCount; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleStep;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * bundleSpacing;

            Vector3[] cableCurve = new Vector3[bundleCurve.Length];
            for (int j = 0; j < bundleCurve.Length; j++)
            {
                cableCurve[j] = bundleCurve[j] + offset;
            }

            CreateCableMesh(cableCurve, $"Cable_{i + 1}");
        }
    }

    private Vector3[] GenerateAverageCurve(List<List<Vector3>> cableCurves)
    {
        int segmentCount = cableCurves[0].Count;
        Vector3[] averageCurve = new Vector3[segmentCount];

        for (int i = 0; i < segmentCount; i++)
        {
            if (i == 0) averageCurve[i] = startPoint.position;
            else if (i == segmentCount - 1) averageCurve[i] = endPoint.position;
            else
            {
                Vector3 sum = Vector3.zero;
                foreach (var curve in cableCurves)
                {
                    sum += curve[i];
                }
                averageCurve[i] = sum / cableCurves.Count;
            }
        }

        return averageCurve;
    }

    private void CreateCableMesh(Vector3[] curvePoints, string name)
    {
        GameObject cable = new GameObject(name);
        cable.transform.SetParent(transform, false);

        MeshFilter meshFilter = cable.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = cable.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = cableMaterial;

        meshFilter.mesh = GenerateTubeMesh(curvePoints);
        bundleCables.Add(cable);
    }

    private Mesh GenerateTubeMesh(Vector3[] curvePoints)
    {
        int ringVertexCount = radialSegments + 1;
        int vertexCount = (curveSegments + 1) * ringVertexCount;
        int triangleCount = curveSegments * radialSegments * 6;

        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[triangleCount];
        Vector3[] normals = new Vector3[vertexCount];

        float angleStep = 360f / radialSegments;

        for (int i = 0; i <= curveSegments; i++)
        {
            Vector3 center = curvePoints[i];

            Vector3 forward = (i < curvePoints.Length - 1)
                ? (curvePoints[i + 1] - curvePoints[i]).normalized
                : (curvePoints[i] - curvePoints[i - 1]).normalized;

            Vector3 up = Vector3.up;
            Vector3 right = Vector3.Cross(up, forward).normalized;
            up = Vector3.Cross(forward, right).normalized;

            for (int j = 0; j <= radialSegments; j++)
            {
                float angle = Mathf.Deg2Rad * j * angleStep;
                Vector3 localPos = right * Mathf.Cos(angle) * cableRadius + up * Mathf.Sin(angle) * cableRadius;
                vertices[i * ringVertexCount + j] = center + localPos;
                normals[i * ringVertexCount + j] = localPos.normalized;
            }
        }

        int tIndex = 0;
        for (int i = 0; i < curveSegments; i++)
        {
            for (int j = 0; j < radialSegments; j++)
            {
                int current = i * ringVertexCount + j;
                int next = current + ringVertexCount;

                triangles[tIndex++] = current;
                triangles[tIndex++] = current + 1;
                triangles[tIndex++] = next;

                triangles[tIndex++] = next;
                triangles[tIndex++] = current + 1;
                triangles[tIndex++] = next + 1;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;

        return mesh;
    }
}
