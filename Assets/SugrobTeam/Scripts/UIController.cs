using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour, IDisableAfterLoading
{
    [Header("Scene Manager")]
    [SerializeField] string nextSceneName;
    private SceneLoader sceneLoader;

    [Header("UI")]
    private VisualElement _mainMenu;
    private VisualElement _startBt;
    private VisualElement _aboutBt;
    private VisualElement _exitBt;
    private VisualElement _closeWindowBt;
    private VisualElement _aboutWindow;
    private VisualElement _loadScreen;
    private VisualElement _loadIcon;

    private void Start()
    {
        sceneLoader = GetComponent<SceneLoader>();

        var root = GetComponent<UIDocument>().rootVisualElement;

        _mainMenu = root.Q<VisualElement>("mainMenu");
        _startBt = root.Q<Button>("startBt");
        _aboutBt = root.Q<Button>("aboutBt");
        _exitBt = root.Q<Button>("exitBt");
        _closeWindowBt = root.Q<Button>("closeWindowBt");
        _aboutWindow = root.Q<VisualElement>("aboutWindow");

        _loadScreen = root.Q<VisualElement>("loadScreen");
        _loadIcon = root.Q<VisualElement>("loadIconSpin");

        _startBt.RegisterCallback<ClickEvent>(OnStartClick);
        _aboutBt.RegisterCallback<ClickEvent>(OnOpenAboutWindow);
        _closeWindowBt.RegisterCallback<ClickEvent>(OnCloseAboutWindow);
        _aboutWindow.RegisterCallback<TransitionEndEvent>(OnAboutWindowClose);
        _exitBt.RegisterCallback<ClickEvent>(OnExitApp);
    }

    private void OnStartClick(ClickEvent evt)
    {
        Debug.Log("Включить экран и запустить загрузку через другой скрипт");
        _loadScreen.style.display = DisplayStyle.Flex;
        _loadScreen.AddToClassList("loadScreen--Load");

        RotationIcon();
        sceneLoader.OnStartGame(nextSceneName);
    }
    private void RotationIcon()
    {
        _loadIcon.ToggleInClassList("loadIcon--Rotate");
        _loadIcon.RegisterCallback<TransitionEndEvent>
        (
            evt => _loadIcon.ToggleInClassList("loadIcon--Rotate")
        );
    }

    private void OnOpenAboutWindow(ClickEvent evt)
    {
        _aboutWindow.style.display = DisplayStyle.Flex;
        _aboutWindow.AddToClassList("aboutWindow--Open");
    }
    private void OnCloseAboutWindow(ClickEvent evt)
    {
        _aboutWindow.RemoveFromClassList("aboutWindow--Open");

    }

    private void OnExitApp(ClickEvent evt)
    {
        Application.Quit();
    }

    private void OnAboutWindowClose(TransitionEndEvent evt)
    {
        if (!_aboutWindow.ClassListContains("aboutWindow--Open"))
        {
            _aboutWindow.style.display = DisplayStyle.None;
        }
    }

    public void DisableAfterLoading()
    {
        _mainMenu.style.display = DisplayStyle.None;
        _aboutWindow.style.display = DisplayStyle.None;
        _loadScreen.style.display = DisplayStyle.None;
    }
}
