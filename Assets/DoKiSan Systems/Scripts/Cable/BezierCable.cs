using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BezierCable : MonoBehaviour
{
    public Transform startPoint;       // Начальная точка кабеля
    public Transform endPoint;         // Конечная точка кабеля
    public Transform controlPoint1;    // Первая контрольная точка
    public Transform controlPoint2;    // Вторая контрольная точка
    public int curveSegments = 20;     // Количество сегментов кривой
    public int radialSegments = 8;     // Количество сегментов для круга (трубка)
    public float cableRadius = 0.1f;   // Радиус кабеля

    private Mesh mesh;

    void Start()
    {
        // Создаем пустой меш
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        // Обновляем меш каждый кадр
        GenerateCableMesh();
    }

    private void GenerateCableMesh()
    {
        Vector3[] curvePoints = GenerateBezierCurve();

        // Преобразуем точки кривой в локальные координаты относительно родителя
        for (int i = 0; i < curvePoints.Length; i++)
        {
            curvePoints[i] = transform.InverseTransformPoint(curvePoints[i]);
        }

        Mesh generatedMesh = ExtrudeTube(curvePoints);
        mesh.Clear();
        mesh.vertices = generatedMesh.vertices;
        mesh.triangles = generatedMesh.triangles;
        mesh.normals = generatedMesh.normals;
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

    private Mesh ExtrudeTube(Vector3[] curvePoints)
    {
        int ringVertexCount = radialSegments + 1;
        int vertexCount = (curveSegments + 1) * ringVertexCount + 2; // +2 для двух центральных вершин
        int triangleCount = curveSegments * radialSegments * 6 + radialSegments * 6; // +6 на торцы

        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[triangleCount];
        Vector3[] normals = new Vector3[vertexCount];

        float angleStep = 360f / radialSegments;

        // Центральные вершины для "крышек"
        Vector3 startCapCenter = curvePoints[0];
        Vector3 endCapCenter = curvePoints[curvePoints.Length - 1];

        vertices[vertexCount - 2] = startCapCenter;
        vertices[vertexCount - 1] = endCapCenter;

        normals[vertexCount - 2] = -Vector3.forward; // Нормаль для первой "крышки" смотрит назад
        normals[vertexCount - 1] = Vector3.forward;  // Нормаль для второй "крышки" смотрит вперёд

        // Генерация вершин и нормалей
        for (int i = 0; i <= curveSegments; i++)
        {
            Vector3 center = curvePoints[i];

            Vector3 forward = (i < curveSegments) ? (curvePoints[i + 1] - curvePoints[i]).normalized : (curvePoints[i] - curvePoints[i - 1]).normalized;
            Vector3 up = Vector3.up;
            Vector3 right = Vector3.Cross(up, forward).normalized;
            up = Vector3.Cross(forward, right).normalized;

            for (int j = 0; j <= radialSegments; j++)
            {
                float angle = Mathf.Deg2Rad * j * angleStep;
                Vector3 localPos = right * Mathf.Cos(angle) * cableRadius + up * Mathf.Sin(angle) * cableRadius;

                int vertexIndex = i * ringVertexCount + j;
                vertices[vertexIndex] = center + localPos;
                normals[vertexIndex] = localPos.normalized;
            }
        }

        int tIndex = 0;

        // Боковые стенки
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

        // Крышка начала
        for (int j = 0; j < radialSegments; j++)
        {
            int current = j;
            triangles[tIndex++] = vertexCount - 2; // Центральная вершина крышки
            triangles[tIndex++] = current + 1;
            triangles[tIndex++] = current;
        }

        // Крышка конца
        int capStartIndex = curveSegments * ringVertexCount;
        for (int j = 0; j < radialSegments; j++)
        {
            int current = capStartIndex + j;
            triangles[tIndex++] = vertexCount - 1; // Центральная вершина крышки
            triangles[tIndex++] = current;
            triangles[tIndex++] = current + 1;
        }

        // Создаем и возвращаем меш
        Mesh tubeMesh = new Mesh();
        tubeMesh.vertices = vertices;
        tubeMesh.triangles = triangles;
        tubeMesh.normals = normals;

        return tubeMesh;
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
