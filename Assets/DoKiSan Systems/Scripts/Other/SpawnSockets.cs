using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSockets : MonoBehaviour
{
    [SerializeField] private GameObject[] objectPool = new GameObject[6];
    [SerializeField] private List<Transform> pointSockets;
    [SerializeField] private List<Transform> _alreadyPoints;


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
}
