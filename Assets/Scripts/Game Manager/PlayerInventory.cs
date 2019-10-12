
// numero string, joka on muotoa 1010100010001001010001000. 0 = että ei ole auki, 1 on auki
// pelaajannimi = tallennusnimessä
// string array[0] = pisteet 1-18 ( 18 merkkiä ), 18-20 = current skin, 21-40 alukset (20 merkkiä)
// string array[1] = materiaalit 1 -> (default 20 merkkiä)
// string array[2] = partikkelit 1 -> (default 20 merkkiä)


using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerInventory : MonoBehaviour
{
    [Header("Players inventory of usable items")]
    public string playerName;
    public long cash;   //is "long" since int cant be this long
    public List<GameObject> usableShips= new List<GameObject>();
    public List<Material> usableMaterials = new List<Material>();
    public List<GameObject> usableParticles = new List<GameObject>();

    [Header("Cached items that can be opened, add all openable items here")]
    public List<Material> cachedMaterials = new List<Material>();
    public List<GameObject> cachedParticles = new List<GameObject>();
    public List<GameObject> cachedShips = new List<GameObject>();

    private string currentMaterials = "0000000000000000000000000000000000000000";
    private string currentParticles = "0000000000000000000000000000000000000000";
    private string currentShips = "00000000000000000000";
    //[Header("Default Data strings if no data was found for current name")]
    private string defaultData1 = "0000000000000000000000000000000000000000"; //2222222222222222220033333333333333333333 // test string
    private string defaultData2 = "0000000000000000000000000000000000000000"; //4444444444444444444444444444444444444444 // test string
    private string defaultData3 = "0000000000000000000000000000000000000000"; //5555555555555555555555555555555555555555 // test string

    private string curSkinString;

    void Awake()
    {
        LoadAndReadData();
        //AddMaterial(cachedMaterials[2]);      //For testing purposes
        //AddParticle(cachedParticles[2]);      //For testing purposes
        //AddShip(cachedShips[2]);              //For testing purposes
        //SetCash(9);                           //For testing purposes
        //ChangeMaterialTo(cachedMaterials[1]); //For testing purposes
    }

    void SaveData()
    {
        string[] newSaveInfo = new string[3];
        newSaveInfo[0] = cash.ToString("000000000000000000") + curSkinString + currentShips;
        newSaveInfo[1] = currentMaterials;
        newSaveInfo[2] = currentParticles;
        PlayerPrefsX.SetStringArray(playerName, newSaveInfo);
        //LoadAndReadData();//Needed if you remove .Adds from addmaterial etc.
    }

    void LoadAndReadData()// For each item marked as "opened", adds them to inventory list for further usage
    {
        /*if (DataRepo.playerName != null || DataRepo.playerName != "")
            playerName = DataRepo.playerName;
        if (DataRepo.playerName == null || DataRepo.playerName == "")
            playerName = "Testi_Pena";*/

        // Get the saved string array data if exists, else create a new array
        string[] loadedData = PlayerPrefsX.GetStringArray(playerName);
        if (loadedData.Length == 0)
            loadedData = new string[3];

        // = = = POINTS/CASH AND SHIP DATA = = =
        if (loadedData[0] == "" || loadedData[0] == null)
            loadedData[0] = defaultData1;

        // points/cash
        string cashString = loadedData[0].Substring(0, 18);
        cash = Convert.ToInt64(cashString); //Replaces the old cash amount

        //current skin
        curSkinString = loadedData[0].Substring(18, 2);

        if(Application.loadedLevelName == "FixedEndlessMode" || Application.loadedLevelName == "RandomEndlessMode" || Application.loadedLevelName == "DemoScene")
            GameObject.Find("PlayerShipMesh").GetComponent<Renderer>().material = cachedMaterials[int.Parse(curSkinString)];

        // ships
        currentShips = loadedData[0].Substring(20, 20);

        string[] eachShipNmbr = new string[currentShips.Length];
        for (int i = 0; i < currentShips.Length; i++)
        {
            eachShipNmbr[i] = currentShips[i].ToString();

            if (eachShipNmbr[i] == "1" && !usableShips.Contains(cachedShips[i]))
                usableShips.Add(cachedShips[i]);
        }

        // = = = MATERIAL DATA = = =
        if (loadedData[1] == "" || loadedData[1] == null)
            loadedData[1] = defaultData2;

        currentMaterials = loadedData[1];

        string[] eachMatNmbr = new string[currentMaterials.Length];
        for (int i = 0; i < currentMaterials.Length; i++)
        {
            eachMatNmbr[i] = currentMaterials[i].ToString();

            if (eachMatNmbr[i] == "1" && !usableMaterials.Contains(cachedMaterials[i]))
                usableMaterials.Add(cachedMaterials[i]);
        }

        // = = = PARTICLE DATA = = =
        if (loadedData[2] == "" || loadedData[2] == null)
            loadedData[2] = defaultData3;

        currentParticles = loadedData[2];

        string[] eachParticleNmbr = new string[currentParticles.Length];
        for (int i = 0; i < currentParticles.Length; i++)
        {
            eachParticleNmbr[i] = currentParticles[i].ToString();

            if (eachParticleNmbr[i] == "1" && !usableParticles.Contains(cachedParticles[i]))
                usableParticles.Add(cachedParticles[i]);
        }
    }

    void AddMaterial (Material mat)
    {
        if (!cachedMaterials.Contains(mat))
        {
            cachedMaterials.Add(mat);
            Debug.LogWarning("The material you tried to add wasnt in cachedMaterials. Adding it now. Add this mat to cachedMaterials off game to fix this error from popping up");
        }

        else if (!usableMaterials.Contains(mat))
        {
            usableMaterials.Add(mat);// Not needed if you load and read data after
            string newString = currentMaterials;

            newString = newString.Remove(cachedMaterials.IndexOf(mat), 1);
            newString = newString.Insert(cachedMaterials.IndexOf(mat), "1");

            currentMaterials = newString;
            SaveData();
        }
        else
            Debug.Log(mat.name + " material is already unlocked!");
    }

    void AddParticle (GameObject p)
    {
        if (!cachedParticles.Contains(p))
        {
            cachedParticles.Add(p);
            Debug.LogWarning("The particle you tried to add wasnt in cachedParticles. Adding it now. Add this p to cachedParticles off game to fix this error from popping up");
        }

        else if (!usableParticles.Contains(p))
        {
            usableParticles.Add(p);//
            string newString = currentParticles;

            newString = newString.Remove(cachedParticles.IndexOf(p), 1);
            newString = newString.Insert(cachedParticles.IndexOf(p), "1");

            currentParticles = newString;
            SaveData();
        }
        else
            Debug.Log(p.name + " particle is already unlocked!");
    }

    void AddShip (GameObject s)
    {
        if (!cachedShips.Contains(s))
        {
            cachedShips.Add(s);
            Debug.LogWarning("The ship you tried to add wasnt in cachedShips. Adding it now. Add this go to cachedShips off game to fix this error from popping up");
        }

        else if (!usableShips.Contains(s))
        {
            usableShips.Add(s);//
            string newString = currentShips;

            newString = newString.Remove(cachedShips.IndexOf(s), 1);
            newString = newString.Insert(cachedShips.IndexOf(s), "1");

            currentShips = newString;
            SaveData();
        }
        else
            Debug.Log(s.name + " ship is already unlocked!");
    }

    void SetCash(int cashChange)
    {
        if (cash + cashChange >= 0)
        { 
            cash = cash + cashChange;
            SaveData();
        }
        else
            Debug.Log("You cant have negative amount of gold, you fool!");
    }

    void OnApplicationQuit ()
    {
        Debug.Log("Saving on app quit");
        SaveData();
    }

    void ChangeMaterialTo (Material mat)
    {
        curSkinString = cachedMaterials.IndexOf(mat).ToString("00");

        GameObject.Find("PlayerShipMesh").GetComponent<Renderer>().material = mat;
        SaveData();
    }
}
