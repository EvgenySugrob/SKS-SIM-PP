using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchPanelSlotInteract : MonoBehaviour, IInteractableObject
{
    [SerializeField] GameObject showPart;
    [SerializeField] PatchPanelInteraction patchPanel;
    private BoxCollider _boxCollider;
    private bool _isBusy = false;

    private void Start()
    {
        _boxCollider= GetComponent<BoxCollider>();
    }

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

    public void BusySlotPP(bool isBusy, PatchPanelInteraction panel)
    {
        _isBusy= isBusy;
        patchPanel = panel;
        _boxCollider.enabled = !isBusy;
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
