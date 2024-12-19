using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StripperInteration : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] DecalProjector projectorCutLine;

    private Material projectorMaterial;
    private bool isEnableStripper=false;
    private string shaderState = "_currentCutPlace";

    private void Start()
    {
        projectorMaterial = projectorCutLine.material;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isEnableStripper = !isEnableStripper;

            SwitchColor(isEnableStripper);
        }
    }

    private void SwitchColor(bool isState)
    {
        int intValue = isState ? 1 : 0;
        projectorMaterial.SetInt(shaderState, intValue);
    }
}
