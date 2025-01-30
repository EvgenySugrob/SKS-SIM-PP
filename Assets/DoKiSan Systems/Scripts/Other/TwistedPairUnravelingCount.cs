using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwistedPairUnravelingCount : MonoBehaviour
{
    [Header("Cable pair info")]
    [SerializeField] int countPairInCable = 4;
    [SerializeField] WorkModeManipulation workModeManipulation;
    [SerializeField] string nameSocket = "Розетка №";
    [SerializeField] Transform nameForPanelDisplay;
    private bool _cableIsStripp = false;

    [Header("End stripper interatcion")]
    [SerializeField] StripperPointInformation pointInformation;
    [SerializeField] StrippingCable strippingCable;
    [SerializeField] StripperInteration stripperInteration;
    [SerializeField] private int _numberSocket;
    private int _allCount = 0;
    private bool _isRotateInMountMontage = false;

    private void Start()
    {
        pointInformation = GetComponent<StripperPointInformation>();
    }
    public void SetNameSocket(string name)
    {
        nameSocket = name;
    }
    public void SetCableNumber(int number)
    {
        _numberSocket = number;
    }
    public int GetCableNumber()
    {
        return _numberSocket;
    }
    public bool CableIsStripp()
    {
        return _cableIsStripp;
    }
    public void SetRotateState(bool isState)
    {
        _isRotateInMountMontage = isState;
    }
    public bool GetRotateState()
    { 
        return _isRotateInMountMontage;
    }

    public void CountUnravelingPair()
    {
        _allCount++;
        if (_allCount<countPairInCable)
        {
            StartCoroutine(RotateInteractPart(90));
        }
        else
        {
            StartCoroutine(ReturnCableInHand(-270));
        }
    }

    private IEnumerator RotateInteractPart(float angle)
    {
        workModeManipulation.WorkState(false);
        yield return RotateOnClick(angle);

        workModeManipulation.WorkState(true);
        _cableIsStripp=true;
        stripperInteration.EndStripping();
        strippingCable.DisableEnableNeeds(true);
    }
    private IEnumerator ReturnCableInHand(float angle)
    {
        yield return StartCoroutine(RotateInteractPart(angle));
        yield return pointInformation.BackInHand();
    }

    private YieldInstruction RotateOnClick(float angle)
    {
        return transform
            .DOLocalRotate(Vector3.right * angle, 1.5f, RotateMode.WorldAxisAdd)
            .Play()
            .WaitForCompletion();
    }

    public void DisplayNameOnPanel(Transform pointOnPanel)
    {
        nameForPanelDisplay.position= pointOnPanel.position;
        nameForPanelDisplay.gameObject.SetActive(true);
    }
}
