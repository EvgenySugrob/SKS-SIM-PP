using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : MonoBehaviour, IInteractableObject,IDisableColliders
{
    [SerializeField] List<Collider> colliders;
    public bool CanInteractable(GameObject objectInteract)
    {
        if(transform.TryGetComponent(out TwistedPairUnravelingCount twistedPairUnraveling))
        {
            if(twistedPairUnraveling.CableIsStripp())
            {
                return true;
            }
        }
        return false;//////Вставить нужные букавы
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
