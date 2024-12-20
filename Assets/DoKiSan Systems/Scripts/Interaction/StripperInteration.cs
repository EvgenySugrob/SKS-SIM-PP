using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] LayerMask layerMask;

    private Material projectorMaterial;
    private bool _isEnableStripper=false;
    private string shaderState = "_currentCutPlace";

    private void Start()
    {
        projectorMaterial = projectorCutLine.material;
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

    public void MoveToStrippingPoint(Transform point)
    {
        stripperPoint = point;
        //движение к точке, поворот точки.
        


        _isEnableStripper = true;
    }
}
