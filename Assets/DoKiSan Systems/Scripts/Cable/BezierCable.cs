using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BezierCable : MonoBehaviour
{
    public Transform startPoint;       // ��������� ����� ������
    public Transform endPoint;         // �������� ����� ������
    public Transform[] controlPoints;  // ������ ����������� �����
    public int curveSegments = 20;     // ���������� ��������� ������
    public int radialSegments = 8;     // ���������� ��������� �����
    public float cableRadius = 0.1f;   // ������ ������
    public bool invertNormals = false; // �������������� ��������
    public bool enableOrbitalAdjustment = true; // ��������� ������������ �������� ����������� �����

    private Mesh mesh;
    private Quaternion lastStartRotation;
    private Quaternion lastEndRotation;


    [Header("Generate Interaction point")]
    [SerializeField] GameObject interactivePointPrefab; // ������ ������������� �����
    [SerializeField] Transform interactivePointParent;
    [SerializeField] int interactivePointsCount = 4;
    [SerializeField] bool isDebug;
    [SerializeField]private bool isDraggingInteractivePoint;
    [SerializeField] CablePointBezier cablePointBezier;
    private List<InteractivePointHandler> interactivePoints = new List<InteractivePointHandler>();
    private Vector3[] bezierCurvePoints;
    private Quaternion interactivPointRotation;
    [SerializeField] bool isInteractivePointBezier = true;
    private bool _isDisabled = false;
    
    private void OnEnable()
    {
        if (!_isDisabled) 
            return;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        lastStartRotation = startPoint.rotation;
        lastEndRotation = endPoint.rotation;

        interactivePointParent = transform;
        interactivPointRotation = transform.rotation;
        GenerateCableMesh();
        GenerateInteractivePoints();
    }

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        lastStartRotation = startPoint.rotation;
        lastEndRotation = endPoint.rotation;

        interactivePointParent = transform;
        interactivPointRotation = transform.rotation;
        GenerateCableMesh();
        GenerateInteractivePoints();
    }

    void Update()
    {
        HandlePointRotation();
        GenerateCableMesh();
        UpdateInteractivePointsPositions();
    }

    private void GenerateCableMesh()
    {
        bezierCurvePoints = GenerateBezierCurve();
        for (int i = 0; i < bezierCurvePoints.Length; i++)
        {
            bezierCurvePoints[i] = transform.InverseTransformPoint(bezierCurvePoints[i]);
        }

        Mesh generatedMesh = ExtrudeTube(bezierCurvePoints);
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

    private void GenerateInteractivePoints()
    {
        if (!isInteractivePointBezier)
            return;
        //// ������� ������ �����
        //foreach (var point in interactivePoints)
        //{
        //    if (point != null)
        //        Destroy(point.gameObject);
        //}
        interactivePoints.Clear();

        // ������ ����� ����� ����� ������
        for (int i = 0; i < interactivePointsCount; i++)
        {
            float t = i / (float)(interactivePointsCount - 1);
            Vector3 positionOnCurve = CalculateBezierPoint(t, GenerateBezierCurve());

            GameObject newPoint = Instantiate(interactivePointPrefab, positionOnCurve, interactivPointRotation, interactivePointParent);
            InteractivePointHandler handler = newPoint.GetComponent<InteractivePointHandler>();
            handler.Initialize(this, i, positionOnCurve);
            interactivePoints.Add(handler);
            cablePointBezier.FillingList(handler);
        }
    }

    private void UpdateInteractivePointsPositions()
    {
        if (isDraggingInteractivePoint) return; // ������� ����������, ���� ���������� ��������������

        for (int i = 0; i < interactivePoints.Count; i++)
        {
            Vector3 positionOnCurve = CalculateBezierPoint(i / (float)(interactivePointsCount - 1), GenerateBezierCurve());
            interactivePoints[i].UpdatePositionOnCurve(positionOnCurve);
        }
    }

    public void SetDraggingInteractivePoint(bool isDragging)
    {
        isDraggingInteractivePoint = isDragging;
    }

    public void UpdateControlPoint(int index, Vector3 offset)
    {
        if (index >= 0 && index < controlPoints.Length)
        {
            controlPoints[index].position = offset;
        }
    }

    private void HandlePointRotation()
    {
        if (!enableOrbitalAdjustment) return; // ���� ������� �����, �������� ���������

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
        int triangleCount = curveSegments * radialSegments * 6 + radialSegments * 6; // ���� ������������� ������

        Vector3[] vertices = new Vector3[vertexCount + 2]; // ��������� 2 ������� ��� ������� ������
        int[] triangles = new int[triangleCount];
        Vector3[] normals = new Vector3[vertexCount + 2];
        Vector2[] uvs = new Vector2[vertexCount + 2]; // ������ ��� UV-���������

        float angleStep = 360f / radialSegments;
        float totalLength = CalculateCurveLength(curvePoints);

        // ��������� ������, �������� � UV ��� ������
        float currentLength = 0f;
        for (int i = 0; i <= curveSegments; i++)
        {
            Vector3 center = curvePoints[i];

            // ��������� ������� �����
            if (i > 0)
            {
                currentLength += Vector3.Distance(curvePoints[i - 1], curvePoints[i]);
            }

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

                // UV-����������: U - ����� ����� ������ (����������� �� totalLength), V - ����� ����������
                float u = currentLength / totalLength; // ���� ���������� �����
                float v = (float)j / radialSegments;   // ���� ������ ����������
                uvs[i * ringVertexCount + j] = new Vector2(u, v);
            }
        }

        // ������ ������
        vertices[vertexCount] = curvePoints[0]; // ������ ������
        vertices[vertexCount + 1] = curvePoints[curvePoints.Length - 1]; // ������ ������
        normals[vertexCount] = invertNormals ? Vector3.back : Vector3.forward;
        normals[vertexCount + 1] = invertNormals ? Vector3.forward : Vector3.back;

        uvs[vertexCount] = new Vector2(0, 0); // UV ��� ������ ������
        uvs[vertexCount + 1] = new Vector2(1, 0); // UV ��� ������ ������

        // ��������� ������������� ��� ������
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

        // ������������ ��� ������ � ������
        for (int j = 0; j < radialSegments; j++)
        {
            if (invertNormals)
            {
                triangles[tIndex++] = vertexCount; // ����������� ������� ������
                triangles[tIndex++] = j + 1;
                triangles[tIndex++] = j;
            }
            else
            {
                triangles[tIndex++] = vertexCount; // ����������� ������� ������
                triangles[tIndex++] = j;
                triangles[tIndex++] = j + 1;
            }
        }

        // ������������ ��� ������ � �����
        int endStartIndex = (curveSegments * ringVertexCount);
        for (int j = 0; j < radialSegments; j++)
        {
            if (invertNormals)
            {
                triangles[tIndex++] = vertexCount + 1; // ����������� ������� �����
                triangles[tIndex++] = endStartIndex + j;
                triangles[tIndex++] = endStartIndex + j + 1;
            }
            else
            {
                triangles[tIndex++] = vertexCount + 1; // ����������� ������� �����
                triangles[tIndex++] = endStartIndex + j + 1;
                triangles[tIndex++] = endStartIndex + j;
            }
        }

        Mesh tubeMesh = new Mesh();
        tubeMesh.vertices = vertices;
        tubeMesh.triangles = triangles;
        tubeMesh.normals = normals;
        tubeMesh.uv = uvs; // ����������� UV-����������

        //for (int i = 0; i < uvs.Length; i++)
        //{
        //    Debug.Log($"UV[{i}] = {uvs[i]}");
        //}

        return tubeMesh;
    }

    private float CalculateCurveLength(Vector3[] curvePoints)
    {
        float length = 0f;
        for (int i = 1; i < curvePoints.Length; i++)
        {
            length += Vector3.Distance(curvePoints[i - 1], curvePoints[i]);
        }
        return length;
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
        _isDisabled = true;
    }
    //public Transform startPoint;       // ��������� ����� ������
    //public Transform endPoint;         // �������� ����� ������
    //public Transform controlPoint1;    // ������ ����������� �����
    //public Transform controlPoint2;    // ������ ����������� �����
    //public int curveSegments = 20;     // ���������� ��������� ������
    //public int radialSegments = 8;     // ���������� ��������� ��� ����� (������)
    //public float cableRadius = 0.1f;   // ������ ������

    //private Mesh mesh;

    //void Start()
    //{
    //    // ������� ������ ���
    //    mesh = new Mesh();
    //    GetComponent<MeshFilter>().mesh = mesh;
    //}

    //void Update()
    //{
    //    // ��������� ��� ������ ����
    //    GenerateCableMesh();
    //}

    //private void GenerateCableMesh()
    //{
    //    Vector3[] curvePoints = GenerateBezierCurve();

    //    // ����������� ����� ������ � ��������� ���������� ������������ ��������
    //    for (int i = 0; i < curvePoints.Length; i++)
    //    {
    //        curvePoints[i] = transform.InverseTransformPoint(curvePoints[i]);
    //    }

    //    Mesh generatedMesh = ExtrudeTube(curvePoints);
    //    mesh.Clear();
    //    mesh.vertices = generatedMesh.vertices;
    //    mesh.triangles = generatedMesh.triangles;
    //    mesh.normals = generatedMesh.normals;
    //}

    //private Vector3[] GenerateBezierCurve()
    //{
    //    Vector3[] points = new Vector3[curveSegments + 1];

    //    for (int i = 0; i <= curveSegments; i++)
    //    {
    //        float t = i / (float)curveSegments;
    //        points[i] = CalculateBezierPoint(t, startPoint.position, controlPoint1.position, controlPoint2.position, endPoint.position);
    //    }

    //    return points;
    //}

    //private Mesh ExtrudeTube(Vector3[] curvePoints)
    //{
    //    int ringVertexCount = radialSegments + 1;
    //    int vertexCount = (curveSegments + 1) * ringVertexCount + 2; // +2 ��� ���� ����������� ������
    //    int triangleCount = curveSegments * radialSegments * 6 + radialSegments * 6; // +6 �� �����

    //    Vector3[] vertices = new Vector3[vertexCount];
    //    int[] triangles = new int[triangleCount];
    //    Vector3[] normals = new Vector3[vertexCount];

    //    float angleStep = 360f / radialSegments;

    //    // ����������� ������� ��� "������"
    //    Vector3 startCapCenter = curvePoints[0];
    //    Vector3 endCapCenter = curvePoints[curvePoints.Length - 1];

    //    vertices[vertexCount - 2] = startCapCenter;
    //    vertices[vertexCount - 1] = endCapCenter;

    //    normals[vertexCount - 2] = -Vector3.forward; // ������� ��� ������ "������" ������� �����
    //    normals[vertexCount - 1] = Vector3.forward;  // ������� ��� ������ "������" ������� �����

    //    // ��������� ������ � ��������
    //    for (int i = 0; i <= curveSegments; i++)
    //    {
    //        Vector3 center = curvePoints[i];

    //        Vector3 forward = (i < curveSegments) ? (curvePoints[i + 1] - curvePoints[i]).normalized : (curvePoints[i] - curvePoints[i - 1]).normalized;
    //        Vector3 up = Vector3.up;
    //        Vector3 right = Vector3.Cross(up, forward).normalized;
    //        up = Vector3.Cross(forward, right).normalized;

    //        for (int j = 0; j <= radialSegments; j++)
    //        {
    //            float angle = Mathf.Deg2Rad * j * angleStep;
    //            Vector3 localPos = right * Mathf.Cos(angle) * cableRadius + up * Mathf.Sin(angle) * cableRadius;

    //            int vertexIndex = i * ringVertexCount + j;
    //            vertices[vertexIndex] = center + localPos;
    //            normals[vertexIndex] = localPos.normalized;
    //        }
    //    }

    //    int tIndex = 0;

    //    // ������� ������
    //    for (int i = 0; i < curveSegments; i++)
    //    {
    //        for (int j = 0; j < radialSegments; j++)
    //        {
    //            int current = i * ringVertexCount + j;
    //            int next = current + ringVertexCount;

    //            triangles[tIndex++] = current;
    //            triangles[tIndex++] = current + 1;
    //            triangles[tIndex++] = next;

    //            triangles[tIndex++] = next;
    //            triangles[tIndex++] = current + 1;
    //            triangles[tIndex++] = next + 1;
    //        }
    //    }

    //    // ������ ������
    //    for (int j = 0; j < radialSegments; j++)
    //    {
    //        int current = j;
    //        triangles[tIndex++] = vertexCount - 2; // ����������� ������� ������
    //        triangles[tIndex++] = current + 1;
    //        triangles[tIndex++] = current;
    //    }

    //    // ������ �����
    //    int capStartIndex = curveSegments * ringVertexCount;
    //    for (int j = 0; j < radialSegments; j++)
    //    {
    //        int current = capStartIndex + j;
    //        triangles[tIndex++] = vertexCount - 1; // ����������� ������� ������
    //        triangles[tIndex++] = current;
    //        triangles[tIndex++] = current + 1;
    //    }

    //    // ������� � ���������� ���
    //    Mesh tubeMesh = new Mesh();
    //    tubeMesh.vertices = vertices;
    //    tubeMesh.triangles = triangles;
    //    tubeMesh.normals = normals;

    //    return tubeMesh;
    //}

    //private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    //{
    //    float u = 1 - t;
    //    float tt = t * t;
    //    float uu = u * u;
    //    float uuu = uu * u;
    //    float ttt = tt * t;

    //    return uuu * p0 + 3 * uu * t * p1 + 3 * u * tt * p2 + ttt * p3;
    //}
}
