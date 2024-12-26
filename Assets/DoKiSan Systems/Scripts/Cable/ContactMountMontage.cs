using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactMountMontage : MonoBehaviour, IInteractableObject
{
    [SerializeField] BoxCollider mainCollider;
    [SerializeField] List<BoxCollider> cableSlotCollider;
    [SerializeField] Transform cableMontagePoint;
    [SerializeField] private Transform _mainParent;
    [SerializeField] private InteractionSystem _interactionSystem;

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
        StartCoroutine(CableToMontageposition(objectInteract.transform));
        _mainParent = transform.parent.parent;
        _interactionSystem = _mainParent.GetComponent<PatchPanelInteraction>().GetInteractionSystem();
        _interactionSystem.StateCablePartMoving(true);
        foreach (BoxCollider collider in cableSlotCollider)
        {
            collider.enabled = true;
        }
        mainCollider.enabled = false;
    }

    private IEnumerator CableToMontageposition(Transform cable)
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
