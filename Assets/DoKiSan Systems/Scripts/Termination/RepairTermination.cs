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

    [Header("WindowInteractWithPP")]
    [SerializeField] GameObject windowInteractPP;
    [SerializeField] GameObject warningWindow;

    public bool GetIsRepairModeActive()
    {
        return isRepairModeActive;
    }
    public void SetIsRepairModeActive(bool isActive)
    {
        isRepairModeActive= isActive;
    }

    public void TryMontagePatchPanel()
    {
        if(panelInteraction.GetIsCanMontage()==false)
        {
            ActiveWarningWindow(true);
        }
        else
        {
            interactionSystem.TakePPinHand(panelInteraction.gameObject);
        }
    }

    public void ActiveWarningWindow(bool isActive)
    {
        warningWindow.SetActive(isActive);

        if (isActive == false)
            ActiveWindowReInstall(false);
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

    public void ActiveWindowReInstall(bool isActive)
    {
        firstPlayerControl.enabled = !isActive;
        interactionSystem.enabled = !isActive;

        Cursor.visible = isActive;
        if (isActive)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;

        windowInteractPP.SetActive(isActive);
    }

    private void StartWithAnimationBack()
    {
        repairUIGroup.SetActive(false);
        cableChecker.RepairActivationWithAnimation();
    }

    private void StartWithoutAnimationBack()
    {
        windowInteractPP.SetActive(false);
        cableChecker.ReInstallWithoutAnimation();
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

            if(Input.GetMouseButtonDown(0) && contact.GetIsTerminationDone())
            {
                termination.SetCurrentPortInteract(contact);
                panelInteraction.ContactMountColliderOffOn(false);
                contact.DisablePortsColliders(true);

                interactionSystem.StateCablePartMoving(true);
                termination.ReadyToStartWork();

                interactionSystem.ClearHand();
                isRepairModeActive= false;
            }
        }
    }
}
