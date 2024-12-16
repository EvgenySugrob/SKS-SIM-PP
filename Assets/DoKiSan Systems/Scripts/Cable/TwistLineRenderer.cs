using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwistLineRenderer : MonoBehaviour
{
    public Transform whitePart;  // Белый провод
    public Transform greenPart;  // Зелёный провод

    public float twistLength = 2f;     // Длина витой пары
    public float twistRadius = 0.02f;  // Радиус скручивания
    public int twistTurns = 5;         // Количество витков
    public bool isTwisted = true;      // Состояние скрученности

    private Vector3[] whiteStartPositions;  // Начальные позиции
    private Vector3[] greenStartPositions;

    private bool isDragging = false;  // Перемещение провода
    private Transform draggedWire;

    void Start()
    {
        // Запоминаем стартовые позиции для "раскручивания"
        whiteStartPositions = GenerateTwist(whitePart, 0);
        greenStartPositions = GenerateTwist(greenPart, Mathf.PI);

        ApplyTwist(); // Скручиваем провода при старте
    }

    void Update()
    {
        // ЛКМ для перемещения части провода
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == whitePart || hit.transform == greenPart)
                {
                    draggedWire = hit.transform;
                    isDragging = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            draggedWire = null;
        }

        if (isDragging && draggedWire != null)
        {
            MoveWire(draggedWire);
        }
    }

    void ApplyTwist()
    {
        if (isTwisted)
        {
            GenerateTwist(whitePart, 0);
            GenerateTwist(greenPart, Mathf.PI);
        }
        else
        {
            UnTwist();
        }
    }

    Vector3[] GenerateTwist(Transform wire, float offsetAngle)
    {
        int segments = 50;  // Количество сегментов (чем больше, тем плавнее)
        Vector3[] positions = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);
            float z = t * twistLength;  // Движение вдоль Z-оси
            float angle = t * twistTurns * Mathf.PI * 2 + offsetAngle;

            float x = Mathf.Cos(angle) * twistRadius;  // Скручивание по X
            float y = Mathf.Sin(angle) * twistRadius;  // Скручивание по Y

            positions[i] = new Vector3(x, y, z);  // Позиция в локальных координатах
        }

        // Устанавливаем локальные позиции
        LineRenderer lr = wire.GetComponent<LineRenderer>();
        if (lr == null)
            lr = wire.gameObject.AddComponent<LineRenderer>();

        lr.useWorldSpace = false;  // ВАЖНО: Использовать локальные координаты
        lr.positionCount = segments;
        lr.SetPositions(positions);

        return positions;
    }

    void UnTwist()
    {
        // Возвращаем провода на исходные позиции
        ResetWire(whitePart, whiteStartPositions);
        ResetWire(greenPart, greenStartPositions);
    }

    void ResetWire(Transform wire, Vector3[] positions)
    {
        LineRenderer lr = wire.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.positionCount = positions.Length;
            lr.SetPositions(positions);
        }
    }

    void MoveWire(Transform wire)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 localHitPoint = wire.parent.InverseTransformPoint(hit.point);  // Конвертируем в локальные координаты
            wire.localPosition = new Vector3(localHitPoint.x, localHitPoint.y, wire.localPosition.z);
        }
    }
}
