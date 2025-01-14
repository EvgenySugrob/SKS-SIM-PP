using DG.Tweening;
using DoKiSan.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableTestChecker : MonoBehaviour, IInteractableObject
{
    [Header("Main position and rotation")]
    [SerializeField] Transform nfActivePosition;
    [SerializeField] Transform _currentCable;
    [SerializeField] bool markingIsActiv = false;
    [SerializeField] bool screenActive = false;
    private Transform _prevCable;
    private Quaternion _startRotation;
    private Quaternion _startEyesRotation;
    private Vector3 _startEyesPosition;
    private Vector3 _startPosition;
    private Transform _startParent;
    private Transform _startEyesParent;

    [Header("Check limits on work")]
    [SerializeField] InteractionSystem interactionSystem;
    [SerializeField] FirstPlayerControl firstPlayerControl;
    [SerializeField] Transform eyesPlayer;
    private bool toolsInHand = false;

    [Header("UITool")]
    [SerializeField] GameObject backgroundUI;
    [SerializeField] RectTransform logoBackground;
    [SerializeField] GameObject mainMenu;

    [Header("Buttons on tool")]
    [SerializeField] BoxCollider[] buttonsColliders = new BoxCollider[7];

    [Header("Sockets")]
    [SerializeField] SpawnSockets spawnSockets;

    private void Start()
    {
        _startPosition = transform.localPosition;
        _startRotation = transform.localRotation;
        _startParent = transform.parent;

        _startEyesPosition = eyesPlayer.localPosition;
        _startEyesRotation = eyesPlayer.localRotation;
        _startEyesParent = eyesPlayer.parent;
    }

    private void Update()
    {
        if(markingIsActiv)
        {

        }
    }

    public void StartChecker()
    {
        if (interactionSystem.IsCablePartMovingActive())
            return;

        markingIsActiv = true;
        gameObject.SetActive(true);

        StartCoroutine(MoveOnJobPosition());
        interactionSystem.ForcedSetHeldObject(gameObject);
        ActiveSockets(true);
        toolsInHand = true;
    }

    private IEnumerator MoveOnJobPosition()
    {
        PlayerControlDisable(false);
        yield return MoveUp();

        PlayerControlDisable(true);
        yield return LoadUITool();

        screenActive = true;
        backgroundUI.SetActive(false);
        mainMenu.SetActive(true);
    }

    private YieldInstruction MoveUp()
    {
        return transform.DOMove(nfActivePosition.position,1f)
            .Play()
            .WaitForCompletion();
    }

    private YieldInstruction LoadUITool()
    {
        backgroundUI.SetActive(true);
        Vector3 grow = new Vector3(1.5f, 1.5f, 1.5f);
        return logoBackground.DOScale(grow, 2f)
            .Play()
            .WaitForCompletion();
    }

    private void ActiveButtonsOnTool(bool isAcive)
    {
        for (int i = 0; i < buttonsColliders.Length; i++)
        {
            buttonsColliders[i].enabled = isAcive;
        }
    }

    private void ActiveSockets(bool isActive)
    {
        spawnSockets.ActiveCollidersOnSockets(isActive);
    }

    public bool ToolsInHand()
    {
        return toolsInHand;
    }

    public bool CanInteractable(GameObject objectInteract)
    {
        bool isInteract = false;

        if(toolsInHand)
            isInteract = true;

        return isInteract;
    }

    public void Interact(GameObject objectInteract)
    {
        PlayerControlDisable(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if(objectInteract.TryGetComponent(out MarkSocket socket))
        {
            StartCoroutine(MoveEyesToPointAndTools(socket.GetEyesPivot(),socket.GetFirstToolPivot()));

            ActiveButtonsOnTool(true);
        }
    }

    private IEnumerator MoveEyesToPointAndTools(Transform eyesPoint, Transform firstPivot)
    {
        eyesPlayer.parent = eyesPoint;
        yield return MoveEyes();

        transform.parent = firstPivot;
        yield return MoveTools();
    }

    private YieldInstruction MoveEyes()
    {
        return DOTween.Sequence()
            .Append(eyesPlayer.DOLocalMove(Vector3.zero, 1f))
            .Join(eyesPlayer.DOLocalRotate(Vector3.zero, 1f))
            .Play()
            .WaitForCompletion();
    }

    private YieldInstruction MoveTools()
    {
        return DOTween.Sequence()
            .Append(transform.DOLocalMove(Vector3.zero, 1f))
            .Join(transform.DOLocalRotate(Vector3.zero, 1f))
            .Play()
            .WaitForCompletion();
    }

    private void PlayerControlDisable(bool isActive)
    {
        interactionSystem.enabled = isActive;
        firstPlayerControl.enabled = isActive;
    }
}
