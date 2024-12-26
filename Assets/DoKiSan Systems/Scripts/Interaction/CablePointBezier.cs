using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CablePointBezier : MonoBehaviour, IDisableColliders
{
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] string typeCable;
    [SerializeField] string typeGroupCable;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }
    public void DisableCollider(bool isActive)
    {
        boxCollider.enabled = isActive;
    }

    public string GetTypeCable()
    {
        return typeCable;
    }
}
