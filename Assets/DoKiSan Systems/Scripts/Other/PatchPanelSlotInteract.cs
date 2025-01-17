using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchPanelSlotInteract : MonoBehaviour, IInteractableObject
{
    [SerializeField] GameObject showPart;
    private bool _isBusy = false;

    public bool CanInteractable(GameObject objectInteract)
    {
        bool isInteract = false;
        if (objectInteract.TryGetComponent(out PatchPanelInteraction patchPanel))
        {
            isInteract = true;
        }
        return isInteract;
    }

    public void Interact(GameObject objectInteract)
    {
        throw new System.NotImplementedException();
    }

    public bool GetBusyState()
    {
        return _isBusy;
    }

    public void ActiveShowPart(bool isActive)
    {
        showPart.SetActive(isActive);
    }

    public bool GetIsActiveShowPart()
    {
        return showPart.activeSelf;
    }
}
