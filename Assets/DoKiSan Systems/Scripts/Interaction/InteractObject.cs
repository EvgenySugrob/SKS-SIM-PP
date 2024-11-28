using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : MonoBehaviour, IInteractableObject
{
    public bool CanInteractable(GameObject objectInteract)
    {
        return false;//////Вставить нужные букавы
    }

    public void Interact(GameObject objectInteract)
    {
        Debug.Log($"{name} взаимодействует с твоей мамой и с {objectInteract.name}");
    }
}
