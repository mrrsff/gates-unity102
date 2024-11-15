using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieSpawner : MonoBehaviour
{
    public Collider spawnArea;
    public GameObject zombiePrefab;
    public float spawnRate = 1f;
    public int maxZombies = 10;
    public float spawnRadius = 5f;
    
    public Transform player;
    public float playerDetectionRadius = 10f;
    
    private float nextSpawnTime;
    public int currentZombies;
    private List<Zombie> zombies = new List<Zombie>();

    private void Start()
    {
        StartCoroutine(IncreaseSpawnRateNMaxZombies());
    }

    private IEnumerator IncreaseSpawnRateNMaxZombies()
    {
        while (true)
        {
            yield return new WaitForSeconds(30f);
            spawnRate *= 0.95f;
            maxZombies += 5;
        }
    }
    
    private void Update()
    {
        // Update zombies list to remove dead zombies
        for (int i = zombies.Count - 1; i >= 0; i--)
        {
            if (!zombies[i] || zombies[i].isDead)
            {
                zombies.RemoveAt(i);
                currentZombies--;
            }
        }
        if (Time.time >= nextSpawnTime && currentZombies < maxZombies)
        {
            nextSpawnTime = Time.time + spawnRate;
            SpawnZombie();
        }
    }
    
    private void SpawnZombie()
    {
        Vector3 randomPosition;
        do
        {
            randomPosition = GetRandomPosition();
        }
        // Check for player proximity
        while (Vector3.Distance(randomPosition, player.position) < playerDetectionRadius);
        
        var zombie = Instantiate(zombiePrefab, randomPosition, Quaternion.identity).GetComponent<Zombie>();
        currentZombies++;
        zombies.Add(zombie);
    }
    
    private Vector3 GetRandomPosition()
    {
        // Get random position in spawn area
        var randomPosition = new Vector3(
            Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
            0,
            Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z)
        );
        return randomPosition;
    }
}
