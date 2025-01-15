using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkSocket : MonoBehaviour,IInteractableObject
{
    public string nameSocket { private set; get; }
    [SerializeField] private bool isMarking = false;

    [Header("Interaction")]
    [SerializeField] MarkCable cable;
    [SerializeField] Transform eyesPivot;
    [SerializeField] Transform jackPartPivot;
    [SerializeField] Transform jactBetweenPivot;
    [SerializeField] Transform firstToolsPivot;
    [SerializeField] Transform secondToolsPivot;

    public void SetNameSocket(string name)
    {
        nameSocket = name;
    }

    public bool CanInteractable(GameObject objectInteract)
    {
        bool isInteract = false;

        if(!isMarking)
            isInteract = true;

        return isInteract;
    }

    public void Interact(GameObject objectInteract)
    {
        Debug.Log("Интерактирую с NF");
    }

    public Transform GetEyesPivot()
    {
        return eyesPivot;
    }
    public Transform GetFirstToolPivot()
    {
        return firstToolsPivot;
    }
    public Transform GetSecondToolPivot()
    {
        return secondToolsPivot;
    }
    public Transform GetJackPartPivot()
    {
        return jackPartPivot;
    }
    public Transform GetJackBetweenPivot()
    {
        return jactBetweenPivot;
    }
}
