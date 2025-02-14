using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackWireSlotInfo : MonoBehaviour
{
    [Header("Wire slot")]
    [SerializeField] CablePointBezier cablePointBezier;
    [SerializeField] Transform positionForStartGroup;
    [SerializeField] Renderer indicatorMaterial;
    [SerializeField] bool isBusy = false;
    [SerializeField] BoxCollider slotCollider;
    [Range(0f, 1f)]
    [SerializeField] float alphaMaterial;
    [SerializeField] float alphaSpeed;
    private float _timeOffset;
    private Material _instanceMaterial;
    private bool _isActiveAnimation=false;

    private void Start()
    {
        _instanceMaterial = new Material(indicatorMaterial.material);
        indicatorMaterial.material = _instanceMaterial;
    }

    private void Update()
    {
        ChangeAlpha();
    }

    public Transform GetPositionForStartGroup()
    {
        return positionForStartGroup;
    }

    public void ColliderActive(bool isActive)
    {
        slotCollider.enabled = isActive;
    }

    public void SetBusySlot(bool isState)
    {
        isBusy= isState;
    }

    public bool GetStateSlot()
    {
        return isBusy;
    }

    public void SetCableInSlot(CablePointBezier cable)
    {
        cablePointBezier = cable;
    }

    public CablePointBezier GetCablePoint()
    {
        return cablePointBezier;
    }

    public void SelectPort(bool isSelect)
    {
        _isActiveAnimation= isSelect;
    }

    private void ChangeAlpha()
    {
        if(_isActiveAnimation)
        {
            alphaMaterial = (Mathf.Sin((Time.time + _timeOffset) * alphaSpeed * 2 * Mathf.PI) + 1) / 2;
        }
        else
        {
            alphaMaterial = 0;
        }

        if(_instanceMaterial!=null)
        {
            Color alphaColor = indicatorMaterial.material.color;
            alphaColor.a = alphaMaterial;
            indicatorMaterial.material.color = alphaColor;
        }
    }

    public void SwapWireSlot(CablePointBezier wireToSwap,Vector3 startPosition)
    {
        if(wireToSwap.GetJackSlot() != null)
        {
            JackWireSlotInfo jackSlot = wireToSwap.GetJackSlot();

            cablePointBezier.transform.position = wireToSwap.GetJackSlot().transform.position;
            cablePointBezier.GetStartGroup().position = wireToSwap.GetJackSlot().GetPositionForStartGroup().position;

            wireToSwap.transform.position = transform.position;
            wireToSwap.GetStartGroup().position = positionForStartGroup.position;

            cablePointBezier.SetJackSlot(wireToSwap.GetJackSlot());
            wireToSwap.SetJackSlot(this);

            jackSlot.SetCableInSlot(cablePointBezier);
            SetCableInSlot(wireToSwap);
        }
        else
        {
            cablePointBezier.transform.position = startPosition;
            cablePointBezier.GetStartGroup().position = wireToSwap.GetStartGroup().position;

            wireToSwap.transform.position = transform.position;
            wireToSwap.GetStartGroup().position = positionForStartGroup.position;

            cablePointBezier.SetJackSlot(null);
            wireToSwap.SetJackSlot(this);

            SetCableInSlot(wireToSwap);
        }
    }
}
