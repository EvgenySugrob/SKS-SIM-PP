using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrippingCable : MonoBehaviour
{
    [SerializeField] InteractionSystem interactionSystem;
    [SerializeField] StripperInteration stripperInteration;
    [SerializeField] GameObject stripper;
    [SerializeField] Transform pointForInteractionCable;
    [SerializeField] TwistedPairUnravelingCount unravelingCount;
    
    public void StripperStartInteraction()
    {
        if (interactionSystem.GetHeldObject() != null)
        {
            unravelingCount = interactionSystem.GetHeldObject().GetComponent<TwistedPairUnravelingCount>();
            if (!stripperInteration.StateEnableStripper() && !unravelingCount.CableIsStripp())
            {
                DisableEnableNeeds(false);
                StartProcess();
            }
        }
    }

    public void DisableEnableNeeds(bool isActive)
    {
        interactionSystem.enabled = isActive;
        interactionSystem.SetInteract(isActive);
        stripper.SetActive(!isActive);
    }

    private void StartProcess()
    {
        GameObject heldObject = interactionSystem.GetHeldObject();
        Transform point = heldObject.GetComponent<StripperPointInformation>().GetStripperPoint();
        Transform distantPoint = heldObject.GetComponent<StripperPointInformation>().GetDistanPoint();

        StartCoroutine(BeginProccesCut(heldObject,point,distantPoint));
        stripperInteration.SetEnableSripperState(true);
    }

    #region MoveToCutPosition
    private IEnumerator BeginProccesCut(GameObject heldObject, Transform point,Transform distantPoint)
    {
        stripperInteration.SetDistantPoint(distantPoint);
        yield return StartCoroutine(ProcessMoveToCutPosition(heldObject));
        yield return StartCoroutine(ProcessMoveToPoint(point));
    }
    private IEnumerator ProcessMoveToCutPosition(GameObject heldObject)
    {
        StripperPointInformation cableInteractionPart = heldObject.GetComponent<StripperPointInformation>();
        yield return cableInteractionPart.SetCutPosition(pointForInteractionCable); 
    }
    private IEnumerator ProcessMoveToPoint(Transform point)
    {
        yield return stripperInteration.MoveToPoint(point);
        stripperInteration.ActiveRangeUI(true);
    }
    #endregion
}
