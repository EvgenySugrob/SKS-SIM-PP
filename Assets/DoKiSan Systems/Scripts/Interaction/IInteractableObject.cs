using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractableObject
{
    bool CanInteractable(GameObject objectInteract);
    void Interact(GameObject objectInteract);
}
