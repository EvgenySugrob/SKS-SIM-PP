using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSockets : MonoBehaviour
{
    [SerializeField] private GameObject[] objectPool = new GameObject[6];
    [SerializeField] private MarkCable[] cablePool = new MarkCable[6];
    [SerializeField] private List<Transform> pointSockets;
    [SerializeField] private List<Transform> _alreadyPoints;

    private string _nameSocket = "������� �";


    private void Start()
    {
        SpawnSocket();
    }

    private void SpawnSocket()
    {
        for(int i = 0; i< objectPool.Length; i++) 
        {
            Transform targetPoint = GetListTransformPoint();

            objectPool[i].transform.position = targetPoint.position;
            objectPool[i].transform.rotation = targetPoint.rotation;
            objectPool[i].SetActive(true);

            MarkSocket mark = objectPool[i].GetComponent<MarkSocket>();
            mark.SetNameSocket(_nameSocket + (i+1).ToString());

            cablePool[i].SetSocket(mark,mark.GetNumberSocket(),mark.nameSocket);
        }
    }

    private Transform GetListTransformPoint()
    {
        return pointSockets[GetRandomIndex()];
    }

    private int GetRandomIndex()
    {
        int index = Random.Range(0, pointSockets.Count);

        if (_alreadyPoints.Contains(pointSockets[index]))
        {
            return GetRandomIndex();
        }
        else
        {
            _alreadyPoints.Add(pointSockets[index]);
            return index;
        }
    }

    public void ActiveCollidersOnSockets(bool isActive)
    {
        for (int i = 0; i < objectPool.Length; i++)
        {
            if (objectPool[i].GetComponent<MarkSocket>().GetMarkingDone() == false)
            {
                objectPool[i].GetComponent<BoxCollider>().enabled = isActive;
            }
        }
    }

    public void ActiveAllColliders(bool isActive)
    {
        for (int i = 0; i < objectPool.Length; i++)
        {
            objectPool[i].GetComponent<BoxCollider>().enabled = isActive;
        }
    }

    public List<MarkCable> GetCablePool()
    {
        List<MarkCable> cableList= new List<MarkCable>();
        for (int i = 0; i < cablePool.Length; i++)
        {
            cableList.Add(cablePool[i]);
        }
        return cableList;
    }
}
