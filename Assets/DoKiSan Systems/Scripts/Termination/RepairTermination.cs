using DoKiSan.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairTermination : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] FirstPlayerControl firstPlayerControl;
    [SerializeField] InteractionSystem interactionSystem;
    [SerializeField] OutlineDetection outlineDetection;
    [SerializeField] bool isRepairModeActive = false;

    [Header("Patch panel")]
    [SerializeField] PatchPanelInteraction panelInteraction;

    [Header("Termination")]
    [SerializeField] Termination termination;

    [Header("CableTestChecker")]
    [SerializeField] CableTestChecker cableChecker;

    [Header("UIGroupRepair")]
    [SerializeField] GameObject repairUIGroup;

    public bool GetIsRepairModeActive()
    {
        return isRepairModeActive;
    }
    public void SetIsRepairModeActive(bool isActive)
    {
        isRepairModeActive= isActive;
    }

    public void ActiveRepairMode()
    {
        if(cableChecker.GetIsNfrInSocket())
        {
            StartWithAnimationBack();
        }
        else
        {
            StartWithoutAnimationBack();
        }
    }

    private void StartWithAnimationBack()
    {
        repairUIGroup.SetActive(false);
        cableChecker.RepairActivationWithAnimation();
    }

    private void StartWithoutAnimationBack()
    {

    }

    private void Update()
    {
        if(isRepairModeActive)
        {
            SearchContactMount();
        }
    }

    private void SearchContactMount()
    {
        if(outlineDetection.GetCurrentObject().GetComponent<ContactMountMontage>())
        {
            ContactMountMontage contact = outlineDetection.GetCurrentObject().GetComponent<ContactMountMontage>();

            if(Input.GetMouseButtonDown(0))
            {
                termination.SetCurrentPortInteract(contact);
                panelInteraction.ContactMountColliderOffOn(false);
                contact.DisablePortsColliders(true);

                interactionSystem.StateCablePartMoving(true);
                termination.ReadyToStartWork();

                interactionSystem.ClearHand();
            }
        }
    }
}
