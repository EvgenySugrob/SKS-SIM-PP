using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    [SerializeField] List<Outline> outlines;
    [SerializeField] bool outlineActive = false;
    public void EnableOutline(bool isActive)
    {
        foreach (Outline outline in outlines)
        {
            outline.enabled = isActive;
        }
        outlineActive = isActive;
    }

    public void AddObjectToList(Outline firstOutlineObject,Outline secondOutlineObject)
    {
        if (!outlines.Contains(firstOutlineObject))
            outlines.Add(firstOutlineObject);
        if (!outlines.Contains(secondOutlineObject))
            outlines.Add(secondOutlineObject);
    }
    public void AddObjectToList(Outline outlineObject)
    {
        if (!outlines.Contains(outlineObject))
            outlines.Add(outlineObject);
    }
}
