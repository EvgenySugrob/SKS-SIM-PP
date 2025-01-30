using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactMountMontage : MonoBehaviour, IInteractableObject
{
    [SerializeField] BoxCollider mainCollider;
    [SerializeField] List<ContactPortInteract> contactPortInteracts;
    [SerializeField] Transform cableMontagePoint;
    [SerializeField] private Transform _mainParent;
    [SerializeField] private InteractionSystem _interactionSystem;

    [Header("Termination")]
    [SerializeField] PatchPanelInteraction patchPanel;
    [SerializeField] Termination termination;
    [SerializeField] Transform terminationPoint;
    [SerializeField] bool isTeminationDone = false;

    [Header("Transform for nameSocket")]
    [SerializeField] PortConnectInfo portConnectInfo;
    [SerializeField] Transform pointforName;

    [Header("RotatePartAfterMontage")]
    [SerializeField] RotateInteractionPivot rotateInteractionPivot;

    public bool CanInteractable(GameObject objectInteract)
    {
        bool isInteract = false;
        if(objectInteract.TryGetComponent(out TwistedPairUnravelingCount twistedPair))
        {
            if(twistedPair.CableIsStripp() && !isTeminationDone)
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
            isInteract= false;
        }
        return isInteract;
    }

    public void Interact(GameObject objectInteract)
    {
        StartCoroutine(CableToMontagePosition(objectInteract.transform));

        objectInteract.GetComponent<TwistedPairUnravelingCount>().DisplayNameOnPanel(pointforName);
        portConnectInfo.SetNumberCable(objectInteract.GetComponent<TwistedPairUnravelingCount>().GetCableNumber());
        rotateInteractionPivot.CheckAndSetObjectForRotate(objectInteract.transform);

        _mainParent = transform.parent.parent;
        _interactionSystem = _mainParent.GetComponent<PatchPanelInteraction>().GetInteractionSystem();
        _interactionSystem.StateCablePartMoving(true);

        DisablePortsColliders(true);
        
        mainCollider.enabled = false;
        patchPanel.ContactMountColliderOffOn(false);

        TerminationToolSetCurrenInfo(objectInteract);
        _interactionSystem.ClearHand();
    }

    public void ColliderDisable(bool isActive)
    {
        mainCollider.enabled = isActive;
        //if(isActive)
        //    patchPanel.DisableAllContactMount(false);
    }

    public void ColliderOffOn(bool isActive)
    {
        mainCollider.enabled = isActive;
    }

    public void TerminationDone(bool isDone)
    {
        isTeminationDone = isDone;
    }

    public bool CheckAllPortReady()
    {
        bool isReady = true;

        foreach (ContactPortInteract port in contactPortInteracts)
        {
            if(!port.GetStateSlot())
            {
                isReady = false;
                break;
            }
        }
        return isReady;
    }

    public Transform GetTerminationPoint()
    {
        return terminationPoint;
    }

    public void DisablePortsColliders(bool isActive)
    {
        foreach (ContactPortInteract port in contactPortInteracts)
        {
            port.ActiveBoxColliderPort(isActive);
        }
    }

    private void TerminationToolSetCurrenInfo(GameObject objectInteract)
    {
        termination = objectInteract.GetComponent<InteractObject>().GetTerminationTool();
        termination.SetCurrentPortInteract(this);
    }
    private IEnumerator CableToMontagePosition(Transform cable)
    {
        cable.parent = cableMontagePoint;
        Vector3 endRotate = new Vector3(-180f, 0f, -90f);
        yield return DOTween.Sequence()
            .Append(cable.DOLocalMove(Vector3.zero,1f))
            .Join(cable.DOLocalRotate(endRotate,1f))
            .Play()
            .WaitForCompletion();
    }

    public bool GetIsTerminationDone()
    {
        return isTeminationDone;
    }
}
