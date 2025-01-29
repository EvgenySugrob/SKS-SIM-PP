using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateInteractionPivot : MonoBehaviour
{
    [SerializeField] float angleRotate;
    private Transform _interactPartCable;
    private Vector3 _finalAngleRotation;


    public void CheckAndSetObjectForRotate(Transform objectRotate)
    {
        if(objectRotate.GetComponent<TwistedPairUnravelingCount>() && objectRotate.GetComponent<TwistedPairUnravelingCount>().GetRotateState() == false)
        {
            _interactPartCable = objectRotate;
        }
    }

    public void RotationInteractPartCable()
    {
        if(_interactPartCable != null)
            StartCoroutine(RotateCable());
    }

    private IEnumerator RotateCable()
    {
        Vector3 oldAngle = _interactPartCable.localEulerAngles;
        _finalAngleRotation = new Vector3(oldAngle.x,oldAngle.y,angleRotate);

        yield return _interactPartCable.DOLocalRotate(_finalAngleRotation, 0.5f)
            .Play()
            .WaitForCompletion();
        _interactPartCable.GetComponent<TwistedPairUnravelingCount>().SetRotateState(true);
        _interactPartCable = null;
    }
}
