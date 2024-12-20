using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StripperPointInformation : MonoBehaviour
{
    [SerializeField] Transform stripperPoint;

    public Transform GetStripperPoint()
    {
        return stripperPoint;
    }
}
