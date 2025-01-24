using DG.Tweening;
using DoKiSan.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

public class CableTestChecker : MonoBehaviour, IInteractableObject
{
    [Header("Main position and rotation")]
    [SerializeField] OutlineDetection outlineDetection;
    [SerializeField] Transform nfActivePosition;
    [SerializeField] Transform nfPortCheckPosition;
    [SerializeField] Transform nfsTool;
    [SerializeField] Transform jackPortScan;
    [SerializeField] Transform[] pathJackToRJConnect = new Transform[0];
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
    private Vector3[] rjSlotPath = new Vector3[3];

    [Header("Check limits on work")]
    [SerializeField] InteractionSystem interactionSystem;
    [SerializeField] FirstPlayerControl firstPlayerControl;
    [SerializeField] Transform eyesPlayer;
    [SerializeField]private bool toolsInHand = false;

    [Header("UITool")]
    [SerializeField] GameObject backgroundUI;
    [SerializeField] RectTransform logoBackground;
    [SerializeField] GameObject mainMenu;
    [SerializeField] CableTesterUIControl uiControll;

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
    [SerializeField] Transform patchCordParent;
    private Vector3 _startPatchCordPosition;
    private Vector3 _startPartInToolPosition;
    private Vector3 _startPartInSocketPosition;

    [Header("NFS tool working")]
    [SerializeField] float detectionRange = 5;
    [SerializeField] Camera playerCamera;
    private List<MarkCable> _cableList = new List<MarkCable>();
    private MarkCable _currentMarkCable;
    private NfsController _nfsController;

    [Header("Check for correct termination")]
    [SerializeField] PatchPanelInteraction panelInteraction;
    [SerializeField] PortConnectInfo currentPortConnect;
    [SerializeField] Transform nfrChecker;
    [SerializeField] Transform nfrJack;
    [SerializeField] Transform nfrJackTool;
    [SerializeField] Transform nfrJackSocket;
    [SerializeField] Transform nfrJackConnectPosition;
    [SerializeField] Transform jackFirstPoint;
    [SerializeField] Transform jackFinalPoint;
    [SerializeField] bool isSearchtSocketTermination;
    [SerializeField] bool isNfrInSocket;
    private Vector3 _startNfrPosition;
    private Vector3 _nfrJackStartPosition;
    private Vector3 _nfrJackToolPartStartPosition;
    private Vector3 _nfrJackSocketStartPosition;

    private void Start()
    {
        _cableList = spawnSockets.GetCablePool();

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

        _startNfrPosition = nfrChecker.localPosition;
        _nfrJackStartPosition = nfrJack.localPosition;
        _nfrJackToolPartStartPosition = nfrJackTool.localPosition;
        _nfrJackSocketStartPosition = nfrJackSocket.localPosition;

        for (int i = 0; i < pathJackToRJConnect.Length; i++)
        {
            rjSlotPath[i] = pathJackToRJConnect[i].localPosition;
        }
    }

    private void Update()
    {
        if(markingIsActiv)
        {
            SearchMarkingCable();
        }
        else
        {
            _nfsController.DisableAllDiods();
        }
    }

    private bool IsNearbyCable(MarkCable cable)
    {
        if (cable == null || currentSocket == null) return false;

        int targetCableIndex = _cableList.FindIndex(cable => cable.GetBoundSocket() == currentSocket);
        int nearbyCableIndex = _cableList.IndexOf(cable);

        return Mathf.Abs(targetCableIndex - nearbyCableIndex) == 1;
    }

    private void SearchMarkingCable()
    {
        GameObject detectedObject = outlineDetection.GetCurrentObject();
        _currentMarkCable = null;
        int countLight = 0;

        if (detectedObject != null && detectedObject.GetComponent<MarkCable>())
        {
            MarkCable markCable = detectedObject.GetComponent<MarkCable>();

            float distanceToObject = Vector3.Distance(detectedObject.transform.position, playerCamera.transform.position);

            if (distanceToObject <= detectionRange)
            {
                _currentMarkCable = markCable;

                if (_currentMarkCable.GetBoundSocket() == currentSocket)
                {
                    countLight = 3; 
                    if(Input.GetMouseButtonDown(0))
                    {
                        StartCoroutine(NfsTesterBackInHand());
                        _currentMarkCable.StartMarkingCable(this);
                        markingIsActiv = false;
                        firstPlayerControl.enabled=false;
                        nfsTool.gameObject.SetActive(false);
                    }
                }
                else if (IsNearbyCable(_currentMarkCable))
                {
                    countLight = 2; 
                }
                else
                {
                    countLight = 1; 
                }
            }
        }
        _nfsController.EnableDiods(countLight);
    }

    public void ReturnTesterInHand()
    {
        PatchCordOnStartPosition();
        TesterOnStartPosition();
    }

    private void PatchCordOnStartPosition()
    {
        partInTools.localPosition = _startPartInToolPosition;
        partInSocket.localPosition = _startPartInSocketPosition;
        patchCord.localPosition = _startPatchCordPosition;
        
        patchCord.gameObject.SetActive(false);
    }

    private void TesterOnStartPosition()
    {
        transform.gameObject.SetActive(false);
        transform.parent = _startParent;
        transform.localPosition = _startPosition;
        transform.localRotation = _startRotation;

        uiControll.OpenMainMenuAfterWork();

        EnableTester();
    }

    private IEnumerator NfsTesterBackInHand()
    {
        yield return nfsTool.DOLocalMove(_startNFSToolPosition, 1f)
            .Play()
            .WaitForCompletion();
    }


    public void StartChecker()
    {
        if (interactionSystem.GetHeldObject() != null && interactionSystem.GetHeldObject()!=transform.gameObject)
            return;
        if (interactionSystem.IsCablePartMovingActive())
            return;

        if(!markingIsActiv && !isSearchtSocketTermination && !toolsInHand)
        {
            EnableTester();
        }
        else if(!markingIsActiv && !isSearchtSocketTermination && toolsInHand)
        {
            DisableTester();
        }
    }

    private void EnableTester()
    {
        gameObject.SetActive(true);

        StartCoroutine(MoveOnJobPosition());
        interactionSystem.ForcedSetHeldObject(gameObject);
        ActiveSockets(true);
        toolsInHand = true;

        if(panelInteraction.GetTesterDoneInteract() && !panelInteraction.GetMountingState())
        {
            panelInteraction.Flip(true);
        }
    }
    private void DisableTester()
    {
        StartCoroutine(MoveOnStartPosition());
        interactionSystem.ForcedSetHeldObject(null);
        ActiveSockets(false);
        toolsInHand = false;

        if (panelInteraction.GetTesterDoneInteract() && !panelInteraction.GetMountingState())
        {
            panelInteraction.Flip(false);
        }
    }

    private IEnumerator MoveOnStartPosition()
    {
        PlayerControlDisable(false);
        yield return transform.DOLocalMove(_startPosition, 0.5f)
            .Play()
            .WaitForCompletion();
        //gameObject.SetActive(false);
        PlayerControlDisable(true);
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

        if (objectInteract.GetComponent<TwistedPairUnravelingCount>())
            isInteract = false;

        Debug.Log(isInteract + " Tester");
        return isInteract;
    }

    public void Interact(GameObject objectInteract)
    {
        if(objectInteract.TryGetComponent(out MarkSocket socket) && !isSearchtSocketTermination)
        {
            PlayerControlDisable(false);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            currentSocket = socket;
            StartCoroutine(MoveEyesToPointAndTools(currentSocket.GetEyesPivot(),currentSocket.GetFirstToolPivot()));
        }

        if(objectInteract.TryGetComponent(out PatchPanelInteraction checkCorrect) && !isSearchtSocketTermination)
        {
            if(checkCorrect.GetFliping())
            {
                ShowNfr();
            }
        }

        if(objectInteract.TryGetComponent(out MarkSocket socketTermination) && isSearchtSocketTermination)
        {
            SearchSocketsForCheckTermination(socketTermination);
        }
        
        if(objectInteract.GetComponent<PatchPanelInteraction>() && isSearchtSocketTermination &&isNfrInSocket)
        {
            if(checkCorrect.GetFliping())
            {
                PlayerMoveOnPositionSelectPort();
            }
        }

        if(objectInteract.GetComponent<PortConnectInfo>() && isSearchtSocketTermination && isNfrInSocket)
        {
            currentPortConnect = objectInteract.GetComponent<PortConnectInfo>();
            ConnectWithPort(currentPortConnect.GetPatchCordConnection(),currentPortConnect.GetPatchCordBetweenConection());
        }
    }

    private void PlayerMoveOnPositionSelectPort()
    {
        panelInteraction.PlayerMoveOnFlipPosition();
    }

    private void ShowNfr()
    {
        StartCoroutine(DisableNFAndShowNfr());
    }
    private IEnumerator DisableNFAndShowNfr()
    {
        PlayerControlDisable(false);
        isSearchtSocketTermination = true;
        nfrChecker.gameObject.SetActive(true);

        yield return DOTween.Sequence()
            .Append(transform.DOLocalMove(_startPosition, 0.3f))
            .SetEase(Ease.Linear)
            .Append(nfrChecker.DOLocalMove(nfActivePosition.localPosition, 0.3f))
            .Play()
            .WaitForCompletion();

        PlayerControlDisable(true);
        spawnSockets.ActiveAllColliders(true);
        
    }

    private void SearchSocketsForCheckTermination(MarkSocket socket)
    {
        PlayerControlDisable(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        currentSocket = socket;

        StartCoroutine(MoveEyesAnNfr(currentSocket.GetEyesPivot(), currentSocket.GetNfrPivot()));
    }

    private IEnumerator MoveEyesAnNfr(Transform eyesPoint, Transform firstPivot)
    {
        eyesPlayer.parent = eyesPoint;
        yield return MoveEyes();

        nfrChecker.parent = firstPivot;
        yield return MoveNfr();

        nfrJack.gameObject.SetActive(true);

        yield return ConnectWithNfr();
        yield return ConncetWithSocket();

        //¬озможно стоить ограничить доступ к розеткам 
        //»ли добавить возможность забрать NFR и отменить его установку
        isNfrInSocket= true;

        yield return EyesReturnBack();
        yield return MoveUp();

        PlayerControlDisable(true);
    }

    private YieldInstruction MoveNfr()
    {
        return nfrChecker.DOLocalMove(Vector3.zero, 0.5f)
            .Play()
            .WaitForCompletion();
    }

    private YieldInstruction ConnectWithNfr()
    {
        return DOTween.Sequence()
            .Append(nfrJack.DOMove(nfrJackConnectPosition.position,0.8f))
            .Append(nfrJackTool.DOMove(jackFirstPoint.position, 0.3f))
            .SetEase(Ease.Linear)
            .Append(nfrJackTool.DOMove(jackFinalPoint.position,0.3f))
            .Play()
            .WaitForCompletion();
    }
    private YieldInstruction ConncetWithSocket()
    {
        return DOTween.Sequence()
            .Append(nfrChecker.DOMove(currentSocket.GetSecondToolPivot().position,1f))
            .Append(nfrJackSocket.DOMove(currentSocket.GetJackBetweenPivot().position,0.4f))
            .Append(nfrJackSocket.DOMove(currentSocket.GetJackPartPivot().position,0.4f))
            .Play()
            .WaitForCompletion();
    }

    public void StartSearch()
    {
        ActiveButtonsOnTool(false);
        spawnSockets.ActiveCollidersOnSockets(false);
        StartCoroutine(SerachToolPosition());
    }

    public bool GetIsSearchSocketTermimnation()
    {
        return isSearchtSocketTermination;
    }

    private IEnumerator MoveEyesToPointAndTools(Transform eyesPoint, Transform firstPivot)
    {
        eyesPlayer.parent = eyesPoint;
        yield return MoveEyes();

        transform.parent = firstPivot;
        yield return MoveTools();
        ActiveButtonsOnTool(true);
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
            .Append(partInSocket.DOMove(currentSocket.GetJackBetweenPivot().position, 0.5f)
            .SetEase(Ease.Linear))
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

    public bool GetIsNfrInSocket()
    {
        return isNfrInSocket;
    }
    public PortConnectInfo GetCurrentPort()
    {
        return currentPortConnect;
    }

    private void ConnectWithPort(Transform patchCordEndPosition,Transform patchCordBetweenPosition)
    {
        ActiveButtonsOnTool(false);
        PlayerControlDisable(false);
        panelInteraction.PortsEnable(false);
        StartCoroutine(PatchCordConnectWithPort(patchCordEndPosition,patchCordBetweenPosition));
    }
    private IEnumerator PatchCordConnectWithPort(Transform patchCordEndPosition, Transform patchCordEndBetweenPosition)
    {
        patchCord.gameObject.SetActive(true);

        yield return PatchCordMove(patchCordConnectPosition.position);
        yield return PatchCordConnectWithNF();
        yield return PatchCordConnectWithPortSlot(patchCordEndPosition,patchCordEndBetweenPosition);
        yield return MoveNfOnCheckPosition();
        ActiveButtonsOnTool(true);
        //PlayerControlDisable(true);
    }
    private YieldInstruction PatchCordMove(Vector3 position)
    {
        return patchCord.DOMove(position,0.5f)
            .Play()
            .WaitForCompletion();
    }
    private YieldInstruction PatchCordConnectWithNF()
    {
        for (int i = 0; i < pathJackToRJConnect.Length; i++)
        {
            rjSlotPath[i] = pathJackToRJConnect[i].position;
        }

        return DOTween.Sequence()
            .Append(partInTools.DOPath(rjSlotPath,1f,PathType.CatmullRom))
            .SetEase(Ease.Linear)
            .Join(partInTools.DORotateQuaternion(pathJackToRJConnect[1].rotation,0.5f))
            .Play()
            .WaitForCompletion();
    }
    private YieldInstruction PatchCordConnectWithPortSlot(Transform endPosition,Transform betweenPosition)
    {
        partInSocket.parent = endPosition.parent;

        return DOTween.Sequence()
            .Append(partInSocket.DOMove(betweenPosition.position, 0.5f))
            .SetEase(Ease.Linear)
            .Join(partInSocket.DORotateQuaternion(betweenPosition.rotation,0.5f))
            .Append(partInSocket.DOMove(endPosition.position,0.5f))
            .Play()
            .WaitForCompletion();
    }
    private YieldInstruction MoveNfOnCheckPosition()
    {
        return transform.DOMove(nfPortCheckPosition.position,0.8f)
            .Play()
            .WaitForCompletion();
    }
}
