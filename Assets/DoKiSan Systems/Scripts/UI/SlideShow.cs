using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideShow : MonoBehaviour
{
    [SerializeField] Texture[] slides;
    [SerializeField] RawImage imageDisplay;
    [SerializeField] float timeBetweenSlides = 2f;
    [SerializeField] float transitionDuration = 1f;
    private int _currentSlideIndex = 0;

    void Start()
    {
        if (slides.Length > 0 && imageDisplay != null)
        {
            imageDisplay.texture = slides[_currentSlideIndex];
            imageDisplay.color = new Color(1, 1, 1, 1); 
            StartCoroutine(ChangeSlide());
        }
        else
        {
            Debug.LogWarning("Нет текстур для слайд-шоу или RawImage не назначен!");
        }
    }

    IEnumerator ChangeSlide()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSlides);

            yield return StartCoroutine(FadeOutAndIn());
        }
    }

    IEnumerator FadeOutAndIn()
    {
        float elapsedTime = 0f;
        Color startColor = imageDisplay.color;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / transitionDuration);
            imageDisplay.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        _currentSlideIndex = (_currentSlideIndex + 1) % slides.Length;
        imageDisplay.texture = slides[_currentSlideIndex];

        elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / transitionDuration);
            imageDisplay.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
    }

}
