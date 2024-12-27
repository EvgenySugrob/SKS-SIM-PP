using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngineInternal;

public class OutlineDetection : MonoBehaviour
{
    [Header("Outline")]
    [SerializeField] OutlineManager currentManager;
    [SerializeField] float distanceRay=3f;
    private OutlineManager _prevManager;
    private GameObject _currentObject;
    private Camera _mainCamera;

    [Header("Switch cursor")]
    [SerializeField] Texture2D[] animationFrame;
    [SerializeField] float frameRate = 2f;
    [SerializeField] Vector2 hotSpot = Vector2.zero;
    private int _currentFrame = 0;
    private float _timer;
    private bool _isCursorAnimate;
    

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

            if(_currentObject.CompareTag("Manipulation") && _currentObject.GetComponent<BezierTwistedPair>()||_currentObject.CompareTag("ChangeIcon"))
            {
                if(!_isCursorAnimate)
                {
                    _isCursorAnimate = true;
                    _currentFrame = 0;
                    _timer = 0;
                }
                AnimateCursor();
            }
            else
            {
                if (_isCursorAnimate)
                {
                    _isCursorAnimate = false;
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                }
            }
        }
        else
        {
            if(currentManager != null)
            {
                currentManager.EnableOutline(false);
                currentManager= null;
                if (_isCursorAnimate)
                {
                    _isCursorAnimate = false;
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                }
            }
        }
    }

    private void AnimateCursor()
    {
        if (animationFrame.Length == 0)
            return;

        _timer += Time.deltaTime;

        if(_timer>=1f/frameRate)
        {
            _timer -= 1/frameRate;
            _currentFrame = (_currentFrame+1)%animationFrame.Length;
            Cursor.SetCursor(animationFrame[_currentFrame], hotSpot, CursorMode.Auto);
        }
    }
}
