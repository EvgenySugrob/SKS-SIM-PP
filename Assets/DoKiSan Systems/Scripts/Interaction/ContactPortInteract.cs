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
    [SerializeField] string typeCablePort;
    [SerializeField] string typeGroupCable;
    private bool _isBusy;


    private void Start()
    {
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
}
