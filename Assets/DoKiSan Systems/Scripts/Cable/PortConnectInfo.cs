using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortConnectInfo : MonoBehaviour, IInteractableObject
{
    [Header("ContactInfo")]
    [SerializeField] ContactMountMontage contactMountMontage;
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] ContactPortInteract[] contactPortInteracts = new ContactPortInteract[0];
    private bool _isSort = false;
    [SerializeField]private bool _isConnectingPair = false;
    [SerializeField] private int _cableNumber;

    [Header("PatchCordPosition")]
    [SerializeField] Transform patchCordPoint;
    [SerializeField] Transform patchCordBetweenPoint;

    private void Start()
    {
        boxCollider= GetComponent<BoxCollider>();
    }

    public bool CanInteractable(GameObject objectInteract)
    {
        bool isInteract = false;

        if(objectInteract.GetComponent<CableTestChecker>())
        {
            CableTestChecker cable = objectInteract.GetComponent<CableTestChecker>();
            if(cable.GetIsSearchSocketTermimnation() && cable.GetIsNfrInSocket())
            {
                isInteract = true;
            }
        }

        return isInteract;
    }

    public void Interact(GameObject objectInteract)
    {
        _isConnectingPair = contactMountMontage.GetIsTerminationDone();

        if(!_isSort)
        {
            Array.Sort(contactPortInteracts, (a, b) => a.GetTypeNumberCablePort()
            .CompareTo(b.GetTypeNumberCablePort()));

            _isSort= true;
        }
    }

    public Transform GetPatchCordConnection()
    {
        return patchCordPoint;
    }
    public Transform GetPatchCordBetweenConection()
    {
        return patchCordBetweenPoint;
    }

    public void EnableColliders(bool isActive)
    {
        boxCollider.enabled = isActive;
    }

    public bool IsConnectingPair()
    {
        return _isConnectingPair;
    }

    public ContactPortInteract[] GetArrayContact()
    {
        return contactPortInteracts;
    }
    
    public void SetNumberCable(int number)
    {
        _cableNumber= number;
    }
    public int GetNumberCable()
    {
        return _cableNumber;
    }
}
