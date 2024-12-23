using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCable : MonoBehaviour,IDisableColliders
{
    [SerializeField] List<Collider> colliders;
    public void DisableCollider(bool isActive)
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = isActive;
        }
    }
}
