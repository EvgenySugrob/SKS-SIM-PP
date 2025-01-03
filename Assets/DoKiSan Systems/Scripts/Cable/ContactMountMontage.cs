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
    [SerializeField] Termination termination;
    [SerializeField] Transform terminationPoint;

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
            isInteract= false;
        }
        return isInteract;
    }

    public void Interact(GameObject objectInteract)
    {
        StartCoroutine(CableToMontagePosition(objectInteract.transform));
        _mainParent = transform.parent.parent;
        _interactionSystem = _mainParent.GetComponent<PatchPanelInteraction>().GetInteractionSystem();
        _interactionSystem.StateCablePartMoving(true);
        foreach(ContactPortInteract port in contactPortInteracts)
        {
            port.ActiveBoxColliderPort(true);
        }
        
        mainCollider.enabled = false;
        TerminationToolSetCurrenInfo(objectInteract);
        _interactionSystem.ClearHand();
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
}
