using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTestingAnimation : MonoBehaviour
{
    [SerializeField] RectTransform icon;
    [SerializeField] Ease easeType;
    [SerializeField] LoopType loopType;
    [SerializeField] float duration = 1f;
    [SerializeField] private Vector3 rotationAngle = new Vector3(0,0, 360f);
    private Vector3 _startAngle = new Vector3(0, 0, 0);
    private Tween _iconRotate;
    private bool _isLoop = true;

    private void OnEnable()
    {
        _iconRotate = icon.DORotate(rotationAngle, duration, RotateMode.FastBeyond360)
            .SetEase(easeType)
            .SetLoops(_isLoop?-1:1, loopType)
            .SetAutoKill(false);

        if (_iconRotate != null)
        {
            _iconRotate.Restart();
        }
    }

    private void OnDisable()
    {
        _iconRotate.Pause();

        icon.localEulerAngles= _startAngle;
    }
}
