using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableImage : MonoBehaviour, ICheckingMenu
{
    [SerializeField] Image selectableBackground;
    [SerializeField] GameObject needsMenuOpen;
    [SerializeField] CableTesterUIControl testerUIControl;
    [SerializeField] string nameMenu;

    private Tween _animationFadeIn;
    private Tween _animationFadeOut;

    private void Awake()
    {
        _animationFadeIn = selectableBackground.DOFade(1f, 0.2f)
            .SetAutoKill(false);
        _animationFadeOut = selectableBackground.DOFade(0f, 0.2f)
            .SetAutoKill(false);
    }

    public void FadeInIcon()
    {
        _animationFadeIn.Restart();
    }
    public void FadeOutIcon() 
    {
        _animationFadeOut.Restart();
    }

    public void SetSelfCheckMenu()
    {
        Debug.Log("Выполняюсь с " + gameObject.name);
        testerUIControl.OpenWindowSelectebleWindow(needsMenuOpen,nameMenu);
    }
}
