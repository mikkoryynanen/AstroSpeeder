using UnityEngine;
using System.Collections.Generic;

public enum Spawnables { Empty, Asteroid1, Asteroid2, Asteroid3, SpaceScrap }

// Custom serializable class
[System.Serializable]
public class FixedSpawns    //If change these, youll need to set em in "CustomFixedSpawnInspector" aswell or it'll error
{
    public string desc;// Helper
    public string spawnPos1;
    public string spawnPos2;
    public string spawnPos3;
    public string spawnPos4;
    public string spawnPos5;
    public string spawnPos6;
    public string spawnPos7;
    public string spawnPos8;
    public string spawnPos9;
}

public class FixedEndlessSpawner : MonoBehaviour
{
    public float timeBetweenAsteroids;
    static public float asteroidSpeedTimer;
    static public bool canSpawnObjects;
    private int curSpawnWave;
    public GameObject[] spawnableObjects;
    private GameObject[] nextSpawnPoints;
    public GameObject[] pickUps;
    public GameObject[] spawnPoints;
    private GameObject spawnedShield;

    [Header("Pooling variables")]
    public PickUpSpawner pickUpSpawner;
    public int asteroid1PooledAmount = 50;
    public int asteroid2PooledAmount = 20;
    public int asteroid3PooledAmount = 20;
    public int asteroid4PooledAmount = 20;
    public int asteroid5PooledAmount = 20;
    List<GameObject> asteroid1Pool;
    List<GameObject> asteroid2Pool;
    List<GameObject> asteroid3Pool;
    List<GameObject> asteroid4Pool;
    List<GameObject> asteroid5Pool;

    [Header("PreSpawn Variables")]
    private float distToSpawner = 1100f;
    public float emptyStartDistance = 60f;
    private float distance;
    public Transform asteroidSpawner;
    public Transform player;
    public float distToReach = 50f;
    public float spawnInterval = 50f;

    [Header("A1-A5 = asteroids, P1 = pickUp, _,E = empty")]
    public List<FixedSpawns> fixedRows = new List<FixedSpawns>();

    private bool fixedEndless;

    void Start()
    {
        // If game mode has been set as fixed, then this script will start spawning
        if (DataRepo.fixedEndless)
            StartFixedEndless();
    }

    void StartFixedEndless()
    {
        fixedEndless = true;
        SetSpawns();
        curSpawnWave = 0;

        PoolAsteroids();
        PoolPickUpAndEffects();

        PreSpawnAsteroids();
    }

    void Update()
    {   
        if (fixedEndless)
        {
            //Spawn a wave of asteroids every x units player has travelled
            if (player.position.z >= distToReach)
            {
                SpawnObjectWavesV2(false);
                distToReach = distToReach + spawnInterval;
            }
        }
    }

    void PoolPickUpAndEffects()
    {
        //PreSpawn one shield pickup
        spawnedShield = Instantiate(pickUps[1]) as GameObject;
        pickUpSpawner.pickUp = spawnedShield;
        spawnedShield.SetActive(false);

        GameObject pickUpEffect = Instantiate(pickUps[2]) as GameObject;
        pickUpSpawner.pickUpEffect = pickUpEffect;
        pickUpEffect.SetActive(false);
    }

    void PoolAsteroids()
    {
        asteroid1Pool = new List<GameObject>();
        for (int i = 0; i < asteroid1PooledAmount; i++)
        {
            GameObject go = Instantiate(spawnableObjects[0]) as GameObject;
            go.SetActive(false);
            asteroid1Pool.Add(go);
        }

        asteroid2Pool = new List<GameObject>();
        for (int i = 0; i < asteroid2PooledAmount; i++)
        {
            GameObject go = Instantiate(spawnableObjects[1]) as GameObject;
            go.SetActive(false);
            asteroid2Pool.Add(go);
        }

        asteroid3Pool = new List<GameObject>();
        for (int i = 0; i < asteroid3PooledAmount; i++)
        {
            GameObject go = Instantiate(spawnableObjects[2]) as GameObject;
            go.SetActive(false);
            asteroid3Pool.Add(go);
        }

        asteroid4Pool = new List<GameObject>();
        for (int i = 0; i < asteroid4PooledAmount; i++)
        {
            GameObject go = Instantiate(spawnableObjects[3]) as GameObject;
            go.SetActive(false);
            asteroid4Pool.Add(go);
        }

        asteroid5Pool = new List<GameObject>();
        for (int i = 0; i < asteroid5PooledAmount; i++)
        {
            GameObject go = Instantiate(spawnableObjects[4]) as GameObject;
            go.SetActive(false);
            asteroid5Pool.Add(go);
        }
    }

    void PreSpawnAsteroids()
    {
        if (asteroidSpawner == null)
            asteroidSpawner = this.gameObject.transform;

        distToSpawner = asteroidSpawner.position.z - player.position.z;
        distance = emptyStartDistance;

        while (distance <= distToSpawner)
        {
            distance = distance + spawnInterval;

            SpawnObjectWavesV2(true);
        }
    }

    //Finds the spawn gameobjects if not set manually
    void SetSpawns()
    {
        if (spawnPoints.Length != 9)
            spawnPoints = new GameObject[9];
        if (spawnPoints[0] == null)
            spawnPoints[0] = GameObject.Find("AsteroidSpawnPoint_1");
        if (spawnPoints[1] == null)
            spawnPoints[1] = GameObject.Find("AsteroidSpawnPoint_2");
        if (spawnPoints[2] == null)
            spawnPoints[2] = GameObject.Find("AsteroidSpawnPoint_3");
        if (spawnPoints[3] == null)
            spawnPoints[3] = GameObject.Find("AsteroidSpawnPoint_4");
        if (spawnPoints[4] == null)
            spawnPoints[4] = GameObject.Find("AsteroidSpawnPoint_5");
        if (spawnPoints[5] == null)
            spawnPoints[5] = GameObject.Find("AsteroidSpawnPoint_6");
        if (spawnPoints[6] == null)
            spawnPoints[6] = GameObject.Find("AsteroidSpawnPoint_7");
        if (spawnPoints[7] == null)
            spawnPoints[7] = GameObject.Find("AsteroidSpawnPoint_8");
        if (spawnPoints[8] == null)
            spawnPoints[8] = GameObject.Find("AsteroidSpawnPoint_9");
    }

    void SpawnObjectWavesV2(bool preSpawning)
    {
        if (curSpawnWave >= fixedRows.Count)
        {
            curSpawnWave = 0;
        }
        FixedSpawns unpackedElement = fixedRows[curSpawnWave];

        List<string> tempList = new List<string>();

        tempList.Add(unpackedElement.spawnPos1);
        tempList.Add(unpackedElement.spawnPos2);
        tempList.Add(unpackedElement.spawnPos3);
        tempList.Add(unpackedElement.spawnPos4);
        tempList.Add(unpackedElement.spawnPos5);
        tempList.Add(unpackedElement.spawnPos6);
        tempList.Add(unpackedElement.spawnPos7);
        tempList.Add(unpackedElement.spawnPos8);
        tempList.Add(unpackedElement.spawnPos9);

        int curSpawnPos = 0;
        foreach (string id in tempList)
        {
            if (id != "")
            {
                if (id == "A1")
                {
                    for (int i = 0; i < asteroid1Pool.Count; i++)
                    {
                        if (!asteroid1Pool[i].activeInHierarchy)
                        {
                            if (preSpawning)
                                asteroid1Pool[i].transform.position = new Vector3(spawnPoints[curSpawnPos].transform.position.x, spawnPoints[curSpawnPos].transform.position.y, distance);

                            else
                                asteroid1Pool[i].transform.position = spawnPoints[curSpawnPos].transform.position;

                            asteroid1Pool[i].SetActive(true);
                            break;
                        }
                    }
                }

                if (id == "A2")
                {
                    for (int i = 0; i < asteroid2Pool.Count; i++)
                    {
                        if (!asteroid2Pool[i].activeInHierarchy)
                        {
                            if (preSpawning)
                                asteroid2Pool[i].transform.position = new Vector3(spawnPoints[curSpawnPos].transform.position.x, spawnPoints[curSpawnPos].transform.position.y, distance);

                            else
                                asteroid2Pool[i].transform.position = spawnPoints[curSpawnPos].transform.position;

                            asteroid2Pool[i].SetActive(true);
                            break;
                        }
                    }
                }

                if (id == "A3")
                {
                    for (int i = 0; i < asteroid3Pool.Count; i++)
                    {
                        if (!asteroid3Pool[i].activeInHierarchy)
                        {
                            if (preSpawning)
                                asteroid3Pool[i].transform.position = new Vector3(spawnPoints[curSpawnPos].transform.position.x, spawnPoints[curSpawnPos].transform.position.y, distance);

                            else
                                asteroid3Pool[i].transform.position = spawnPoints[curSpawnPos].transform.position;

                            asteroid3Pool[i].SetActive(true);
                            break;
                        }
                    }
                }

                if (id == "A4")
                {
                    for (int i = 0; i < asteroid4Pool.Count; i++)
                    {
                        if (!asteroid4Pool[i].activeInHierarchy)
                        {
                            if (preSpawning)
                                asteroid4Pool[i].transform.position = new Vector3(spawnPoints[curSpawnPos].transform.position.x, spawnPoints[curSpawnPos].transform.position.y, distance);

                            else
                                asteroid4Pool[i].transform.position = spawnPoints[curSpawnPos].transform.position;

                            asteroid4Pool[i].SetActive(true);
                            break;
                        }
                    }
                }
                if (id == "A5")
                {
                    for (int i = 0; i < asteroid5Pool.Count; i++)
                    {
                        if (!asteroid5Pool[i].activeInHierarchy)
                        {
                            if (preSpawning)
                                asteroid5Pool[i].transform.position = new Vector3(spawnPoints[curSpawnPos].transform.position.x, spawnPoints[curSpawnPos].transform.position.y, distance);

                            else
                                asteroid5Pool[i].transform.position = spawnPoints[curSpawnPos].transform.position;

                            asteroid5Pool[i].SetActive(true);
                            break;
                        }
                    }
                }
                if (id == "P1")
                {
                    if (preSpawning)
                        spawnedShield.transform.position = new Vector3(spawnPoints[curSpawnPos].transform.position.x, spawnPoints[curSpawnPos].transform.position.y, distance);

                    else
                        spawnedShield.transform.position = spawnPoints[curSpawnPos].transform.position;

                    spawnedShield.SetActive(true);
                }
            }
            curSpawnPos++;
        }
        curSpawnWave++;
    }
}
