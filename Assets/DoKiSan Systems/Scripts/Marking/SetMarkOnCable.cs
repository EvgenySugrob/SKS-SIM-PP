using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SetMarkOnCable : MonoBehaviour
{
    [SerializeField] DecalProjector[] decals = new DecalProjector[2];
    [SerializeField] Material[] numbersMaterial = new Material[6];

    public void SetNumberOnDecal(int number)
    {
        Material currenMat=null;

        for (int i = 0; i < numbersMaterial.Length; i++)
        {
            if (number == i+1)
                currenMat = numbersMaterial[i];
        }

        for (int i = 0; i < decals.Length; i++)
        {
            decals[i].material = currenMat;
            decals[i].gameObject.SetActive(true);
        }
    }
}
