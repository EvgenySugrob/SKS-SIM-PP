using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MarkSocket : MonoBehaviour,IInteractableObject
{
    public string nameSocket { private set; get; }
    [SerializeField] private bool _isMarking = false;
    [SerializeField] private bool _isTerminationCheck = false;
    [SerializeField] int numberSocket;

    [Header("Interaction")]
    [SerializeField] MarkCable cable;
    [SerializeField] Transform eyesPivot;
    [SerializeField] Transform jackPartPivot;
    [SerializeField] Transform jactBetweenPivot;
    [SerializeField] Transform firstToolsPivot;
    [SerializeField] Transform secondToolsPivot;
    [SerializeField] Transform nfrPivot;

    [Header("Decal material")]
    [SerializeField] DecalProjector numberDecal;
    [SerializeField] Texture2D imageIcon;
    private Material _decalMaterial;

    private void Start()
    {
        _decalMaterial = new Material(numberDecal.material);
        numberDecal.material = _decalMaterial;
        _decalMaterial.SetTexture("_numberIcon", imageIcon);
    }

    public void SetNameSocket(string name)
    {
        nameSocket = name;
    }

    public bool CanInteractable(GameObject objectInteract)
    {
        bool isInteract = false;

        if(!_isMarking)
            isInteract = true;

        if(objectInteract.GetComponent<CableTestChecker>().GetIsSearchSocketTermimnation())
        {
            isInteract = true;
        }

        return isInteract;
    }

    public void MarkingDone()
    {
        _isMarking = true;
        transform.GetComponent<BoxCollider>().enabled = false;
        _decalMaterial.SetInt("_markingDone", 1);
    }

    public bool GetMarkingDone()
    {
        return _isMarking; 
    }
    public bool GetTerminationCheck()
    {
        return _isTerminationCheck;
    }
    public void Interact(GameObject objectInteract)
    {
        
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
    public int GetNumberSocket()
    {
        return numberSocket;
    }
    public Transform GetNfrPivot()
    {
        return nfrPivot;
    }
}
