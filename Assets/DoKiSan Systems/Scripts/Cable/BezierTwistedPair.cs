using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BezierTwistedPair : MonoBehaviour
{
    [Header("Bezier Settings")]
    public Transform startPoint;       // ��������� ����� ������
    public Transform endPoint;         // �������� ����� ������
    public Transform controlPoint1;    // ������ ����������� �����
    public Transform controlPoint2;    // ������ ����������� �����

    [Header("Cable Settings")]
    public int curveSegments = 20;     // ���������� ��������� ������
    public int radialSegments = 8;     // ���������� ��������� ��� ����� (������)
    public float cableRadius = 0.05f;  // ������ ������ �������

    [Header("Twist Settings")]
    public bool isTwisted = true;      // ���� ������������
    public float twistIntensity = 2.0f; // ���������� ������

    [Header("Material wires")]
    public Material wire1Mat;
    public Material wire2Mat;
    // ��������� ���� ��� ���� ��������
    private MeshFilter wire1MeshFilter;
    private MeshFilter wire2MeshFilter;

    private void Start()
    {
        // ������ ��� ������� ��� ���� ��������
        GameObject wire1 = new GameObject("Wire1");
        wire1.transform.SetParent(transform, false);
        wire1.transform.localPosition = Vector3.zero; // ��������� �������
        wire1MeshFilter = wire1.AddComponent<MeshFilter>();
        wire1.AddComponent<MeshRenderer>().sharedMaterial = wire1Mat;

        GameObject wire2 = new GameObject("Wire2");
        wire2.transform.SetParent(transform, false);
        wire2.transform.localPosition = Vector3.zero; // ��������� �������
        wire2MeshFilter = wire2.AddComponent<MeshFilter>();
        wire2.AddComponent<MeshRenderer>().sharedMaterial = wire2Mat;

        UpdateCables();
    }

    private void Update()
    {
        UpdateCables();
    }

    private void UpdateCables()
    {
        // ��������� ������� ������
        Vector3[] baseCurve = GenerateBezierCurve();

        // ����������� ����� � ��������� ������������
        for (int i = 0; i < baseCurve.Length; i++)
        {
            baseCurve[i] = transform.InverseTransformPoint(baseCurve[i]);
        }

        // ��������� ��������� ������
        Vector3[] curve1 = new Vector3[baseCurve.Length];
        Vector3[] curve2 = new Vector3[baseCurve.Length];

        for (int i = 0; i < baseCurve.Length; i++)
        {
            float angle = isTwisted ? Mathf.PI * 2 * twistIntensity * (i / (float)curveSegments) : 0;

            Vector3 forward = (i < baseCurve.Length - 1)
                ? (baseCurve[i + 1] - baseCurve[i]).normalized
                : (baseCurve[i] - baseCurve[i - 1]).normalized;

            // ��������� ���
            Vector3 up = Vector3.up;
            Vector3 right = Vector3.Cross(up, forward).normalized;
            up = Vector3.Cross(forward, right).normalized;

            // �������� ��� ���� �������
            Vector3 offset1 = right * Mathf.Cos(angle) * cableRadius + up * Mathf.Sin(angle) * cableRadius;
            Vector3 offset2 = right * Mathf.Cos(angle + Mathf.PI) * cableRadius + up * Mathf.Sin(angle + Mathf.PI) * cableRadius;

            curve1[i] = baseCurve[i] + offset1;
            curve2[i] = baseCurve[i] + offset2;
        }

        // ��������� ���� ��� ���� ������
        wire1MeshFilter.mesh = GenerateTubeMesh(curve1);
        wire2MeshFilter.mesh = GenerateTubeMesh(curve2);
    }

    private Vector3[] GenerateBezierCurve()
    {
        Vector3[] points = new Vector3[curveSegments + 1];

        for (int i = 0; i <= curveSegments; i++)
        {
            float t = i / (float)curveSegments;
            points[i] = CalculateBezierPoint(t, startPoint.position, controlPoint1.position, controlPoint2.position, endPoint.position);
        }

        return points;
    }

    private Mesh GenerateTubeMesh(Vector3[] curvePoints)
    {
        int ringVertexCount = radialSegments + 1;
        int vertexCount = (curveSegments + 1) * ringVertexCount + 2; // +2 ��� ����������� ������
        int triangleCount = curveSegments * radialSegments * 6 + radialSegments * 6; // +6 �� ������

        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[triangleCount];
        Vector3[] normals = new Vector3[vertexCount];

        float angleStep = 360f / radialSegments;

        // ����������� ������� ��� "������"
        Vector3 startCapCenter = curvePoints[0];
        Vector3 endCapCenter = curvePoints[curvePoints.Length - 1];

        vertices[vertexCount - 2] = startCapCenter;
        vertices[vertexCount - 1] = endCapCenter;

        normals[vertexCount - 2] = -Vector3.forward; // ������� ��� ������ "������" ������� �����
        normals[vertexCount - 1] = Vector3.forward;  // ������� ��� ������ "������" ������� �����

        // ��������� ������ � ��������
        for (int i = 0; i <= curveSegments; i++)
        {
            Vector3 center = curvePoints[i];

            Vector3 forward = (i < curvePoints.Length - 1) ? (curvePoints[i + 1] - curvePoints[i]).normalized : (curvePoints[i] - curvePoints[i - 1]).normalized;
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

        // ������� ������
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

        // ������ ������
        for (int j = 0; j < radialSegments; j++)
        {
            int current = j;
            triangles[tIndex++] = vertexCount - 2; // ����������� ������� ������
            triangles[tIndex++] = current + 1;
            triangles[tIndex++] = current;
        }

        // ������ �����
        int capStartIndex = curveSegments * ringVertexCount;
        for (int j = 0; j < radialSegments; j++)
        {
            int current = capStartIndex + j;
            triangles[tIndex++] = vertexCount - 1; // ����������� ������� ������
            triangles[tIndex++] = current;
            triangles[tIndex++] = current + 1;
        }

        // ������� � ���������� ���
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;

        return mesh;
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        return uuu * p0 + 3 * uu * t * p1 + 3 * u * tt * p2 + ttt * p3;
    }
}
