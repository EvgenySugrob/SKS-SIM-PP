using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkCable : MonoBehaviour
{
    [SerializeField] MarkSocket socket;
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] Transform cableMarkPart;
    [SerializeField] Transform cableOnMarkPosition;
    [SerializeField] SetMarkOnCable setMarkOnCable;

    private Vector3 _markPosition = new Vector3(-0.004f, 0, 0);
    private int _numberSocket = 0;
    private Vector3 _startCableMarkPartPosition;
    private Quaternion _startCableMarkPartRotation;
    private bool _isMark = false;

    private void Start()
    {
        boxCollider= GetComponent<BoxCollider>();
        _startCableMarkPartPosition = cableMarkPart.position;
        _startCableMarkPartRotation= cableMarkPart.rotation;
    }

    public void SetSocket(MarkSocket markSocket,int number)
    {
        socket = markSocket;
        _numberSocket = number;
    }

    public void Marking()
    {
        boxCollider.enabled = false;
        _isMark = true;
    }

    public MarkSocket GetBoundSocket()
    {
        return socket;
    }

    public void StartMarkingCable(CableTestChecker tester)
    {
        setMarkOnCable.SetNumberOnDecal(_numberSocket);

        StartCoroutine(MoveInteractpartOnPosition(tester));
        Debug.Log("После корутины");
        Marking();
        
    }

    private IEnumerator MoveInteractpartOnPosition(CableTestChecker tester)
    {
        yield return MoveInteract();
        yield return new WaitForSeconds(0.5f);
        yield return MoveMarkOnCable();
        yield return new WaitForSeconds(1f);
        yield return ReturnInteractCable();
        tester.ReturnTesterInHand();
        Debug.Log("Из корутины - готово");
    }

    private YieldInstruction MoveInteract()
    {
        return DOTween.Sequence()
            .Append(cableMarkPart.DOMove(cableOnMarkPosition.position, 1f))
            .Join(cableMarkPart.DORotateQuaternion(cableOnMarkPosition.rotation,1f))
            .Play()
            .WaitForCompletion();
    }

    private YieldInstruction MoveMarkOnCable()
    {
        setMarkOnCable.gameObject.SetActive(true);

        return setMarkOnCable.transform.DOLocalMove(_markPosition,2f)
            .Play() 
            .WaitForCompletion();
    }
    private YieldInstruction ReturnInteractCable()
    {
        return DOTween.Sequence()
            .Append(cableMarkPart.DOMove(_startCableMarkPartPosition,1f))
            .Join(cableMarkPart.DORotateQuaternion(_startCableMarkPartRotation,1f))
            .Play()
            .WaitForCompletion();
    }
}
