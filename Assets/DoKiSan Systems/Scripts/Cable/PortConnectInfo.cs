using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortConnectInfo : MonoBehaviour, IInteractableObject
{
    [Header("ContactInfo")]
    [SerializeField] ContactMountMontage contactMountMontage;
    [SerializeField] List<string> cableColorList = new List<string>();

    [Header("PatchCordPosition")]
    [SerializeField] Transform patchCordPoint;

    public bool CanInteractable(GameObject objectInteract)
    {
        bool isInteract = false;


        return isInteract;
    }

    public void Interact(GameObject objectInteract)
    {
        
    }

    //Получение цветов кабеля
}
