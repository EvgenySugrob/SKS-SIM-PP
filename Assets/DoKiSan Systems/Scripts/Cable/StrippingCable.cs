using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrippingCable : MonoBehaviour
{
    [SerializeField] InteractionSystem interactionSystem;
    [SerializeField] StripperInteration stripperInteration;
    [SerializeField] GameObject stripper;
    
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
        stripperInteration.MoveToStrippingPoint(point);
    }
}
