using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactPortInteract : MonoBehaviour
{
    [Header("Selected part")]
    [SerializeField] Renderer matRenderer;
    [Range(0f, 1f)]
    [SerializeField] float alphaMaterial;
    [SerializeField] float alphaSpeed;
    [SerializeField] bool isSelected;
    private Material _instanceMaterial;
    private float _timeOffset;
    private Material _selectMaterial;

    [Header("InteractionPart")]
    [SerializeField] Transform pointBeforeDriving;
    [SerializeField] Transform pointAfterDriving;
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] string typeCablePort;
    [SerializeField] int typeNumberCablePort;
    [SerializeField] string typeGroupCable;
    [SerializeField] CablePointBezier cablePoint;

    [Header("Termination")]
    [SerializeField] Transform point1;
    [SerializeField] Transform point2;
    [SerializeField] private bool _isBusy;
    [SerializeField] private bool _terminationDone;


    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        _selectMaterial = transform.GetChild(0).transform.GetComponent<MeshRenderer>().sharedMaterial;
        matRenderer = transform.GetChild(0).transform.GetComponent<Renderer>();
        if(matRenderer!=null)
        {
            _instanceMaterial = new Material(matRenderer.material);
            matRenderer.material = _instanceMaterial;
        }
        _timeOffset = Random.Range(0f, Mathf.PI * 2);
    }

    private void Update()
    {
        ChangeAlpha();
    }
    private void ChangeAlpha()
    {
        if (isSelected) 
        {
            alphaMaterial = (Mathf.Sin((Time.time +_timeOffset) * alphaSpeed * 2 * Mathf.PI) + 1) / 2;
        }
        else
        {
            alphaMaterial = 0;
        }
        if(_instanceMaterial!=null)
        {
            Color alphaColor = matRenderer.material.color;
            alphaColor.a = alphaMaterial;
            matRenderer.material.color = alphaColor;
        }
    }

    public void ActiveBoxColliderPort(bool isActive)
    {
        boxCollider.enabled = isActive;
    }

    public void SelectPort(bool isActive)
    {
        isSelected = isActive;
    }
    public bool GetSelectState()
    {
        return isSelected;
    }
    public Transform GetPointBeforeDriving()
    {
        return pointBeforeDriving;
    }
    public bool GetStateSlot()
    {
        return _isBusy;
    }
    public void SetStateSlot(bool isState)
    {
        _isBusy= isState;
    }

    public void SetTerminationState(bool isDone)
    {
        _terminationDone = isDone;
    }
    public bool GetTerminationState()
    {
        return _terminationDone;
    }

    public bool CheckSlotAndCAble(string cableType)
    {
        bool isMatches = false;

        if (typeCablePort == cableType)
        {
            Debug.Log("Есть схожесть");
            isMatches = true;
        }
        else
        {
            Debug.Log("Слот и кабель не подходят");
            isMatches = false;
        }

        return isMatches;
    }

    public void SetCablePoint(CablePointBezier cable)
    {
        cablePoint = cable;
    }

    public Transform GetFirstPoint()
    {
        return point1;
    }
    public Transform GetSecondPoint()
    {
        return point2;
    }

    public void SwapWireSlot(CablePointBezier wireToSwap)
    {
        ContactPortInteract swapPort = wireToSwap.GetPortInteract();

        wireToSwap.transform.parent = transform;
        cablePoint.transform.parent = swapPort.transform;

        wireToSwap.transform.position = pointBeforeDriving.position;
        cablePoint.transform.position = swapPort.GetPointBeforeDriving().position;

        swapPort.SetCablePoint(cablePoint);
        SetCablePoint(wireToSwap);

        cablePoint.SetPort(swapPort);
        wireToSwap.SetPort(this);
    }

    public void CableAfterDriving()
    {
        cablePoint.transform.position = pointAfterDriving.position;
        cablePoint.GetComponent<IDisableColliders>().DisableCollider(false);
        cablePoint.ActiveInteractivePoint(false);
        SetTerminationState(true);
    }

    public void CableRemoveFromPort()
    {
        cablePoint.transform.position = pointBeforeDriving.position;
        cablePoint.GetComponent<IDisableColliders>().DisableCollider(true);
        cablePoint.ActiveInteractivePoint(true);
        SetTerminationState(false);
    }

    public string GetTypeCableColorPort()
    {
        return typeCablePort;
    }

    public string GetTypeCableColor()
    {
        return cablePoint.GetTypeCable();
    }
    public int GetTypeNumberCablePort()
    {
        return typeNumberCablePort;
    }

    public int GetTypeIndexCablePort()
    {
        return cablePoint.GetIndexNumberCable();
    }
}
