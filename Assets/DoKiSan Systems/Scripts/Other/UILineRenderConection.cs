using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILineRenderConection : MonoBehaviour
{
    [SerializeField] LineRenderer[] connectLine = new LineRenderer[8];
    [SerializeField] PointForLineRender[] upPoints = new PointForLineRender[8];
    [SerializeField] PointForLineRender[] downPoints= new PointForLineRender[8];
    [SerializeField] LineRenderer gLine;
    [SerializeField] PointForLineRender[] gUpDownPoints = new PointForLineRender[2];
    [SerializeField] TMP_Text resultText;


    public void SetPortsInfo(ContactPortInteract[] ports)
    {
        for (int i = 0; i < connectLine.Length; i++)
        {
            int indexDownPoints = ports[i].GetTypeIndexCablePort() - 1;

            connectLine[i].SetPosition(0, upPoints[i].GetWorldPositionPoint());
            connectLine[i].SetPosition(1, downPoints[indexDownPoints].GetWorldPositionPoint());

            connectLine[i].enabled= true;
        }

        gLine.SetPosition(0, gUpDownPoints[0].GetWorldPositionPoint());
        gLine.SetPosition(1, gUpDownPoints[1].GetWorldPositionPoint());
        gLine.enabled= true;
    }
}
