using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PickUpSpawner : MonoBehaviour
{


    [Header("PickUps and effects, make sure they are at matching place in array")]
    public GameObject[] spawnablePickUps;
    public GameObject[] pickUpSpawnEffects;
    public GameObject[] pickUpEffects;
    public GameObject[] spawnPoints;

    [Header("Spawning variables")]
    public float silverSpawnChance = 80f;
    public float spawnTime = 1f;
    public float despawnTime = 0.5f;
    public float pickUpDuration = 5f;
    public float timeBetweenPickups = 5f;
    //public float slowDropMinSpeed = 150f;
    public GameObject player;
    //public bool followPlayer;//Temp for fixed endless mode
    public bool spawningSilver = true;
    public bool spawningGold = true;

    private List<GameObject> tempList;
    [Header("Pooling lists")]
    public List<GameObject> pickUpPool;
    public List<GameObject> pickUpSpawnEffectPool;
    public List<GameObject> pickUpEffectPool;

    //Private vars to hold data for last pickups and effects
    private int randomSpawn;
    public GameObject pickUp;
    public GameObject pickUpEffect;
    private PlayerController playerController;

//    public Image shieldSlider;
    private int spawned;
    private int randomPickUp = 0;

    void Start()
    {
        if (DataRepo.randomEndless)
        {
            StartingSort();
            StartCoroutine("YieldTillNextSpawn");

            if (playerController == null)
                playerController = player.GetComponent<PlayerController>();

//            if (shieldSlider == null)
//                shieldSlider = GameObject.Find("ShieldBar").GetComponent<Image>();
        }
    }
    /*
        int ByDistance(GameObject a, GameObject b)
        {
            float distToA = Vector3.Distance(player.transform.position, a.transform.position);
            float distToB = Vector3.Distance(player.transform.position, b.transform.position);
            return distToA.CompareTo(distToB);
        }
    */
    /*  IEnumerator SpawnPickUp()
        {
            //Set spawnPoints in a list where [0] is closest etc.
            //tempList = new List<GameObject>(spawnPoints);
            //tempList.Sort(ByDistance);

            //Randomize the spawn pos excluding the 3 closest ones
            //randomSpawn = Random.Range(3, tempList.Count);


            //Pick random spawnPoint to spawn
            randomSpawn = Random.Range(0, spawnPoints.Length);

            //Randomize the spawning pickUp
            int randomPickUp;
            //if (playerController.speed < slowDropMinSpeed)//Disabled as slowMo is no longer live
            //    randomPickUp = Random.Range(0, spawnablePickUps.Length -1);
            //else
            /*if (shieldSlider.fillAmount == 1) // This bit made spawner not to spawn shield regens when full shields
                randomPickUp = Random.Range(0, spawnablePickUps.Length - 1);
            else
                randomPickUp = Random.Range(0, spawnablePickUps.Length);*/

    /*        int randomRoll = Random.Range(0, 99);

            // Check what to spawn, also check if its available to spawn
            if (randomRoll < silverSpawnChance)
            {
                if (spawningSilver)
                    randomPickUp = 1;
                else
                    randomPickUp = 3;
            }
            else
            {
                if (spawningGold)
                    randomPickUp = 0;
                else
                    randomPickUp = 3;
            }

            if (randomPickUp == 0 || randomPickUp == 1)
            {
                //Enable and move the flickering effect of pickup at the spawnLocation
                GameObject spawnEffect = pickUpSpawnEffectPool[randomPickUp];
                spawnEffect.SetActive(true);
                spawnEffect.transform.position = spawnPoints[randomSpawn].transform.position;
                spawnEffect.transform.parent = spawnPoints[randomSpawn].transform;

                //Wait for spawnTime amount and then spawn the actual pickUp and deactivate the effect
                yield return new WaitForSeconds(spawnTime);
                //Disable the effect
                spawnEffect.SetActive(false);

                //Enable and spawn to assigned spawnLocation
                pickUp = pickUpPool[randomPickUp];
                pickUp.SetActive(true);
                pickUp.transform.position = spawnPoints[randomSpawn].transform.position;
                pickUp.transform.parent = spawnPoints[randomSpawn].transform;

                pickUpEffect = pickUpEffectPool[randomPickUp];
                yield return new WaitForSeconds(pickUpDuration);

                //If pickup is still up after its duration, start despawning
                //     if (pickUp.activeInHierarchy == true)
                {
                    pickUp.SetActive(false);
                    //pickUpEffect.SetActive(true);
                    //pickUpEffect.transform.position = spawnPoints[randomSpawn].transform.position;
                    //pickUpEffect.transform.parent = spawnPoints[randomSpawn].transform;

                    //Disable the effect on pickUp despawn
                    //yield return new WaitForSeconds(despawnTime);
                    //pickUpEffect.SetActive(false);
                }
            }
            //Start counter after pickUp duration has run out
            StartCoroutine("YieldTillNextSpawn");
            yield return true;
        }*/

    IEnumerator SpawnPickUp()
    {
        //Pick random spawnPoint to spawn
        randomSpawn = Random.Range(0, spawnPoints.Length);

        // If spawner has spawned 3 silvers, then spawn gold
        if (spawned == 3)
        {// Gold
            if (spawningGold)
                randomPickUp = 0;
            else if (spawningSilver && !spawningGold)
                randomPickUp = 1;
            else
                randomPickUp = 3;
            spawned = 0;
        }
        else
        {// Silver
            if (spawningSilver)
                randomPickUp = 1;
            else
                randomPickUp = 3;
            spawned++;
        }

        if (randomPickUp == 0 || randomPickUp == 1)
        {
            //Enable and move the flickering effect of pickup at the spawnLocation
            GameObject spawnEffect = pickUpSpawnEffectPool[randomPickUp];
            spawnEffect.SetActive(true);
            spawnEffect.transform.position = spawnPoints[randomSpawn].transform.position;
            spawnEffect.transform.parent = spawnPoints[randomSpawn].transform;

            //Wait for spawnTime amount and then spawn the actual pickUp and deactivate the effect
            yield return new WaitForSeconds(spawnTime);
            //Disable the effect
            spawnEffect.SetActive(false);

            //Enable and spawn to assigned spawnLocation
            pickUp = pickUpPool[randomPickUp];
            pickUp.SetActive(true);
            pickUp.transform.position = spawnPoints[randomSpawn].transform.position;
            pickUp.transform.parent = spawnPoints[randomSpawn].transform;

            pickUpEffect = pickUpEffectPool[randomPickUp];
            yield return new WaitForSeconds(pickUpDuration);

            //If pickup is still up after its duration, start despawning
            //     if (pickUp.activeInHierarchy == true)
            {
                pickUp.SetActive(false);
                //pickUpEffect.SetActive(true);
                //pickUpEffect.transform.position = spawnPoints[randomSpawn].transform.position;
                //pickUpEffect.transform.parent = spawnPoints[randomSpawn].transform;

                //Disable the effect on pickUp despawn
                //yield return new WaitForSeconds(despawnTime);
                //pickUpEffect.SetActive(false);
            }
        }
        //Start counter after pickUp duration has run out
        StartCoroutine("YieldTillNextSpawn");
        yield return true;
    }

    IEnumerator YieldTillNextSpawn()
    {
        yield return new WaitForSeconds(timeBetweenPickups);

        //After counter is finished, start spawning PickUp
        StartCoroutine("SpawnPickUp");
        yield return true;
    }

    public IEnumerator PickedUp()
    {
        //When pickup, this gets called. Hides the pickup and shows pickUp Effect
        Vector3 lastSpot = pickUp.transform.position;

        pickUp.SetActive(false);
        pickUpEffect.SetActive(true);
        pickUpEffect.transform.position = lastSpot;
        pickUpEffect.transform.parent = spawnPoints[randomSpawn].transform;

        //Disable the effect on pickUp despawn
        yield return new WaitForSeconds(despawnTime);
        pickUpEffect.SetActive(false);

        yield return true;
    }

    void StartingSort()
    {
        //Spawn each pickUp and spawnEffect on SpawnPoint1 on start and pool from there
        foreach (GameObject pickupGo in spawnablePickUps)
        {
            GameObject pickUp = Instantiate(pickupGo, spawnPoints[0].transform.position, Quaternion.identity) as GameObject;
            pickUp.transform.parent = spawnPoints[0].transform;
            pickUpPool.Add(pickUp);
            pickUp.SetActive(false);
        }
        foreach (GameObject spawnEffect in pickUpSpawnEffects)
        {
            GameObject pickUpSpawnEffect = Instantiate(spawnEffect, spawnPoints[0].transform.position, Quaternion.identity) as GameObject;
            pickUpSpawnEffect.transform.parent = spawnPoints[0].transform;
            pickUpSpawnEffect.transform.parent = spawnPoints[0].transform;
            pickUpSpawnEffectPool.Add(pickUpSpawnEffect);
            pickUpSpawnEffect.SetActive(false);
        }
        foreach (GameObject effect in pickUpEffects)
        {
            GameObject pickUpEffect = Instantiate(effect, spawnPoints[0].transform.position, Quaternion.identity) as GameObject;
            pickUpEffect.transform.parent = spawnPoints[0].transform;
            pickUpEffect.transform.parent = spawnPoints[0].transform;
            pickUpEffectPool.Add(pickUpEffect);
            pickUpEffect.SetActive(false);
        }
    }
}
