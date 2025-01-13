using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableTestChecker : MonoBehaviour
{
    [Header("Main position and rotation")]
    [SerializeField] Transform nfActivePosition;
    [SerializeField] Transform _currentCable;
    [SerializeField] bool markingIsActiv = false;
    [SerializeField] bool screenActive = false;
    private Transform _prevCable;
    private Quaternion _startRotation;
    private Vector3 _startPosition;
    private Transform _startParent;

    [Header("Check limits on work")]
    [SerializeField] InteractionSystem interactionSystem;

    [Header("UITool")]
    [SerializeField] GameObject backgroundUI;
    [SerializeField] RectTransform logoBackground;
    [SerializeField] GameObject mainMenu;

    private void Start()
    {
        _startPosition = transform.localPosition;
        _startRotation = transform.localRotation;
        _startParent = transform.parent;
    }

    private void Update()
    {
        if(markingIsActiv)
        {

        }
    }

    public void StartChecker()
    {
        if (interactionSystem.IsCablePartMovingActive())
            return;

        markingIsActiv = true;
        gameObject.SetActive(true);

        StartCoroutine(MoveOnJobPosition());
        //StartCoroutine(LoadUITool());
    }

    private IEnumerator MoveOnJobPosition()
    {
        yield return MoveUp();
        yield return LoadUITool();
        screenActive = true;
        backgroundUI.SetActive(false);
        mainMenu.SetActive(true);
    }

    private YieldInstruction MoveUp()
    {
        return transform.DOMove(nfActivePosition.position,1f)
            .Play()
            .WaitForCompletion();
    }

    private YieldInstruction LoadUITool()
    {
        backgroundUI.SetActive(true);
        Vector3 grow = new Vector3(1.5f, 1.5f, 1.5f);
        return logoBackground.DOScale(grow, 2f)
            .Play()
            .WaitForCompletion();
    }
}
