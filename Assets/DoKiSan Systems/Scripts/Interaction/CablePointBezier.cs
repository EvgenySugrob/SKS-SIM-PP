using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CablePointBezier : MonoBehaviour, IDisableColliders
{
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] string typeCable;
    [SerializeField] string typeGroupCable;
    [SerializeField] List<InteractivePointHandler> interactivePointList = new List<InteractivePointHandler>();
    [SerializeField] Transform endPoint;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        int indexLastChild = transform.childCount - 1;
        endPoint = transform.GetChild(indexLastChild);
    }
    public void DisableCollider(bool isActive)
    {
        boxCollider.enabled = isActive;
    }

    public string GetTypeCable()
    {
        return typeCable;
    }

    public void FillingList(InteractivePointHandler point)
    {
        interactivePointList.Add(point);
    }
    public void ActiveInteractivePoint(bool isActive)
    {
        foreach(InteractivePointHandler point in interactivePointList)
        {
            point.DisableCollider(isActive);
        }
    }
}
