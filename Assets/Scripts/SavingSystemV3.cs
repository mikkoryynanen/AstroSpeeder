using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;

//v2 Array of strings, joissa stringit ovat numeroina. Valuutat lukuna, avatut 1=auki, 0=kiinni
//string array[0] = hopea valuutta 1-18 (18 merkkiä), kulta valuutta 18-> (18 merkkiä?)
//string array[1] = avatut materiaalit/tekstuurit 1 -> (defaulttina 20 merkkiä)
//string array[2] = avatut thrusters 1 -> (defaulttina 20)
//string array[3] = avatut valot lights 1-> (default 20)
//string array[4] = customizationin current skin, lights, thrusters ja asetukset
//array 4 muotoa 112233344455567, jossa 1 = curSKin, 2 = curLights, 3 = curThrusters
//  4 = musicVolume, 5 = SFXVolume, 6 = soundEnabled, 7 = vibrate Enabled
//186,0.67,2.492 is default number 0 emission

public class SavingSystemV3 : MonoBehaviour
{
    //Google play manager
    public GooglePlayManager GPManager;
    public Text debugText;

    public enum moduleType
    {
        none,
        strafeSpeed,
        acceleration,
        shieldRegen,
        shieldAmount,
        silverCollectRate,
        goldCollectRate,
        pickUpSpawnRate,
        superShieldCooldown,
        superShieldDuration,
        asteroidSpawnRate,
        scoreModifier,
        scoreClimbRate,
        scoreMultiplierDecrease
    }

    [Serializable]
    public class MaterialHolder
    {
        public string name;
        public int silverCost;
        public int goldCost;
        public bool isOwned;
        public Material material;
    }
    [Serializable]
    public class ThrusterHolder
    {
        public string name;
        public int silverCost;
        public int goldCost;
        public bool isOwned;
    }
    [Serializable]
    public class ThrusterHolderByColor
    {
        public string name;
        public int silverCost;
        public int goldCost;
        public bool isOwned;
        public GameObject thruster;
        public List<ThrusterHolder> thrustersByColor = new List<ThrusterHolder>();
    }
    [Serializable]
    public class ThrusterColorHolder
    {
        public string name;
        public Color thrusterColor;
    }
    [Serializable]
    public class ColorHolder
    {
        public string name;
        public int silverCost;
        public int goldCost;
        public bool isOwned;
        [Range(0f, 359f)]
        public float HHue;
        [Range(0f, 1f)]
        public float SSaturation = 1f;
        [Range(0f, 10f)]
        public float VBrightness = 1f;
    }
    [Serializable]
    public class ModuleHolder
    {
        public string name;
        public string description;
        public int silverCost;
        public int goldCost;
        public bool isOwned;
        public moduleType modEffectType1;
        public float effect1AmountPercent;
        public moduleType modEffectType2;
        public float effect2AmountPercent;
        public moduleType modEffectType3;
        public float effect3AmountPercent;
        public moduleType modEffectType4;
        public float effect4AmountPercent;
        public bool superShieldUsable = true;
        public bool shieldRegenActive = true;
        public bool spawningSilver = true;
        public bool spawningGold = true;
        public bool shieldActive = true;
        public int orderInShop = 0;
    }

    public GameObject sampleButton;
    public GameObject headerButton;
    public GameObject thrusterColorButton;
    private Transform shipMaterialContentPanel;
    private Transform shipThrustersContentPanel;
    private Transform shipThrusterColorContentPanel;
    private Transform shipLightsContentPanel;

    [Header("Players inventory of usable items")]
    public string playerName;   //NO LONGER NEEDED
    public long silverCurrency;   //is "long" since int cant be this long
    public long goldCurrency;   //is "long" since int cant be this long

    [Header("Cached items that can be opened, add all openable items here")]
    public List<MaterialHolder> cachedMaterials = new List<MaterialHolder>();
    int ownedSkins;
    public List<ThrusterHolderByColor> cachedThrusters = new List<ThrusterHolderByColor>();
    public List<ThrusterColorHolder> cachedThrusterColors = new List<ThrusterColorHolder>();
    public List<ColorHolder> emissionColorsCache = new List<ColorHolder>();
    public List<ModuleHolder> cachedModules = new List<ModuleHolder>();

    private string currentMaterials = "0000000000000000000000000000000000000000";
    private string currentThrusters = "0000000000000000000000000000000000000000";
    private string currentLights = "0000000000000000000000000000000000000000";
    private string currentCustomization = "00000000000000";
    private string currentModules = "0000000000000000000000000000000000000000";//(40characters)

    [Header("Default Data strings if no data was found")]  //0000000000000000000000000000000000000000
    //For currencies; 2222222222222222223333333333333333330000 where 2:s are silver, 3:s are gold
    public string defaultData1 = "0000000000000000000000000000000000000000"; //2222222222222222223333333333333333330000 // test string
    //For materials; 40 characters
    public string defaultData2 = "1000000000000000000000000000000000000000"; //4444444444444444444444444444444444444444 // test string
    //For Thrusters;  250/10 = 25. 250 characters, 25 different color thrusters for 10 different thrusters
    // First char of the string stands for the default skin. If its open, then also the header is open. Rest 39 are the other colors for that thruster
    public string defaultData3 = "1000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"; //5555555555555555555555555555555555555555 // test string
    //For Lights 40 characters    1000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
    public string defaultData4 = "1000000000000000000000000000000000000000"; //6666666666666666666666666666666666666666 // test string
    //For customization and settings; where 1 = curSKin, 2 = curLights, 3 = curThrusters
    //  4 = musicVolume, 5 = SFXVolume, 6 = soundEnabled, 7 = vibrate Enabled
    public string defaultData5 = "000000007007500"; //11223344455567
    public string defaultData6 = "1000000000000000000000000000000000000000"; // Modules default data, 1st module is the default one

    private Renderer playerRenderer;
    public static long personalRandomModeHighscore;
    public static long personalFixedModeHighscore;
    public static long personalContestModeHighscore;
    private string curSkinString;
    private string curLightsString;
    private string curThrustersString;
    private string curThrusterColorString;
    private string curModulesString;
    private string musicVolumeString;
    private string SfxVolumeString;
    [HideInInspector] public string soundsEnabledString;
    [HideInInspector] public string vibrateEnabledString;
    [HideInInspector] public int curSkin = 0;
    [HideInInspector] public int curEmission = 0;
    [HideInInspector] public int curThrusters = 0;
    [HideInInspector] public int curThrusterColor = 0;
    [HideInInspector] public int curModule = 0;
    public AudioMixer master;
    private float startMasterVolume = 0f;
    [HideInInspector] public float musicVolume;
    [HideInInspector] public float sfxVolume;

    [HideInInspector] public List<GameObject> materialButtons;
    [HideInInspector] public List<GameObject> thrusterButtons;
    [HideInInspector] public List<GameObject> thrusterHeaderButtons;
    [HideInInspector] public List<GameObject> lightButtons;
    [HideInInspector] public List<GameObject> moduleButtons;

    public List<Color> thrusterDefaultColors;

    private string[] updatedData;

    //So the "Start" will run on levelChange as this script is carried over the scenes
    void OnLevelWasLoaded()
    {
        Start();
        Time.timeScale = 1;
        //Debug.Log("Loaded Level");
    }

    //Used to be awake, but setting volume didnt work in there
    void Start()
    {
        //Find GooglePlayManager 
#if UNITY_ANDROID
        GPManager = GameObject.Find("GooglePlayManager").GetComponent<GooglePlayManager>();
#endif

        if (Application.loadedLevelName != "SplashScreen")
        {
            ConfigVars();
            LoadAndReadData();
            //CheckAndSetOwned(); 
        }
        //master.GetFloat("MainVolume", out startMasterVolume);

        CheckForSkinsAchievement();

        //Function giving money in editor only
        /*#if UNITY_EDITOR
        silverCurrency = 900000;
        goldCurrency = 900000;
        #endif*/
    }

    public void CheckForSkinsAchievement()
    {
        ownedSkins = 0;
        //Check how many skins does the player have
        for (int i = 0; i < cachedMaterials.Count; i++)
        {
            if (cachedMaterials[i].isOwned)
            {
                ownedSkins++;
                //Debug.Log("ownedSkins: " + ownedSkins);

                if (ownedSkins == 6 && !PlayerPrefsX.GetBool("Achievement_5Skins"))
                {
                    GPManager.Skins();
                    //Debug.Log("5 skins");
                }

                if (ownedSkins == 11 && !PlayerPrefsX.GetBool("Achievement_10Skins"))
                {
                    GPManager.MoreSkins();
                    //Debug.Log("10 skins");
                }
            }
        }
    }

    // Collect the data from the strings and save it as array of strings
    public void SaveData()
    {
        string[] newSaveInfo = new string[9];
        newSaveInfo[0] = silverCurrency.ToString("000000000000000000") + goldCurrency.ToString("000000000000000000");
        newSaveInfo[1] = currentMaterials;
        newSaveInfo[2] = currentThrusters;
        newSaveInfo[3] = currentLights;

        float fixedMusicVolume = musicVolume + 80f;
        float fixedSfxVolume = sfxVolume + 80f;

        newSaveInfo[4] = curSkin.ToString("00") + curEmission.ToString("00") + curThrusters.ToString("0") +
            curThrusterColor.ToString("00") + fixedMusicVolume.ToString("000") + fixedSfxVolume.ToString("000")
            + soundsEnabledString + vibrateEnabledString;

        newSaveInfo[5] = personalRandomModeHighscore.ToString();
        newSaveInfo[6] = personalFixedModeHighscore.ToString();
        newSaveInfo[7] = personalContestModeHighscore.ToString();

        newSaveInfo[8] = currentModules + curModule.ToString("00");
        //Debug.Log(newSaveInfo[8]);

        PlayerPrefsX.SetStringArray(playerName, newSaveInfo);
    }

    // Get saved data and process it
    void LoadAndReadData()
    {
        // Get the saved string array data if exists, else create a new array
        string[] loadedData = PlayerPrefsX.GetStringArray(playerName);

        // IF there is no data whatsoever, create new data array and set to be 1st run
        if (loadedData.Length == 0)
        {
            loadedData = new string[9];
            //Debug.Log("Running game for the 1st time!");
            DataRepo.isFirstRun = true;
        }

        // If did find saveData, but it was done with older version of a script
        if (loadedData.Length != 9)
        {
            Debug.Log("Loading Outdated data, fixing the save files");
            LoadOutdatedData(loadedData);
            loadedData = updatedData;
        }

        // = = = SILVER AND GOLD CURRENCY DATA = = =
        //If theres no data, create new one
        if (loadedData[0] == "" || loadedData[0] == null)
            loadedData[0] = defaultData1;

        // silver currency
        string silverCashString = loadedData[0].Substring(0, 18);
        silverCurrency = Convert.ToInt64(silverCashString);

        // gold currency
        string goldCashString = loadedData[0].Substring(18, 18);
        goldCurrency = Convert.ToInt64(goldCashString);

        // = = = CUSTOMIZATION AND SETTINGS DATA = = =
        //If theres no data, create new one
        if (loadedData[4] == "" || loadedData[4] == null)
            loadedData[4] = defaultData5;

        // current skin
        curSkinString = loadedData[4].Substring(0, 2);
        Int32.TryParse(curSkinString, out curSkin);
        //if (Application.loadedLevelName == "FixedEndlessMode" || Application.loadedLevelName == "RandomEndlessMode" || Application.loadedLevelName == "DemoScene")
        playerRenderer.material = cachedMaterials[int.Parse(curSkinString)].material;

        // current lights - NOW USES THE "THRUSTER COLOR" -
/*      curLightsString = loadedData[4].Substring(2, 2);
        Int32.TryParse(curLightsString, out curEmission);
        //if (Application.loadedLevelName == "FixedEndlessMode" || Application.loadedLevelName == "RandomEndlessMode" || Application.loadedLevelName == "DemoScene")
        SetEmission(emissionColorsCache[curEmission].HHue, emissionColorsCache[curEmission].SSaturation, emissionColorsCache[curEmission].VBrightness);*/

        // current thrusters type
        curThrustersString = loadedData[4].Substring(4, 1);
        Int32.TryParse(curThrustersString, out curThrusters);
        //!!        cachedThrusters[curThrusters].thruster.SetActive(true); is getting set below?
        //Debug.Log(curThrusters);

        // current thrusters color
        curThrusterColorString = loadedData[4].Substring(5, 2);
        Int32.TryParse(curThrusterColorString, out curThrusterColor);
        cachedThrusters[curThrusters].thruster.SetActive(true);

        playerRenderer.material.SetColor("_EmissionColor", cachedThrusterColors[curThrusterColor].thrusterColor);

        // If default color selected, then use the cached own defaults made into the particle
        /*
        if (curThrusterColor == 0)
            cachedThrusters[curThrusters].thruster.GetComponent<ParticleSystem>().startColor = thrusterDefaultColors[curThrusters];
        else
            cachedThrusters[curThrusters].thruster.GetComponent<ParticleSystem>().startColor = cachedThrusterColors[curThrusterColor].thrusterColor;
        */

        // If thruster is NOT unique
        if(curThrusters <= 4)
        {
            // If color is "default", use the individual default color of the thruster
            if (curThrusterColor == 0)
                cachedThrusters[curThrusters].thruster.GetComponent<ParticleSystem>().startColor = thrusterDefaultColors[curThrusters];

            else
                cachedThrusters[curThrusters].thruster.GetComponent<ParticleSystem>().startColor = cachedThrusterColors[curThrusterColor].thrusterColor;
        }

        // If thruster IS unique, set the individual default
        else
        {
            cachedThrusters[curThrusters].thruster.GetComponent<ParticleSystem>().startColor = thrusterDefaultColors[curThrusters];
        }

        // = = = SETTINGS DATA = = =
        // Music volume
        musicVolumeString = loadedData[4].Substring(7, 3);
        float.TryParse(musicVolumeString, out musicVolume);
        musicVolume = musicVolume - 80f;
        master.SetFloat("VolumeParameter", musicVolume);

        //SFX Volume
        SfxVolumeString = loadedData[4].Substring(10, 3);
        float.TryParse(SfxVolumeString, out sfxVolume);
        sfxVolume = sfxVolume - 80f;
        master.SetFloat("SFXParameter", sfxVolume);

        // Mute Toggle
        soundsEnabledString = loadedData[4].Substring(13, 1);
        if (soundsEnabledString == "0")
            master.SetFloat("MainVolume", -5f);
        //master.SetFloat("MainVolume", startMasterVolume);
        else if (soundsEnabledString == "1")
            master.SetFloat("MainVolume", -80f);

        // Vibration Toggle
        vibrateEnabledString = loadedData[4].Substring(14, 1);
        if (vibrateEnabledString == "0")
            DataRepo.canVibrate = true;
        else if (vibrateEnabledString == "1")
            DataRepo.canVibrate = false;

        // = = = = PERSONAL HIGHSCORE DATA = = = =
        // Personal randomEndless highscore
        if (loadedData[5] == "" || loadedData[5] == null)
            loadedData[5] = "0";

        personalRandomModeHighscore = Convert.ToInt64(loadedData[5]);

        // Personal fixedEndless highscore
        if (loadedData[6] == "" || loadedData[6] == null)
            loadedData[6] = "0";

        personalFixedModeHighscore = Convert.ToInt64(loadedData[6]);

        // Personal contestEndless highscore
        if (loadedData[7] == "" || loadedData[7] == null)
            loadedData[7] = "0";

        personalContestModeHighscore = Convert.ToInt64(loadedData[7]);

        // = = = MATERIAL DATA = = = 
        //If theres no data, create new one
        if (loadedData[1] == "" || loadedData[1] == null)
            loadedData[1] = defaultData2;

        currentMaterials = loadedData[1];

        // Chops and creates an array of the each number of the string, and iterates them if open
        string[] eachMatNmbr = new string[currentMaterials.Length];
        for (int i = 0; i < currentMaterials.Length; i++)
        {
            eachMatNmbr[i] = currentMaterials[i].ToString();

            if (eachMatNmbr[i] == "1")
                cachedMaterials[i].isOwned = true;
        }

        // = = = THRUSTERS DATA = = =
        //If theres no data, create new one
        if (loadedData[2] == "" || loadedData[2] == null)
            loadedData[2] = defaultData3;

        currentThrusters = loadedData[2];

        // Loop through every thruster type, check corresponding characters and set open if opened
        for (int thrusterIndex = 0; thrusterIndex < cachedThrusters.Count; thrusterIndex++)
        {
            // Get the corresponding characters in the string and add them to the array, then check and set isOwned accordingly
            int b = 0;
            for (int i = (0 + (cachedThrusterColors.Count * thrusterIndex)); i < (cachedThrusterColors.Count * (thrusterIndex + 1)); i++)
            {
                // Chops and creates an array of the colours count, and iterates them if open
                string[] eachThrusterNmbr = new string[cachedThrusterColors.Count];

                eachThrusterNmbr[b] = currentThrusters[i].ToString();

                if (eachThrusterNmbr[b] == "1")
                    cachedThrusters[thrusterIndex].thrustersByColor[b].isOwned = true;

                b++;
            }

            if (cachedThrusters[thrusterIndex].thrustersByColor[0].isOwned == true)
            {
                cachedThrusters[thrusterIndex].isOwned = true;
            }
        }

        // = = = LIGHTS DATA = = =
        //If theres no data, create new one
        if (loadedData[3] == "" || loadedData[3] == null)
            loadedData[3] = defaultData4;

        currentLights = loadedData[3];

        // Chops and creates an array of the each number of the string, and iterates them if open
        string[] eachLightNmbr = new string[currentLights.Length];
        for (int i = 0; i < currentLights.Length; i++)
        {
            eachLightNmbr[i] = currentLights[i].ToString();

            if (eachLightNmbr[i] == "1")
                emissionColorsCache[i].isOwned = true;
        }

        // = = = MODULES DATA = = =
        // If there was no data, create a new one
        if (loadedData[8] == "" || loadedData[8] == null)
            loadedData[8] = defaultData6;

        // current module
        curModulesString = loadedData[8].Substring(28, 2);
        Int32.TryParse(curModulesString, out curModule);

        // unlocked modules Up to 28 slots
        currentModules = loadedData[8].Substring(0, 28);

        // Chops and creates an array of the each number of the string, and iterates them if open
        string[] eachModuleNmbr = new string[currentModules.Length];
        for (int i = 0; i < currentModules.Length; i++)
        {
            eachModuleNmbr[i] = currentModules[i].ToString();

            if (eachModuleNmbr[i] == "1")
                cachedModules[i].isOwned = true;
        }

        //Make sure there is no contraversy between inspector and code
        //    CheckAndSetOwned();
    }
    /*
    void CheckAndSetOwned()
    {
        // Check and set every "owned" into the string
        foreach (MaterialHolder mat in cachedMaterials)
        {
            if (mat.isOwned)
            {
                int id = cachedMaterials.IndexOf(mat);
                string updatedCurrentMats = currentMaterials;
                updatedCurrentMats = updatedCurrentMats.Remove(id, 1);
                updatedCurrentMats = updatedCurrentMats.Insert(id, "1");
                currentMaterials = updatedCurrentMats;
            }
        }

        for (int thrusterIndex = 0; thrusterIndex < cachedThrusters.Count; thrusterIndex++)
        {
            for (int i = 0; i < cachedThrusterColors.Count; i++)
            {
                if (cachedThrusters[thrusterIndex].thrustersByColor[i].isOwned)
                {
                    int id = i + (cachedThrusters.Count * thrusterIndex);
                    string updatedCurrentThrusters = currentThrusters;
                    updatedCurrentThrusters = updatedCurrentThrusters.Remove(id, 1);
                    updatedCurrentThrusters = updatedCurrentThrusters.Insert(id, "1");
                    currentThrusters = updatedCurrentThrusters;
                    Debug.Log(updatedCurrentThrusters);
                }
            }
        }

        foreach (ColorHolder light in emissionColorsCache)
        {
            if (light.isOwned)
            {
                int id = emissionColorsCache.IndexOf(light);
                string updatedCurrentLights = currentLights;
                updatedCurrentLights = updatedCurrentLights.Remove(id, 1);
                updatedCurrentLights = updatedCurrentLights.Insert(id, "1");
                currentLights = updatedCurrentLights;
            }
        }
    }*/

    // Set material as open and save changes in string 
    public void SetAndOpenMaterial(int id)
    {
        cachedMaterials[id].isOwned = true;
        string updatedCurrentMats = currentMaterials;
        updatedCurrentMats = updatedCurrentMats.Remove(id, 1);
        updatedCurrentMats = updatedCurrentMats.Insert(id, "1");
        currentMaterials = updatedCurrentMats;
        curSkin = id;
    }

    // Set thruster color as open and save changes in string
    public void SetAndOpenThruster(int typeId, int colorId)
    {
/*        // Check if color chosen only has one color
        if (typeId == 5 || typeId == 6 || typeId == 7 || typeId == 8)
            colorId = 0;

        cachedThrusters[typeId].thrustersByColor[colorId].isOwned = true;*/
        cachedThrusters[0].thrustersByColor[colorId].isOwned = true;
//        int charInt = colorId + (cachedThrusterColors.Count * typeId);
        string updatedCurrentThrusters = currentThrusters;
        updatedCurrentThrusters = updatedCurrentThrusters.Remove(colorId, 1);
        updatedCurrentThrusters = updatedCurrentThrusters.Insert(colorId, "1");
        currentThrusters = updatedCurrentThrusters;
        curThrusters = typeId;
        curThrusterColor = colorId;
    }

    // Set emissive light open and save changes in save string
    public void SetAndOpenLight(int id)
    {
        emissionColorsCache[id].isOwned = true;
        string updatedCurrentLights = currentLights;
        updatedCurrentLights = updatedCurrentLights.Remove(id, 1);
        updatedCurrentLights = updatedCurrentLights.Insert(id, "1");
        currentLights = updatedCurrentLights;
        curEmission = id;
    }

    // Set thruster type open and save changes in save string
    public void UnlockThrusterType(int id)
    {
        cachedThrusters[id].thrustersByColor[0].isOwned = true;
        int typeInt = cachedThrusterColors.Count * id;
        string updatedCurrentThrusters = currentThrusters;
        updatedCurrentThrusters = updatedCurrentThrusters.Remove(typeInt, 1);
        updatedCurrentThrusters = updatedCurrentThrusters.Insert(typeInt, "1");
        currentThrusters = updatedCurrentThrusters;
    }

    // Set module open and save changes in save string
    public void SetAndOpenModule(int id)
    {
        cachedModules[id].isOwned = true;
        string updatedCurrentModules = currentModules;
        updatedCurrentModules = updatedCurrentModules.Remove(id, 1);
        updatedCurrentModules = updatedCurrentModules.Insert(id, "1");
        currentModules = updatedCurrentModules;
        curModule = id;
    }

    void ConfigVars()
    {
        //Gets all the thrusters under PlayerShipThrusters and set them as new references for the scene
        GameObject player = GameObject.Find("PlayerShipThrusters");

        if (thrusterDefaultColors.Count == 0)
            thrusterDefaultColors = new List<Color>();

        foreach (Transform child in player.transform)
        {
            cachedThrusters[child.GetSiblingIndex()].thruster = child.gameObject;
            thrusterDefaultColors.Add(child.gameObject.GetComponent<ParticleSystem>().startColor);
        }//cachedThrusters[curThrusters].thruster.GetComponent<ParticleSystem>().startColor = cachedThrusterColors[curThrusterColor].thrusterColor;

        //Get the playersMesh
        playerRenderer = GameObject.Find("PlayerShipMesh").GetComponent<Renderer>();
    }

    //Run this when you enter customization menu to add buttons for each mat/thruster/light
    public void PopulateLists()
    {
        if (shipMaterialContentPanel == null)
            shipMaterialContentPanel = GameObject.Find("SkinContentPanel").transform;

        if (shipThrustersContentPanel == null)
            shipThrustersContentPanel = GameObject.Find("ThrustersContentPanel").transform;

        if (shipThrusterColorContentPanel == null)
            shipThrusterColorContentPanel = GameObject.Find("ThrustersColorContentPanel").transform;

        if (shipLightsContentPanel == null) // Works as modulePanel now
            shipLightsContentPanel = GameObject.Find("LightsContentPanel").transform;

        if (shipMaterialContentPanel.childCount == 0 || shipThrustersContentPanel.childCount == 0 || shipLightsContentPanel.childCount == 0)
        {
            materialButtons = new List<GameObject>();
            thrusterHeaderButtons = new List<GameObject>();
            thrusterButtons = new List<GameObject>();
            moduleButtons = new List<GameObject>();

            //Populate Materials
            foreach (var mat in cachedMaterials)
            {
                //Instantiate buttons for each material and parent/scale it under materials panel
                GameObject newButton = Instantiate(sampleButton) as GameObject;
                newButton.transform.SetParent(shipMaterialContentPanel);
                newButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                SampleButton button = newButton.GetComponent<SampleButton>();
                button.nameLabel.text = mat.name;
                button.matReference = cachedMaterials.IndexOf(mat);
                button.type = "material";
                button.isOwned = mat.isOwned;
                button.silverCost = mat.silverCost;
                button.goldCost = mat.goldCost;
                materialButtons.Add(newButton);
            }

            //Populate Thrusters headers and thrusters
            foreach (var thr in cachedThrusters)
            {
                //Instantiate header buttons for each thruster and parent/scale it under thrusters panel
                GameObject newButton = Instantiate(headerButton) as GameObject;
                newButton.transform.SetParent(shipThrustersContentPanel);
                newButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                newButton.transform.SetAsLastSibling(); // So as it starts from 1st, it should be in correct order?
                HeaderButton button = newButton.GetComponent<HeaderButton>();
                button.nameLabel.text = thr.name;
                button.headerReference = cachedThrusters.IndexOf(thr);
                button.isOwned = thr.isOwned;
                //button.isOwned = thr.thrustersByColor[0].isOwned;
                button.silverCost = thr.silverCost;
                button.goldCost = thr.goldCost;
                thrusterHeaderButtons.Add(newButton);
            }
                // Populate the "foldered" thruster colors under the header Button
//              foreach (var ct in thr.thrustersByColor) // Otettu pois ylemmästä thruster forloopista, näin spawnaa vain kerran
            foreach (var ct in cachedThrusters[0].thrustersByColor)
            {
                //GameObject newCtButton = Instantiate(sampleButton) as GameObject;
                GameObject newCtButton = Instantiate(thrusterColorButton) as GameObject;
                newCtButton.transform.SetParent(shipThrusterColorContentPanel);
                newCtButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1.02f, 1);
                newCtButton.transform.SetAsLastSibling(); // So as it starts from 1st, it should be in correct order?
                SampleButton buttonCt = newCtButton.GetComponent<SampleButton>();
                buttonCt.nameLabel.text = ct.name;
                //buttonCt.nameLabel.color = cachedThrusterColors[thr.thrustersByColor.IndexOf(ct)].thrusterColor;
                buttonCt.nameLabel.color = cachedThrusterColors[cachedThrusters[0].thrustersByColor.IndexOf(ct)].thrusterColor;
//tarviiko?     buttonCt.thrusterTypeReference = cachedThrusters.IndexOf(thr);
                //buttonCt.thrusterColorReference = cachedThrusters[cachedThrusters.IndexOf(thr)].thrustersByColor.IndexOf(ct);
                buttonCt.thrusterColorReference = cachedThrusters[0].thrustersByColor.IndexOf(ct);

                buttonCt.name = ct.name;
                buttonCt.type = "thruster";
                buttonCt.isOwned = ct.isOwned;
                buttonCt.silverCost = ct.silverCost;
                buttonCt.goldCost = ct.goldCost;
                thrusterButtons.Add(newCtButton);
            }

            /*
            //Populate Lights
            foreach (var light in emissionColorsCache)
            {
                //Instantiate buttons for each light and parent/scale it under lights panel
                GameObject newButton = Instantiate(sampleButton) as GameObject;
                newButton.transform.SetParent(shipLightsContentPanel);
                newButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                SampleButton button = newButton.GetComponent<SampleButton>();
                button.nameLabel.text = light.name;
                button.lightReference = emissionColorsCache.IndexOf(light);
                button.type = "light";
                button.isOwned = light.isOwned;
                button.silverCost = light.silverCost;
                button.goldCost = light.goldCost;
                lightButtons.Add(newButton);
            }*/

            // Populate Modules and set them in the order of "orderInShop"
            for (int i = 0; i < cachedModules.Count; i++)
            {
                foreach (var module in cachedModules)
                {
                    if (module.orderInShop == i) 
                    {
                        // Instantiate buttons for each module and parent/scale it under modules panel
                        GameObject newButton = Instantiate(sampleButton) as GameObject;
                        newButton.transform.SetParent(shipLightsContentPanel);
                        newButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                        SampleButton button = newButton.GetComponent<SampleButton>();
                        button.nameLabel.text = module.name;
                        button.moduleReference = cachedModules.IndexOf(module);
                        button.type = "module";
                        button.isOwned = module.isOwned;
                        button.silverCost = module.silverCost;
                        button.goldCost = module.goldCost;
                        moduleButtons.Add(newButton);
                    }
                }
            }
            /*
            // Populate Modules
            foreach (var module in cachedModules)
            {
                // Instantiate buttons for each module and parent/scale it under modules panel
                GameObject newButton = Instantiate(sampleButton) as GameObject;
                newButton.transform.SetParent(shipLightsContentPanel);
                newButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                SampleButton button = newButton.GetComponent<SampleButton>();
                button.nameLabel.text = module.name;
                button.moduleReference = cachedModules.IndexOf(module);
                button.type = "module";
                button.isOwned = module.isOwned;
                button.silverCost = module.silverCost;
                button.goldCost = module.goldCost;
                moduleButtons.Add(newButton);
            } */
        }

        //Disable thrusterpanel and lightspanel now that the setup is done
        if (shipThrustersContentPanel.gameObject.activeInHierarchy != false)
            GameObject.Find("ThrustersPanel").SetActive(false);
        //if (shipThrusterColorContentPanel.gameObject.activeInHierarchy != false)
        //    GameObject.Find("ThrusterColorsPanel").SetActive(false);
        if (shipLightsContentPanel.gameObject.activeInHierarchy != false)
            GameObject.Find("LightsPanel").SetActive(false);
    }

    // Save the progress when exiting the game
    void OnApplicationQuit()
    {
        //Debug.Log("Saving on app quit");
        SaveData();
    }

    void LoadOutdatedData (string [] loadedData)
    {
        updatedData = new string[9];

        // Check which string do have data and use them for new one
        for (int i = 0; i < loadedData.Length; i ++)
        {
            if(loadedData[i] != "" || loadedData[i] != null)
            {
                updatedData[i] = loadedData[i];
                //Debug.Log("Received old data of " + i);
            }
        }

        // Check the each string for nulls/emptys individually
        for (int i = 0; i < updatedData.Length; i ++)
        {
            // If the part of the string is empty/ null, then add the default bit in it
            if(updatedData[i] == "" || updatedData[i] == null)
            {
                if (i == 0)
                    updatedData[0] = defaultData1;
                if (i == 1)
                    updatedData[1] = defaultData2;
                if (i == 2)
                    updatedData[2] = defaultData3;
                if (i == 3)
                    updatedData[3] = defaultData4;
                if (i == 4)
                    updatedData[4] = defaultData5;
                if (i == 5)
                    updatedData[5] = "0";
                if (i == 6)
                    updatedData[6] = "0";
                if (i == 7)
                    updatedData[7] = "0";
                if (i == 8)
                    updatedData[8] = defaultData6;

                //Debug.Log("Set fixed info on " + i);
            }
        }

        // Set the default colors string in the loadedData[2] so the old saves all start with default open
        updatedData[2] = defaultData3.Substring(0, cachedThrusters[0].thrustersByColor.Count) + updatedData[2].Substring(cachedThrusters[0].thrustersByColor.Count);
       // Debug.Log(updatedData[2]);
    }

    // ========================= COLOR CONVERSION =================================
    /*          THESE ARE NO LONGER USED SINCE 1.2-> AS THE LIGHTS PANEL WAS REMOVED
        public void SetEmission(float h, float s, float v)
        {
            if (playerRenderer == null)
                playerRenderer = GameObject.Find("PlayerShipMesh").GetComponent<Renderer>();
            Color col = ColorFromHSV(h, s, v, 1f);
            playerRenderer.material.SetColor("_EmissionColor", col);
        }

        public static Color ColorFromHSV(float h, float s, float v, float a = 1)
        {
            // no saturation, we can return the value across the board (grayscale)
            if (s == 0)
                return new Color(v, v, v, a);

            // which chunk of the rainbow are we in?
            float sector = h / 60;

            // split across the decimal (ie 3.87 into 3 and 0.87)
            int i = (int)sector;
            float f = sector - i;

            float p = v * (1 - s);
            float q = v * (1 - s * f);
            float t = v * (1 - s * (1 - f));

            // build our rgb color
            Color color = new Color(0, 0, 0, a);

            switch (i)
            {
                case 0:
                    color.r = v;
                    color.g = t;
                    color.b = p;
                    break;

                case 1:
                    color.r = q;
                    color.g = v;
                    color.b = p;
                    break;

                case 2:
                    color.r = p;
                    color.g = v;
                    color.b = t;
                    break;

                case 3:
                    color.r = p;
                    color.g = q;
                    color.b = v;
                    break;

                case 4:
                    color.r = t;
                    color.g = p;
                    color.b = v;
                    break;

                default:
                    color.r = v;
                    color.g = p;
                    color.b = q;
                    break;
            }

            return color;
        }

        public static void ColorToHSV(Color color, out float h, out float s, out float v)
        {
            float min = Mathf.Min(Mathf.Min(color.r, color.g), color.b);
            float max = Mathf.Max(Mathf.Max(color.r, color.g), color.b);
            float delta = max - min;

            // value is our max color
            v = max;

            // saturation is percent of max
            if (!Mathf.Approximately(max, 0))
                s = delta / max;
            else
            {
                // all colors are zero, no saturation and hue is undefined
                s = 0;
                h = -1;
                return;
            }

            // grayscale image if min and max are the same
            if (Mathf.Approximately(min, max))
            {
                v = max;
                s = 0;
                h = -1;
                return;
            }

            // hue depends which color is max (this creates a rainbow effect)
            if (color.r == max)
                h = (color.g - color.b) / delta;            // between yellow & magenta
            else if (color.g == max)
                h = 2 + (color.b - color.r) / delta;                // between cyan & yellow
            else
                h = 4 + (color.r - color.g) / delta;                // between magenta & cyan

            // turn hue into 0-360 degrees
            h *= 60;
            if (h < 0)
                h += 360;
        }*/
}
