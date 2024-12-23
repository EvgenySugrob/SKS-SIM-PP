using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCutOnPart : MonoBehaviour
{
    [Header("Mesh cut")]
    [SerializeField] Transform cutPart;
    [SerializeField] float offsetY = -2.5f;

    [Header("TwistedPair")]
    [SerializeField] List<BezierTwistedPair> bezierTwistedPairs;

    public YieldInstruction DeletCutPart()
    {
        foreach (BezierTwistedPair twisedPair in bezierTwistedPairs)
        {
            twisedPair.enabled = true;
        }

        return cutPart.transform
            .DOLocalMoveY(offsetY, 1f)
            .Play()
            .WaitForCompletion();
    }

    public void DisableCutPart()
    {
       cutPart.gameObject.SetActive(false);
    }
}
