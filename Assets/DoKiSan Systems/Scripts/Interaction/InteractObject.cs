using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : MonoBehaviour, IInteractableObject,IDisableColliders
{
    [SerializeField] List<Collider> colliders;
    public bool CanInteractable(GameObject objectInteract)
    {
        bool isInteract = false;
        if(objectInteract.TryGetComponent(out PatchPanelInteraction patchPanel))
        {
            if(patchPanel.GetMountingState()==false)
            {
                isInteract= true;
            }
            else
            {
                isInteract= false;
            }
        }
        else
        {
            isInteract= false;
        }


        return true;
    }

    public void DisableCollider(bool isActive)
    {
        foreach(Collider collider in colliders)
        {
            collider.enabled = isActive;
        }
    }

    public void Interact(GameObject objectInteract)
    {
        Debug.Log($"{name} взаимодействует с твоей мамой и с {objectInteract.name}");
    }
}
