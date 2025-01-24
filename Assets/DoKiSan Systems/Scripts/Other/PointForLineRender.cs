using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointForLineRender : MonoBehaviour
{
    [SerializeField] Transform pointForLineRender;
    [SerializeField] int index;

    public Vector3 GetWorldPositionPoint()
    {
        return pointForLineRender.position;
    }
}
