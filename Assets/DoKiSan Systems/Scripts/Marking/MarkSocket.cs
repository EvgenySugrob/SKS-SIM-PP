using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkSocket : MonoBehaviour
{
    public string nameSocket { private set; get; }

    public void SetNameSocket(string name)
    {
        nameSocket = name;
        Debug.Log(nameSocket);
    }
}
