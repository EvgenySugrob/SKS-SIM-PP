using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractivePuchok : MonoBehaviour
{
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] OutlineManager outlineManager;
    [SerializeField] Transform pointContainer;
    [SerializeField] Transform notInteractPuchok;

    private Vector3 _startPositionContainer;
    private Vector3 _startPositionNotInteractPuchok;
    private bool _startPositionRemeber = false;
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void ActiveCollider(bool isActive)
    {
        boxCollider.enabled = isActive;
    }

    public void OutlineActive(bool isActive)
    {
        outlineManager.EnableOutline(isActive);
    }

    public Transform GetNotInteractPuchok()
    {
        return notInteractPuchok;
    }

    public void SetNewPosition(Vector3 position)
    {
        if(!_startPositionRemeber)
        {
            _startPositionContainer = pointContainer.position;
            _startPositionNotInteractPuchok =notInteractPuchok.position;
            
            _startPositionRemeber=true;
        }
        StartCoroutine(MoveCable(pointContainer,position));
    }

    public void SetNotInteractNewPosition(Vector3 position)
    {
        StartCoroutine(MoveCable(notInteractPuchok,position));
    }

    private IEnumerator MoveCable(Transform point,Vector3 position)
    {
        yield return point.DOMove(position, 0.8f)
            .Play()
            .WaitForCompletion();
    }
}
