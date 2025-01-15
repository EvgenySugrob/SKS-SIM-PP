using DG.Tweening;
using DoKiSan.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableTestChecker : MonoBehaviour, IInteractableObject
{
    [Header("Main position and rotation")]
    [SerializeField] OutlineDetection outlineDetection;
    [SerializeField] Transform nfActivePosition;
    [SerializeField] Transform nfsTool;
    [SerializeField] Transform jackPortScan;
    [SerializeField] bool markingIsActiv = false;
    [SerializeField] bool screenActive = false;
    private Transform _prevCable;
    private Quaternion _startRotation;
    private Quaternion _startEyesRotation;
    private Vector3 _startEyesPosition;
    private Vector3 _startPosition;
    private Vector3 _startNFSToolPosition;
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
    [SerializeField] MarkSocket currentSocket;

    [Header("Patch cord")]
    [SerializeField] Transform patchCord;
    [SerializeField] Transform partInTools;
    [SerializeField] Transform partInSocket;
    [SerializeField] Transform patchCordConnectPosition;
    private Vector3 _startPatchCordPosition;
    private Vector3 _startPartInToolPosition;
    private Vector3 _startPartInSocketPosition;

    private MarkCable _currentMarkCable;
    private NfsController _nfsController;

    private void Start()
    {
        _startPosition = transform.localPosition;
        _startRotation = transform.localRotation;
        _startParent = transform.parent;

        _startNFSToolPosition = nfsTool.localPosition;
        _nfsController = nfsTool.GetComponent<NfsController>();

        _startEyesPosition = eyesPlayer.localPosition;
        _startEyesRotation = eyesPlayer.localRotation;
        _startEyesParent = eyesPlayer.parent;

        _startPatchCordPosition = patchCord.localPosition;
        _startPartInToolPosition = partInTools.localPosition;
        _startPartInSocketPosition = partInSocket.localPosition;
}

    private void Update()
    {
        if(markingIsActiv)
        {
            SearchMarkingCable();
        }
    }

    private void SearchMarkingCable()
    {
        GameObject objectdetect = outlineDetection.GetCurrentObject();
        int countLight = 0;
        if (outlineDetection.GetCurrentObject().GetComponent<MarkCable>())
        {
            _currentMarkCable = objectdetect.GetComponent<MarkCable>();
        }
        else
        {
            _currentMarkCable=null;
        }

        if(_currentMarkCable!=null && _currentMarkCable.GetBoundSocket() != currentSocket)
        {
            countLight = 1;
        }
        else if(_currentMarkCable!=null && _currentMarkCable.GetBoundSocket() == currentSocket)
        {
            countLight = 3;
            //вкл диодов идиотов
        }
        _nfsController.EnableDiods(countLight);
    }

    public void StartChecker()
    {
        if (interactionSystem.IsCablePartMovingActive())
            return;

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
            currentSocket = socket;
            StartCoroutine(MoveEyesToPointAndTools(currentSocket.GetEyesPivot(),currentSocket.GetFirstToolPivot()));

            ActiveButtonsOnTool(true);
        }
    }

    public void StartSearch()
    {
        ActiveButtonsOnTool(false);
        StartCoroutine(SerachToolPosition());
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

    private IEnumerator SerachToolPosition()
    {
        patchCord.gameObject.SetActive(true);
        yield return ConnectWithTool();
        yield return ConnectWithSocket();
        yield return EyesReturnBack();
        yield return NFSToolOnPosition();
        firstPlayerControl.enabled = true;
        markingIsActiv = true;
    }

    private YieldInstruction ConnectWithTool()
    {
        return DOTween.Sequence()
            .Append(patchCord.DOMove(patchCordConnectPosition.position, 1f))
            .Append(partInTools.DOMove(jackPortScan.position, 0.5f))
            .Play()
            .WaitForCompletion();
    }

    private YieldInstruction ConnectWithSocket()
    {
        return DOTween.Sequence()
            .Append(transform.DOMove(currentSocket.GetSecondToolPivot().position, 1f))
            .Append(partInSocket.DOMove(currentSocket.GetJackBetweenPivot().position, 0.5f).SetEase(Ease.Linear))
            .Append(partInSocket.DOMove(currentSocket.GetJackPartPivot().position, 0.5f))
            .Play() 
            .WaitForCompletion();
    }
    private YieldInstruction EyesReturnBack()
    {
        eyesPlayer.parent = _startEyesParent;

        return DOTween.Sequence()
            .Append(eyesPlayer.DOLocalMove(_startEyesPosition,1f))
            .Join(eyesPlayer.DOLocalRotateQuaternion(_startEyesRotation,1f))
            .Play()
            .WaitForCompletion();
    }

    private YieldInstruction NFSToolOnPosition()
    {
        nfsTool.gameObject.SetActive(true);

        return nfsTool.DOLocalMove(nfActivePosition.localPosition,1f)
            .Play() 
            .WaitForCompletion();
    }

    private void PlayerControlDisable(bool isActive)
    {
        interactionSystem.enabled = isActive;
        firstPlayerControl.enabled = isActive;
    }
}
