using DG.Tweening;
using DoKiSan.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchCordCreate : MonoBehaviour
{
    [Header("Left part")]
    [SerializeField] Transform leftPart;
    [SerializeField] TwistedPairUnravelingCount leftPair;
    [SerializeField] BoxCollider leftCollider;
    private Vector3 _startLocalLeftPosition;
    private Quaternion _startLocalLeftRotation;

    [Header("Right part")]
    [SerializeField] Transform rightPart;
    [SerializeField] TwistedPairUnravelingCount rightPair;
    [SerializeField] BoxCollider rightCollider;
    private Vector3 _startLocalRightPosition;
    private Quaternion _startLocalRightRotation;

    [Header("General setting")]
    [SerializeField] bool isDoneStripperWork = false;
    [SerializeField] bool isHand=false;
    [SerializeField] bool isCrimping = false;
    private BoxCollider _boxCollider;
    private Vector3 _startPatchCorPosition;
    private Quaternion _startPatchCorRotation;

    [Header("Stripper")]
    [SerializeField] StripperInteration stripperInteration;
    [SerializeField] StrippingCable strippingCable;

    [Header("Player")]
    [SerializeField] Transform patchCordPivot;
    [SerializeField] FirstPlayerControl playerControl;
    [SerializeField] InteractionSystem interactionSystem;

    private void Start()
    {
        _startPatchCorPosition = transform.position;
        _startPatchCorRotation = transform.rotation;

        _startLocalLeftPosition = leftPair.transform.localPosition;
        _startLocalLeftRotation = leftPair.transform.localRotation;

        _startLocalRightPosition = rightPair.transform.localPosition;
        _startLocalRightRotation = rightPair.transform.localRotation;

        _boxCollider = GetComponent<BoxCollider>();
    }

    public bool GetCrimpingState()
    {
        return isCrimping;
    }

    public void SetCrimpingState(bool state)
    {
        isCrimping = state;
    }

    public void DisableControl(bool state)
    {
        playerControl.enabled = state;
        interactionSystem.enabled = state;
    }

    public void DisableCollider(bool state)
    {
        _boxCollider.enabled = state;
    }

    public bool GetStateStrippingState()
    {
        return isDoneStripperWork;
    }

    public bool CheckAllPArtStripping()
    {
        if (leftPair.CableIsStripp() && rightPair.CableIsStripp())
        {
            isDoneStripperWork = true;
        }

        return isDoneStripperWork;
    }

#region LeftRightPartStripping

    public void SetLeftPartInHand()
    {
        isHand= true;
        interactionSystem.ForcedSetHeldObject(leftPair.gameObject);
        _boxCollider.enabled = false;
        transform.parent = patchCordPivot;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }
    public void SetRightPartInHand()
    {
        interactionSystem.ForcedSetHeldObject(rightPair.gameObject);
        StartCoroutine(ReturnPartInStartPosition(leftPair.transform,_startLocalLeftPosition,_startLocalLeftRotation));
    }

    private IEnumerator ReturnPartInStartPosition(Transform partPatchCord,Vector3 position, Quaternion rotation)
    {
        DisableControl(false);

        yield return DOTween.Sequence()
            .Append(partPatchCord.DOLocalMove(position, 0.5f))
            .Join(partPatchCord.DOLocalRotateQuaternion(rotation, 0.5f))
            .Play()
            .WaitForCompletion();

        strippingCable.StripperStartInteraction();
        DisableControl(true);
    }

    public void ReturnRightPartBack()
    {
        StartCoroutine(ReturnPartInStartPosition(rightPair.transform, _startLocalRightPosition, _startLocalRightRotation));
    }

    #endregion

    public void PatchCordOnTable()
    {
        transform.parent = null;
        StartCoroutine(ReturnPatchCordOnTable());
    }

    private IEnumerator ReturnPatchCordOnTable()
    {
        yield return DOTween.Sequence()
            .Append(transform.DOMove(_startPatchCorPosition,1f))
            .Join(transform.DORotateQuaternion(_startPatchCorRotation,1f))
            .Play()
            .WaitForCompletion();
        isHand= false;
    }

    public bool GetIsHandState()
    {
        return isHand;
    }

    public void EnabledLeftRightCollider(bool isEnable)
    {
        leftCollider.enabled = isEnable;
        rightCollider.enabled = isEnable;
    }

    public void CheckLastColliderEnable()
    {
        JackConnetctAndSetting leftJack = leftPair.GetComponent<JackConnetctAndSetting>();

        if (!leftJack.GetEndContactCrimping())
        {
            leftCollider.enabled=true;
        }
        else
        {
            rightCollider.enabled = true;
        }
    }

    public bool AllPartCrimping()
    {
        bool endCrimping = false;

        JackConnetctAndSetting leftJack = leftPair.GetComponent<JackConnetctAndSetting>();
        JackConnetctAndSetting rightJack = rightPair.GetComponent<JackConnetctAndSetting>();

        if(leftJack.GetEndContactCrimping()&&rightJack.GetEndContactCrimping())
        {
            endCrimping = true;
            SetCrimpingState(true);
        }
        else if(!leftJack.GetEndContactCrimping())
        {
            leftCollider.enabled = true;
        }
        else 
        {
            rightCollider.enabled = true;
        }
        return endCrimping;
    }
}
