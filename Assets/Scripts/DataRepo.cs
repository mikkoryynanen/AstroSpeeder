using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataRepo : MonoBehaviour {
    //Player attributes PLACEHOLDER
    //   static public int currencySilver = 2000;
    //   static public int currencyGold = 20;
    static public bool gameOpened = true;

    //Settings
    static public bool canVibrate = true;
    static public bool isFirstRun;
    static public bool gameRetry;
    static public bool loginOpenedOnce;
    static public bool iosLoggedIn = false;
    //Which gamemode is selected
    static public bool fixedEndless = false;
    static public bool randomEndless = true;
    static public bool competitiveMode = false;

    public static string playerUsername = "";
    public static string playerEmail = "";

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start ()
    {
        // Set orientation to automatically change between landscapes.
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.AutoRotation;

        // Cap the framerate
        Application.targetFrameRate = 60;

        // Set screen to not fall a sleep if screen has not been touched
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Screen.fullScreen = false;
    }
}
