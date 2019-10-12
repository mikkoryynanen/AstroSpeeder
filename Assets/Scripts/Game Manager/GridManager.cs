using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GridManager : MonoBehaviour {

    public GameObject grid;
    public List<GameObject> gridPool;

    public int gridsToPool = 3;

    public float spacing = 2f;
    private float oldZ = 0f;
    private float newZ = 0f;
    private float positionMultiplier;
    public float distanceTraveled = 0f;

    Transform camera;
    public float lastPosition;
    bool canSpawn = true;
    bool canDespawn = false;
    public int previousGridIndex = 0;
    int activeGridAmmount = 0;


    void Start () {
        camera = Camera.main.transform;

        //Pool grids
        gridPool = new List<GameObject>();
	    for(int i = 0; i < gridsToPool; i++) {
            GameObject obj = (GameObject)Instantiate(grid);
            obj.SetActive(false);
            gridPool.Add(obj);
        }
	}

    void Update () {
        /*newZ = camera.transform.position.z;
        distanceTraveled += newZ - oldZ;
        oldZ = newZ;

        if(distanceTraveled >= spacing) {
            distanceTraveled = 0f;
            positionMultiplier += 606f;
            spawnNewObject();
        }*/

        if(camera.transform.position.z >= lastPosition - 1212 && canSpawn) {
            canSpawn = false;

            DeSpawnGrid();
            SpawnGrid();
        }
	}

    void SpawnGrid() {
        for(int i = 0; i < gridPool.Count; i++) {
            if(!gridPool[i].activeInHierarchy) {
                gridPool[i].transform.position = new Vector3(0, 0, lastPosition);
                gridPool[i].SetActive(true);
                break;
            }
        }

        lastPosition += 606f;
        canSpawn = true;
    }

    void DeSpawnGrid() {
        //gridPool[0].SetActive(false);

        for(int i = 0; i < gridPool.Count; i++) {
            if(gridPool[i].activeInHierarchy) {
                gridPool[i].transform.position = new Vector3(0, 0, lastPosition);
                gridPool[i].SetActive(true);
                break;
            }
        }
    }
}
