using DoKiSan.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseApp : MonoBehaviour
{
    [SerializeField] GameObject dialogsWindow;
    [SerializeField] InteractionSystem interactionSystem;
    [SerializeField] FirstPlayerControl firstPlayerControl;
    private bool _windowOpen = false;

    public void OpenDialog(bool isOpen)
    {
        _windowOpen= isOpen;

        dialogsWindow.SetActive(_windowOpen);

        interactionSystem.enabled = !_windowOpen;
        firstPlayerControl.enabled = !_windowOpen;

    }

    public void CloseApplication()
    {
        Application.Quit();
    }
}
