using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class OutlineDetection : MonoBehaviour
{
    [SerializeField] OutlineManager currentManager;
    [SerializeField] float distanceRay=3f;
    private OutlineManager _prevManager;
    private GameObject _currentObject;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray,out RaycastHit hit,distanceRay))
        {
            _currentObject = hit.collider.gameObject;
            if (_currentObject.GetComponent<OutlineManager>())
            {
                currentManager = _currentObject.GetComponent<OutlineManager>();

                if(_prevManager == null)
                {
                    _prevManager = currentManager;
                }
                if(_prevManager!=currentManager)
                {
                    _prevManager.EnableOutline(false);
                    _prevManager = currentManager;
                }
            }

            if(currentManager != null && _currentObject.GetComponent<OutlineManager>())
            {
                currentManager.EnableOutline(true);
            }
            else if (currentManager != null && _currentObject.GetComponent<OutlineManager>()==false)
            {
                currentManager.EnableOutline(false);
            }
        }
        else
        {
            if(currentManager != null)
            {
                currentManager.EnableOutline(false);
                currentManager= null;
            }
        }
    }
}
