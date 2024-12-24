using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwistedPairUnravelingCount : MonoBehaviour
{
    [Header("Cable pair info")]
    [SerializeField] int countPairInCable = 4;
    [SerializeField] WorkModeManipulation workModeManipulation;

    [Header("End stripper interatcion")]
    [SerializeField] StripperPointInformation pointInformation;
    private int _allCount = 0;

    private void Start()
    {
        pointInformation = GetComponent<StripperPointInformation>();
    }
    public void CountUnravelingPair()
    {
        _allCount++;
        if (_allCount<countPairInCable)
        {
            StartCoroutine(RotateInteractPart(90));
        }
        else
        {
            StartCoroutine(RotateInteractPart(-270));
//pointInforamtion - возврат обратно на начальную позицию перед началом работы со стриппером
        }
    }

    private IEnumerator RotateInteractPart(float angle)
    {
        workModeManipulation.WorkState(false);
        yield return RotateOnClick(angle);
        workModeManipulation.WorkState(true);
    }
    private YieldInstruction RotateOnClick(float angle)
    {
        return transform
            .DOLocalRotate(Vector3.right * angle, 1.5f, RotateMode.WorldAxisAdd)
            .Play()
            .WaitForCompletion();
    }

}
