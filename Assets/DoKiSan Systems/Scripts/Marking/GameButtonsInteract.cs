using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameButtonsInteract : MonoBehaviour
{
    [Header("Event")]
    [SerializeField] UnityEvent onClick;

    [Header("ButtonAnimation")]
    [SerializeField] float offsetBt = 0.00111f;
    [SerializeField] float duration = 0.2f;
    [SerializeField] Ease easeType = Ease.Linear;
    [SerializeField] LoopType loopType = LoopType.Yoyo;
    private Tween tweenBt;

    private void Awake()
    {
        tweenBt = transform.DOLocalMoveY(offsetBt,duration)
            .SetEase(easeType)
            .SetLoops(2,loopType)
            .SetAutoKill(false);
    }

    private void OnMouseDown()
    {
        onClick?.Invoke();
        tweenBt.Restart();
    }
}
