using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BezierPhysicsCable : MonoBehaviour
{
    public Transform startPoint;       // Начальная точка кабеля
    public Transform endPoint;         // Конечная точка кабеля
    public Transform[] controlPoints;  // Массив контрольных точек
    public int curveSegments = 20;     // Количество сегментов кривой
    public int radialSegments = 8;     // Количество сегментов трубы
    public float cableRadius = 0.1f;   // Радиус кабеля
    public bool invertNormals = false; // Инвертирование нормалей
    public bool enableOrbitalAdjustment = true; // Включение орбитального смещения контрольных точек

    private Mesh mesh;
    private Quaternion lastStartRotation;
    private Quaternion lastEndRotation;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        lastStartRotation = startPoint.rotation;
        lastEndRotation = endPoint.rotation;
    }

    void Update()
    {
        HandlePointRotation();
        GenerateCableMesh();
    }

    private void GenerateCableMesh()
    {
        Vector3[] curvePoints = GenerateBezierCurve();

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
        Vector3[] bezierPoints = new Vector3[2 + controlPoints.Length];
        bezierPoints[0] = startPoint.position;
        bezierPoints[bezierPoints.Length - 1] = endPoint.position;

        for (int i = 0; i < controlPoints.Length; i++)
        {
            bezierPoints[i + 1] = controlPoints[i].position;
        }

        for (int i = 0; i <= curveSegments; i++)
        {
            float t = i / (float)curveSegments;
            points[i] = CalculateBezierPoint(t, bezierPoints);
        }

        return points;
    }

    private void HandlePointRotation()
    {
        if (!enableOrbitalAdjustment) return; // Если галочка снята, смещение отключено

        if (startPoint.rotation != lastStartRotation)
        {
            RotateControlPointsAround(startPoint, 0, lastStartRotation);
            lastStartRotation = startPoint.rotation;
        }

        if (endPoint.rotation != lastEndRotation)
        {
            RotateControlPointsAround(endPoint, controlPoints.Length - 1, lastEndRotation);
            lastEndRotation = endPoint.rotation;
        }
    }

    private void RotateControlPointsAround(Transform pivot, int controlIndex, Quaternion lastRotation)
    {
        if (controlPoints.Length == 0 || controlIndex < 0 || controlIndex >= controlPoints.Length) return;

        Transform controlPoint = controlPoints[controlIndex];
        Quaternion deltaRotation = pivot.rotation * Quaternion.Inverse(lastRotation);
        Vector3 direction = controlPoint.position - pivot.position;
        float distance = direction.magnitude;

        controlPoint.position = pivot.position + (deltaRotation * direction.normalized * distance);

        for (int i = controlIndex + 1; i < controlPoints.Length; i++)
        {
            Vector3 nextDirection = controlPoints[i].position - controlPoint.position;
            float nextDistance = nextDirection.magnitude;
            controlPoints[i].position = controlPoint.position + (deltaRotation * nextDirection.normalized * nextDistance);
        }
    }

    private Mesh ExtrudeTube(Vector3[] curvePoints)
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

            Vector3 forward = (i < curveSegments) ? (curvePoints[i + 1] - curvePoints[i]).normalized : (curvePoints[i] - curvePoints[i - 1]).normalized;
            Vector3 up = Vector3.up;
            Vector3 right = Vector3.Cross(up, forward).normalized;
            up = Vector3.Cross(forward, right).normalized;

            if (invertNormals)
            {
                right = -right;
                up = -up;
            }

            for (int j = 0; j <= radialSegments; j++)
            {
                float angle = Mathf.Deg2Rad * j * angleStep;
                Vector3 localPos = right * Mathf.Cos(angle) * cableRadius + up * Mathf.Sin(angle) * cableRadius;
                vertices[i * ringVertexCount + j] = center + localPos;
                normals[i * ringVertexCount + j] = invertNormals ? -localPos.normalized : localPos.normalized;
            }
        }

        int tIndex = 0;
        for (int i = 0; i < curveSegments; i++)
        {
            for (int j = 0; j < radialSegments; j++)
            {
                int current = i * ringVertexCount + j;
                int next = current + ringVertexCount;

                if (invertNormals)
                {
                    triangles[tIndex++] = current;
                    triangles[tIndex++] = next;
                    triangles[tIndex++] = current + 1;

                    triangles[tIndex++] = next;
                    triangles[tIndex++] = next + 1;
                    triangles[tIndex++] = current + 1;
                }
                else
                {
                    triangles[tIndex++] = current;
                    triangles[tIndex++] = current + 1;
                    triangles[tIndex++] = next;

                    triangles[tIndex++] = next;
                    triangles[tIndex++] = current + 1;
                    triangles[tIndex++] = next + 1;
                    
                }
            }
        }

        Mesh tubeMesh = new Mesh();
        tubeMesh.vertices = vertices;
        tubeMesh.triangles = triangles;
        tubeMesh.normals = normals;
        return tubeMesh;
    }

    private Vector3 CalculateBezierPoint(float t, Vector3[] points)
    {
        Vector3 result = Vector3.zero;
        int n = points.Length - 1;
        for (int i = 0; i <= n; i++)
        {
            float binomial = BinomialCoefficient(n, i);
            float powT = Mathf.Pow(t, i);
            float powU = Mathf.Pow(1 - t, n - i);
            result += binomial * powT * powU * points[i];
        }
        return result;
    }

    private int BinomialCoefficient(int n, int k)
    {
        if (k > n) return 0;
        if (k == 0 || k == n) return 1;

        int result = 1;
        for (int i = 1; i <= k; i++)
        {
            result *= n--;
            result /= i;
        }
        return result;
    }

    private void OnDisable()
    {
        if (mesh != null)
        {
            Destroy(mesh);
            mesh = null;
        }
    }
}
