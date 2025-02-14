using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CablePointBezier : MonoBehaviour, IDisableColliders
{
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] string typeCable;
    [SerializeField] int indexNumberCable;
    [SerializeField] string typeGroupCable;
    [SerializeField] List<InteractivePointHandler> interactivePointList = new List<InteractivePointHandler>();
    [SerializeField] Transform endPoint;
    [SerializeField] private int needIndexInteractivePoint = 3;
    [SerializeField] ContactPortInteract portInteract;
    [SerializeField] bool alreadyInstal = false;
    [SerializeField] Transform startGroup;
    [SerializeField] JackWireSlotInfo jackWireSlotInfo;

    [Header("Spawn particle")]
    [SerializeField] GameObject prefabParticle;
    [SerializeField] Transform startParticlePosition;
    [SerializeField] float impulseStrength = 1f;
    [SerializeField] float secondsToDispawn = 2f;
    [SerializeField] BoxCollider particleCollider;
    [SerializeField] Rigidbody rbParticle;
    [SerializeField] bool isFirstCut;
    private Coroutine _disableParticle;
    private Quaternion _prefabParticleStartRotation;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        int indexLastChild = transform.childCount - 1;
        endPoint = transform.GetChild(indexLastChild);
        _prefabParticleStartRotation = prefabParticle.transform.rotation;
    }
    public void DisableCollider(bool isActive)
    {
        boxCollider.enabled = isActive;
    }
    public bool GetAlreadyInstal()
    {
        return alreadyInstal;
    }
    public void SetAlreadyInstal(bool isInstal)
    {
        alreadyInstal = isInstal;
    }
    public string GetTypeCable()
    {
        return typeCable;
    }

    public Transform GetStartGroup()
    {
        return startGroup;
    }

    public bool IsNullPortInteractSlot()
    {
        if (portInteract == null)
            return true;
        else 
            return false;
    }

    public void SetPort(ContactPortInteract port)
    {
        if (port == null && portInteract!=null)
        {
            portInteract.SetStateSlot(false);
        }
        portInteract = port;
    }

    public ContactPortInteract GetPortInteract()
    {
        return portInteract;
    }

    public void FillingList(InteractivePointHandler point)
    {
        interactivePointList.Add(point);
    }
    public void ActiveInteractivePoint(bool isActive)
    {
        foreach(InteractivePointHandler point in interactivePointList)
        {
            if(point.GetIndexInteractivePoint() == needIndexInteractivePoint)
            {
                point.DisableCollider(isActive);
            }
                
        }
    }
    public int GetIndexNumberCable()
    {
        return indexNumberCable;
    }

    public void FirstCutCheck()
    {
        if(!isFirstCut)
        {
            SpawnParticle();
        }
    }

    private void SpawnParticle()
    {
        isFirstCut = true;
        particleCollider.enabled = true;
        rbParticle.isKinematic = false;
        prefabParticle.SetActive(true);

        rbParticle.AddRelativeForce(Vector3.up * impulseStrength, ForceMode.Impulse);

        _disableParticle = StartCoroutine(TimerDisableParticle());
    }

    private IEnumerator TimerDisableParticle()
    {
        yield return new WaitForSeconds(secondsToDispawn);
        DispawnParticle();
    }

    private void DispawnParticle()
    {
        rbParticle.isKinematic = true;
        particleCollider.enabled = false;
        prefabParticle.SetActive(false);
        prefabParticle.transform.rotation = _prefabParticleStartRotation;
        prefabParticle.transform.position = endPoint.position;
    }

    public void SetJackSlot(JackWireSlotInfo slot)
    {
        jackWireSlotInfo= slot;
    }
    public JackWireSlotInfo GetJackSlot()
    {
        return jackWireSlotInfo;
    }
}
