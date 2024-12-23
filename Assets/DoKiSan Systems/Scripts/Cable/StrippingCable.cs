using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrippingCable : MonoBehaviour
{
    [SerializeField] InteractionSystem interactionSystem;
    [SerializeField] StripperInteration stripperInteration;
    [SerializeField] GameObject stripper;
    [SerializeField] Transform pointForInteractionCable;
    
    public void StripperStartInteraction()
    {
        if(!stripperInteration.StateEnableStripper())
        {
            DisableEnableNeeds(false);
            StartProcess();
        }
    }

    private void DisableEnableNeeds(bool isActive)
    {
        interactionSystem.enabled = isActive;
        stripper.SetActive(!isActive);
    }

    private void StartProcess()
    {
        GameObject heldObject = interactionSystem.GetHeldObject();
        Transform point = heldObject.GetComponent<StripperPointInformation>().GetStripperPoint();

        StartCoroutine(BeginProccesCut(heldObject,point));
        stripperInteration.SetEnableSripperState(true);
    }

    #region MoveToCutPosition
    private IEnumerator BeginProccesCut(GameObject heldObject, Transform point)
    {
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
    }
    #endregion
}
