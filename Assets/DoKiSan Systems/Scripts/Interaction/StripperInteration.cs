using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class StripperInteration : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] DecalProjector projectorCutLine;
    [SerializeField] float rayDistance;
    [SerializeField] float offsetZ = 0.15f;
    [SerializeField] Transform stripperPoint;
    [SerializeField] Transform stripperRotationPivot;
    [SerializeField] Transform stripperUperPart;
    [SerializeField] LayerMask layerMask;

    private Material projectorMaterial;
    [SerializeField]private bool _isEnableStripper=false;
    private string shaderState = "_currentCutPlace";
    private Transform startParent;
    private Vector3 startStripperPosition;

    private void OnDisable()
    {
        SwitchColor(false);
    }
    private void Start()
    {
        projectorMaterial = projectorCutLine.material;
        startParent = transform.parent;
        startStripperPosition = transform.localPosition;
    }
    private void Update()
    {
        if (_isEnableStripper)
            DetectionCutbleSection();
    }

    private void DetectionCutbleSection()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance,layerMask))
        {
            GameObject targetObjecet = hit.collider.gameObject;

            Vector3 stripperPosition = hit.point;
            stripperPoint.position = stripperPosition;
            transform.localPosition = new Vector3(transform.position.x, stripperPoint.localPosition.y, offsetZ);


            if (targetObjecet.CompareTag("cutPlace"))
            {
                SwitchColor(true);
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 pointForRtPivot = targetObjecet.transform.position;
                    _isEnableStripper = false;
                    targetObjecet.GetComponent<IDisableColliders>().DisableCollider(false);
                    StartCoroutine(StartCutProcess(pointForRtPivot));
                }
            }
            else
            {
                SwitchColor(false);
            }
        }
    }

    private void SwitchColor(bool isState)
    {
        int intValue = isState ? 1 : 0;
        projectorMaterial.SetInt(shaderState, intValue);
    }
    public bool StateEnableStripper()
    {
        return _isEnableStripper;
    }

    public void SetEnableSripperState(bool state)
    {
        _isEnableStripper = state;
    }
    public void EndStripping()
    {
        SetEnableSripperState(false);
        projectorCutLine.enabled = true;
        SwitchColor(false);
    }

    private IEnumerator StartCutProcess(Vector3 pointForRtPivot)
    {
        projectorCutLine.enabled= false;
        yield return BeginMoveRotateRotationPivot(pointForRtPivot);

        BodyCutOnPart cutPart = transform.parent.GetComponent<BodyCutOnPart>();
        yield return cutPart.DeletCutPart();
        cutPart.DisableCutPart();

        yield return ReturnFromStartPosition();
        gameObject.SetActive(false);
    }

    private YieldInstruction BeginMoveRotateRotationPivot(Vector3 point)
    {
        Vector3 startPoint = stripperRotationPivot.transform.localPosition;

        return DOTween.Sequence()
            .Append(stripperRotationPivot.DOMove(point,1f))
            .Append(stripperUperPart.DOLocalRotate(Vector3.right*20,1f,RotateMode.LocalAxisAdd))
            .Append(stripperRotationPivot.DOLocalRotate(Vector3.up*360,0.75f,RotateMode.FastBeyond360))
            .Append(stripperUperPart.DOLocalRotate(Vector3.left*20,1f,RotateMode.LocalAxisAdd))
            .Append(stripperRotationPivot.DOLocalMove(startPoint,1f))
            .Play()
            .WaitForCompletion();
    }
    private YieldInstruction ReturnFromStartPosition()
    {
        transform.parent = startParent;
        return transform
            .DOLocalMove(startStripperPosition, 1f)
            .Play()
            .WaitForCompletion();
    }

    public YieldInstruction MoveToPoint(Transform point)
    {
        stripperPoint = point;
        transform.parent = point.parent;
        return transform
            .DOMove(stripperPoint.position, 1f)
            .Play()
            .WaitForCompletion();
    }
}
