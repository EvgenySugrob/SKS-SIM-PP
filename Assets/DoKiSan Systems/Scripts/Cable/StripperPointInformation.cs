using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StripperPointInformation : MonoBehaviour
{
    [SerializeField] Transform stripperPoint;
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    public YieldInstruction SetCutPosition(Transform partPoint)
    {
        _startPosition = partPoint.position;
        _startRotation = partPoint.rotation;

        return DOTween.Sequence()
            .Append(transform.DOMove(partPoint.position, 1f))
            .Join(transform.DORotateQuaternion(partPoint.rotation, 1f))
            .Play()
            .WaitForCompletion();
    }

    public Transform GetStripperPoint()
    {
        return stripperPoint;
    }
}
