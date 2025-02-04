using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CablePointBezier : MonoBehaviour, IDisableColliders
{
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] string typeCable;
    [SerializeField] int indexNumberCable;
    [SerializeField] string typeGroupCable;
    [SerializeField] List<InteractivePointHandler> interactivePointList = new List<InteractivePointHandler>();
    [SerializeField] Transform endPoint;
    [SerializeField] private int needIndexInteractivePoint = 3;
    [SerializeField] ContactPortInteract portInteract;
    [SerializeField] bool alreadyInstal = false;

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
    public bool GetAlreadyInstal()
    {
        return alreadyInstal;
    }
    public void SetAlreadyInstal(bool isInstal)
    {
        alreadyInstal = isInstal;
    }
    public string GetTypeCable()
    {
        return typeCable;
    }

    public bool IsNullPortInteractSlot()
    {
        if (portInteract == null)
            return true;
        else 
            return false;
    }

    public void SetPort(ContactPortInteract port)
    {
        if (port == null && portInteract!=null)
        {
            portInteract.SetStateSlot(false);
        }
        portInteract = port;
    }

    public ContactPortInteract GetPortInteract()
    {
        return portInteract;
    }

    public void FillingList(InteractivePointHandler point)
    {
        interactivePointList.Add(point);
    }
    public void ActiveInteractivePoint(bool isActive)
    {
        foreach(InteractivePointHandler point in interactivePointList)
        {
            if(point.GetIndexInteractivePoint() == needIndexInteractivePoint)
            {
                point.DisableCollider(isActive);
            }
                
        }
    }
    public int GetIndexNumberCable()
    {
        return indexNumberCable;
    }
}
