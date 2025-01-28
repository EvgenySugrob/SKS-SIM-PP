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
}
