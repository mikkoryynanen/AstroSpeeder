using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
//Google play 
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(GooglePlayCloudSave))]
public class GooglePlayManager : MonoBehaviour {

    public GooglePlayCloudSave googlePlayCloudSave;
    public IAPManager IAPManager;
    public Animator googlePlayWindowAnim;
    public SavingSystemV3 savingSystem;
    public GameObject googlePlayLoginWindow;
    public Text loginText;
    public Toggle rememberSelToggle;
    public GameObject loginAnimation;
    public CustomizationManager customizationManager;

    public CreateScrollList scrollList;
    GameObject player;
    //public GameObject newHighScore;

    public MainMenuManager mainMenuManager;

    public string personalHighscoreRandomMode;
    public string personalHighscoreFixedMode;
    public string personalHighscoreContestMode;

    private string string1 = "kek";
    private string string2 = "kek";
    private string string3 = "3";
    private int its;

    void Start () {
        //DontDestroyOnLoad(this);

        //Try to find the player
        if (!player) {
            player = GameObject.Find("Player");
        }

        if(!IAPManager && Application.loadedLevel == 1) {
            IAPManager = GameObject.Find("IAPManager").GetComponent<IAPManager>();
        }

        //GOOGLEP PLAY INITIALIZATION
        //Activate save games
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().RequireGooglePlus().EnableSavedGames().Build();
       PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        //PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();

        if (DataRepo.gameOpened)
        {
            /*
                CheckLoginStatus();
                DataRepo.gameOpened = false;

                Social.localUser.Authenticate((bool success) =>
                {
                    // If player was already succesfully logged in, then directly log in without popUps  
                    if (success)
                        LogPlayerIn();
                    // Else show the login Popup
                    else
                        CheckLoginStatus();
                });*/

            int loginPref = PlayerPrefs.GetInt("LoginPref");

            if (loginPref == null)
                loginPref = 0;

            // If pref is 0, then show the window for login/offline selection
            if (loginPref == 0)
            {
                CheckLoginStatus();
            }

            // If pref is 1, then automatically attempt log player in to the GP
            if(loginPref == 1)
            {
                Social.localUser.Authenticate((bool success) =>
                {
                    // If player was already succesfully logged in, then directly log in without popUps  
                    if (success)
                        LogPlayerIn();
                    // Else show the login Popup
                    else
                        CheckLoginStatus();
                });
            }

            // If pref is 2, then automatically log player in as "offline"
            if(loginPref == 2)
            {
                // Just dont pop up anything and do not log in
            }

            DataRepo.gameOpened = false;
        }
    }

    void OnLevelWasLoaded(int level) {
        player = GameObject.FindGameObjectWithTag("Player");

        //Find shit depending on the level you currently are in
        if(level == 1) {
            savingSystem = GameObject.FindWithTag("DataRepo").GetComponent<SavingSystemV3>();
            savingSystem.CheckForSkinsAchievement();

            if (!scrollList) {
                scrollList = GameObject.Find("CustomizeWindowCarage").GetComponent<CreateScrollList>();
            }
        }
        /*
        if(level == 0)
            gpApiClient.connect();*/
    }

    /*public void CheckForSkinsAchievement() {
        if(savingSystem != null)
            savingSystem.CheckForSkinsAchievement();
    }*/

    void Update() {
        //Find a better implementation option for these, Running in update function is a horrible idea!

         if(MainMenuManager.currentMenu == 3) {
            /*PlayGamesPlatform.Instance.LoadScores("CgkIku2IwNIREAIQAg", LeaderboardStart.PlayerCentered, 100, LeaderboardCollection.Public, LeaderboardTimeSpan.AllTime, (data) =>
            {
                
            });*/
            //Social.ShowLeaderboardUI();
        }

        // Make sure IAP is up and alive when in mainMenu scene
        if (Application.loadedLevel == 1)
        {
            if (IAPManager.enabled == false)
                IAPManager.enabled = true;
        }
        //When game is over
        /* if(ManagerScript.gameOver) {
            bool postedScore = false;

            if(!postedScore) {
                postedScore = true;
            }
        }*/

       /* Social.localUser.Authenticate((bool success) => {
            GooglePlayGames.OurUtils.PlayGamesHelperObject.RunOnGameThread(
                () => {
                    string3 = ("Local user's email is " + ((PlayGamesLocalUser)Social.localUser).Email);
                });
        });*/

        if(DataRepo.playerEmail == "" || DataRepo.playerEmail == null)
        {
            if (Social.localUser.authenticated)// This bit is just to keep from spamming If player prefers to play offline
            {
                Social.localUser.Authenticate((bool success) =>
                {
                    GooglePlayGames.OurUtils.PlayGamesHelperObject.RunOnGameThread(
                        () =>
                        {
                            DataRepo.playerEmail = (((PlayGamesLocalUser)Social.localUser).Email);
                        });
                });
            }
        }

        if (DataRepo.playerUsername == "" || DataRepo.playerUsername == null)
        {
            if (Social.localUser.authenticated)// This bit is just to keep from spamming If player prefers to play offline
            {
                Social.localUser.Authenticate((bool success) =>
                {
                    GooglePlayGames.OurUtils.PlayGamesHelperObject.RunOnGameThread(
                        () =>
                        {
                            DataRepo.playerUsername = (((PlayGamesLocalUser)Social.localUser).userName);
                        });
                });
            }
        }
    }

    public void GameOverPostScoreToHighscoreList() {
        long longPlayerScore = Convert.ToInt64(PlayerController.currentScore);

        Social.ReportScore(longPlayerScore, "CgkIwrG6zpcEEAIQHA", (bool success) => {

        });

        /*if (DataRepo.fixedEndless) {
            Social.ReportScore(longPlayerScore, "CgkIwrG6zpcEEAIQGQ", (bool success) => {
            });
        }
        if(DataRepo.randomEndless) {
            Social.ReportScore(longPlayerScore, "CgkIwrG6zpcEEAIQAQ", (bool success) => {
            });
        }

        if(DataRepo.competitiveMode) {
            Social.ReportScore(longPlayerScore, "CgkIwrG6zpcEEAIQGg", (bool success) => {
            });
        }*/
    }

    public void LogPlayerIn () {
        Social.localUser.Authenticate((bool success) => {
        if (success) {
                googlePlayWindowAnim.SetBool("Hide", true);
                Invoke("AnimationReset", 0.4f);
                //loginText.text = "Logging in...";

                // Get and save data fetched from googlePlay leaderboards
                GetHighscoresData();

                //When logged into google play, try to log IAPManager
                IAPManager.enabled = true;
                IAPManager.IAPLogin();

                //Load saved game from the cloud
                //googlePlayCloudSave.Save();
            }
            else {
                //loginText.text = "Login failed";
                loginAnimation.SetActive(false);
            }
        });
        //#endif
    }

    public void CheckLoginStatus() {
        if(!Social.localUser.authenticated && Application.loadedLevel == 1) {
            googlePlayLoginWindow.SetActive(true);
            DataRepo.loginOpenedOnce = true;
        }
    }

    void AnimationReset() {
        googlePlayWindowAnim.SetBool("Hide", false);
        mainMenuManager.canCloseGame = true;
    }

    void OnEnable() {
        //if(Application.loadedLevel != 2)
            //loginText.text = "Login";
    }

    public void LogPlayerOut() {
        PlayGamesPlatform.Instance.SignOut();
    }

    /*public void GiveAchievement() {
        Social.ReportProgress(testAchievement, 100.0f, (bool success) => {
            //Handle success and failure
        });
    }*/

    public void ShowAchievements() {
        if(Social.localUser.authenticated) {
            Social.ShowAchievementsUI();
        }
        else {
            googlePlayLoginWindow.SetActive(true);
        }
    }

    public void ShowLeaderboard() {
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }
        else
        {
            googlePlayLoginWindow.SetActive(true);
        }
    }

    //Achievements=============================================================================
    public void PlayerReached250Speed() {
        Social.ReportProgress("CgkIwrG6zpcEEAIQDQ", 100.0f, (bool success) => {
            PlayerPrefsX.SetBool("PlayerReached250Speed", true);
        });
    }

    public void PlayerReached500Speed() {
        Social.ReportProgress("CgkIwrG6zpcEEAIQDg", 100.0f, (bool success) => {
            PlayerPrefsX.SetBool("PlayerReached500Speed", true);
        });
    }

    public void PlayerReached750Speed() {
        Social.ReportProgress("CgkIwrG6zpcEEAIQDw", 100.0f, (bool success) => {
            PlayerPrefsX.SetBool("PlayerReached750Speed", true);
        });
    }

    public void HitAsteroid() {
        Social.ReportProgress("CgkIwrG6zpcEEAIQFA", 100.0f, (bool success) => {
            PlayerPrefsX.SetBool("HitAsteroid", true);
        });
    }

     public void BrokeTheGame() {
        Social.ReportProgress("CgkIwrG6zpcEEAIQFw", 100.0f, (bool success) => {
            
        });
    }

    public void BeginnerSurvivor() {
        Social.ReportProgress("CgkIwrG6zpcEEAIQCg", 100.0f, (bool success) => {

        });
    }

    public void Survivor() {
        Social.ReportProgress("CgkIwrG6zpcEEAIQCw", 100.0f, (bool success) => {
        });
    }

    public void BestSurvivor() {
        Social.ReportProgress("CgkIwrG6zpcEEAIQDA", 100.0f, (bool success) => {
            PlayerPrefsX.SetBool("SurvivorAchievementsGranted", true);
        });
    }

    public void Skins() {
        Social.ReportProgress("CgkIwrG6zpcEEAIQEg", 100.0f, (bool success) => {
            PlayerPrefsX.SetBool("Achievement_5Skins", true);
        });
    }

    public void MoreSkins() {
        Social.ReportProgress("CgkIwrG6zpcEEAIQEw", 100.0f, (bool success) => {
            PlayerPrefsX.SetBool("Achievement_10Skins", true);
        });
    }

    public void CreditToTheTeam() {
        Social.ReportProgress("CgkIwrG6zpcEEAIQGA", 100.0f, (bool success) => {
            PlayerPrefsX.SetBool("CreditAchievementGranted", true);
        });
    }

    public void Travelled50kUnitsTotal()
    {
        Social.ReportProgress("CgkIwrG6zpcEEAIQAg", 100.0f, (bool success) => {
        });
    }

    public void Travelled100kUnitsTotal()
    {
        Social.ReportProgress("CgkIwrG6zpcEEAIQAw", 100.0f, (bool success) => {
        });
    }

    public void Travelled500kUnitsTotal()
    {
        Social.ReportProgress("CgkIwrG6zpcEEAIQBA", 100.0f, (bool success) => {
        });
    }

    public void Travelled1mUnitsTotal()
    {
        Social.ReportProgress("CgkIwrG6zpcEEAIQBQ", 100.0f, (bool success) => {
        });
    }

    public void Travelled10mUnitsTotal()
    {
        Social.ReportProgress("CgkIwrG6zpcEEAIQBg", 100.0f, (bool success) => {
            PlayerPrefsX.SetBool("Achievement_TravelAchievementsGranted", true);
        });
    }

    public void SilverCoins()
    {
        Social.ReportProgress("CgkIwrG6zpcEEAIQEA", 100.0f, (bool success) => {
            PlayerPrefsX.SetBool("Achievement_SilverCoins", true);
        });

    }

    public void SilverSpender()
    {
        Social.ReportProgress("CgkIwrG6zpcEEAIQEQ", 100.0f, (bool success) => {
            PlayerPrefsX.SetBool("Achievement_SilverSpender", true);
        });
    }

    public void ItsOver9000() {
        Social.ReportProgress("CgkIwrG6zpcEEAIQFg", 100.0f, (bool success) => {
            PlayerPrefsX.SetBool("Achievement_ItsOver9000", true);
        });
    }

    //NYI
    public void UnlockedAllCosmetics() {
        Social.ReportProgress("CgkIvpev3o0fEAIQHA", 100.0f, (bool success) => {
            PlayerPrefsX.SetBool("Achievement_UnlockedAllCosmetics", true);
        });
    } 

    //Incimental Achievements==================================================================
    public void AddPlayedGames() {
        int savedPlayedGames = PlayerPrefs.GetInt("GamesPlayed");
        PlayerPrefs.SetInt("GamesPlayed", savedPlayedGames += 1);
        //Debug.Log("savedPlayedGames " + PlayerPrefs.GetInt("GamesPlayed"));

         if (PlayerPrefs.GetInt("GamesPlayed") == 50 && !PlayerPrefsX.GetBool("50GamesPlayed")) {
            Play50Games();
            PlayerPrefsX.SetBool("50GamesPlayed", true);
            //Debug.Log("50 games played");
        }
            
        if(PlayerPrefs.GetInt("GamesPlayed") == 100 && !PlayerPrefsX.GetBool("100GamesPlayed")) {
            Play100Games();
            PlayerPrefsX.SetBool("100GamesPlayed", true);
            //Debug.Log("100 games played");
        } 

        if(PlayerPrefs.GetInt("GamesPlayed") == 200 && !PlayerPrefsX.GetBool("200GamesPlayed")) {
            Play200Games();
            PlayerPrefsX.SetBool("200GamesPlayed", true);
            //Debug.Log("200 games played");
        }

        if (PlayerPrefs.GetInt("GamesPlayed") <= 50)
        {
            Play50Games();
        }

        if (PlayerPrefs.GetInt("GamesPlayed") <= 100)
        {
            Play100Games();
        }

        if (PlayerPrefs.GetInt("GamesPlayed") <= 200)
        {
            Play200Games();
        }
    }

    public void Play50Games() {
        PlayGamesPlatform.Instance.IncrementAchievement("CgkIwrG6zpcEEAIQBw", 1, (bool success) => {
            PlayerPrefsX.SetBool("50GamesPlayed", true);
        });
    }

    public void Play100Games() {
        PlayGamesPlatform.Instance.IncrementAchievement("CgkIwrG6zpcEEAIQCA", 1, (bool success) => {
            PlayerPrefsX.SetBool("100GamesPlayed", true);
        });
    }

    public void Play200Games() {
        PlayGamesPlatform.Instance.IncrementAchievement("CgkIwrG6zpcEEAIQCQ", 1, (bool success) => {
            PlayerPrefsX.SetBool("200GamesPlayed", true);

        });
    }

    public void HitAsteroids() {
        PlayGamesPlatform.Instance.IncrementAchievement("CgkIwrG6zpcEEAIQFQ", 1, (bool success) => {
        });
    }
    
    /*void OnGUI ()
    {
        GUILayout.Label(string1);
        GUILayout.Label(string2);
        GUILayout.Label(string3);
        GUILayout.Label(string4);
        GUILayout.Label(string5);
    }*/

    void GetHighscoresData ()
    {
        // Retrieve the highscore data from Google Play, randomEndless
        PlayGamesPlatform.Instance.LoadScores(
          "CgkIwrG6zpcEEAIQAQ",
           LeaderboardStart.PlayerCentered,
           1,
           LeaderboardCollection.Public,
           LeaderboardTimeSpan.AllTime,
       (LeaderboardScoreData data) =>
       {
           /*Debug.Log(data.Valid);
           Debug.Log(data.Id);
           Debug.Log(data.PlayerScore);
           Debug.Log(data.PlayerScore.userID); // Player unique ID
           Debug.Log(data.PlayerScore.formattedValue); // highest score posted on GL leaderboards*/

           // Get and set the players highscore if any is found
           if (data.PlayerScore.formattedValue == null)
               personalHighscoreRandomMode = "0";
           else
               personalHighscoreRandomMode = data.PlayerScore.formattedValue;

           // Check which score is higher, local or leaderboard info.
           if (Convert.ToInt64(personalHighscoreRandomMode) < SavingSystemV3.personalRandomModeHighscore)
           {
               // If local score is higher than peceived data from the GP. Keep the local saved info as it is
           }
           else if (Convert.ToInt64(personalHighscoreRandomMode) >= SavingSystemV3.personalRandomModeHighscore)
               SavingSystemV3.personalRandomModeHighscore = Convert.ToInt64(personalHighscoreRandomMode);
       });

        // FixedEndless
        PlayGamesPlatform.Instance.LoadScores(
          "CgkIwrG6zpcEEAIQGQ",
           LeaderboardStart.PlayerCentered,
           1,
           LeaderboardCollection.Public,
           LeaderboardTimeSpan.AllTime,
        (LeaderboardScoreData data) => {
            /*Debug.Log(data.Valid);
            Debug.Log(data.Id);
            Debug.Log(data.PlayerScore);
            Debug.Log(data.PlayerScore.userID); // Player unique ID
            Debug.Log(data.PlayerScore.formattedValue); // highest score posted on GL leaderboards*/

            // Get and set the players highscore if any is found
            if (data.PlayerScore.formattedValue == null)
                personalHighscoreFixedMode = "0";
            else
                personalHighscoreFixedMode = data.PlayerScore.formattedValue;

            // Check which score is higher, local or leaderboard info.
            if (Convert.ToInt64(personalHighscoreFixedMode) < SavingSystemV3.personalFixedModeHighscore)
            {
                // If local score is higher than peceived data from the GP. Keep the local saved info as it is
            }
            else if (Convert.ToInt64(personalHighscoreFixedMode) >= SavingSystemV3.personalFixedModeHighscore)
                SavingSystemV3.personalFixedModeHighscore = Convert.ToInt64(personalHighscoreFixedMode);
        });

        // ContestMode
        PlayGamesPlatform.Instance.LoadScores(
          "CgkIwrG6zpcEEAIQGg",
           LeaderboardStart.PlayerCentered,
           1,
           LeaderboardCollection.Public,
           LeaderboardTimeSpan.AllTime,
        (LeaderboardScoreData data) => {
            /*Debug.Log(data.Valid);
            Debug.Log(data.Id);
            Debug.Log(data.PlayerScore);
            Debug.Log(data.PlayerScore.userID); // Player unique ID
            Debug.Log(data.PlayerScore.formattedValue); // highest score posted on GL leaderboards*/

            // Get and set the players highscore if any is found
            if (data.PlayerScore.formattedValue == null)
                personalHighscoreContestMode = "0";
            else
                personalHighscoreContestMode = data.PlayerScore.formattedValue;

            // Check which score is higher, local or leaderboard info.
            if (Convert.ToInt64(personalHighscoreContestMode) < SavingSystemV3.personalContestModeHighscore)
            {
                // If local score is higher than peceived data from the GP. Keep the local saved info as it is
            }
            else if (Convert.ToInt64(personalHighscoreContestMode) >= SavingSystemV3.personalContestModeHighscore)
                SavingSystemV3.personalContestModeHighscore = Convert.ToInt64(personalHighscoreContestMode);

            // Aaand save the data
            savingSystem.SaveData();
        });
    }

    public void SetLoginPref ()
    {
        if(rememberSelToggle.isOn == true)
        {
            PlayerPrefs.SetInt("LoginPref", 1);
            Debug.Log("Set int to 1");
        }
        else
        {
            PlayerPrefs.SetInt("LoginPref", 0);
        }
    }

    public void SetOfflinePref()
    {
        if (rememberSelToggle.isOn == true)
        {
            PlayerPrefs.SetInt("LoginPref", 2);
            Debug.Log("Set int to 2");
        }
        else
        {
            PlayerPrefs.SetInt("LoginPref", 0);
        }
    }
    /*
    void OnGUI()
    {
        GUILayout.Label(string1);
        GUILayout.Label(string2);
        GUILayout.Label(string3);
    }*/
}
