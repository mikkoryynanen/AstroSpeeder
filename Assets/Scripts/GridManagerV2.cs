using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManagerV2 : MonoBehaviour {

    // Pool 3 grids. Spawn them next to each other at start(as player starts within the 1st one)
    // When player passes the edge of the new grid, the one behind gets transform.positioned
    //  to the front of them all. 
    // LENGHT BETWEEN GRID PIVOTS 607.1375

    public float distanceToReach;
    public Transform player;
    public GameObject grid;
    public GameObject gridParent;
    public int lastGrid;
    public List<GameObject> pooledGrids;

    public Material gridMaterial;
    public Color gridColor;
    public Color[] colorsToTintTo;
    public Gradient tintGradient;
    public Gradient tintGradient2;
    public List<Transform> pooledGridsPanels;
    public float colorChangeInterval = 1.5f;
    [Range(0, 1)]
    public float alphaValue = 0.27f;

    public PlayerController playercontroller;

    void Start ()
    {
        if (player == null)
            player = GameObject.Find("Player").transform;

        if (playercontroller == null)
            playercontroller = GameObject.Find("Player").GetComponent<PlayerController>();

        lastGrid = -1;
        PoolAndSetGrids();

        gridColor = gridMaterial.GetColor("_TintColor");

        InvokeRepeating("ChangeGridColor", 0, colorChangeInterval);
    }
	
	void Update ()
    {
        //If player has entered the second piece of grid
	    if(player.position.z >= distanceToReach)
        {
            MoveGrid();
        }
    }

    void PoolAndSetGrids ()
    {
        pooledGrids = new List<GameObject>();

        for (int i = 0; i < 3; i++)//3 = pooledGridAmount
        {
            //Instatiate, parent under "Grids" mother GO and pool
            GameObject go = (GameObject)Instantiate(grid);
            go.transform.parent = gridParent.transform;
            pooledGrids.Add(go);

            //Set the starting Grids in line
            if (i == 0)
                pooledGrids[i].transform.position = new Vector3(0f, 0f, -5f);
            if (i == 1)
                pooledGrids[i].transform.position = new Vector3(0f, 0f, 602.1375f);
            if (i == 2)
                pooledGrids[i].transform.position = new Vector3(0f, 0f, 1209.275f);
        }
        distanceToReach = pooledGrids[1].transform.position.z;

        //Get sidepanels from each pooledGrid
        for(int i = 0; i < pooledGrids.Count; i++) {
            for(int m = 0; m < 4; m++) {
                pooledGridsPanels.Add(pooledGrids[i].transform.Find(m.ToString()));
            }
        }
    }

    //Get the last piece of the grid and set it to be 1st
    void MoveGrid ()
    {
        distanceToReach += 607.1375f;
        lastGrid += 1;

        if (lastGrid > 2)
            lastGrid = 0;

        pooledGrids[lastGrid].transform.position = new Vector3(0f, 0f, distanceToReach + 607.1375f);
    }

    void ChangeGridColor() {
        //Change grid color over time

        gridColor = tintGradient2.Evaluate(playercontroller.speed / 500);//750
        gridColor.a = alphaValue;
        gridMaterial.SetColor("_TintColor", gridColor);
        //Debug.Log("Changed color " + gridMaterial.GetColor("_TintColor"));
    }
}
