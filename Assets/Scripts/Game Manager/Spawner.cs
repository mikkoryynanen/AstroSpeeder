using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

    public bool spawnAsteroids = true;

    public float timeBetweenAsteroids;
    public float timeBetweenRareSpawns;

    [Header("Spawn rates")]
    public float spawnRate;
    public float collectableSpawnRate = 7.5f;
    public float rareSpawnRate = 25f;
    public float difficultyChangeInterval = 20f;

    public int asteroidsToSpawn = 10;

    static public bool canSpawnAsteroids = true;
    static public bool spawnCheckpoint = false;

    public GameObject[] asteroids;
    public GameObject[] spawnpoints;

    private GameObject currentSpawnpoint;

    private int currentSpawnpointNumber = 0;
    private int currentAsteroidNumber = 0;

    [Header("Pooling variables")]
    public int asteroidPooledAmount = 20;
    public int rareSpawnPooledAmmount = 5;
    public int collectablePooledAmmount = 5;
    List<GameObject> asteroidPool;//Increased to 60, default 40
    List<GameObject> rareSpawnPool;
    List<GameObject> collectablePool;

    [Header("PreSpawn Variables")]
    private float distToSpawner = 1100f;
    public float emptyStartDistance = 60f;
    private float distance;
    public Transform asteroidSpawner;
    public Transform player;
    public float distToReach = 50f;
    public float spawnInterval = 50f;

    private bool randomEndless;
    private GameObject lastSpawn;

    void Start ()
    {
        if (DataRepo.randomEndless || DataRepo.competitiveMode)     //Remove competitiveMode from here when season 1 is over
            StartRandomEndless();
    }

    void StartRandomEndless()
    {
        randomEndless = true;
        //Reset values
        spawnRate = 0.6f;

        //Pool asteroids
        asteroidPool = new List<GameObject>();

        for(int i = 0; i < asteroidPooledAmount; i++) {
            GameObject obj = (GameObject)Instantiate(asteroids[Random.Range(0, asteroids.Length)]);
            obj.SetActive(false);
            asteroidPool.Add(obj);
        }

        currentSpawnpointNumber = 0;
        
        PreSpawnAsteroids();
    }

    void Update ()
    {
        if (randomEndless)
        {
            //Spawn a wave of asteroids every x units player has travelled
            if (player.position.z >= distToReach)
            {
                SpawnAsteroid();
                distToReach = distToReach + spawnInterval;
            }
        }
    }

    void SpawnAsteroid()
    {
        for (int i = 0; i < asteroidPool.Count; i++)
        {
            if (!asteroidPool[i].activeInHierarchy)
            {
                // Reroll spawnPos while its the same as the last one
                while(currentSpawnpoint == lastSpawn)
                {
                    currentSpawnpoint = spawnpoints[Random.Range(0, spawnpoints.Length)];
                }

                lastSpawn = currentSpawnpoint;
                asteroidPool[i].transform.position = currentSpawnpoint.transform.position;
                asteroidPool[i].SetActive(true);
                break;
            }
        }
    }

    void PreSpawnAsteroids ()
    {
        distToSpawner = asteroidSpawner.position.z - player.position.z;
        distance = emptyStartDistance;

        while (distance <= distToSpawner)
        {
            //float distAdd = Random.Range(startSpawnIntervalMin, startSpawnIntervalMax);
            distance = distance + spawnInterval;

            for (int i = 0; i < asteroidPool.Count; i++)
            {
                if (!asteroidPool[i].activeInHierarchy)
                {
                    // Reroll spawnPoint if its the same as on the last time
                    while (currentSpawnpoint == lastSpawn)
                    {
                        currentSpawnpoint = spawnpoints[Random.Range(0, spawnpoints.Length)];
                    }
                    lastSpawn = currentSpawnpoint;
                    asteroidPool[i].transform.position = new Vector3(currentSpawnpoint.transform.position.x, currentSpawnpoint.transform.position.y, distance);
                    asteroidPool[i].SetActive(true);
                    break;
                }
            }
        }
    }
}
