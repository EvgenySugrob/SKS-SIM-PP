using DG.Tweening;
using DoKiSan.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamFormation : MonoBehaviour
{
    [Header("AllInteractivePartCable")]
    [SerializeField] List<InteractivePuchok> interactivePartsCable;
    [SerializeField] bool isFormationProgress = false;
    [SerializeField] bool isSelectedCable = false;
    [SerializeField] List<Transform> secondInteractivePartsCable;
    [SerializeField] List<GameObject> cableForBeaming;
    [SerializeField] float radius = 0.01f;
    [SerializeField] float notInteractRadius = 0.005f;

    [Header("Player")]
    [SerializeField] Transform eyesPlayer;
    [SerializeField] FirstPlayerControl playerControl;
    [SerializeField] InteractionSystem interactionSystem;
    [SerializeField] OutlineDetection outlineDetection;
    [SerializeField] Camera playerCamera;
    [SerializeField] float distance = 2f;
    private Transform _eyesStartParent;
    private Vector3 _eyesStartPosition;
    private Quaternion _eyesStartRotation;
    private GameObject _currentObj;
    private InteractivePuchok _currentPuchok;
    private InteractivePuchok _prevPuchok;

    [Header("PatchPanel")]
    [SerializeField] PatchPanelInteraction panelInteraction;
    [SerializeField] Transform formationBetweenPoint;
    [SerializeField] Transform formationPoint;

    [Header("UIBack")]
    [SerializeField] GameObject uiFormation;
    [SerializeField] GameObject backBt;
    [SerializeField] GameObject acceptFormation;

    [Header("Crimping")]
    [SerializeField] LineRenderer[] crimpingLine = new LineRenderer[6];

    private void Start()
    {
        _eyesStartParent = eyesPlayer.parent;
    }

    private void Update()
    {
        if(isFormationProgress)
        {
            SearchInteractionPart();
        }
    }

    private void SearchInteractionPart()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray,out RaycastHit hit, distance))
        {
            _currentObj = hit.collider.gameObject;

            if(_currentObj.GetComponent<InteractivePuchok>())
            {
                _currentPuchok = _currentObj.GetComponent<InteractivePuchok>();

                if(_prevPuchok == null)
                {
                    _prevPuchok = _currentPuchok;
                }
                if(_prevPuchok!=_currentPuchok)
                {
                    _prevPuchok.OutlineActive(false);
                    _prevPuchok = _currentPuchok;
                }
            }

            if(_currentPuchok != null && _currentObj.GetComponent<InteractivePuchok>())
            {
                _currentPuchok.OutlineActive(true);

                if (Input.GetMouseButtonDown(0))
                {
                    SelectInteractivePuchok();
                }
            }
            else if(_currentPuchok != null && _currentObj.GetComponent<InteractivePuchok>() ==false)
            {
                _currentPuchok.OutlineActive(false);
            }
        }
        else
        {
            if(_currentPuchok!= null)
            {
                _currentPuchok.OutlineActive(false);
                _currentPuchok=null;
            }
        }
    }

    private void SelectInteractivePuchok()
    {
        if(!cableForBeaming.Contains(_currentObj))
        {
            cableForBeaming.Add(_currentObj);
            _currentPuchok.SelectedOnGroup(true);
            Debug.Log($"Dobavil {_currentObj.name}");
        }
        else
        {
            cableForBeaming.Remove(_currentObj);
            _currentPuchok.SelectedOnGroup(false);
            Debug.Log($"Ubral {_currentObj.name}");
        }

        if(cableForBeaming.Count>0)
        {
            isSelectedCable = true;
            backBt.SetActive(false);
            acceptFormation.SetActive(true);
        }
        else
        {
            isSelectedCable = false;
            backBt.SetActive(true);
            acceptFormation.SetActive(false);
        }

    }

    public void BeamFormationCableInteractionPart()
    {
        acceptFormation.SetActive(false);

        Vector3 center = Vector3.zero;
        List<Transform> notInteractPoints = new List<Transform>();

        foreach (GameObject cable in cableForBeaming)
        {
            center += cable.transform.position;
            notInteractPoints.Add(cable.GetComponent<InteractivePuchok>().GetNotInteractPuchok());
        }

        center /= cableForBeaming.Count;

        float angleStep = 360f / cableForBeaming.Count;

        for (int i = 0; i < cableForBeaming.Count; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;

            Vector3 newPosition = new Vector3(
            center.x + radius * Mathf.Cos(angle),
            center.y,
            center.z + radius * Mathf.Sin(angle));

            cableForBeaming[i].GetComponent<InteractivePuchok>().SetNewPosition(newPosition);
        }
        NotInteractCalculatePosition(notInteractPoints);
        
    }

    private void NotInteractCalculatePosition(List<Transform> notInteractPoints)
    {
        Vector3 center = Vector3.zero;
        foreach (Transform point in notInteractPoints)
        {
            center += point.transform.position;
        }

        center /= notInteractPoints.Count;

        float angleStep = 360f / notInteractPoints.Count;

        for (int i = 0; i < cableForBeaming.Count; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;

            Vector3 newPosition = new Vector3(
            center.x + notInteractRadius * Mathf.Cos(angle),
            center.y,
            center.z + notInteractRadius * Mathf.Sin(angle));

            cableForBeaming[i].GetComponent<InteractivePuchok>().SetNotInteractNewPosition(newPosition);
        }

        foreach (GameObject puchock in cableForBeaming)
        {
            puchock.GetComponent<InteractivePuchok>().SelectedOnGroup(false);
        }

        cableForBeaming.Clear();
        backBt.SetActive(true);
    }

    public void StartFormationFormationBtClick()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        StartMoveEyesToPoint();
    }

    public void ReturnToMainViewBtClick()
    {
        uiFormation.SetActive(false);
        backBt.SetActive(false);

        playerControl.enabled = false;
        playerControl.SwitchTypeMovePlayer(false);

        StartCoroutine(MoveEyesBack());
    }

    private void StartMoveEyesToPoint()
    {
        panelInteraction.DisableCollider(false);
        outlineDetection.ForceDisableOutline();
        outlineDetection.enabled = false;

        isFormationProgress = true;

        _eyesStartPosition = eyesPlayer.position;
        _eyesStartRotation = eyesPlayer.rotation;

        DisableControl(false);
        StartCoroutine(MoveEyes());
    }

    private IEnumerator MoveEyes()
    {
        eyesPlayer.parent = null;

        yield return Move(formationBetweenPoint,formationPoint.position,formationPoint.rotation);

        InteractivePartCableActive(true);
        eyesPlayer.parent = formationPoint;
        playerControl.SwitchTypeMovePlayer(true);
        playerControl.enabled = true;

        uiFormation.SetActive(true);
        backBt.SetActive(true);
    }

    private YieldInstruction Move(Transform betweenPosition, Vector3 endPosition, Quaternion endRotation)
    {
        return DOTween.Sequence()
            .Append(eyesPlayer.DOMove(betweenPosition.position, 1f).SetEase(Ease.Linear))
            .Append(eyesPlayer.DOMove(endPosition, 1f))
            .Join(eyesPlayer.DORotateQuaternion(endRotation, 1f))
            .Play()
            .WaitForCompletion();
    }

    private IEnumerator MoveEyesBack()
    {
        InteractivePartCableActive(false);
        eyesPlayer.parent = null;

        yield return Move(formationBetweenPoint,_eyesStartPosition,_eyesStartRotation);

        eyesPlayer.parent = _eyesStartParent;
        DisableControl(true);
        panelInteraction.DisableCollider(true);

        isFormationProgress = false;
        outlineDetection.enabled = true;
    }

    private void DisableControl(bool isActive)
    {
        interactionSystem.enabled = isActive;
        playerControl.enabled= isActive;
    }

    private void InteractivePartCableActive(bool isActive)
    {
        foreach (InteractivePuchok puchok in interactivePartsCable)
        {
            puchok.ActiveCollider(isActive);
        }
    }
}
