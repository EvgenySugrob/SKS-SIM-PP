using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkCable : MonoBehaviour
{
    [SerializeField] MarkSocket socket;

    public void SetSocket(MarkSocket markSocket)
    {
        socket = markSocket;
    }
}
