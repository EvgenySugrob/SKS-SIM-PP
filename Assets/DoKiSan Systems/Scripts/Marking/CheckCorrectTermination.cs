using DG.Tweening;
using DoKiSan.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCorrectTermination : MonoBehaviour,IInteractableObject
{
    [Header("PP params")] 
    [SerializeField] PatchPanelInteraction patchPanelInteraction;
    [SerializeField] Transform ppPointFlip;
    [SerializeField] BoxCollider ppCollider;
    [SerializeField] BoxCollider faceCollider;
    [SerializeField] Transform flipPoint;
    [SerializeField] float yPosition = 0.8748f;
    [SerializeField] float animationSpeed = 0.33f;
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    [Header("Player params")]
    [SerializeField] FirstPlayerControl playerControl;

    private void Start()
    {
        patchPanelInteraction = GetComponent<PatchPanelInteraction>();
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    public void FlipingPanel(bool isFlip)
    {
        if (isFlip)
            StartCoroutine(FlipPanelFaceUp());
        else
            StartCoroutine(FlipPanelFaceDown());
    }

    private IEnumerator FlipPanelFaceUp()
    {
        ppCollider.enabled = false;

        Vector3 correctStartPosition = new Vector3(_startPosition.x,yPosition,_startPosition.z);

        yield return DOTween.Sequence()
            .Append(transform.DOMove(flipPoint.position, animationSpeed))
            .SetEase(Ease.Linear)
            .Append(transform.DORotateQuaternion(flipPoint.rotation, animationSpeed))
            .SetEase(Ease.Linear)
            .Append(transform.DOMove(correctStartPosition, animationSpeed))
            .SetEase(Ease.Linear)
            .Play()
            .WaitForCompletion();

        faceCollider.enabled = true;
    }

    private IEnumerator FlipPanelFaceDown()
    {
        faceCollider.enabled = false;

        yield return DOTween.Sequence()
           .Append(transform.DOMove(flipPoint.position, animationSpeed))
           .SetEase(Ease.Linear)
           .Append(transform.DORotateQuaternion(_startRotation, animationSpeed))
           .SetEase(Ease.Linear)
           .Append(transform.DOMove(_startPosition, animationSpeed))
           .SetEase(Ease.Linear)
           .Play()
           .WaitForCompletion();

        ppCollider.enabled = true;
    }

    public bool CanInteractable(GameObject objectInteract)
    {
        if (objectInteract.GetComponent<CableTestChecker>())
            return true;
        else
            return false;
    }

    public void Interact(GameObject objectInteract)
    {
        throw new System.NotImplementedException();
    }
}
