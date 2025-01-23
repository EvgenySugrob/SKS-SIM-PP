using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortConnectInfo : MonoBehaviour, IInteractableObject
{
    [Header("ContactInfo")]
    [SerializeField] ContactMountMontage contactMountMontage;
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] List<string> cableColorList = new List<string>();

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
    //Получение цветов кабеля
}
