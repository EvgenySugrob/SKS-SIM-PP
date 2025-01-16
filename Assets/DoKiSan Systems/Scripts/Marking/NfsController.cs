using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NfsController : MonoBehaviour
{
    [SerializeField] Renderer[] indicatorColor = new Renderer[3];

    [SerializeField] Material deafultMaterial;
    [SerializeField] Material colorfullMaterial;

    public void DisableAllDiods()
    {
        for (int i = 0; i < indicatorColor.Length; i++)
        {
            indicatorColor[i].material = deafultMaterial;
        }
    }

    public void EnableDiods(int countDiodsActive)
    {
        for (int i = 0; i < indicatorColor.Length; i++)
        {
            indicatorColor[i].material = i < countDiodsActive ? colorfullMaterial : deafultMaterial;
        }
    }
}
