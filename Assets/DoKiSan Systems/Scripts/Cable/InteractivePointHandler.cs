using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractivePointHandler : MonoBehaviour, IDisableColliders
{
    [SerializeField] SphereCollider sphereCollider;
    private BezierCable cable;
    [SerializeField]private int indexOnCurve;
    private Vector3 initialPosition;
    [SerializeField] bool isDrag;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void DisableCollider(bool isActive)
    {
        sphereCollider.enabled = isActive;
        meshRenderer.enabled = isActive;
    }

    public void Initialize(BezierCable cable, int indexOnCurve, Vector3 initialPosition)
    {
        this.cable = cable;
        this.indexOnCurve = indexOnCurve;
        this.initialPosition = initialPosition;
    }

    public void OnDrag(Vector3 newPosition)
    {
        if (!isDrag)
            return;
        cable.SetDraggingInteractivePoint(true);

        transform.position = newPosition;

        Vector3 offset = transform.position;
        cable.UpdateControlPoint(indexOnCurve,offset);

    }

    public void UpdatePositionOnCurve(Vector3 newPosition)
    {
        if (isDrag == false)
        {
            transform.position = newPosition;
            initialPosition = newPosition;
        }
    }

    public void IsDraging(bool isActive)
    {
        isDrag = isActive;

        if (!isActive) // Когда перетаскивание завершено
        {
            cable.SetDraggingInteractivePoint(false);
        }
    }
    public int GetIndexInteractivePoint()
    {
        return indexOnCurve;
    }
}
