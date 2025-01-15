using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NfsController : MonoBehaviour
{
    [SerializeField] Renderer[] indicatorColor = new Renderer[3];

    [SerializeField] Material deafultMaterial;
    [SerializeField] Material colorfullMaterial;

    public void CountLightEnable(int countLight)
    {
        switch (countLight) 
        {
            case 0:
               DisableAllDiods();
                break;
            case 1:
                
                break;
            case 2:
                break;
            case 3:
                EnableAllDiods();
                break;
        }
    }

    private void DisableAllDiods()
    {
        for (int i = 0; i < indicatorColor.Length; i++)
        {
            indicatorColor[i].material = deafultMaterial;
        }
    }
    private void EnableAllDiods()
    {
        for (int i = 0; i < indicatorColor.Length; i++)
        {
            indicatorColor[i].material = colorfullMaterial;
        }
    }
    public void EnableDiods(int countDiodsActive)
    {
        if(countDiodsActive>0)
        {
            Renderer[] countAciveDiods = new Renderer[countDiodsActive];

            for (int i = 0; i < countAciveDiods.Length; i++)
            {
                countAciveDiods[i] = indicatorColor[i];
                countAciveDiods[i].material = colorfullMaterial;
            }
        }
        else
        {
            DisableAllDiods();
        }
        
    }
}
