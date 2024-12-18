using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWorkSpace : MonoBehaviour
{
    [SerializeField]private List<BoxCollider> boxColliderList;

    public void WorkSapceEnable(bool state)
    {
        foreach (BoxCollider collider in boxColliderList)
        {
            collider.enabled = state;
        }
    }
}
