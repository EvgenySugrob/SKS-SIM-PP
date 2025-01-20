using DG.Tweening;
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
    [SerializeField] private bool _isMounted = false;

    [Header("Montage interaction")]
    [SerializeField] int countCableOnScene = 6;
    [SerializeField] private bool _isCanMontage = false;
    [SerializeField] OutlineDetection outlineDetection;
    [SerializeField] GameObject slotsContainer;
    [SerializeField] PatchPanelSlotInteract[] ppSlots = new PatchPanelSlotInteract[4];
    [SerializeField] private PatchPanelSlotInteract _mountingSlot;
    private int _currentCountMontageCable = 0;
    private bool _inHand = false;
    private PatchPanelSlotInteract _currentSlot;
    private PatchPanelSlotInteract _prevSlot;

    [Header("Tester done interaction")]
    [SerializeField] bool _isTesterDoneInteract = false;

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

            if(Input.GetMouseButtonDown(0))
            {
                playerControl.enabled= false;
                interactionSystem.enabled= false;

                _inHand = false;
                _currentSlot.BusySlotPP(true,this);
                _isMounted = true;
                _mountingSlot = _currentSlot;
                
                Transform pointMontage = _currentSlot.transform;
                interactionSystem.ClearHand();

                StartCoroutine(MountingPatchPanel(pointMontage));
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

    private IEnumerator MountingPatchPanel(Transform mountingPoint)
    {
        yield return DOTween.Sequence()
            .Append(transform.DOMove(mountingPoint.position,1f))
            .Join(transform.DORotateQuaternion(mountingPoint.rotation,1f))
            .Play()
            .WaitForCompletion();
        transform.parent = mountingPoint.parent;
        _currentSlot.ActiveShowPart(false);

        interactionSystem.enabled = true;
        playerControl.enabled = true;
        this.enabled = false;
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

        //if(objectInteract.TryGetComponent(out CableTestChecker cableChecker))
        //{
        //    if(_isTesterDoneInteract)
        //    {
        //        isInteract = true;
        //    }
        //}
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
    public void ReleaseSlot()
    {
        _isMounted= false;
        _mountingSlot.BusySlotPP(false, null);
    }

    public bool GetIsCanMontage()
    {
        return _isCanMontage;
    }
    
    public void CableTerminationCountCheck()
    {
        _currentCountMontageCable++;
        CheckOnTesterInteractionDone();

        if(_currentCountMontageCable == countCableOnScene)
        {
            _isCanMontage = true;
            //_currentCountMontageCable = 0;
        }
    }
    private void CheckOnTesterInteractionDone()
    {
        if(_currentCountMontageCable>0)
        {
            _isTesterDoneInteract= true;
        }
        else if (_currentCountMontageCable == 0)
        {
            _isTesterDoneInteract= false;
        }
    }

    public void SetInHandState(bool isState)
    {
        _inHand = isState;
        slotsContainer.SetActive(isState);

        if(_mountingSlot!=null)
        {
            ReleaseSlot();
            this.enabled = isState;
        }
    }
}
