using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackConnetctAndSetting : MonoBehaviour
{
    [Header("General")]
    [SerializeField] bool isActiveWork = false;
    [SerializeField] bool wiresIsDone = false;
    [SerializeField] bool isJackMontageDone = false;
    [SerializeField] bool isContactsCrimping=false;
    [SerializeField] CablePointBezier cablePoint;
    [SerializeField] Camera mainCamera;
    [SerializeField] float interactDistance = 2f;
    private float crimpingContactProgress = 0f;
    private Vector3 _prevPosition;
    private Vector3 _startPosition;
    private bool _isSwiping;

    [Header("Point information")]
    [SerializeField] Transform cameraPoint;
    [SerializeField] Transform startPoint;

    [Header("JackModel")]
    [SerializeField] Transform jack;
    [SerializeField] GameObject cableMeshFlat;
    [SerializeField] GameObject mainCableBody;
    [SerializeField] JackWireSlotInfo[] wiresSlots = new JackWireSlotInfo[8];
    [SerializeField] JackWireSlotInfo currentSlot;
    [SerializeField] Transform contacts;
    private Tween contactsAnim;
    private float startOffsetY;
    private float offsetY = -0.00403f;

    private void Start()
    {
        Debug.Log(gameObject.name);
        startOffsetY = contacts.localPosition.y;

        contactsAnim = contacts.DOLocalMoveY(offsetY, 1f)
            .SetAutoKill(false)
            .SetEase(Ease.Linear)
            .Pause();
    }

    private void Update()
    {
        if(isActiveWork)
        {
            ProcessClickDown();
            ProcessClickUp();
            ProcessSwipe();
        }
    }

    public Transform GetCameraPoint()
    {
        return cameraPoint;
    }

    public void StartJackMontage()
    {
        isActiveWork = true;

        for (int i = 0; i < wiresSlots.Length; i++)
        {
            wiresSlots[i].gameObject.SetActive(true);
            wiresSlots[i].ColliderActive(true);
        }
        jack.gameObject.SetActive(true);
        cableMeshFlat.SetActive(true);
        mainCableBody.SetActive(false);
    }

    public void EndJackMontage()
    {
        isActiveWork = false;

        for (int i = 0; i < wiresSlots.Length; i++)
        {
            wiresSlots[i].ColliderActive(false);
            wiresSlots[i].gameObject.SetActive(false);
        }
    }

    private void ProcessClickUp()
    {
        if (Input.GetMouseButtonUp(0) == false)
            return;

        if (cablePoint != null)
        {
            if (currentSlot != null)
            {
                if (!currentSlot.GetStateSlot())
                {
                    cablePoint.SetJackSlot(currentSlot);
                    cablePoint.transform.position = currentSlot.transform.position;
                    cablePoint.GetStartGroup().position = currentSlot.GetPositionForStartGroup().position;
                    cablePoint.GetComponent<IDisableColliders>().DisableCollider(true);

                    currentSlot.SetBusySlot(true);
                    currentSlot.SetCableInSlot(cablePoint);
                }
                else
                {
                    currentSlot.SwapWireSlot(cablePoint, _startPosition);
                    cablePoint.GetComponent<IDisableColliders>().DisableCollider(true);
                }
                currentSlot.SelectPort(false);

                if(CheckWiresReplaceInSlots())
                {
                    //показывать кнопку UI дл€ перехода к нат€гиванию джека
                    ActiveJackCollider();
                }
            }
            else
            {
                cablePoint.transform.position = _startPosition;
                cablePoint.GetComponent<IDisableColliders>().DisableCollider(true);
            }
            
        }

        cablePoint = null;
        _isSwiping = false;
    }

    private void ProcessSwipe()
    {
        if (_isSwiping == false)
            return;

        if (_prevPosition == Input.mousePosition)
            return;

        //if (cablePoint == null || _interactPointHandler == null)
        //    return;

        if (cablePoint != null)
        {
            _prevPosition = Input.mousePosition;

            Ray touchRay = GetRay(_prevPosition);
            float fixedY = cablePoint.transform.position.y;
            Plane plane = new Plane(Vector3.up, new Vector3(0, fixedY, 0));

            if (plane.Raycast(touchRay, out float hitDistance))
            {
                Vector3 point = touchRay.GetPoint(hitDistance);
                point.y = fixedY;
                if (cablePoint != null)
                {
                    cablePoint.transform.position = point;
                }
            }

            if (Physics.Raycast(touchRay, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out JackWireSlotInfo jackWireSlot))
                {
                    if (currentSlot == null)
                    {
                        currentSlot = jackWireSlot;
                    }
                    if (jackWireSlot != currentSlot)
                    {
                        currentSlot.SelectPort(false);
                        currentSlot = jackWireSlot;
                    }
                }
                else
                {
                    if (currentSlot != null)
                    {
                        currentSlot.SelectPort(false);
                    }
                    currentSlot = null;
                }

                if (currentSlot != null && jackWireSlot == currentSlot)
                {
                    currentSlot.SelectPort(true);
                }
                else if (currentSlot != null && jackWireSlot != currentSlot)
                {
                    currentSlot.SelectPort(false);
                }
            }
        }
    }

    private void ProcessClickDown()
    {
        if (Input.GetMouseButtonDown(0) == false)
            return;

        _isSwiping = true;
        _prevPosition = Input.mousePosition;

        if (Physics.Raycast(GetRay(_prevPosition), out RaycastHit hit))
        {
            if (hit.collider.TryGetComponent(out CablePointBezier cablePoint))
            {
                this.cablePoint = cablePoint;
                _startPosition = cablePoint.transform.position;
                cablePoint.GetComponent<IDisableColliders>().DisableCollider(false);
            }

            if (hit.collider.tag == jack.tag && wiresIsDone)
            {
                StartCoroutine(StartAnimationJack());
            }
        }
    }

    private Ray GetRay(Vector3 position) => mainCamera.ScreenPointToRay(position);

    private bool CheckWiresReplaceInSlots()
    {
        bool isDone = true;

        for (int i = 0; i < wiresSlots.Length; i++)
        {
            if (wiresSlots[i].GetStateSlot() == false)
            {
                isDone = false;
                break;
            }
        }

        return isDone;
    }

    public void ActiveJackCollider()
    {
        wiresIsDone= true;
        jack.GetComponent<BoxCollider>().enabled= true;
    }

    private IEnumerator StartAnimationJack()
    {
        isActiveWork = false;

        for (int i = 0; i < wiresSlots.Length; i++)
        {
            wiresSlots[i].ColliderActive(false);
            wiresSlots[i].GetCablePoint().DisableCollider(false);
            wiresSlots[i].gameObject.SetActive(false);
        }

        jack.GetComponent<BoxCollider>().enabled= false;
        jack.GetComponent<OutlineManager>().EnableOutline(false);

        yield return jack.DOLocalMove(Vector3.zero,1f)
            .Play()
            .WaitForCompletion();

        isJackMontageDone= true;
    }

    public bool GetMontageDone()
    {
        return isJackMontageDone;
    }

    public void ContactCrimpingProgress(float progress)
    {
        if (crimpingContactProgress < progress)
        {
            crimpingContactProgress = progress;
            contactsAnim.Goto(crimpingContactProgress * contactsAnim.Duration(), false);
        }
    }

    public void EndContactCrimpingProgress()
    {
        isContactsCrimping = true;
    }

    public bool GetEndContactCrimping()
    {
        return isContactsCrimping;
    }

    public Transform GetStartPoint()
    {
        return startPoint;
    }

    public int[] GetArrayIndexCable()
    {
        int[] arrayCableIndex = new int[wiresSlots.Length];

        for (int i = 0; i < wiresSlots.Length; i++)
        {
            arrayCableIndex[i] = wiresSlots[i].GetCablePoint().GetIndexNumberCable();
        }

        return arrayCableIndex;
    }
}
