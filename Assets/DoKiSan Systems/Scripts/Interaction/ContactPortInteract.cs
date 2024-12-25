using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactPortInteract : MonoBehaviour
{
    [SerializeField] Material selectMaterial;
    [SerializeField] Renderer matRenderer;
    [Range(0f, 1f)]
    [SerializeField] float alphaMaterial;

    private void Start()
    {
        selectMaterial = transform.GetChild(0).transform.GetComponent<MeshRenderer>().sharedMaterial;
        matRenderer = transform.GetChild(0).transform.GetComponent<Renderer>();
        if(matRenderer!=null)
        {
            matRenderer.material = new Material(matRenderer.material);
        }
    }

    private void Update()
    {
        Color alphaColor = matRenderer.material.color;
        alphaColor.a = alphaMaterial;
        matRenderer.material.color = alphaColor;
    }
}
