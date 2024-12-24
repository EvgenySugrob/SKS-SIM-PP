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
            }
        }

        heldObject.transform.SetParent(null);
        heldObject = null;
    }

    public GameObject GetHeldObject()
    {
        return heldObject;
    }
}
