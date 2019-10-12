using System;
using UnityEngine;
using System.Collections;
//Google play 
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi.SavedGame;

public class GooglePlayCloudSave : MonoBehaviour {
System.Action<bool> mAuthCallback;

    GameData slot0;

    bool mSaving;
    private Texture2D mScreenImage;

    public SavingSystemV3 savingSystem;
    public CustomizationManager customizationManager;

    void Start () {

        /*slot0 = new GameData("New game");
        mAuthCallback = (bool success) => {

            if (success) {
                Debug.Log("Authentication was successful!");
                slot0.State = "Click load or save";
            }
            else {
                Debug.LogWarning("Authentication failed!");
            }

      };


        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();
        PlayGamesPlatform.InitializeInstance(config);

        // Activate the Play Games platform. This will make it the default
        // implementation of Social.Active
        PlayGamesPlatform.Activate();

        // enable debug logs (note: we do this because this is a sample; on your production
        // app, you probably don't want this turned on by default, as it will fill the user's
        // logs with debug info).
        //PlayGamesPlatform.DebugLogEnabled = true;

        //Login explicitly for this sample, usually this would be silent
        //PlayGamesPlatform.Instance.Authenticate(mAuthCallback, false);*/


    }

    public void CaptureScreenshot() {
        mScreenImage = new Texture2D(Screen.width, Screen.height);
        mScreenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        mScreenImage.Apply();
    }

    public void Load() {
        mSaving = false;
        ((PlayGamesPlatform)Social.Active).SavedGame.ShowSelectSavedGameUI("Select game to load", 4, false, false, SavedGameSelected);
    }

    public void Save() {
        int idx = slot0.Data.IndexOf("_");
            if (idx > 0) {
                int val = Convert.ToInt32(slot0.Data.Substring(idx+1));
                val++;
                slot0.Data = "Save_" + val;
            }
            else {
                slot0.Data = "Save_0";
            }
            mSaving = true;
            //CaptureScreenshot();
            ((PlayGamesPlatform)Social.Active).SavedGame.ShowSelectSavedGameUI("Save game progress", 4, true, true, SavedGameSelected);
    }

    /*protected virtual void OnGUI() {

        Screen.fullScreen = true;

        int buttonHeight = Screen.height / 20;
        int buttonWidth = Screen.width / 5;

        GUI.skin.label.fontSize = 60;
        GUI.skin.button.fontSize = 60;


        Rect statusRect = new Rect(10,20,Screen.width,200);
        Rect dataRect  = new Rect( 10, 250, Screen.width,100);

        Rect b1Rect = new Rect(10, 800, buttonWidth, buttonHeight);
        Rect b2Rect = new Rect(b1Rect.x + 20 + buttonWidth, b1Rect.y, buttonWidth, buttonHeight);

        if(!Social.localUser.authenticated) {
            if(GUI.Button(b1Rect, "Signin")) {
                Social.localUser.Authenticate(mAuthCallback);
            }
        }
        else {
            if(GUI.Button(b1Rect, "Load")) {
                mSaving = false;
                ((PlayGamesPlatform)Social.Active).SavedGame.ShowSelectSavedGameUI("Select game to load",
                                                                                   4,false,false,SavedGameSelected);
            }

            GUI.Label(dataRect, slot0.Data);
        }

        if(GUI.Button(b2Rect, "Save")) {
            int idx = slot0.Data.IndexOf("_");
            if (idx > 0) {
                int val = Convert.ToInt32(slot0.Data.Substring(idx+1));
                val++;
                slot0.Data = "Save_" + val;
            }
            else {
                slot0.Data = "Save_0";
            }
            mSaving = true;
            CaptureScreenshot();
            ((PlayGamesPlatform)Social.Active).SavedGame.ShowSelectSavedGameUI("Save game progress",
                                                                               4,true,true,SavedGameSelected);
        }

        GUI.Label(statusRect, slot0.State);

    }*/

    public void SavedGameSelected(SelectUIStatus status, ISavedGameMetadata game) {

        if (status == SelectUIStatus.SavedGameSelected) {
            string filename = game.Filename;
            Debug.Log("opening saved game:  " + game);
            if(mSaving && (filename == null || filename.Length == 0)) {
                filename = "save" + DateTime.Now.ToBinary();
            }
            if (mSaving) {
                slot0.State = "Saving to " + filename;
            }
            else {
                slot0.State = "Loading from " + filename;
            }
            //open the data.
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(filename,
                                                                                             DataSource.ReadCacheOrNetwork,
                                                                                             ConflictResolutionStrategy.UseLongestPlaytime,
                                                                                             SavedGameOpened);
            //Update wallet
            //Save data
            savingSystem.SaveData();
            //Update the wallet
            customizationManager.UpdateWallet();

        } else {
            Debug.LogWarning("Error selecting save game: " + status);
        }

    }

    public void SavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game) {
        if(status == SavedGameRequestStatus.Success) {
            if( mSaving) {
                slot0.State = "Opened, now writing";
                byte[] pngData = (mScreenImage!=null) ?mScreenImage.EncodeToPNG():null;
                Debug.Log("Saving to " + game);
                byte[] data = slot0.ToBytes();
                TimeSpan playedTime = slot0.TotalPlayingTime;
                SavedGameMetadataUpdate.Builder builder =  new 
                    SavedGameMetadataUpdate.Builder()
                        .WithUpdatedPlayedTime(playedTime)
                        .WithUpdatedDescription("Saved Game at " + DateTime.Now);

                if (pngData != null) {
                    Debug.Log("Save image of len " + pngData.Length);
                    builder = builder.WithUpdatedPngCoverImage(pngData);
                }
                else {
                    Debug.Log ("No image avail");
                }
                SavedGameMetadataUpdate updatedMetadata  = builder.Build();
                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game,updatedMetadata,data,SavedGameWritten);
            } else {
                slot0.State = "Opened, reading...";
                ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game,SavedGameLoaded);
            }
        } else {
            Debug.LogWarning("Error opening game: " + status);
        }
    }

    public void SavedGameLoaded(SavedGameRequestStatus status, byte[] data) {
        if (status == SavedGameRequestStatus.Success) {
            Debug.Log("SaveGameLoaded, success=" + status);
            slot0 = GameData.FromBytes(data);
        } else {
            Debug.LogWarning("Error reading game: " + status);
        }
    }


    public void SavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game) {
        if(status == SavedGameRequestStatus.Success) {
            Debug.Log ("Game " + game.Description + " written");
            slot0.State = "Saved!";
        } else {
            Debug.LogWarning("Error saving game: " + status);
        }
    }


    public class GameData  {

        private TimeSpan mPlayingTime;
        private DateTime mLoadedTime;
        string mData;
        string mState;

        static readonly string HEADER = "GDv1";

        public GameData(string data) {

            mData = data;
            mState = "Initialized, modified";
            mPlayingTime = new TimeSpan();
            mLoadedTime = DateTime.Now;

        }

        public TimeSpan TotalPlayingTime {
            get {
                TimeSpan delta = DateTime.Now.Subtract(mLoadedTime);
                return mPlayingTime.Add(delta);
            }
        }

        public override string ToString () {
            string s = HEADER + ":" + mData;
            s += ":" + TotalPlayingTime.TotalMilliseconds;
            return s;
        }

        public byte[] ToBytes() {
            return System.Text.ASCIIEncoding.Default.GetBytes(ToString());
        }


        public static GameData FromBytes (byte[] bytes) {
            return FromString(System.Text.ASCIIEncoding.Default.GetString(bytes));
        }

        public static GameData FromString (string s) {
            GameData gd = new GameData("initializing from string");
            string[] p = s.Split(new char[] { ':' });
            if (!p[0].StartsWith(HEADER)) {
                Debug.LogError("Failed to parse game data from: " + s);
                return gd;
            }
            gd.mData = p[1];
            double val = Double.Parse(p[2]);
            gd.mPlayingTime = TimeSpan.FromMilliseconds(val>0f?val:0f);

            gd.mLoadedTime = DateTime.Now;
            gd.mState = "Loaded successfully";
            return gd;
        }

        public string Data {
            get {
                return mData;
            }

            set {
                mData = value;
                mState += ", modified";
            }

        }

        public string State {
            get {
                return mState;
            }

            set {
                mState = value;
            }
        }

    }
}
