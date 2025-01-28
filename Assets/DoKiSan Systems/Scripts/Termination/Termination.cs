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
    [SerializeField] GameObject btCanvas;
    [SerializeField] GameObject btBackView;

    [Header("Remove mode")]
    [SerializeField] bool isRemoveMod=false;
    [SerializeField] Transform removeModels;
    private ContactPortInteract _currentPort;
    private ContactPortInteract _prevPort;

    private void Start()
    {
        _startPosition = transform.localPosition;
        _startToolPosition = terminationTool.localPosition;
        _startToolRotation = terminationTool.localEulerAngles;
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
                ContactPortInteract port = targetObject.GetComponent<ContactPortInteract>();

                if (Input.GetMouseButtonDown(0))
                {
                    if(port.GetStateSlot() && !port.GetTerminationState())
                    {
                        StartCoroutine(MoveCutPositionAndCutCable(_currentPort.GetFirstPoint(), _currentPort.GetSecondPoint()));
                    }
                    else if(port.GetStateSlot() && port.GetTerminationState())
                    {
                        StartCoroutine(MoveRemovePosition(_currentPort.GetFirstPoint(), _currentPort.GetSecondPoint()));
                    }
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
    private void ActiveReturnBtCanvas(bool isActive)
    {
        btBackView.SetActive(isActive);
        btCanvas.SetActive(isActive);
    }
    public void BtBackViewOnClick()
    {
        transform.parent = _startParent;
        terminationTool.position = _startPosition;
        _isActive = false;

        currentContactMount.DisablePortsColliders(false);
        currentContactMount.ColliderDisable(true);
        currentContactMount.TerminationDone(true);
        currentContactMount = null;

        panelInteraction.DisableCollider(true);
        panelInteraction.CableTerminationCountCheck();
        interactionSystem.SetInteract(true);

        if(_currentPort!=null)
        {
            _currentPort.SelectPort(false);
        }
        _currentPort = null;

        interactionSystem.StateCablePartMoving(false);
        interactionSystem.enabled = true;

        terminationTool.localPosition = _startToolPosition;
        transform.parent = _startParent;
        transform.localPosition = _startPosition;

        gameObject.SetActive(false);

        firstPlayerControl.MoveEyesToHead();
        ActiveReturnBtCanvas(false);
    }

    private IEnumerator MoveRemovePosition(Transform point1, Transform point2)
    {
        _isWorkProgress = false;

        terminationTool.gameObject.SetActive(false);
        removeModels.gameObject.SetActive(true);

        yield return MoveRemovePositionAndRemove(point1, point2);
        yield return new WaitForSeconds(.3f);

        _currentPort.CableRemoveFromPort();
        yield return ReturnBackFromRemovePosition(point1);
        

        if (WorkIsDone(-1))
        {
            //что-то будет
        }

        ActiveReturnBtCanvas(false);

        removeModels.gameObject.SetActive(false);
        terminationTool.gameObject.SetActive(true);
        _isWorkProgress = true;
    }

    private YieldInstruction MoveRemovePositionAndRemove(Transform point1, Transform point2)
    {
        return DOTween.Sequence()
            .Append(removeModels.DOMove(point1.position, animationDuration))
            .Join(removeModels.DOLocalRotate(Vector3.zero, animationDuration))
            .Append(removeModels.DOMove(point2.position, animationDuration))
            .Play()
            .WaitForCompletion();
    }

    private YieldInstruction ReturnBackFromRemovePosition(Transform point1)
    {
        return DOTween.Sequence()
            .Append(removeModels.DOMove(point1.position, 0.2f))
            .Append(removeModels.DOLocalMove(_startToolPosition, 0.2f))
            .Join(removeModels.DOLocalRotate(_startToolRotation, 0.2f))
            .Play()
            .WaitForCompletion();
    }

    private IEnumerator MoveCutPositionAndCutCable(Transform point1, Transform point2)
    {
        _isWorkProgress = false;
        _currentPort.SelectPort(false);

        yield return MoveCutPositionAndCut(point1,point2);
        _currentPort.CableAfterDriving();
        yield return new WaitForSeconds(.3f);
        yield return ReturnBack(point1);

        if (WorkIsDone(1))
        {
            ActiveReturnBtCanvas(true);
            //yield return EndWorkProgress();
            //firstPlayerControl.MoveEyesToHead();
        }
        else
        {
            ActiveReturnBtCanvas(false);
            
        }
        _isWorkProgress = true;
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
        //_currentPort.ActiveBoxColliderPort(false);
        
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
        else if (currentContactMount.CheckAllPortReady()&&_isActive)
        {
            _isWorkProgress = false;
            interactionSystem.SetInteract(true);
            interactionSystem.enabled = true;

            terminationTool.localPosition = _startToolPosition;
            transform.parent = _startParent;
            transform.localPosition = _startPosition;

            _isActive= false;
            gameObject.SetActive(false);
        }
        
    }

    public bool WorkIsDone(int i)
    {
        Debug.Log(i);
        bool isDone = false;

        _currentCountIsDone += i;
        Debug.Log(_currentCountIsDone);

        if(_currentCountIsDone<0)
            _currentCountIsDone = 0;
        
        if (_currentCountIsDone == _countPorts)
        {
            isDone = true;
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
