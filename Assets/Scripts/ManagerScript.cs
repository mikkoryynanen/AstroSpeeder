using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using GooglePlayGames;

public class ManagerScript : MonoBehaviour {

    [Header("Email Data")]
    public string emailRecipient = "astrospeederresults@gmail.com";
    public string emailSubject = "Score Screenshot";
    public string emailBody = "This is for testing SMTP mail from GMAIL";
    public string smtpServerClient = "smtp.gmail.com";
    public int smtpServerPort = 587;

    [Header("Info Screen")]
    public MusicPlayer musicPlayer;
    public GameObject infoScreen;
    public Button infoScreenButton;
    public Text infoScreenButtonText;
    public int countdownNumber = 5;

    [Header("GameOver Window Texts")]
    public Text personalHighScoreText;
    public Text nonChangingCurrentRunScoreText;
    public Text currentRunScoreText;
    public Text currentTimeSurvivedText;
    public Text currentDistanceText;
    public Text currentHighestVelocityText;
    public GameObject newHighScoreText;
    public Text pickUpsCurrencyText;
    public Text submitText;
    public Button submitButton;

    public GameObject UI;
    public GameObject pauseMenu;
    public GameObject gameOverWindow;
    public GameObject playerShipModel;

    public CameraController cameraController;

    static public bool gameOver = false;

    bool gamePaused = false;

    private bool runOnce = false;
    private bool changedMusic = false;

    public GooglePlayManager GPManager;
    public IOSManager GCManager;

    [Header("Currency Conversion")]
    public float changeRate = 0.2f;

    public float gainedPoints;//Points to be reduced
    public float totalCurrency;//total currency coverted from the points
    public float PointsToCurrencyConvertRatio = 0.001f;// Default 0.001f;

    private float deductPointsAmount;
    private float increaseCurrencyAmount;

    private float visualCurrencyAmount;
    private bool runOnceGO;
    private bool runningConversion;
    private bool checked9000Achievement;

    public Button retryButton;
    public Button mainMenuButton;

    public SavingSystemV3 savingSystem;
    public LevelLoading levelLoading;
    public GameObject retryErrorText;
    public GameObject gameOverRetryErrorText;

    public int silverFromPickUps;
    public int goldFromPickUps;

    [Header("New highscore Floating Effect variables")]
    private RectTransform myTransform;
    private float value;
    private bool goingDown;

    private bool authenticated;

    void Start () {
        Time.timeScale = 1.0f;

        gamePaused = false;
        changedMusic = false;
        checked9000Achievement = false;

        cameraController.enabled = true;
        newHighScoreText.SetActive(false);

        if(infoScreen.activeInHierarchy == false)
            infoScreen.SetActive(true);

        //Show info screen 1st times playing
        if (DataRepo.isFirstRun == true) {
            Time.timeScale = 0f;
            gamePaused = true;
            //InvokeRepeating("InfoScreenButtonCountDown", 1, 1);
            StartCoroutine("RealTimeCountDown");
            infoScreenButtonText.text = countdownNumber.ToString();
        }
        else {
            infoScreen.SetActive(false);
            gamePaused = false;
        }

        if(DataRepo.gameRetry) {
            DataRepo.gameRetry = false;
        }

        #if UNITY_ANDROID
        GPManager = GameObject.Find("GooglePlayManager").GetComponent<GooglePlayManager>();
        #endif

        #if UNITY_IOS
        GCManager = GameObject.Find("GameCenterManager").GetComponent<IOSManager>();
        #endif

        if (savingSystem == null)
            savingSystem = GameObject.FindWithTag("DataRepo").GetComponent<SavingSystemV3>();
    }
	
	void Update () {
        //print("Game over: " + gameOver);

        if (gameOver) {
            cameraController.enabled = false;
            //Write to gameOverWindow
            currentTimeSurvivedText.text = Mathf.Round(PlayerController.gameTime).ToString();
            currentDistanceText.text = Mathf.Round(PlayerController.distanceTraveled).ToString();
            currentHighestVelocityText.text = Mathf.Round(PlayerController.topSpeed).ToString();

            if (silverFromPickUps != 0 && goldFromPickUps != 0)
                pickUpsCurrencyText.text = "<color=silver>" + silverFromPickUps + " C</color>, <color=#ffc100ff>" + goldFromPickUps + " P</color>";

            if(silverFromPickUps != 0 && goldFromPickUps == 0)
                pickUpsCurrencyText.text = "<color=silver>" + silverFromPickUps + " C</color>";

            if (goldFromPickUps == 0 && silverFromPickUps == 0)
                pickUpsCurrencyText.text = " - ";

            if(silverFromPickUps == 0 && goldFromPickUps != 0)
                pickUpsCurrencyText.text = "<color=#ffc100ff>" + goldFromPickUps + " P</color>";

            UI.SetActive(false);
            gameOverWindow.SetActive(true);
            playerShipModel.SetActive(true);
            Time.timeScale = 0.0f;

//            CheckAndSetSubmitButton();

            //Change music
            if(!changedMusic) {
                musicPlayer.PlayGameOverIntro();
                changedMusic = true;
            }

            //Tap / click to skip Converting points
            if(Input.GetMouseButtonDown(0))
            {
                if (runningConversion)
                {
                    visualCurrencyAmount = totalCurrency;
                    gainedPoints = 0;
                }
            }

            OnGameOverScreenOpen();

            //Check for played time achievement
            if (!PlayerPrefsX.GetBool("Achievement_ItsOver9000") && !checked9000Achievement) {
                PlayerPrefs.SetInt("Achievement_ItsOver9000_Value", PlayerPrefs.GetInt("Achievement_ItsOver9000_Value") + (int)PlayerController.gameTime);
                //Debug.Log("Achievement_ItsOver9000_Value : " + PlayerPrefs.GetInt("Achievement_ItsOver9000_Value"));

                if (PlayerPrefs.GetInt("Achievement_ItsOver9000_Value") > 9000) {
                    #if UNITY_ANDROID
                    GPManager.ItsOver9000();
                    #endif
                    #if UNITY_IOS
                    GameCenterManager.SubmitAchievement(100f, "Achievement_ItsOver9000", true);
                    #endif
                }

                checked9000Achievement = true;
            }
        }

        //Pause menu
        if(Input.GetKeyDown(KeyCode.Escape) && !gameOver) {
            if(!gamePaused) {
                UI.SetActive(false);
                pauseMenu.SetActive(true);
                Time.timeScale = 0f;
                gamePaused = true;
            }
            else {
                UI.SetActive(true);
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
                gamePaused = false;
            }
        }
	}

/*    void CheckAndSetSubmitButton ()
    {
        Social.localUser.Authenticate((bool success) => {
            if (success)
            {
                authenticated = true;
                submitText.text = "Submit";
            }
            else
            {
                authenticated = false;
                submitText.text = "Offline";
            }
        });
    }*/

    public void IOSPauseGame() {
        UI.SetActive(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
    }

    public void CloseInfoScreen() {
        //PlayerPrefsX.SetBool("ShowInfoScreen", false);
        infoScreen.SetActive(false);
        Time.timeScale = 1f;
        DataRepo.isFirstRun = false;
        gamePaused = false;
    }

    void InfoScreenButtonCountDown() {
        infoScreenButtonText.text = countdownNumber.ToString();

        if (countdownNumber > 0) {
            countdownNumber--;
        }
        else {
            infoScreenButton.interactable = true;
            //infoScreenButtonText.text = "Got it";
            infoScreenButtonText.text = "tap anywhere to continue";
            CancelInvoke("InfoScreenButtonCountDown");
        }
    }

    private IEnumerator RealTimeCountDown()
    {
        infoScreenButtonText.text = countdownNumber.ToString();
        if (countdownNumber > 0)
        {
            yield return StartCoroutine(WaitForRealSeconds(1));
            countdownNumber--;
            StartCoroutine("RealTimeCountDown");
        }
        else
        {
            infoScreenButton.interactable = true;
            //infoScreenButtonText.text = "Got it";
            infoScreenButtonText.text = "tap anywhere to continue";
        }
    }

    public static IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }

    public void ReturnButton() {
        UI.SetActive(true);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }

    public void RetryButton() {
        gameOverWindow.SetActive(false);
        gameOver = false;
        DataRepo.gameRetry = true;
  //      levelLoading.StartLoadingLevel(Application.loadedLevel);

        //THIS IS DISABLED DURING SEASON 1
        /*if (savingSystem.goldCurrency >= 40 && Social.localUser.authenticated) {
            if(Application.loadedLevel == 3) {
                retryErrorText.SetActive(false);
                //Detract gold currency 
                savingSystem.goldCurrency -= 40;
                //save currency change
                savingSystem.SaveData();
            }
            
            //Debug.Log("savingSystem.goldCurrency: " + savingSystem.goldCurrency);
            gameOverWindow.SetActive(false);
            gameOver = false;
            DataRepo.gameRetry = true;
            levelLoading.StartLoadingLevel(Application.loadedLevel);
        }
        else {
            if(Application.loadedLevel == 3) {
                if(gameOver) {
                    gameOverRetryErrorText.SetActive(true);
                }
              else {
                 retryErrorText.SetActive(true);
                }
            }
            
        }*/
    }

    public void BackToGameButton() {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }

     public void BackToMainMenuButton() {
        gameOverWindow.SetActive(false);
        gameOver = false;
 //       levelLoading.StartLoadingLevel(1);
    }

    void OnGameOverScreenOpen()
    {
        if (runOnceGO == false)
        {
            //Add played games achievemnt
            #if UNITY_ANDROID
            GPManager.AddPlayedGames();
            #endif
            
            #if UNITY_IOS
            GameCenterManager.SubmitAchievement(2f, "Achievement_50Games", true);
            GameCenterManager.SubmitAchievement(1f, "Achievement_100Games", true);
            GameCenterManager.SubmitAchievement(0.5f, "Achievement_200Games", true);
            #endif

            // Check and set highscores
            CheckPersonalHighscore();
            GPManager.GameOverPostScoreToHighscoreList();

             #if UNITY_IOS
            long longPlayerScore = Convert.ToInt64(PlayerController.currentScore);
            if (DataRepo.fixedEndless) {
                GameCenterManager.ReportScore(longPlayerScore, "Leaderboard_FixedMode");
            }
            if(DataRepo.randomEndless) {
                GameCenterManager.ReportScore(longPlayerScore, "Leadeboard_RandomMode");
            }

            if(DataRepo.competitiveMode) {
                GameCenterManager.ReportScore(longPlayerScore, "Leaderboard_ContestMode");
            }
            #endif

            currentRunScoreText.text = Mathf.Round(PlayerController.currentScore).ToString();
            nonChangingCurrentRunScoreText.text = Mathf.Round(PlayerController.currentScore).ToString();
            //Get and set the points amounts for visual
            gainedPoints = PlayerController.currentScore; //This should get points player gathered
            totalCurrency = gainedPoints * PointsToCurrencyConvertRatio;

            //Save the points and currency
            SaveScore();

            //Makes the amounts changing to a percentual amount => should take about the same time to convert
            deductPointsAmount = gainedPoints * 0.012f;
            increaseCurrencyAmount = totalCurrency * 0.012f;

            runningConversion = false;
            StartCoroutine("WaitForConversion");
            runOnceGO = true;
        }
    }

    void CheckPersonalHighscore ()
    {
        long playerScore = Convert.ToInt64(PlayerController.currentScore);
        // If run made was in randomEndless
        if (DataRepo.randomEndless)
        {
            personalHighScoreText.text = SavingSystemV3.personalRandomModeHighscore.ToString();

            if (playerScore > SavingSystemV3.personalRandomModeHighscore)
            {
                newHighScoreText.SetActive(true);
                StartCoroutine("StartFloating");
                //Debug.Log(playerScore + " did beat the previous highscore of  " + SavingSystemV3.personalRandomModeHighscore);
                SavingSystemV3.personalRandomModeHighscore = playerScore;
            }
        }

        // If run made was in fixedEndless
        if (DataRepo.fixedEndless)
        {
            personalHighScoreText.text = SavingSystemV3.personalFixedModeHighscore.ToString();

            if (playerScore > SavingSystemV3.personalFixedModeHighscore)
            {
                newHighScoreText.SetActive(true);
                StartCoroutine("StartFloating");
                //Debug.Log(playerScore + " did beat the previous highscore of  " + SavingSystemV3.personalFixedModeHighscore);
                SavingSystemV3.personalFixedModeHighscore = playerScore;
            }
        }

        // If run made was in randomEndless
        if (DataRepo.competitiveMode)
        {
            personalHighScoreText.text = SavingSystemV3.personalContestModeHighscore.ToString();

            if (playerScore > SavingSystemV3.personalContestModeHighscore)
            {
                newHighScoreText.SetActive(true);
                StartCoroutine("StartFloating");
                //Debug.Log(playerScore + " did beat the previous highscore of  " + SavingSystemV3.personalContestModeHighscore);
                SavingSystemV3.personalContestModeHighscore = playerScore;
            }
        }
    }

    IEnumerator WaitForConversion ()
    {
        yield return StartCoroutine(WaitForRealSeconds(2));
        StartCoroutine("VisualCurrencyConversion");
    }

    IEnumerator VisualCurrencyConversion ()
    {
        runningConversion = true;
        //If values still need changing, do so
        if (gainedPoints < 0 || visualCurrencyAmount < totalCurrency)
        {
            yield return StartCoroutine(WaitForRealSeconds(0.1f));
            if (gainedPoints > 0)
            {
                gainedPoints -= deductPointsAmount;
            }
            if (visualCurrencyAmount < totalCurrency)
            {
                visualCurrencyAmount += increaseCurrencyAmount;
            }
            
            //If values are what they should be, then fix them and set buttons active
            if (gainedPoints <= 0 && visualCurrencyAmount >= totalCurrency)
            {
                visualCurrencyAmount = totalCurrency;
                gainedPoints = 0;
                StartCoroutine(EnableButtons());
            }

            //ReRun function to set values if not done yet
            else
                StartCoroutine("VisualCurrencyConversion");

            //Build the string
            if(gainedPoints != 0)
                currentRunScoreText.text = gainedPoints.ToString("F0") + " = <color=silver>" + visualCurrencyAmount.ToString("F0") + " C</color>";
            else
                currentRunScoreText.text = "<color=silver>" + visualCurrencyAmount.ToString("F0") + " C</color>";

            //currentRunScoreText.text = visualCurrencyAmount.ToString("F0") + " C + " + silverFromPickUps + "C, " + goldFromPickUps + "P from pickups";
        }
        //Just to make sure points will be set correctly and buttons enabled
        else
        {
            visualCurrencyAmount = totalCurrency;
            gainedPoints = 0;
            //Set the buttons active
            StartCoroutine(EnableButtons());
            runningConversion = false;
        }
    }

    IEnumerator EnableButtons ()
    {
        yield return StartCoroutine(WaitForRealSeconds(0.5f));
        retryButton.interactable = true;
        mainMenuButton.interactable = true;

        if(Social.localUser.authenticated)
            submitButton.interactable = true;
        else
            submitText.text = "offline";
    }

    void SaveScore ()
    {
        int gainedCurrency = (int)Mathf.Round(totalCurrency);
        savingSystem.silverCurrency += gainedCurrency;
        savingSystem.SaveData();

        //Add to Total silver coins ammount
        PlayerPrefs.SetFloat("Achievement_SilverCoinsAmmount", PlayerPrefs.GetFloat("Achievement_SilverCoinsAmmount") + gainedCurrency);

        //Check achievement
        if (!PlayerPrefsX.GetBool("Achievement_SilverCoins") && PlayerPrefs.GetFloat("Achievement_SilverCoinsAmmount") >= 2500)
        {
#if UNITY_ANDROID
            GPManager.SilverCoins();
#endif
            
#if UNITY_IOS
            GameCenterManager.SubmitAchievement(100f, "Achievement_SilverEarned", true);
#endif
        }
    }

    IEnumerator StartFloating()
    {
        if (myTransform == null)
            myTransform = newHighScoreText.GetComponent<RectTransform>();

        yield return StartCoroutine(WaitForRealSeconds(0.05f));

        if (myTransform.localScale.x <= 1)
        {
            value = 1;
            goingDown = false;
        }
        if (myTransform.localScale.x >= 1.05f)
        {
            value = 1.05f;
            goingDown = true;
        }
        if (!goingDown)
            value += 0.005f;

        if (goingDown)
            value -= 0.005f;

        myTransform.localScale = new Vector3(value, value, value);
        StartCoroutine("StartFloating");
    }

    // ==================================== EMAIL FUNCTIONS ==================================================
    public void CallTheEmailSequence()
    {
        submitText.text = "Submitting..";
        // Has to be called as coroutine at the end of frame as it only can render screenshots then
        StartCoroutine(TakeScreenshot());
    }

    IEnumerator TakeScreenshot()
    {
/*      // First possible way of taking a screenshot, works
        yield return new WaitForEndOfFrame();
        var tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        tex.Apply();
        var path = Path.Combine(Application.persistentDataPath, "screenshot.png");
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(path, bytes);*/
        submitText.text = "Submitting..";
        yield return new WaitForEndOfFrame();
        // 1st way, should work over multiple devices
        //CaptureScreenshot();

        //StartCoroutine(CaptureScreenshotSequence());

        // 3rd way, works on pc atleast
        //SaveTextureToFile();

        // 4th way, takes an jpg
        SaveTextureToFileJPG();
    }

    void SendScreenshotEmail(string path)
    {
        try
        {
            submitText.text = "Sending";
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("astrospeederresults@gmail.com");
            mail.To.Add(emailRecipient);
            mail.Subject = emailSubject;
            //mail.Body = ((PlayGamesLocalUser)Social.localUser).Email;
            //mail.Body = emailBody;
            mail.Body = "Player Username: " + DataRepo.playerUsername + "\n" + "Player Email: " + DataRepo.playerEmail + "\n" + "Module used: " + savingSystem.cachedModules[savingSystem.curModule].name;
            mail.Attachments.Add(new Attachment(path));

            SmtpClient smtpServer = new SmtpClient(smtpServerClient);//"smtp.gmail.com"
            smtpServer.Port = smtpServerPort;//587
            smtpServer.Credentials = new System.Net.NetworkCredential("astrospeederresults@gmail.com", "astrospeeder1") as ICredentialsByHost;
            smtpServer.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };
            smtpServer.Send(mail);
            //Debug.Log("success");
            submitText.text = "Success!";
            submitButton.interactable = false;
        }

        catch (Exception e)
        {
            //string6 = e.ToString();
            submitText.text = "Failed";
        }
    }

    // 1st way. Prolly works better over multiple devices
    void CaptureScreenshot ()
    {
        /*if (System.IO.File.Exists("screenshot.png"))
        {
            System.IO.File.Delete(Application.persistentDataPath + "screenshot.png");
            Debug.Log("Deleted older screenshot");
        }*/
        //submitText.text = "Capturing";

        //Application.CaptureScreenshot("screenshot.png");
        Application.CaptureScreenshot(Application.persistentDataPath + "/screenshot.png");
        string path = Application.persistentDataPath + "/screenshot.png";
        //Debug.Log(Application.persistentDataPath + "/screenshot.png");
        SendScreenshotEmail(path);
    }

    // 1st way. Prolly works better over multiple devices
    IEnumerator CaptureScreenshotSequence()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/screenshot.png"))
        {
            System.IO.File.Delete(Application.persistentDataPath + "/screenshot.png");
            //Debug.Log("Deleted older screenshot");
        }

        //Application.CaptureScreenshot("screenshot.png");
        Application.CaptureScreenshot(Application.persistentDataPath + "/screenshot.png");
        string path = Application.persistentDataPath + "/screenshot.png";
        //Debug.Log(Application.persistentDataPath + "/screenshot.png");

        yield return new WaitForChangedResult();
        SendScreenshotEmail(path);
    }

    // 3rd way. Works on PC 
    public void SaveTextureToFile()
    {
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0 ,0, Screen.width, Screen.height), 0, 0);

        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        if (Application.platform != RuntimePlatform.Android)
            File.WriteAllBytes("screenshot.png", bytes);
        else
        {
            //Debug.Log("Android save file. path = " + Application.persistentDataPath + "/" + fileName + ".png");
            File.WriteAllBytes(Application.persistentDataPath + "/" +"screenshot.png", bytes);
        }

        SendScreenshotEmail(Application.persistentDataPath + "/" + "screenshot.png");
    }

    // 4th way. Works on PC and android. Makes a screenshot as JPG
    public void SaveTextureToFileJPG()
    {
        //submitText.text = "Almost done";
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

        texture.Apply();

        byte[] bytes = texture.EncodeToJPG();

        //Debug.Log("Android save file. path = " + Application.persistentDataPath + "/" + fileName + ".png");
        File.WriteAllBytes(Application.persistentDataPath + "/" + "screenshot.jpg", bytes);

        SendScreenshotEmail(Application.persistentDataPath + "/" + "screenshot.jpg");
    }
// For email sending debugging purposes
/*    void OnGUI()
    {
        GUILayout.Label(string1);
        GUILayout.Label(string2);
        GUILayout.Label(string3);
        GUILayout.Label(string4);
        GUILayout.Label(string5);
        GUILayout.Label(string6);
        //GUILayout.Label(string7);
    }*/
}
