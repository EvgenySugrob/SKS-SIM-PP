using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkCable : MonoBehaviour
{
    [SerializeField] MarkSocket socket;
    [SerializeField] BoxCollider boxCollider;

    private void Start()
    {
        boxCollider= GetComponent<BoxCollider>();
    }

    public void SetSocket(MarkSocket markSocket)
    {
        socket = markSocket;
    }

    public void Marking()
    {
        boxCollider.enabled = false;
    }

    public MarkSocket GetBoundSocket()
    {
        return socket;
    }
}
