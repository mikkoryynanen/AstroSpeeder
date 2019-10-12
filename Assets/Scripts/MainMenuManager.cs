using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using UnityEngine.Analytics;

[RequireComponent (typeof(AudioSource))]
[RequireComponent(typeof(CustomizationManager))]
public class MainMenuManager : MonoBehaviour {
    [Header("Competitive Mode")]
    public int competitiveEntryCost;
    public string competitiveInfo;
    public Text competitiveInfoText;
    public List<GameObject> modeButtons;
    public GameObject confirmationWindow;
    public Button confirmationButton;
    public Text confirmationButtonText;
    public Text moreInfoLinkText;

    [Header("General")]
    public GameObject infoScreen;
    public Text currentVersionText;
    public Button leftArrow;
    public Button rightArrow;
    public RectTransform menuWindows;
    public GameObject topUI;
    public GameObject GpLoginWindow;
    public LevelLoading levelLoading;
    private GooglePlayManager gpManager;

    Animator anim;
    static public int currentMenu;

    [Header("Customize window")]
    //public GameObject playerShip;
    public Camera objectCamera;

    [Header("Windows")]
    public Text[] topTexts;
    public Color textColor;
    public GameObject closeGameWindow;
    public GameObject restOfTheUI;
    public RectTransform shopWindow;
    bool closeGameWindowOpen = false;

    [Header("Menu texts")]
    public Text playText;
    public Text customizeText;
    public Text settingsText;
    public Text highScoreText;
    public Text creditsText;

    [Header("Audio")]
    public AudioMixer master;
    public AudioMixerGroup musicAudioGroup;
    public AudioMixerGroup sfxAudioGroup;

    [Header("AudioClips")]
    AudioSource audio;
    public AudioClip buttonPressSound;
    public AudioClip shipSelectionSound;
    public AudioClip backSound;

    [Header("Sliders in settings menu")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Toggle Buttons")]
    public Toggle enableSoundToggle;
    public Toggle enableVibrateToggle;
    bool soundMuted = false;
    bool screenOrientation = true;

    private SavingSystemV3 savingSystem;
    public float timeInCreditScreen;

    //Mobile keyboard
    private TouchScreenKeyboard keyboard;
    public bool canCloseGame = false;


    void OnLevelWasLoaded()
    {
        //Make sure the player ship is not shown when entering mainMenu
        objectCamera.enabled = false;
    }

    void Start() {
        //Get Current version number
        currentVersionText.text = "Build: " + Application.version;

        audio = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        #if UNITY_ANDROID
        gpManager = GameObject.Find("GooglePlayManager").GetComponent<GooglePlayManager>();
        #endif

        GetAndSetSavedInfo();

        //Reset values
        DataRepo.fixedEndless = false;
        DataRepo.randomEndless = false;
        DataRepo.competitiveMode = false;
        currentMenu = 0;
        //playerShip.SetActive(false);
        objectCamera.enabled = false;
        
        //Top Texts
        for (int i = 1; i < topTexts.Length; i++) {
            topTexts[i].color = new Color(textColor.r, textColor.b, textColor.g, 0.5f);
        }

        //THIS COMPILES FOR ANDROID
#if UNITY_ANDROID || !UNITY_IPHONE || !UNITY_IOS
        //Check if player is logged in, if not show login window the first time game opens
        // if player is NOT authenticated and pref isnt set to "autoLogin to offline"
        if(!Social.localUser.authenticated && PlayerPrefs.GetInt("LoginPref") != 2) {
            GpLoginWindow.SetActive(true);
            canCloseGame = false;
        }
        //if player is logged in dont show google play login window
        else {
            GpLoginWindow.SetActive(false);
            canCloseGame = true;
        }
#endif
    }

    void Update() {
        //Debug.Log("canCloseGame " + canCloseGame);
        //Close game
        if (Input.GetKeyDown(KeyCode.Escape) && !closeGameWindowOpen && canCloseGame) {
            if (GpLoginWindow.activeInHierarchy == true)
                GpLoginWindow.SetActive(false);
            closeGameWindow.SetActive(true);
            restOfTheUI.SetActive(false);
            closeGameWindowOpen = true;
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && closeGameWindowOpen && canCloseGame) {
            closeGameWindow.SetActive(false);
            restOfTheUI.SetActive(true);
            closeGameWindowOpen = false;
        }

        //When in Credits menu
        if(currentMenu == 4) {
            if(!PlayerPrefsX.GetBool("CreditAchievementGranted")) {
                //Collect time in credits screen time
                timeInCreditScreen += Time.deltaTime;
                PlayerPrefs.SetFloat("Achievement_TimeSpentInCredits", timeInCreditScreen);
                //Debug.Log(PlayerPrefs.GetFloat("Achievement_TimeSpentInCredits"));

                if(timeInCreditScreen >= 300) {
                    gpManager.CreditToTheTeam();
                    Debug.Log("You are credit to the team");
                }
            }
        }
    }

    //From the main menu------------------------------------
    public void CanCloseGame(bool ableToCloseGame) {
        canCloseGame = ableToCloseGame;
    }

    void EnableShip() {
        //playerShip.SetActive(true);
        objectCamera.enabled = true;
    }

    public void CloseGame() {
        Application.Quit();
    }

    public void CancelCloseGame() {
        closeGameWindow.SetActive(false);
        restOfTheUI.SetActive(true);
        closeGameWindowOpen = false;
    }

    public void RightArrow() {
        //Sound effect
        audio.PlayOneShot(buttonPressSound);

        if(currentMenu < 4) {
            currentMenu++;

            menuWindows.localPosition = new Vector3(-1150f * currentMenu, -40, 0);

            leftArrow.interactable = true;
        }

        if(currentMenu > 3) {
            rightArrow.interactable = false;
        }

        //Top name color change
        for(int i = 0; i < topTexts.Length; i++) {
            if(i == currentMenu) {
                topTexts[i].color = new Color(textColor.r, textColor.b, textColor.g, 1f);
            }
            else {
                topTexts[i].color = new Color(textColor.r, textColor.b, textColor.g, 0.5f);
            }
        }

        //Show playerShip when at customize screen
        if(currentMenu == 1)
        {
            //playerShip.SetActive(true);
            objectCamera.enabled = true;
        }
        else
        {
            //playerShip.SetActive(false);
            objectCamera.enabled = false;
        }

        //When in Credits menu
        if(currentMenu == 4) {
            //Set timeInCreditScreen value to saved value
            timeInCreditScreen = PlayerPrefs.GetFloat("Achievement_TimeSpentInCredits");
        }
    }

    public void LeftArrow() {
        //Sound effect
        audio.PlayOneShot(buttonPressSound);

        if (currentMenu > 0) {
            currentMenu--;

            menuWindows.localPosition = new Vector3(-1150 * currentMenu, -40, 0);
        }

        if(currentMenu < 1) {
            leftArrow.interactable = false;
        }

        if(currentMenu < 4) {
            rightArrow.interactable = true;
        }

        //Top name color change
        for (int i = 0; i < topTexts.Length; i++) {
            if(i == currentMenu) {
                topTexts[i].color = new Color(textColor.r, textColor.b, textColor.g, 1f);
            }
            else {
                topTexts[i].color = new Color(textColor.r, textColor.b, textColor.g, 0.5f);
            }
        }

         //Show playerShip when at customize screen
        if(currentMenu == 1) {
            objectCamera.enabled = true;
        }
        else {
            objectCamera.enabled = false;
        }
    }

    //Game mode selection window---------------------------
    public void ContestModeMoreInfoLink() {
        Application.OpenURL("https://www.facebook.com/Nitomani-780372235376739/?fref=ts");      
    } 

    public void StartCompetitiveMode() {
        canCloseGame = false;
        //Sound effect
        audio.PlayOneShot(buttonPressSound);
        //Update wallet
        GetComponent<CustomizationManager>().UpdateWallet();
        Debug.Log("savingSystem.goldCurrency: " + savingSystem.goldCurrency);

        /*if (savingSystem.goldCurrency >= 40) {
            canCloseGame = false;
            //Sound effect
            audio.PlayOneShot(buttonPressSound);
            //Detract the cost
            savingSystem.goldCurrency -= 40;
            //Update wallet
            GetComponent<CustomizationManager>().UpdateWallet();
            Debug.Log("savingSystem.goldCurrency: " + savingSystem.goldCurrency);
        }*/
    }
    public void CompetitiveMode() {
        if(Social.localUser.authenticated) {
            //Sound effect
            audio.PlayOneShot(buttonPressSound);
            // Open confirmation window when attempting to enter competitive mode
            confirmationWindow.SetActive(true);
            //competitiveInfoText.text = competitiveInfo;

            //And hide the mode buttons
            for (int i = 0; i < modeButtons.Count; i++)
                modeButtons[i].SetActive(false);

            /*if(savingSystem.goldCurrency < competitiveEntryCost) // Hardcoded value as temp
            {
                confirmationButton.interactable = false;
            //    confirmationButtonText.text = "Can't afford";
            }

            else if (savingSystem.goldCurrency >= competitiveEntryCost)
            {
                confirmationButton.interactable = true;
            //    confirmationButtonText.text = "Confirm";
            }*/
        }
        else {
            GpLoginWindow.SetActive(true);
        }

/*#if UNITY_EDITOR
        //Sound effect
            audio.PlayOneShot(buttonPressSound);
            // Open confirmation window when attempting to enter competitive mode
            confirmationWindow.SetActive(true);
#endif*/
    }

    public void EnterCompetitiveMode ()
    {
        //Perform this when in android
#if UNITY_ANDROID
        if (Social.localUser.authenticated)
        {
            audio.PlayOneShot(buttonPressSound);

            //And reset the UI
            confirmationWindow.SetActive(false);

            for (int i = 0; i < modeButtons.Count; i++)
                modeButtons[i].SetActive(true);

            // Deduct the gold from player and save it to avoid cheating
            /*savingSystem.goldCurrency -= competitiveEntryCost;                //Disabled during the first competition
            savingSystem.SaveData();*/

            //levelLoading.StartLoadingLevel(3);

            // TEMP, competitive = randomEndless
            DataRepo.fixedEndless = false;
            DataRepo.randomEndless = false;
            DataRepo.competitiveMode = true;
            levelLoading.StartLoadingLevel(3);
        }
#endif
        //Only in editor
/*#if UNITY_EDITOR && !UNITY_ANDROID
        audio.PlayOneShot(buttonPressSound);

        //And reset the UI
        confirmationWindow.SetActive(false);

        for (int i = 0; i < modeButtons.Count; i++)
            modeButtons[i].SetActive(true);

        // Deduct the gold from player and save it to avoid cheating
        savingSystem.goldCurrency -= competitiveEntryCost;                //Disabled during the first competition
        savingSystem.SaveData();


        //DataRepo.competitiveMode = true;
        //levelLoading.StartLoadingLevel(3);

        // TEMP, competitive = randomEndless
        DataRepo.fixedEndless = false;
        DataRepo.randomEndless = true;
        levelLoading.StartLoadingLevel(3);
#endif*/
    }

    public void CancelCompetitiveEntry ()
    {
        canCloseGame = true;
        audio.PlayOneShot(backSound);
        confirmationWindow.SetActive(false);

        //And show the mode buttons
        for (int i = 0; i < modeButtons.Count; i++)
            modeButtons[i].SetActive(true);
    }

    public void FixedEndlessMode() {
        //Sound effect
        audio.PlayOneShot(buttonPressSound);

        DataRepo.randomEndless = false;
        DataRepo.fixedEndless = true;

        Application.LoadLevel(2);
    }

    public void RandomEndlessMode() {
        //Sound effect
        audio.PlayOneShot(buttonPressSound);

        DataRepo.fixedEndless = false;
        DataRepo.randomEndless = true;

        Application.LoadLevel(2);
    }

    public void PlayButton() {
        //Sound effect
        audio.PlayOneShot(buttonPressSound);

        DataRepo.fixedEndless = false;
        DataRepo.randomEndless = true;

        levelLoading.StartLoadingLevel(2);
    }

    public void InfoButton() {
        infoScreen.SetActive(true);
    }

    public void CloseInfoScreen() {
        infoScreen.SetActive(false);
    }
    //--------------------------------------------------------
    //Settings Window-------------------------------------------
    public void SetMusicLevel(float lvl) {
        master.SetFloat("VolumeParameter", musicVolumeSlider.value);

        savingSystem.musicVolume = musicVolumeSlider.value;
        savingSystem.SaveData();
    }

    public void SetSFXLevel(float lvl) {
        master.SetFloat("SFXParameter", sfxVolumeSlider.value);

        savingSystem.sfxVolume = sfxVolumeSlider.value;
        savingSystem.SaveData();

        //Sound effect on end of drag. Cancels previous effect is one running.
        //There for only last drag frame will go through
        CancelInvoke("WaitForEndDrag");
        Invoke("WaitForEndDrag", 0.1f);
    }

    void WaitForEndDrag ()
    {
        audio.PlayOneShot(buttonPressSound);
    }

    public void MuteSound() {
        if(!soundMuted) {
            master.SetFloat("MainVolume", -80f);
            soundMuted = true;

            savingSystem.soundsEnabledString = "1";
            savingSystem.SaveData();

            //Sound effect
            audio.PlayOneShot(buttonPressSound);
        }
        else {
            master.SetFloat("MainVolume", -5f);
            soundMuted = false;

            savingSystem.soundsEnabledString = "0";
            savingSystem.SaveData();
        }
    }

    public void EnableDisableVibrate() {
        if(!DataRepo.canVibrate) {
            DataRepo.canVibrate = true;
            Handheld.Vibrate();

            savingSystem.vibrateEnabledString = "0";
            savingSystem.SaveData();

            //Sound effect
            audio.PlayOneShot(buttonPressSound);
        }
        else {
            DataRepo.canVibrate = false;

            savingSystem.vibrateEnabledString = "1";
            savingSystem.SaveData();
        }
    }

    public void Back() {
        //Sound effect
        audio.PlayOneShot(backSound);
    }

    public void QuitGame() {
        //Sound effect
        audio.PlayOneShot(backSound);

        Application.Quit();
    }

    //Shop window
    public void ShopOpen() {
#if UNITY_ANDROID && !UNITY_EDITOR && !UNITY_IOS
        if(Social.localUser.authenticated) {
            //Sound effect
            audio.PlayOneShot(buttonPressSound);

            topUI.SetActive(false);
            restOfTheUI.SetActive(false);
            objectCamera.enabled = false;

            shopWindow.localPosition = new Vector3(0, 0, 0);
        }
        else {
            //GpLoginWindow.SetActive(true);
        }
#endif

#if UNITY_EDITOR
        //Sound effect
        audio.PlayOneShot(buttonPressSound);

        topUI.SetActive(false);
        restOfTheUI.SetActive(false);
        objectCamera.enabled = false;

        shopWindow.localPosition = new Vector3(0, 0, 0);
#endif
    }

    public void ShopClose() {
        //Sound effect
        audio.PlayOneShot(backSound);

        topUI.SetActive(true);
        restOfTheUI.SetActive(true);
        if(currentMenu == 1)
            objectCamera.enabled = true;

        shopWindow.localPosition = new Vector3(0, -866, 0);
    }

    void GetAndSetSavedInfo()
    {
        if(savingSystem == null)
            savingSystem = GameObject.FindWithTag("DataRepo").GetComponent<SavingSystemV3>();

        musicVolumeSlider.value = savingSystem.musicVolume;
        sfxVolumeSlider.value = savingSystem.sfxVolume;

        if (savingSystem.soundsEnabledString == "1")
            enableSoundToggle.isOn = false;

        if (savingSystem.vibrateEnabledString == "1")
        {
            DataRepo.canVibrate = true;
            enableVibrateToggle.isOn = false;
        }
    }

    public void SelectMenu(int n)
    {
        audio.PlayOneShot(buttonPressSound);
        currentMenu = n;
        menuWindows.localPosition = new Vector3(-1150 * currentMenu, -40, 0);

        //Top name color change
        for (int i = 0; i < topTexts.Length; i++)
        {
            if (i == currentMenu)
            {
                topTexts[i].color = new Color(textColor.r, textColor.b, textColor.g, 1f);
            }
            else {
                topTexts[i].color = new Color(textColor.r, textColor.b, textColor.g, 0.5f);
            }
        }

        //Set arrows to be interractable if needed
        if (currentMenu == 0)
            leftArrow.interactable = false;
        else if (currentMenu >= 1)
            leftArrow.interactable = true;

        if (currentMenu == 4)
            rightArrow.interactable = false;
        else if (currentMenu <= 3)
            rightArrow.interactable = true;

        //Show playerShip when at customize screen
        if (currentMenu == 1)
        {
            objectCamera.enabled = true;
        }
        else {
            objectCamera.enabled = false;
        }

        //When in Credits menu
        if(currentMenu == 4) {
            //Set timeInCreditScreen value to saved value
            timeInCreditScreen = PlayerPrefs.GetFloat("Achievement_TimeSpentInCredits");
        }
    }

    //iOS Only=================================================================================
    public void IOSExitGame() {
        closeGameWindow.SetActive(true);
        restOfTheUI.SetActive(false);
        closeGameWindowOpen = true;
    }
    //=========================================================================================
}
