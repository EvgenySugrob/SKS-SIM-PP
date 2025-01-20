using DG.Tweening;
using DoKiSan.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Termination : MonoBehaviour
{
    [Header("Temrmination")]
    [SerializeField] float animationDuration = 1f;
    [SerializeField] Transform terminationTool;
    [SerializeField] Camera playerCamera;
    [SerializeField] float distanceRay;
    private Vector3 _startPosition;
    private Vector3 _startToolPosition;
    private Vector3 _startToolRotation;
    [SerializeField] Transform _startParent;
    [SerializeField] bool _isActive = false;
    private bool _isWorkProgress = false;
    private int _countPorts = 8;
    private int _currentCountIsDone = 0;

    [Header("Check before work")]
    [SerializeField] InteractionSystem interactionSystem;
    [SerializeField] ContactMountMontage currentContactMount;
    [SerializeField] FirstPlayerControl firstPlayerControl;
    [SerializeField] PatchPanelInteraction panelInteraction;
    private ContactPortInteract _currentPort;
    private ContactPortInteract _prevPort;

    private void Start()
    {
        _startPosition = transform.localPosition;
        _startToolPosition = terminationTool.localPosition;
        _startToolRotation = terminationTool.localEulerAngles;
        Debug.Log(_startToolRotation);
        _startParent = transform.parent;
    }

    private void Update()
    {
        if (_isActive && _isWorkProgress)
        {
            CheckPortForTermination();
        }
    }

    private void CheckPortForTermination()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray,out RaycastHit hit,distanceRay))
        {
            GameObject targetObject = hit.collider.gameObject;
            Vector3 toolPosition = hit.point;
            transform.position = new Vector3(toolPosition.x,transform.position.y,toolPosition.z);

            if (NeedsPortOutline(targetObject))
            {
                if(Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(MoveCutPositionAndCutCable(_currentPort.GetFirstPoint(),_currentPort.GetSecondPoint()));
                }
            }
        }
    }

    private bool NeedsPortOutline(GameObject targetObject)
    {
        bool needsPort = false;

        if (targetObject.CompareTag("TerminationCheck"))
        {
            _currentPort = targetObject.GetComponent<ContactPortInteract>();

            if (_prevPort == null)
            {
                _prevPort = _currentPort;
            }
            if (_prevPort != _currentPort)
            {
                _prevPort.SelectPort(false);
                _prevPort = _currentPort;
            }

            if (_currentPort != null)
            {
                _currentPort.SelectPort(true);
            }
            needsPort = true;
        }
        else if (_currentPort != null)
        {
            _currentPort.SelectPort(false);
            _currentPort = null;

            needsPort = false;
        }

        return needsPort;
    }

    private IEnumerator MoveCutPositionAndCutCable(Transform point1, Transform point2)
    {
        _isWorkProgress = false;

        yield return MoveCutPositionAndCut(point1,point2);
        _currentPort.CableAfterDriving();
        yield return new WaitForSeconds(.5f);
        yield return ReturnBack(point1);

        if (WorkIsDone())
        {
            transform.parent = _startParent;
            currentContactMount = null;
            yield return EndWorkProgress();
            _isActive= false;
            firstPlayerControl.MoveEyesToHead();
            panelInteraction.DisableCollider(true);
            panelInteraction.CableTerminationCountCheck();
            interactionSystem.SetInteract(true);

            _currentPort.SelectPort(false);
            _currentPort = null;

            interactionSystem.StateCablePartMoving(false);
            interactionSystem.enabled = true;

            terminationTool.localPosition = _startToolPosition;
            transform.parent = _startParent;
            transform.localPosition = _startPosition;

            gameObject.SetActive(false);
        }
        else
        {
            _isWorkProgress = true;
        }
    }

    private YieldInstruction EndWorkProgress()
    {
        return terminationTool.DOMove(_startPosition,1f)
            .Play()
            .WaitForCompletion();
    }

    private YieldInstruction MoveCutPositionAndCut(Transform point1,Transform point2)
    {
        return DOTween.Sequence()
            .Append(terminationTool.DOMove(point1.position, animationDuration))
            .Join(terminationTool.DOLocalRotate(Vector3.zero, animationDuration))
            .Append(terminationTool.DOMove(point2.position, animationDuration))
            .Play()
            .WaitForCompletion();
    }

    private YieldInstruction ReturnBack(Transform point1)
    {
        _currentPort.ActiveBoxColliderPort(false);
        _currentPort.SelectPort(false);
        return DOTween.Sequence()
            .Append(terminationTool.DOMove(point1.position,animationDuration))
            .Append(terminationTool.DOLocalMove(_startToolPosition,animationDuration))
            .Join(terminationTool.DOLocalRotate(_startToolRotation,animationDuration))
            .Play()
            .WaitForCompletion();
    }

    public void SetCurrentPortInteract(ContactMountMontage contactMount)
    {
        currentContactMount = contactMount;
    }

    public void ReadyToStartWork()
    {
        if (currentContactMount.CheckAllPortReady() && !_isActive)
        {
            _isActive = true;
            interactionSystem.SetInteract(false);
            interactionSystem.enabled = false;
            gameObject.SetActive(true);

            StartCoroutine(MoveOnWorkPosition());

            transform.parent = null;
            _isWorkProgress = true;
        }
        
    }

    public bool WorkIsDone()
    {
        bool isDone = false;

        _currentCountIsDone++;

        if (_currentCountIsDone == _countPorts)
        {
            isDone = true;
            _currentCountIsDone = 0;
        }

        return isDone;
    }

    private IEnumerator MoveOnWorkPosition()
    {
        yield return transform
            .DOLocalMove(currentContactMount.GetTerminationPoint().position, animationDuration)
            .Play()
            .WaitForCompletion();
        yield return new WaitForSeconds(0.5f);
    }
}
