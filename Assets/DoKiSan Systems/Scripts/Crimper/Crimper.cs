using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] PatchCordCreate patchCord;
    [SerializeField] JackReplace jackReplace;
    [SerializeField] JackConnetctAndSetting jack;
    [SerializeField] Transform jackBetweenPoint;
    [SerializeField] Transform jackFinalPoint;
    [SerializeField] Transform jackHandPoint;
    private Vector3 _jackStartPosition;
    private Quaternion _jackStartRotation;

    [Header("UI elements")]
    [SerializeField] GameObject uiGroup;
    [SerializeField] Slider crimpingSlider;
    [SerializeField] GameObject acceptBt;
    [SerializeField] GameObject viewProgressBt;
    [SerializeField] GameObject continueBt;
    [SerializeField] Tween crimpingAnim;
    private float maxValueSlider;
    private float _lastSliderValue;

    private void Start()
    {
        maxValueSlider = crimpingSlider.maxValue;

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
        _jackStartPosition = jack.transform.localPosition;
        _jackStartRotation= jack.transform.localRotation;

        yield return SetEyesPosition(viewPosition);

        yield return SetCrimperPosition(crimperHandPoint);

        yield return SetJactPosition(jackBetweenPoint, jackFinalPoint);

        uiGroup.SetActive(true);
        viewProgressBt.SetActive(true);
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

    private YieldInstruction SetCrimperPosition(Vector3 position,Quaternion rotation)
    {
        return DOTween.Sequence()
            .Append(transform.DOMove(position, 0.5f))
            .Join(transform.DORotateQuaternion(rotation, 0.5f))
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
        if(value<maxValueSlider)
        {
            crimpingAnim.Goto(value * crimpingAnim.Duration(), false);
            _lastSliderValue = value;

            jack.ContactCrimpingProgress(value);
        }
        else
        {
            crimpingSlider.interactable = false;
            viewProgressBt.SetActive(false);
            acceptBt.SetActive(true);
        }
        
    }

    public void AcceptCrimping()
    {
        acceptBt.SetActive(false);

        jack.EndContactCrimpingProgress();

        StartCoroutine(AcceptProgress());
    }

    private IEnumerator AcceptProgress()
    {
        yield return SliderDown();

        uiGroup.SetActive(false);
        crimpingSlider.interactable = true;

        yield return SetJactPosition(jackBetweenPoint, jack.GetStartPoint());
        yield return SetCrimperPosition(_crimperStartPosition,_crimperStartRotation);

        jack = null;
        _crimperIsActive = false;
        
        if(patchCord.AllPartCrimping())
        {
            jackReplace.ReturnToMainWork();
        }
        else
        {
            jackReplace.MoveEyesAfterFirstJackComplate();
        }
    }

    public void ViewProgress()
    {
        StartCoroutine(SliderReturnBack());
    }

    private IEnumerator SliderReturnBack()
    {
        viewProgressBt.SetActive(false);
        crimpingSlider.interactable = false;

        yield return SliderDown();

        yield return SetJactPosition(jackBetweenPoint,jackHandPoint);

        continueBt.SetActive(true);

    }

    private YieldInstruction SliderDown()
    {
        return crimpingSlider.DOValue(0, 0.5f)
            .SetEase(Ease.Linear)
            .Play()
            .WaitForCompletion();
    }

    public void ContinueProgress()
    {
        StartCoroutine(Continue());
    }

    private IEnumerator Continue()
    {
        continueBt.SetActive(false);

        yield return SetJactPosition(jackBetweenPoint, jackFinalPoint);

        crimpingSlider.interactable = true;
        viewProgressBt.SetActive(true);
    }
}
