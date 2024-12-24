using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkModeManipulation : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] float distanceRay;
    [SerializeField] bool isWork = true;

    public void CheckObject()
    {
        if(isWork)
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, distanceRay))
            {
                GameObject currentHitObject = hit.collider.gameObject;

                if (currentHitObject.CompareTag("Manipulation"))
                {
                    if (currentHitObject.TryGetComponent(out IManipulate manipelate))
                        manipelate.Manipulate();
                }
            }
        }
    }

    public void WorkState(bool isState)
    {
        isWork = isState;
    }
}
