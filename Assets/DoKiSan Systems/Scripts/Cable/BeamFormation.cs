using DG.Tweening;
using DoKiSan.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamFormation : MonoBehaviour
{
    [Header("AllInteractivePartCable")]
    [SerializeField] List<Transform> interactivePartsCable;
    [SerializeField] bool isFormationProgress = false;
    [SerializeField] bool isSelectedCable = false;
    [SerializeField] List<Transform> secondInteractivePartsCable;
    [SerializeField] List<GameObject> cableForBeaming;

    [Header("Player")]
    [SerializeField] Transform eyesPlayer;
    [SerializeField] FirstPlayerControl playerControl;
    [SerializeField] InteractionSystem interactionSystem;
    [SerializeField] OutlineDetection outlineDetection;
    private Transform _eyesStartParent;
    private Vector3 _eyesStartPosition;
    private Quaternion _eyesStartRotation;

    [Header("PatchPanel")]
    [SerializeField] PatchPanelInteraction panelInteraction;
    [SerializeField] Transform formationBetweenPoint;
    [SerializeField] Transform formationPoint;

    [Header("UIBack")]
    [SerializeField] GameObject ui;
    [SerializeField] GameObject backBt;
    [SerializeField] GameObject acceptFormation;

    private void Start()
    {
        _eyesStartParent = eyesPlayer.parent;
    }

    public void StartFormationFormationBtClick()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        StartMoveEyesToPoint();
    }

    public void ReturnToMainViewBtClick()
    {
        ui.SetActive(false);
        backBt.SetActive(false);

        playerControl.enabled = false;
        playerControl.SwitchTypeMovePlayer(false);

        StartCoroutine(MoveEyesBack());
    }

    private void StartMoveEyesToPoint()
    {
        isFormationProgress = true;
        outlineDetection.enabled = false;

        _eyesStartPosition = eyesPlayer.position;
        _eyesStartRotation = eyesPlayer.rotation;

        DisableControl(false);
        panelInteraction.DisableCollider(false);

        StartCoroutine(MoveEyes());
    }

    private IEnumerator MoveEyes()
    {
        eyesPlayer.parent = null;

        yield return Move(formationBetweenPoint,formationPoint.position,formationPoint.rotation);

        eyesPlayer.parent = formationPoint;
        playerControl.SwitchTypeMovePlayer(true);
        playerControl.enabled = true;

        ui.SetActive(true);
        backBt.SetActive(true);
    }

    private YieldInstruction Move(Transform betweenPosition, Vector3 endPosition, Quaternion endRotation)
    {
        return DOTween.Sequence()
            .Append(eyesPlayer.DOMove(betweenPosition.position, 1f).SetEase(Ease.Linear))
            ///.Join(eyesPlayer.DORotateQuaternion(betweenPosition.rotation, 1f))
            .Append(eyesPlayer.DOMove(endPosition, 1f))
            .Join(eyesPlayer.DORotateQuaternion(endRotation, 1f))
            .Play()
            .WaitForCompletion();
    }

    private IEnumerator MoveEyesBack()
    {
        eyesPlayer.parent = null;

        yield return Move(formationBetweenPoint,_eyesStartPosition,_eyesStartRotation);

        eyesPlayer.parent = _eyesStartParent;
        DisableControl(true);
        panelInteraction.DisableCollider(false);

        isFormationProgress = false;
        outlineDetection.enabled = true;
    }

    private void DisableControl(bool isActive)
    {
        interactionSystem.enabled = isActive;
        playerControl.enabled= isActive;
    }
}
