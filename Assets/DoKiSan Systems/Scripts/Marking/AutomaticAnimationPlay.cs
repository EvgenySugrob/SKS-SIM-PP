using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticAnimationPlay : MonoBehaviour
{
    [SerializeField] RectTransform icon;
    [SerializeField] float duration = 2f;
    [SerializeField] Ease easeType;
    [SerializeField] LoopType loopType;
    [SerializeField] float _maxScale = 1.5f;
    private Vector3 _startScale = new Vector3(1f,1f,1f);
    private Tween _iconScale;
    private bool _isLoop = true;

    private void OnEnable()
    {
        Vector3 growScale = new Vector3(_maxScale, _maxScale, _maxScale);
        _iconScale = icon.DOScale(growScale,duration)
            .SetEase(easeType)
            .SetLoops(_isLoop?-1:1,loopType)
            .SetAutoKill(false);

        if (_iconScale!=null)
        {
            _iconScale.Restart();
        }
    }

    private void OnDisable()
    {
        _iconScale.Pause();

        icon.localScale = _startScale;
    }
}
