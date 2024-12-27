using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractivePointHandler : MonoBehaviour, IDisableColliders
{
    [SerializeField] SphereCollider sphereCollider;
    private BezierCable cable;
    private int indexOnCurve;
    private Vector3 initialPosition;
    [SerializeField] bool isDrag;

    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    public void DisableCollider(bool isActive)
    {
        sphereCollider.enabled = isActive;
    }

    public void Initialize(BezierCable cable, int indexOnCurve, Vector3 initialPosition)
    {
        this.cable = cable;
        this.indexOnCurve = indexOnCurve;
        this.initialPosition = initialPosition;
    }

    public void OnDrag(Vector3 newPosition)
    {
        Vector3 delta = newPosition - initialPosition;
        cable.UpdateControlPoint(indexOnCurve, delta);
    }

    public void UpdatePositionOnCurve(Vector3 newPosition)
    {
        if (isDrag==false)
        {
            transform.position = newPosition;
            initialPosition = newPosition;
        }
    }

    public void IsDraging(bool isACtive)
    {
        isDrag = isACtive;
    }
}
