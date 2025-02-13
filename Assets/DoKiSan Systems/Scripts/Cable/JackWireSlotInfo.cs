using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackWireSlotInfo : MonoBehaviour
{
    [Header("Wire slot")]
    [SerializeField] CablePointBezier cablePointBezier;
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
}
