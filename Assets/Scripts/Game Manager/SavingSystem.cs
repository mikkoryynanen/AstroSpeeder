/*              DISABLED SO IT WONT INTERFERE WITH THE SAVING SYSTEMV3
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;

//v2 Array of strings, joissa stringit ovat numeroina. Valuutat lukuna, avatut 1=auki, 0=kiinni
//string array[0] = hopea valuutta 1-18 (18 merkkiä), kulta valuutta 18-> (18 merkkiä?)
//string array[1] = avatut materiaalit/tekstuurit 1 -> (defaulttina 20 merkkiä)
//string array[2] = avatut thrusters 1 -> (defaulttina 20)
//string array[3] = avatut valot lights 1-> (default 20)
//string array[4] = customizationin current skin, lights, thrusters ja asetukset
//array 4 muotoa 11223344455567, jossa 1 = curSKin, 2 = curLights, 3 = curThrusters
//  4 = musicVolume, 5 = SFXVolume, 6 = soundEnabled, 7 = vibrate Enabled
//186,0.67,2.492 is default number 0 emission

// ToDo: 
// BUG: EMISSION IS NOT GETTING SET PROPERLY?
// SET SKIN ETC FUNCTIONS in customization
// Main Menu figure out to "OnEndDrag"
// Set Points in gameOver

public class SavingSystem : MonoBehaviour
{
    [Serializable]
    public class ColorHolder
    {
        public string name;
        [Range(0f, 359f)]
        public float HHue;
        [Range(0f, 1f)]
        public float SSaturation = 1f;
        [Range(0f, 10f)]
        public float VBrightness = 1f;
    }

    [Header("Players inventory of usable items")]
    public string playerName;   //NO LONGER NEEDED
    static public long +;   //is "long" since int cant be this long
    static public long goldCurrency;   //is "long" since int cant be this long

    public List<Material> usableMaterials = new List<Material>();
    public List<GameObject> usableThrusters = new List<GameObject>();
    public List<ColorHolder> usableLights = new List<ColorHolder>();

    [Header("Cached items that can be opened, add all openable items here")]
    public List<Material> cachedMaterials = new List<Material>();
    public List<GameObject> cachedThrusters = new List<GameObject>();
    public List<ColorHolder> emissionColorsCache = new List<ColorHolder>();

    private string currentMaterials = "0000000000000000000000000000000000000000";
    private string currentThrusters = "0000000000000000000000000000000000000000";
    private string currentLights = "0000000000000000000000000000000000000000";
    private string currentCustomization = "00000000000000"; //KESKEN

    [Header("Default Data strings if no data was found")]  //0000000000000000000000000000000000000000
    //For currencies; 2222222222222222223333333333333333330000 where 2:s are silver, 3:s are gold
    public string defaultData1 = "0000000000000000000000000000000000000000"; //2222222222222222223333333333333333330000 // test string
    //For materials; 
    public string defaultData2 = "1000000000000000000000000000000000000000"; //4444444444444444444444444444444444444444 // test string
    //For Thrusters;
    public string defaultData3 = "1000000000000000000000000000000000000000"; //5555555555555555555555555555555555555555 // test string
    //For Lights
    public string defaultData4 = "1000000000000000000000000000000000000000"; //6666666666666666666666666666666666666666 // test string
    //For customization and settings; where 1 = curSKin, 2 = curLights, 3 = curThrusters
    //  4 = musicVolume, 5 = SFXVolume, 6 = soundEnabled, 7 = vibrate Enabled
    public string defaultData5 = "00000007007500"; //11223344455567 //STILL NEEDS TO BE SORTED

    private Renderer playerRenderer;
    private string curSkinString;
    private string curLightsString;
    private string curThrustersString;
    private string musicVolumeString;
    private string SfxVolumeString;
    [HideInInspector] public string soundsEnabledString;
    [HideInInspector] public string vibrateEnabledString;
    [HideInInspector] public int curSkin = 0;
    [HideInInspector] public int curEmission = 0;
    [HideInInspector] public int curThrusters = 0;
    public AudioMixer master;
    private float startMasterVolume;
    [HideInInspector] public float musicVolume;
    [HideInInspector] public float sfxVolume;

    void OnLevelWasLoaded()//So the "Start" will run on levelChange as this script is carried over the scenes
    {
        Start();
        Time.timeScale = 1;
    }

    void Start()//Used to be awake, but setting volume didnt work in there
    {
        if (Application.loadedLevelName != "SplashScreen")
        {
            ConfigVars();
            LoadAndReadData();
        }
        //AddMaterial(cachedMaterials[2]);      //For testing purposes
        //AddParticle(cachedParticles[2]);      //For testing purposes
        //AddShip(cachedShips[2]);              //For testing purposes
        //SetCash(9);                           //For testing purposes
        //ChangeMaterialTo(cachedMaterials[1]); //For testing purposes
        master.GetFloat("MainVolume", out startMasterVolume);
    }

    public void SaveData()
    {
        string[] newSaveInfo = new string[5];
        newSaveInfo[0] = silverCurrency.ToString("000000000000000000") + goldCurrency.ToString("000000000000000000");
        newSaveInfo[1] = currentMaterials;
        newSaveInfo[2] = currentThrusters;
        newSaveInfo[3] = currentLights;
        float fixedMusicVolume = musicVolume + 80f;
        float fixedSfxVolume = sfxVolume + 80f;
        newSaveInfo[4] = curSkinString + curLightsString + curThrustersString + fixedMusicVolume.ToString("000")
            + fixedSfxVolume.ToString("000") + soundsEnabledString + vibrateEnabledString;
        PlayerPrefsX.SetStringArray(playerName, newSaveInfo);
        //LoadAndReadData();//Needed if you remove .Adds from addmaterial etc.
    }

    void LoadAndReadData()// For each item marked as "opened", adds them to inventory list for further usage
    {
        // Get the saved string array data if exists, else create a new array
        string[] loadedData = PlayerPrefsX.GetStringArray(playerName);
        if (loadedData.Length == 0)//Comment this IF out if want to test "1st run"
        {
            // IF THIS WAS USED THEN IT MEANS ITS THE 1sT RUN
            loadedData = new string[5];
            Debug.Log("Running game for the 1st time!");
            DataRepo.isFirstRun = true;
        }

        // = = = SILVER AND GOLD CURRENCY DATA = = =
        if (loadedData[0] == "" || loadedData[0] == null)
            loadedData[0] = defaultData1;

        // silver currency
        string silverCashString = loadedData[0].Substring(0, 18);
        silverCurrency = Convert.ToInt64(silverCashString); 

        // gold currency
        string goldCashString = loadedData[0].Substring(18, 18);
        goldCurrency = Convert.ToInt64(goldCashString);

        // = = = CUSTOMIZATION AND SETTINGS DATA = = =
        if (loadedData[4] == "" || loadedData[4] == null)
            loadedData[4] = defaultData5;

        // current skin
        curSkinString = loadedData[4].Substring(0, 2);
        Int32.TryParse(curSkinString, out curSkin);
        //if (Application.loadedLevelName == "FixedEndlessMode" || Application.loadedLevelName == "RandomEndlessMode" || Application.loadedLevelName == "DemoScene")
        playerRenderer.material = cachedMaterials[int.Parse(curSkinString)];

        // current lights
        curLightsString = loadedData[4].Substring(2, 2);
        Int32.TryParse(curLightsString, out curEmission);
        //if (Application.loadedLevelName == "FixedEndlessMode" || Application.loadedLevelName == "RandomEndlessMode" || Application.loadedLevelName == "DemoScene")
        SetEmission(emissionColorsCache[curEmission].HHue, emissionColorsCache[curEmission].SSaturation, emissionColorsCache[curEmission].VBrightness);

        // current thrusters
        curThrustersString = loadedData[4].Substring(4, 2);
        Int32.TryParse(curThrustersString, out curThrusters);
        cachedThrusters[curThrusters].SetActive(true);

        // = = = SETTINGS DATA = = =
        if (loadedData[4] == "" || loadedData[4] == null)
            loadedData[4] = defaultData5;
        
        // Music volume
        musicVolumeString = loadedData[4].Substring(6, 3);
        float.TryParse(musicVolumeString, out musicVolume);//float curMVolume = float.Parse(musicVolumeString);
        musicVolume = musicVolume - 80f;
        master.SetFloat("VolumeParameter", musicVolume);

        //SFX Volume
        SfxVolumeString = loadedData[4].Substring(9, 3);
        float.TryParse(SfxVolumeString, out sfxVolume);//float cursfxVolume = float.Parse(SfxVolumeString);
        sfxVolume = sfxVolume - 80f;
        master.SetFloat("SFXParameter", sfxVolume);

        // Mute Toggle
        soundsEnabledString = loadedData[4].Substring(12, 1);
        if (soundsEnabledString == "0")
            master.SetFloat("MainVolume", startMasterVolume);
        else if (soundsEnabledString == "1")
            master.SetFloat("MainVolume", -80f);

        // Vibration Toggle
        vibrateEnabledString = loadedData[4].Substring(13, 1);
        if (vibrateEnabledString == "0")
            DataRepo.canVibrate = true;
        else if (vibrateEnabledString == "1")
            DataRepo.canVibrate = false;

        // = = = MATERIAL DATA = = = // YOURE HERE
        if (loadedData[1] == "" || loadedData[1] == null)
            loadedData[1] = defaultData2;

        currentMaterials = loadedData[1];

        // Chops and creates an array of the each number of the string, and iterates them if open
        string[] eachMatNmbr = new string[currentMaterials.Length];
        for (int i = 0; i < currentMaterials.Length; i++)
        {
            eachMatNmbr[i] = currentMaterials[i].ToString();

            if (eachMatNmbr[i] == "1" && !usableMaterials.Contains(cachedMaterials[i]))
                usableMaterials.Add(cachedMaterials[i]);
        }

        // = = = THRUSTERS DATA = = =
        if (loadedData[2] == "" || loadedData[2] == null)
            loadedData[2] = defaultData3;

        currentThrusters = loadedData[2];

        // Chops and creates an array of the each number of the string, and iterates them if open
        string[] eachParticleNmbr = new string[currentThrusters.Length];
        for (int i = 0; i < currentThrusters.Length; i++)
        {
            eachParticleNmbr[i] = currentThrusters[i].ToString();

            if (eachParticleNmbr[i] == "1" && !usableThrusters.Contains(cachedThrusters[i]))
                usableThrusters.Add(cachedThrusters[i]);
        }

        // = = = LIGHTS DATA = = =
        if (loadedData[3] == "" || loadedData[3] == null)
            loadedData[3] = defaultData4;

        currentLights = loadedData[3];

        // Chops and creates an array of the each number of the string, and iterates them if open
        string[] eachLightNmbr = new string[currentLights.Length];
        for (int i = 0; i < currentLights.Length; i++)
        {
            eachLightNmbr[i] = currentLights[i].ToString();

            if (eachLightNmbr[i] == "1" && !usableLights.Contains(emissionColorsCache[i]))
                usableLights.Add(emissionColorsCache[i]);
        }
    }

    void AddMaterial(Material mat)
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

    void AddParticle(GameObject p)
    {
        if (!cachedThrusters.Contains(p))
        {
            cachedThrusters.Add(p);
            Debug.LogWarning("The particle you tried to add wasnt in cachedParticles. Adding it now. Add this p to cachedParticles off game to fix this error from popping up");
        }

        else if (!usableThrusters.Contains(p))
        {
            usableThrusters.Add(p);//
            string newString = currentThrusters;

            newString = newString.Remove(cachedThrusters.IndexOf(p), 1);
            newString = newString.Insert(cachedThrusters.IndexOf(p), "1");

            currentThrusters = newString;
            SaveData();
        }
        else
            Debug.Log(p.name + " particle is already unlocked!");
    }

    void AddLight(ColorHolder l)
    {
        if (!emissionColorsCache.Contains(l))
        {
            emissionColorsCache.Add(l);
            Debug.LogWarning("The light you tried to add wasnt in cachedParticles. Adding it now. Add this p to cachedParticles off game to fix this error from popping up");
        }

        else if (!usableLights.Contains(l))
        {
            usableLights.Add(l);//
            string newString = currentThrusters;

            newString = newString.Remove(emissionColorsCache.IndexOf(l), 1);
            newString = newString.Insert(emissionColorsCache.IndexOf(l), "1");

            currentThrusters = newString;
            SaveData();
        }
        else
            Debug.Log(l.name + " light is already unlocked!");
    }

    public void SetSilverCurrency(int silverChange)
    {
        if (silverCurrency + silverChange >= 0)
        {
            silverCurrency = silverCurrency + silverChange;
            SaveData();
        }
        else
            Debug.Log("You cant have negative amount of silver, you fool!");
    }

    public void SetGoldCurrency(int goldChange)
    {
        if (silverCurrency + goldChange >= 0)
        {
            silverCurrency = silverCurrency + goldChange;
            SaveData();
        }
        else
            Debug.Log("You cant have negative amount of gold, you fool!");
    }

    void OnApplicationQuit()
    {
        Debug.Log("Saving on app quit");
        SaveData();
    }

    void ChangeMaterialTo(Material mat)
    {
        curSkinString = cachedMaterials.IndexOf(mat).ToString("00");

        playerRenderer.material = mat;
        SaveData();
    }

    void ConfigVars ()
    {
        //Gets all the thrusters under PlayerShipThrusters
        GameObject player = GameObject.Find("PlayerShipThrusters");
        cachedThrusters = new List<GameObject>();
        foreach (Transform child in player.transform)
        {
            cachedThrusters.Add(child.gameObject);
        }

        //Get the playersMesh
        playerRenderer = GameObject.Find("PlayerShipMesh").GetComponent<Renderer>();
    }

    // ========================= COLOR CONVERSION =================================

    void SetEmission(float h, float s, float v)
    {
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
    }
}
*/
