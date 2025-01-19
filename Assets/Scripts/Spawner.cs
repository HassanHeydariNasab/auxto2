using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _collectablePrefab;
    [SerializeField] private GameObject[] _obstaclePrefabs;

    private Transform[] _spawnPoints;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        _spawnPoints = GetComponentsInChildren<Transform>();
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            var spawnPoint = _spawnPoints[i];
            var a = Random.Range(0, 4);
            if (a > 0)
            {
                Instantiate(_collectablePrefab, spawnPoint.position, Quaternion.Euler(90, 0, 0));
                gameManager.starsCount++;
            }
            else
            {
                Instantiate(_obstaclePrefabs[Random.Range(0, _obstaclePrefabs.Length)], spawnPoint.position, Quaternion.Euler(90, 0, 0));
            }
        }

        gameManager.Score = gameManager.Score;

    }

}