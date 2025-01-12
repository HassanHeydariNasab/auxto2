using System.Collections.Generic;
using UnityEngine;

public class CollectablesSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Transform[] _spawnPoints;

    void Start()
    {
        _spawnPoints = GetComponentsInChildren<Transform>();
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            var spawnPoint = _spawnPoints[i];
            Instantiate(_prefab, spawnPoint.position, spawnPoint.rotation);
        }

    }
}