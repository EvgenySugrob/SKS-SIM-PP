using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIControlerMainScene : MonoBehaviour,IDisableAfterLoading
{
    [Header("Scene Manager")]
    [SerializeField] string nextSceneName;
    private SceneLoader sceneLoader;

    [Header("UI")]
    private VisualElement _horizontalMenu;
    private VisualElement _hMenuItem0;
    private VisualElement _hMenuItem1;
    private VisualElement _hMenuItem2;
    private VisualElement _subMenuWindow0;
    private VisualElement _subItem0;
    private VisualElement _subItem1;
    private VisualElement _subItem2;
    private VisualElement _loadScreen;
    private VisualElement _loadIcon;
    private VisualElement _closeProgramBt;


    private void Start()
    {
        sceneLoader = GetComponent<SceneLoader>();

        var root = GetComponent<UIDocument>().rootVisualElement;

        _horizontalMenu = root.Q<VisualElement>("horizontalMenu");
        _hMenuItem0 = root.Q<VisualElement>("hMenuItem0");
        _hMenuItem1 = root.Q<VisualElement>("hMenuItem1");
        _hMenuItem2 = root.Q<VisualElement>("hMenuItem2");

        _subMenuWindow0 = root.Q<VisualElement>("subMenuWindow0");
        _subItem0 = root.Q<Button>("subItem0");
        _subItem1 = root.Q<Button>("subItem1");
        _subItem2 = root.Q<Button>("subItem2");

        _loadScreen = root.Q<VisualElement>("loadScreen");
        _loadIcon = root.Q<VisualElement>("loadIconSpin");

        _closeProgramBt = root.Q<Button>("closeProgramBt");

        _hMenuItem0.RegisterCallback<MouseEnterEvent>(OpenSubMenuHItem0);
        _hMenuItem1.RegisterCallback<MouseEnterEvent>(OpenSubMenuHItem1);

        _subMenuWindow0.RegisterCallback<MouseLeaveEvent>(CloseSubMenuHItem0);
        _subItem0.RegisterCallback<ClickEvent>(OnMainMenuClick);

        _closeProgramBt.RegisterCallback<ClickEvent>(OnCloseAppBtClick);

        //PrintAllElements(root); //метод для отладки. Проверка древа элементов
    }

    private void OpenSubMenuHItem1(MouseEnterEvent evt) //Подумать над универсальностью
    {
        //Отключение других подменю
        _subMenuWindow0.AddToClassList("subMenuWindow--Hide");

        //Включение нужного
        Debug.Log("Open subMenu1");
    }

    private void CloseSubMenuHItem0(MouseLeaveEvent evt)
    {
        if(!_subMenuWindow0.worldBound.Contains(evt.mousePosition))
            _subMenuWindow0.AddToClassList("subMenuWindow--Hide");
    }

    private void OpenSubMenuHItem0(MouseEnterEvent evt)
    {
        _subMenuWindow0.RemoveFromClassList("subMenuWindow--Hide");
    }

    private void OnMainMenuClick(ClickEvent evt)
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

    private void OnCloseAppBtClick(ClickEvent evt)
    {
        Debug.Log("Close Program");
        Application.Quit();
    }

    public void DisableAfterLoading()
    {
        _horizontalMenu.style.display = DisplayStyle.None;
        _loadScreen.style.display = DisplayStyle.None;
    }

    //void PrintAllElements(VisualElement element, string indent = "")
    //{
    //    // Выводим имя элемента
    //    Debug.Log(indent + element.name);

    //    // Рекурсивно вызываем для всех дочерних элементов
    //    foreach (var child in element.Children())
    //    {
    //        PrintAllElements(child, indent + "--");
    //    }
    //}
}
