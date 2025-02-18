using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JackReplace : MonoBehaviour
{
    [Header("Jack")]
    [SerializeField] bool isJackSelectActive = false;

    [Header("PatchCord")]
    [SerializeField] PatchCordCreate patchCord;
    [SerializeField] Transform cameraChoisePosition;

    [Header("Crimper")]
    [SerializeField] Crimper crimper;

    [Header("Player and move")]
    [SerializeField] Transform eyesPlayer;
    [SerializeField] Camera playerCamera;
    [SerializeField] OutlineDetection outlineDetection;
    private GameObject _currentObj;
    private Transform _startParentEyes;
    private Vector3 _startPositionEyes;
    private Quaternion _startRotationEyes;

    [Header("CheckBeforeStart")]
    [SerializeField] CableTestChecker checker;
    [SerializeField] InteractionSystem interactionSystem;
    [SerializeField] StripperInteration stripperInteration;
    [SerializeField] Termination termination;

    private void Update()
    {
        if(isJackSelectActive)
        {
            SelectPart();
        }
    }

    private void SelectPart()
    {
        _currentObj = outlineDetection.GetCurrentObject();

        if(_currentObj.GetComponent<JackConnetctAndSetting>())
        {
            if(Input.GetMouseButtonDown(0))
            {
                isJackSelectActive = false;

                JackConnetctAndSetting jackConnect = _currentObj.GetComponent<JackConnetctAndSetting>();
                patchCord.EnabledLeftRightCollider(false);

                eyesPlayer.parent = jackConnect.GetCameraPoint();

                StartCoroutine(MoveOnJackPosition(Vector3.zero));
                jackConnect.StartJackMontage();

                crimper.SetSelectedJack(jackConnect);
            }
        }
    }

    public bool GetJackActive()
    {
        return isJackSelectActive;
    }

    public void StartJackReplace()
    {
        if(patchCord.GetIsHandState() && !isJackSelectActive && patchCord.GetStateStrippingState())
        {
            patchCord.DisableControl(false);
            patchCord.EnabledLeftRightCollider(true);
            patchCord.PatchCordOnTable();

            SaveStartPositionPlayer();
            MovePlayerEyesToSelectPart();
        }
    }

    private void SaveStartPositionPlayer()
    {
        _startParentEyes = eyesPlayer.parent;
        _startPositionEyes = eyesPlayer.localPosition;
        _startRotationEyes = eyesPlayer.localRotation;
    }

    private void MovePlayerEyesToSelectPart()
    {
        eyesPlayer.parent = cameraChoisePosition;
        StartCoroutine(MoveEyes(Vector3.zero,cameraChoisePosition.localRotation));
    }

    private IEnumerator MoveEyes(Vector3 position,Quaternion rotation)
    {
        yield return DOTween.Sequence()
            .Append(eyesPlayer.DOLocalMove(position,1f))
            .Join(eyesPlayer.DOLocalRotateQuaternion(rotation,1f))
            .Play()
            .WaitForCompletion();

        isJackSelectActive = true;
    }

    private IEnumerator MoveOnJackPosition(Vector3 jackPoint)
    {
        yield return eyesPlayer.DOLocalMove(jackPoint,0.5f)
            .Play() 
            .WaitForCompletion();
    }

    public void MoveEyesAfterFirstJackComplate()
    {
        MovePlayerEyesToSelectPart();
    }

    public void ReturnToMainWork()
    {
        StartCoroutine(ReturnToMain());
    }
    
    private IEnumerator ReturnToMain()
    {
        eyesPlayer.parent = _startParentEyes;
        yield return MoveEyes(_startPositionEyes, _startRotationEyes);

        patchCord.DisableControl(true);
        patchCord.DisableCollider(true);
        
        interactionSystem.ClearHand();
        isJackSelectActive= false;
    }
}
