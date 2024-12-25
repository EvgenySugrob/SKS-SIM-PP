using DoKiSan.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchPanelInteraction : MonoBehaviour, IInteractableObject, IDisableColliders
{
    [SerializeField] List<Collider> colliders;
    [SerializeField] FirstPlayerControl playerControl;
    [SerializeField] Transform pointForEyes;
    [SerializeField] InteractionSystem interactionSystem;
    private bool _isMounted = false;

    public bool CanInteractable(GameObject objectInteract)
    {
        bool isInteract = false;
        if(objectInteract.TryGetComponent(out TwistedPairUnravelingCount twistedPair))
        {
            if(twistedPair.CableIsStripp())
            {
                isInteract = true;
            }
            else
            {
                isInteract= false;
            }
        }
        else
        {
            isInteract = false;
        }
        return isInteract;
    }

    public void Interact(GameObject objectInteract)
    {
        playerControl.SwitchTypeMovePlayer(true);
        playerControl.PointForMove(pointForEyes);
        DisableCollider(false);
    }
    public InteractionSystem GetInteractionSystem() 
    {
        return interactionSystem;
    }
    public void DisableCollider(bool isActive)
    {
       foreach(Collider collider in colliders)
        {
            collider.enabled = isActive;
        }
    }

    public bool GetMountingState()
    {
        return _isMounted;
    }
}
