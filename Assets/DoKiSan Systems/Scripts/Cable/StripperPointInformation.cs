using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StripperPointInformation : MonoBehaviour
{
    [SerializeField] Transform stripperPoint;
    [SerializeField] Transform distantPoint;
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    public YieldInstruction SetCutPosition(Transform partPoint)
    {
        _startPosition = partPoint.localPosition;
        _startRotation = partPoint.localRotation;

        return DOTween.Sequence()
            .Append(transform.DOMove(partPoint.position, 1f))
            .Join(transform.DORotateQuaternion(partPoint.rotation, 1f))
            .Play()
            .WaitForCompletion();
    }

    public YieldInstruction BackInHand()
    {
        return DOTween.Sequence()
            .Append(transform.DOLocalMove(Vector3.zero, 1f))
            .Join(transform.DOLocalRotateQuaternion(_startRotation, 1f))
            .Play()
            .WaitForCompletion();
    }

    public Transform GetStripperPoint()
    {
        return stripperPoint;
    }

    public Transform GetDistanPoint()
    {
        return distantPoint;
    }
}
