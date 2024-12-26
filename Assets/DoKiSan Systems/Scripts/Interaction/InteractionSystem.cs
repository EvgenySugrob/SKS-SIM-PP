using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] GameObject heldObject;
    [SerializeField] Camera mainCamera;
    [SerializeField] float interactDistance = 3f;
    [SerializeField] GameObject handSlot;

    [Header("Cabple part manipulation")]
    [SerializeField] bool isCablePartMoving=false;
    private bool _isSwiping;
    private Vector3 _prevPosition;
    private CablePointBezier _currentPointBezier;
    private ContactPortInteract _contactInteract;
    private Vector3 _startPosition;


    private void Update()
    {
        if(isCablePartMoving)
        {
            ProcessClickDown();
            ProcessClickUp();
            ProcessSwipe();
        }
    }

    public void StateCablePartMoving(bool state)
    {
        isCablePartMoving= state;
    }
    private void ProcessClickUp()
    {
        if (Input.GetMouseButtonUp(0) == false)
            return;

        if(_currentPointBezier!=null)
        {
            //if(_contactInteract.CheckSlotAndCAble(_currentPointBezier.GetTypeCable())) //проверка на соответсвие схеме на потом
            //{
                
                
            //}
            if(!_contactInteract.GetStateSlot())
            {
                _currentPointBezier.GetComponent<IDisableColliders>().DisableCollider(true);
                _currentPointBezier.transform.position = _contactInteract.GetPointBeforeDriving().position;
                _contactInteract.SetStateSlot(true);
            }
            else
            {
                _currentPointBezier.transform.position = _startPosition;
            }
        }
        _contactInteract.SelectPort(false);
        _currentPointBezier = null;
        _isSwiping = false;
    }

    private void ProcessSwipe()
    {
        if (_isSwiping == false)
            return;

        if (_prevPosition == Input.mousePosition)
            return;

        if (_currentPointBezier == null)
        return;

        _prevPosition= Input.mousePosition;

        Ray touchRay = GetRay(_prevPosition);
        float fixedY = _currentPointBezier.transform.position.y;
        Plane plane = new Plane(Vector3.up, new Vector3(0,fixedY,0));

        if(plane.Raycast(touchRay, out float hitDistance))
        {
            Vector3 point = touchRay.GetPoint(hitDistance);
            point.y = fixedY;
            _currentPointBezier.transform.position = point;
        }

        if(Physics.Raycast(touchRay,out RaycastHit hit))
        {
            if(hit.collider.TryGetComponent(out ContactPortInteract portInteract))
            {
                if(_contactInteract == null)
                {
                    _contactInteract = portInteract;
                }
                if(portInteract!=_contactInteract)
                {
                    _contactInteract.SelectPort(false);
                    _contactInteract = portInteract;
                }
            }

            if(_contactInteract != null && portInteract == _contactInteract)
            {
                _contactInteract.SelectPort(true);
            }
            else if(_contactInteract != null && portInteract != _contactInteract)
            {
                _contactInteract.SelectPort(false);
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
            if (hit.collider.TryGetComponent(out CablePointBezier cablePoint))
            {
                _startPosition = _currentPointBezier.transform.position;
                _currentPointBezier = cablePoint;
                _currentPointBezier.GetComponent<IDisableColliders>().DisableCollider(false);
            }
                
    }

    private Ray GetRay(Vector3 position) => mainCamera.ScreenPointToRay(position);

    public void HandleInteraction()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            GameObject targetObject = hit.collider.gameObject;

            if (heldObject == null)
            {
                if (targetObject.CompareTag("Interactable"))
                {
                    PickupObject(targetObject);
                }
            }
            else
            {
                if (targetObject.CompareTag("Interactable"))
                {
                    TryInteraction(heldObject, targetObject);
                }
            }
        }
    }
    private void PickupObject(GameObject obj)
    {
        heldObject = obj;
        obj.transform.SetParent(handSlot.transform);
        obj.transform.localPosition = Vector3.zero;
        Debug.Log("Объект поднят: " + obj.name);
    }
    private void TryInteraction(GameObject obj1, GameObject obj2)
    {
        IInteractableObject interactable1 = obj1.GetComponent<IInteractableObject>();
        IInteractableObject interactable2 = obj2.GetComponent<IInteractableObject>();
        Debug.Log("TryInteract");
        if (interactable1 != null && interactable2 != null)
        {
            if (interactable1.CanInteractable(obj2) && interactable2.CanInteractable(obj1))
            {
                interactable1.Interact(obj2);
                interactable2.Interact(obj1);
                Debug.Log($"Взаимодействие между {obj1.name} и {obj2.name}");

                //heldObject.transform.SetParent(null);
                //heldObject = null;
            }
        }
    }

    public GameObject GetHeldObject()
    {
        return heldObject;
    }

    public void ClearHand()
    {
        heldObject.transform.SetParent(null);
        heldObject = null;
    }
}
