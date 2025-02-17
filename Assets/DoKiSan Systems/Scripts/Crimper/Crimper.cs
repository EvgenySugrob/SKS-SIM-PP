using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crimper : MonoBehaviour
{
    [Header("CrimperWorkState")]
    [SerializeField] bool crimperWork = false;
    private bool _crimperIsActive = false;

    [Header("CrimperMainPosition")]
    [SerializeField] Transform crimper;
    [SerializeField] Transform viewPosition;
    [SerializeField] Transform crimperHandPoint;
    [SerializeField] Transform crimpJackPart;
    [SerializeField] Transform endCrimpJackPart;
    private Vector3 _crimperStartPosition;
    private Quaternion _crimperStartRotation;

    [Header("CrimperPart")]
    [SerializeField] Transform crimperPart;

    [Header("LeftLegs")]
    [SerializeField] Transform leftLegs;
    [SerializeField] Transform leftLegsEndPoint;

    [Header("RightLegs")]
    [SerializeField] Transform rightLegs;
    [SerializeField] Transform rightLegsEndPoint;

    [Header("Player")]
    [SerializeField] Transform eyesPlayer;
    [SerializeField] Transform choisePatchcordPardPoint;

    [Header("JackInformation")]
    [SerializeField] JackConnetctAndSetting jack;
    [SerializeField] Transform jackBetweenPoint;
    [SerializeField] Transform jackFinalPoint;
    private Vector3 _jackStartPosition;
    private Quaternion _jackStartRotation;

    [Header("UI elements")]
    [SerializeField] GameObject uiGroup;
    [SerializeField] Tween crimpingAnim;
    private float _lastSliderValue;

    private void Start()
    {
        _crimperStartPosition = transform.localPosition;
        _crimperStartRotation = transform.localRotation;

        crimpingAnim = DOTween.Sequence()
            .Append(crimpJackPart.DOLocalMove(endCrimpJackPart.localPosition, 1f))
            .Join(leftLegs.DOLocalRotateQuaternion(leftLegsEndPoint.localRotation, 1f))
            .Join(rightLegs.DOLocalRotateQuaternion(rightLegsEndPoint.localRotation,1f))
            .SetAutoKill(false)
            .SetEase(Ease.Linear)
            .Pause();
    }

    public void SetSelectedJack(JackConnetctAndSetting selectedJack)
    {
        jack = selectedJack;
    }

    public void ActivateCrimper()
    {
        if(!_crimperIsActive && jack.GetMontageDone())
        {
            transform.gameObject.SetActive(true);
            _crimperIsActive=true;

            StartCoroutine(SetEyesAndJackCorrectPosition());
        }
    }

    private IEnumerator SetEyesAndJackCorrectPosition()
    {
        _jackStartPosition = jack.transform.position;
        _jackStartRotation= jack.transform.rotation;

        yield return SetEyesPosition(viewPosition);

        yield return SetCrimperPosition(crimperHandPoint);

        yield return SetJactPosition(jackBetweenPoint, jackFinalPoint);
    }

    private YieldInstruction SetEyesPosition(Transform endPoint)
    {
        eyesPlayer.parent = viewPosition;

        Quaternion endRotation = endPoint.rotation;

        return DOTween.Sequence()
            .Append(eyesPlayer.DOLocalMove(Vector3.zero,0.5f))
            .Join(eyesPlayer.DORotateQuaternion(endRotation,0.5f))
            .Play()
            .WaitForCompletion();
    }

    private YieldInstruction SetCrimperPosition(Transform position)
    {
        Vector3 endPosition= position.position;
        Quaternion endRotation= position.rotation;

        return DOTween.Sequence()
            .Append(transform.DOMove(endPosition,0.5f))
            .Join(transform.DORotateQuaternion(endRotation,0.5f))
            .Play()
            .WaitForCompletion();
    }

    private YieldInstruction SetJactPosition(Transform between, Transform final)
    {
        return DOTween.Sequence()
            .Append(jack.transform.DOMove(between.position,0.5f))
            .Join(jack.transform.DOLocalRotateQuaternion(between.localRotation,0.5f))
            .Append(jack.transform.DOMove(final.position,0.5f))
            .Join(jack.transform.DOLocalRotateQuaternion(final.localRotation,0.5f))
            .Play()
            .WaitForCompletion();
    }

    public void Crimping(float value)
    {
        crimpingAnim.Goto(value * crimpingAnim.Duration(), true);
        crimpingAnim.Pause();

        _lastSliderValue = value;
    }
}
