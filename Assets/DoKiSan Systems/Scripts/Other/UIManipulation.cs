using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManipulation : MonoBehaviour
{
    [SerializeField] GameObject uiMainGroup;

    public void OpenUIMainGroup()
    {
        uiMainGroup.SetActive(!uiMainGroup.activeSelf);
    }

    public bool MainUIGroupIsActive()
    {
        return uiMainGroup.activeSelf;
    }
}
