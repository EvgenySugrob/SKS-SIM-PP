using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationObjectUI : MonoBehaviour
{
    [SerializeField] Transform objectRotation;
    [SerializeField] Vector3 speedRotation = new Vector3(0f,15f,0f);
    private Quaternion startRotation;
    bool isRot;

    private void Start()
    {
        startRotation = objectRotation.rotation;
    }

    private void Update()
    {
        if (isRot)
        {
            objectRotation.Rotate(speedRotation * Time.deltaTime);
        }
        else
        {
            objectRotation.rotation = startRotation;
        }
    }
    public void RotationObject(bool isRotate)
    {
        if (isRotate)
        {
            isRot = true;
        }
        else 
        {
            isRot= false;
        }
    }
}
