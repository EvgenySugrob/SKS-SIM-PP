using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StripperPointInformation : MonoBehaviour
{
    [SerializeField] Transform stripperPoint;

    public YieldInstruction SetCutPosition(Transform partPoint)
    {
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
