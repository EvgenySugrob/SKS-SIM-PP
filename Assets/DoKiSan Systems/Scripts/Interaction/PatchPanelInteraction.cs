using DoKiSan.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchPanelInteraction : MonoBehaviour, IInteractableObject, IDisableColliders
{
    [Header("Main")]
    [SerializeField] List<Collider> colliders;
    [SerializeField] FirstPlayerControl playerControl;
    [SerializeField] Transform pointForEyes;
    [SerializeField] InteractionSystem interactionSystem;
    private bool _isMounted = false;

    [Header("Montage interaction")]
    [SerializeField] int countCableOnScene = 6;
    [SerializeField] private bool _isCanMontage = false;
    [SerializeField] OutlineDetection outlineDetection;
    [SerializeField] GameObject slotsContainer;
    [SerializeField] PatchPanelSlotInteract[] ppSlots = new PatchPanelSlotInteract[4];
    private int _currentCountMontageCable = 0;
    private bool _inHand = false;
    private PatchPanelSlotInteract _currentSlot;
    private PatchPanelSlotInteract _prevSlot;

    private void Update()
    {
        if(_inHand)
        {
            SeekInteractionSlot();
        }
    }

    private void SeekInteractionSlot()
    {
        GameObject currentObject = outlineDetection.GetCurrentObject();

        if(currentObject.GetComponent<PatchPanelSlotInteract>())
        {
            _currentSlot = currentObject.GetComponent<PatchPanelSlotInteract>();

            if(_prevSlot == null)
            {
                _prevSlot = _currentSlot;
            }
            if(_prevSlot!=_currentSlot)
            {
                _prevSlot.ActiveShowPart(false);
                _prevSlot = _currentSlot;
            }
        }

        if(_currentSlot!=null && currentObject.GetComponent<PatchPanelSlotInteract>())
        {
            _currentSlot.ActiveShowPart(true);
        }
        else if(_currentSlot != null && !currentObject.GetComponent<PatchPanelSlotInteract>())
        {
            _currentSlot.ActiveShowPart(false);
        }
        
    }

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

        if(objectInteract.TryGetComponent(out PatchPanelSlotInteract ppSlotInteract))
        {
            if(!ppSlotInteract.GetBusyState())
            {
                isInteract = true;
            }
        }
        return isInteract;
    }

    public void Interact(GameObject objectInteract)
    {
        if (_inHand)
            return;

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

    public bool GetIsCanMontage()
    {
        return _isCanMontage;
    }

    public void CableTerminationCountCheck()
    {
        _currentCountMontageCable++;

        if(_currentCountMontageCable == countCableOnScene)
        {
            _isCanMontage = true;
            _currentCountMontageCable = 0;
        }
    }

    public void SetInHandState(bool isState)
    {
        _inHand = isState;
        slotsContainer.SetActive(isState);
    }
}
