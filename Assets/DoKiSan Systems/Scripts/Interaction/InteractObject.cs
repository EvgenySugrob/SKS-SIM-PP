using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : MonoBehaviour, IInteractableObject
{
    public bool CanInteractable(GameObject objectInteract)
    {
        return true;//////�������� ������ ������
    }

    public void Interact(GameObject objectInteract)
    {
        Debug.Log($"{name} ��������������� � ����� ����� � � {objectInteract.name}");
    }
}
