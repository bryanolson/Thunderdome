using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    // This script is responsible for spawning enemies
    private double clock = 0f;
    public float spawnDelay = 5f;

    private Transform _spawnLocation;
    public Transform[] enemyTypes;

    private void Start()
    {
        _spawnLocation = GameObject.Find("FirstEnemySpawn").transform;
        foreach (var spawnOption in GetSpawnOptions())
        {
            spawnOption.transform.localScale = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.current.CanSpawnEnemy)
        {
            clock += (Time.deltaTime * GameState.current.SpawnMultiplier);

            if (clock >= spawnDelay)
            {
                Spawn();
                clock -= spawnDelay;
            }

            float percentFull = (float) Math.Min(1, clock / spawnDelay);
            _spawnLocation.localScale = new Vector3(percentFull, percentFull, percentFull);
        }
    }

    void Spawn()
    {
        Transform newEnemy = Instantiate(RandomEnemy(), _spawnLocation.position, _spawnLocation.rotation);
        GameEvents.current.EnemySpawned(newEnemy);
        // Reset size to zero;
        _spawnLocation.localScale = Vector3.zero;
        // Randomly choose spawn location after each spawn.
        _spawnLocation = GetRandomSpawnOption();
    }

    private Transform RandomEnemy()
    {
        var enemyCount = Math.Min(GameState.current.CurrentLevel, enemyTypes.Length);
        return enemyTypes[Random.Range(0, enemyCount)];
    }

    public static Transform GetRandomSpawnOption()
    {
        GameObject[] spawnOptions = GetSpawnOptions();
        return spawnOptions[Random.Range(0, spawnOptions.Length)].transform;
    }

    private static GameObject[] GetSpawnOptions()
    {
        return GameObject.FindGameObjectsWithTag("EnemySpawn");
    }
}