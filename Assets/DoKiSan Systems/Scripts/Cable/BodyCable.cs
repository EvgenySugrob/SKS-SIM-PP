using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class BodyCable : MonoBehaviour,IDisableColliders
{
    [SerializeField] List<Collider> colliders;
    [SerializeField] bool isPatchCordPart = false;

    public void DisableCollider(bool isActive)
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = isActive;
        }
    }

    public bool IsPatchCordPart()
    {
        return isPatchCordPart;
    }    
}
