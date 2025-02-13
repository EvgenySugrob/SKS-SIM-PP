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

    [Header("PatchCord")]
    [SerializeField] PatchCordCreate patchCordCreate;
    [SerializeField] bool isPatchCordPart=false;

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

    }
    private IEnumerator ReturnCableInHand(float angle)
    {
        yield return StartCoroutine(RotateInteractPart(angle));

        _cableIsStripp = true;
        stripperInteration.EndStripping();

        if (isPatchCordPart)
        {
            if(!patchCordCreate.CheckAllPArtStripping())
            {
                patchCordCreate.SetRightPartInHand();
            }
            else
            {
                patchCordCreate.ReturnRightPartBack();
            }
        }
        else
        {
            yield return pointInformation.BackInHand();
        }

        strippingCable.DisableEnableNeeds(true);
    }

    private YieldInstruction RotateOnClick(float angle)
    {
        if(isPatchCordPart)
        {
            return transform
            .DOLocalRotate(Vector3.up * angle, 1.5f, RotateMode.WorldAxisAdd)
            .Play()
            .WaitForCompletion();
        }
        else
        {
            return transform
            .DOLocalRotate(Vector3.right * angle, 1.5f, RotateMode.WorldAxisAdd)
            .Play()
            .WaitForCompletion();
        }
        
    }

    public void DisplayNameOnPanel(Transform pointOnPanel)
    {
        nameForPanelDisplay.position= pointOnPanel.position;
        nameForPanelDisplay.gameObject.SetActive(true);
    }
}
