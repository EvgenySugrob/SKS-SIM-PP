using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILineRenderConection : MonoBehaviour
{
    [Header("LineRenderer")]
    [SerializeField] LineRenderer[] connectLine = new LineRenderer[8];
    [SerializeField] PointForLineRender[] upPoints = new PointForLineRender[8];
    [SerializeField] PointForLineRender[] downPoints= new PointForLineRender[8];
    [SerializeField] LineRenderer gLine;
    [SerializeField] PointForLineRender[] gUpDownPoints = new PointForLineRender[2];
    [SerializeField] TMP_Text resultText;
    List<string> addedPairs = new List<string>();
    string crossMappingInfo = "Cross mapping: ";
    string defaultText = "OK!";
    bool hasCrossMapping = false;

    [Header("RepairBt")]
    [SerializeField] GameObject repairBtGroup;

    public void SetPortsInfo(ContactPortInteract[] ports)
    {
        addedPairs.Clear();
        hasCrossMapping= false;

        for (int i = 0; i < connectLine.Length; i++)
        {
            int indexDownPoints = ports[i].GetTypeIndexCablePort() - 1;

            connectLine[i].SetPosition(0, upPoints[i].GetWorldPositionPoint());
            connectLine[i].SetPosition(1, downPoints[indexDownPoints].GetWorldPositionPoint());

            connectLine[i].enabled= true;

            if (indexDownPoints != i)
            {
                string pair = $"{Math.Min(i + 1, indexDownPoints + 1)}{Math.Max(i + 1, indexDownPoints + 1)}";

                if (!addedPairs.Contains(pair))
                {
                    if (hasCrossMapping)
                        crossMappingInfo += ", ";
                    crossMappingInfo += pair;
                    addedPairs.Add(pair);
                    hasCrossMapping = true;
                }
            }
        }

        //gLine.SetPosition(0, gUpDownPoints[0].GetWorldPositionPoint());
        //gLine.SetPosition(1, gUpDownPoints[1].GetWorldPositionPoint());
        //gLine.enabled= true;

        if (hasCrossMapping)
        {
            resultText.text = crossMappingInfo;
            repairBtGroup.SetActive(true);
        }
        else
        {
            resultText.text = defaultText;
            repairBtGroup.SetActive(false);
        }
    }

    public void SetPatchCordMapping(int[] mainIndexGroup, int[] secondIndexGroup)
    {
        addedPairs.Clear();
        hasCrossMapping = false;
        crossMappingInfo = "Cross mapping: ";

        if (mainIndexGroup.Length != secondIndexGroup.Length || mainIndexGroup.Length != connectLine.Length)
        {
            Debug.LogError("Ошибка: Размеры массивов не совпадают с количеством линий!");
            return;
        }

        for (int i = 0; i < connectLine.Length; i++)
        {
            int indexMain = mainIndexGroup[i] - 1;
            int indexSecond = secondIndexGroup[i] - 1;

            // Соединение точек, аналогично SetPortsInfo
            connectLine[i].SetPosition(0, upPoints[indexMain].GetWorldPositionPoint());
            connectLine[i].SetPosition(1, downPoints[indexSecond].GetWorldPositionPoint());
            connectLine[i].enabled = true;

            // Проверяем, не перепутаны ли провода (индексы должны совпадать)
            if (indexMain != indexSecond)
            {
                string pair = $"{Math.Min(mainIndexGroup[i], secondIndexGroup[i])}{Math.Max(mainIndexGroup[i], secondIndexGroup[i])}";

                if (!addedPairs.Contains(pair))
                {
                    if (hasCrossMapping)
                        crossMappingInfo += ", ";
                    crossMappingInfo += pair;
                    addedPairs.Add(pair);
                    hasCrossMapping = true;
                }
            }
        }

        // Вывод результата
        if (hasCrossMapping)
        {
            resultText.text = crossMappingInfo;
        }
        else
        {
            resultText.text = defaultText;
        }
    }
}
