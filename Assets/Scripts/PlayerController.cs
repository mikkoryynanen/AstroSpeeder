using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {
    public GooglePlayManager GPManager;
    public IOSManager GCManager;

    [Header("Controls")]
    public bool pcControls = false;
    public bool gyroControls = false;
    public bool touchControls = false;

    public GameObject asteroidDestroyParticle;
    public GameObject playerShip;
    public Material[] playerShipMaterials;
    public GameObject[] playerShipThrusters;

    [Header("UI Variables")]
    public Image shieldSlider;
    public Text currentScoreText;
    public Text scoreMultiplierText;
    public Text speedText;
    public Image superShiedSlider;
    public float superShieldMinSwipeDistance = 50f;
    bool rechargeSuperShield = false;
    //public Image speedometerFill;
    //public Text distanceTraveledText;
    //public Text timeText;
    //public Image pickUpTimerFill;

    [Header("Player Variables")]
    public AnimationCurve speedCurve;
    public AnimationCurve speedCurve2;
    public AnimationCurve speedCurveLinear;
    static public float speedTime;
    static public float gameTime = 0;
    public float movementSpeed = 3.0f;
    public float speed;
    static public float forwardMovementSpeed = 135f;
    private float lastForwardSpeed;
    public float leanSpeed = 5f;
    public float shieldLifeTime = 4f;
    static public float distanceTraveled = 0f;
    static public float topSpeed;

    public float xClamp;
    private float scoreMultipler = 1f;
    public float giveScoreInterval = 1;
    public int extraScoreMultiplier = 0;

    public float maxMultiplier = 5f;
    public float scoreIncrement = 0.1f;
    private float scoreBuff = 0f;
    public float scoreBuffAmount = 2f;
    private float trueMultiplier;
    private bool sBuffed;
    private bool superShieldUp;
    //public GameObject continuousShield;

    float smoothInput;
    public float sensitivity = 5f;
    public Vector3 targetAngle;

    public CameraController camController;
    public CameraController gridCamController;

    bool changeCameraOffset = false;
    public bool immortality = false;

    [Header("Audio clips")]
    AudioSource audio;
    public AudioClip collectableSound;
    public AudioClip collisionSound;

    [Header("Particle effects")]
    public GameObject lowShieldParticle;
    public GameObject shieldPickUpParticle;
    public ParticleSystem[] speedEffects;
    GameObject sparkParticle;
    //Current points
    static public float currentScore = 0f;
    public float roundedScore;
    float playerSpeedMultiplier;
    //float animationCurveValue;
    //float scoreMultiplierTimer;
    //bool canCountMultiplier = true;

    //Traveled distance variables
    static public bool canSpawnAsteroids = true;
    bool canEndGame = false;

    bool particleInstantiated = false;
    GameObject instatiatedShieldParticle;

    public LayerMask layerMask;
    Vector3 hitPos;
    Rigidbody rb;

    [Header("Pickup variables")]
    public int goldPickUpAmount = 1;
    public int silverPickUpAmount = 10;
    // Slowmotion
    public float pickupDuration = 5f;
    public float SpeedChangeInSecs = 1f;
    public float SpeedChangeSmoothness = 0.05f;
    public float slowingAmount = 0.5f;
    private float timeScale = 1f;
    private bool pickUpEnded;
    private float durationOnPickUp;
    private bool slowmoAlreadyRunning;
    public float vignetteAmountMax = 0.35f;
    private VignetteAndChromaticAberration vignette;
    private float pickUpFillDeduct;
    public float pickUpFillDeductSmoothness = 0.1f;

    public Text pickUpText;

    public GameObject greenShield;
    public GameObject blueShield;
    public GameObject blueLongShield;

    public GameObject pickUpSystem;

    bool travelDoneAchievementsChecked;

    bool beginnerSurvivor = false;
    bool survivor = false;
    bool bestSurvivor = false;

    bool PlayerReached1stSpeedInterval;
    bool PlayerReached2ndSpeedInterval;
    bool PlayerReached3rdSpeedInterval;

    private Vector3 superShieldGestureStartPos;

    private SavingSystemV3 savingSystem;

    private float modStrafeMultiplier = 1f; // Strafe Movespeed
    private float modSpeedMultiplier = 1f; // Acceleration
    private float modShieldRegenMultiplier = 1f; // Shield Regen Rate
    private float modShieldAmountMultiplier = 1f; // Shield Amount
    private float modSilverCollectMultiplier = 1f; // Silver amount collected
    private float modGoldCollectMultiplier = 1f; // Gold amount collected
    private float modPickUpSpawnRateMultiplier = 1f; // PickUp spawn rate
    private float modSuperShieldCooldownMultiplier = 1f; // Supershield CD
    private float modSuperShieldDurationMultiplier = 1f; // SUpershield duration
    private float modAsteroidSpawnRateMultiplier = 1f; // Asteroid spawn rate
    private float modScoreMultiplier = 1f; // Scores multiplier
    private float modScoreClimbRate = 1f; // Score calculationRate
    private float modScoreMultiplierDecrease = 1f; // Score Reduction Amount

    private bool superShieldUsable = true;
    private bool shieldRegenActive = true;
    private bool shieldActive = true;
    private bool ironmanModeOn;

    public float defaultRegenFrequency = 0.1f;
    public float defaultShieldRegen = 0.001f;

    private float asteroidDmg = 0.25f;
    private float shieldDepleteRate;

    public ManagerScript manager;

    // Ensure the top speed from last round wont be carried over
    void OnLevelWasLoaded()
    {
        topSpeed = 0;
        travelDoneAchievementsChecked = false;

        if (manager == null)
            manager = GameObject.Find("_Manager").GetComponent<ManagerScript>();

        manager.silverFromPickUps = 0;
        manager.goldFromPickUps = 0;
    }

    void Start() {
        if (gyroControls) {
            Input.gyro.enabled = true;
        }

        if(manager == null)
            manager = GameObject.Find("_Manager").GetComponent<ManagerScript>();

        //Reset some values
        distanceTraveled = 0f;
        forwardMovementSpeed = 80f;
        speedTime = 0;
        gameTime = 0;
        currentScore = 0;
        scoreMultipler = 1f;
        ManagerScript.gameOver = false;
        immortality = false;
        ironmanModeOn = false;
        //Write reseted values
        GetUiComponents();
        scoreMultiplierText.text = "X" + scoreMultipler;
        currentScoreText.text = currentScore.ToString("000000");
        //Reset speedEffects particles speed 
        for (int i = 0; i < speedEffects.Length; i++) {
            speedEffects[i].startSpeed = 50f;
        }

        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();

        if (savingSystem == null)
            savingSystem = GameObject.FindWithTag("DataRepo").GetComponent<SavingSystemV3>();

        SetModuleAdjustedValues();

#if UNITY_ANDROID
        GPManager = GameObject.Find("GooglePlayManager").GetComponent<GooglePlayManager>();
#endif

#if UNITY_IOS
        GCManager = GameObject.Find("GameCenterManager").GetComponent<IOSManager>();
#endif

        //Set player ship customization options. Currently being set inside the saving system
        //playerShip.GetComponent<Renderer>().material = playerShipMaterials[PlayerPrefs.GetInt("PlayerShipSkin")]; //PLACEHOLDER come up with a better idea!
        //playerShipThrusters[PlayerPrefs.GetInt("PlayerShipThruster")].SetActive(true);

        rb = GetComponent<Rigidbody>();
        pickUpFillDeduct = 1 / (pickupDuration / pickUpFillDeductSmoothness);
        InvokeRepeating("GiveScore2", 0, giveScoreInterval* modScoreClimbRate);

        // Start the regenating functions
        InvokeRepeating("RegenShield", 0, defaultRegenFrequency);

        PlayerReached1stSpeedInterval = PlayerPrefsX.GetBool("PlayerReached250Speed");
        PlayerReached2ndSpeedInterval = PlayerPrefsX.GetBool("PlayerReached500Speed");
        PlayerReached3rdSpeedInterval = PlayerPrefsX.GetBool("PlayerReached2000Speed");
    }

    void GiveScore() {
        scoreMultipler = Mathf.Clamp(scoreMultipler, 0.0f, 2.9f);
        scoreMultipler += Mathf.Lerp(0f, 3f, 2f * Time.deltaTime);

        scoreMultipler = Mathf.Round(scoreMultipler * 10f) / 10f;
        scoreMultiplierText.text = "X" + (scoreMultipler + extraScoreMultiplier);

        //Increase points with multiplier and add forwardMovementSpeed
        currentScore += (100f * scoreMultipler * extraScoreMultiplier) + forwardMovementSpeed * Time.deltaTime;
        //Round up the currentScore
        float decimalScore = Mathf.Round(currentScore * 1f) / 1f;
        //Write it to UI
        currentScoreText.text = decimalScore.ToString("000000");
        //Rounded score is for saving(keeps it clean and short)
        roundedScore = decimalScore;

        //Reach 9.999.999 points achievemnt
        if (decimalScore >= 9999999) {
#if UNITY_ANDROID
            GPManager.BrokeTheGame();
#endif

#if UNITY_IOS
            GameCenterManager.SubmitAchievement(100f, "Achievement_BrokeTheGame", true);
#endif
        }
    }

    void GiveScore2()
    {
        //Add increment on every call
        if (trueMultiplier < 10)//Should be scoreMultiplier ? So it wont cap during buff
            scoreMultipler += scoreIncrement;
        //Round value to one decimal and make sure it wont go over the maxMultiplier
        double tempMultiplier = System.Math.Round(scoreMultipler, 1);
        scoreMultipler = Mathf.Clamp((float)tempMultiplier, 1f, maxMultiplier);
        //Set possible buff into multiplier
        if (sBuffed == false)
            trueMultiplier = scoreMultipler;
        if (sBuffed == true)
            trueMultiplier = scoreBuff;
        //Write to UI, clean decimals if multiplier is 10,11 or 12. Else show with one decimal
        if (trueMultiplier < 10 || trueMultiplier > 10)
            scoreMultiplierText.text = "X" + trueMultiplier.ToString("F1");
        if (trueMultiplier == 10 || trueMultiplier == 11 || trueMultiplier == 12)
            scoreMultiplierText.text = "X" + trueMultiplier.ToString("F0");

        //Add and multiply gained points
//        currentScore += (100f * forwardMovementSpeed) * trueMultiplier * Time.deltaTime;
        currentScore += modScoreMultiplier * (1.35f * forwardMovementSpeed) * trueMultiplier;

        //Round up the currentScore
        float decimalScore = Mathf.Round(currentScore);
        //Write it to UI
        currentScoreText.text = decimalScore.ToString("000000");
        //Rounded score is for saving
        roundedScore = decimalScore;

        //Reach 999.999 points achievemnt
        if (decimalScore >= 999999)
        {
#if UNITY_ANDROID
            GPManager.BrokeTheGame();
#endif

#if UNITY_IOS
            GameCenterManager.SubmitAchievement(100f, "Achievement_BrokeTheGame", true);
#endif
        }
    }

    void Update() {
        //Increase gameTime ovetime
        gameTime += Time.deltaTime;
        //timeText.text = gameTime.ToString("000");

        //Survivor achievements
        if (!bestSurvivor) {
            //Beginner survivor
            if (gameTime >= 30 && !beginnerSurvivor) {
#if UNITY_ANDROID
                GPManager.BeginnerSurvivor();
#endif

#if UNITY_IOS
                GameCenterManager.SubmitAchievement(100f, "Achievement_30Survived", true);
#endif

                //Debug.Log("BeginnerSurvivor");
                beginnerSurvivor = true;
            }
            //Survivor
            if (gameTime >= 60 && !survivor) {

#if UNITY_ANDROID
                GPManager.Survivor();
#endif

#if UNITY_IOS
                GameCenterManager.SubmitAchievement(100f, "Achievement_60Survived", true);
#endif
                //Debug.Log("Survivor");
                survivor = true;
            }

            //Best Survivor
            if (gameTime >= 120 && !bestSurvivor) {
#if UNITY_ANDROID
                GPManager.BestSurvivor();
#endif

#if UNITY_IOS
                GameCenterManager.SubmitAchievement(100f, "Achievement_120Survived", true);
#endif
                //Debug.Log("BestSurvivor");
                bestSurvivor = true;
            }
        }
        //Debug.DrawLine(transform.position, hitPos, Color.red);

        //Move player forward
        transform.Translate(Vector3.forward * forwardMovementSpeed * modSpeedMultiplier * Time.deltaTime);

        //Increase forwardMovmentSpeed over time by AnimationCurve
        speedTime += Time.deltaTime;

        forwardMovementSpeed = speedCurveLinear.Evaluate(speedTime);

        //Get Top Speed
        if (speed > topSpeed)
        {
            topSpeed = speed;
        }

        //Speed achievements
        if (!PlayerReached1stSpeedInterval && forwardMovementSpeed * modSpeedMultiplier >= 250) {
#if UNITY_ANDROID
            GPManager.PlayerReached250Speed();
#endif

#if UNITY_IOS
            GameCenterManager.SubmitAchievement(100f, "Achievement_250Speed", true);
#endif

            //Debug.Log(250);
            PlayerReached1stSpeedInterval = true;
        }

        if (!PlayerReached2ndSpeedInterval && forwardMovementSpeed * modSpeedMultiplier >= 500) {
#if UNITY_ANDROID
            GPManager.PlayerReached500Speed();
#endif

#if UNITY_IOS
            GameCenterManager.SubmitAchievement(100f, "Achievement_500Speed", true);
#endif
            //Debug.Log(500);
            PlayerReached2ndSpeedInterval = true;
        }

        if (!PlayerReached3rdSpeedInterval && forwardMovementSpeed * modSpeedMultiplier >= 750) {
#if UNITY_ANDROID
            GPManager.PlayerReached750Speed();
#endif

#if UNITY_IOS
            GameCenterManager.SubmitAchievement(100f, "Achievement_750Speed", true);
#endif
            //Debug.Log(750);
            PlayerReached3rdSpeedInterval = true;
        }

        if (!ManagerScript.gameOver) {
            //Speedometer
            speed = Mathf.Round(forwardMovementSpeed * modSpeedMultiplier);
            speedText.text = speed.ToString("000");
            //speedometerFill.fillAmount = (forwardMovementSpeed / 500f);//1000f

            //Distance travelled
            distanceTraveled += (Vector3.Distance(Vector3.zero, transform.position)) / 1000;
            //distanceTraveledText.text = Mathf.Round(distanceTraveled).ToString("000000");
        }
        //When the game is over
        else
        {
            CheckForDistanceTravelledAchievements();
        }

        //Check if shield is 0, destroy ship
        if (shieldSlider.fillAmount <= 0f && canEndGame) {
            ManagerScript.gameOver = true;
        }

        if (shieldSlider.fillAmount <= asteroidDmg && !ironmanModeOn) {
            if (!particleInstantiated) {
                sparkParticle = Instantiate(lowShieldParticle, transform.position, Quaternion.identity) as GameObject;
                sparkParticle.transform.parent = transform;

                particleInstantiated = true;
            }
        }
        else {
            if (particleInstantiated) {
                if (sparkParticle == null)
                    sparkParticle = GameObject.Find("P_Sparks(Clone)");
                Destroy(sparkParticle);
                particleInstantiated = false;
            }
        }

        //Gyro controls ! Does not take the strafeSpeedMultiplier yet!!
        if (gyroControls) {
            //pcControls = false;
            //touchControls = false;

            Vector3 accelerator = Input.acceleration;

            //Clamping
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -xClamp, xClamp);
            transform.position = pos;

            //Movement
            transform.Translate(accelerator.x * 2.5f, 0, -accelerator.z * 3.5f);

            //Ship rotating(Visual only)
            playerShip.transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 180, 0), Quaternion.Euler(0, 180, accelerator.x * 100f), Time.deltaTime * leanSpeed);
        }

        //PC control
        if (pcControls) {
            //gyroControls = false;

            //Clamping
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -xClamp, xClamp);
            transform.position = pos;

            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
            //When going Left
            if (move.x < 0 && transform.position.x > -xClamp) {
                transform.position += move * movementSpeed * Time.deltaTime;
                playerShip.transform.rotation = Quaternion.Slerp(playerShip.transform.rotation, Quaternion.Euler(0, 180, -25), Time.deltaTime * leanSpeed);
            }
            //When going Right
            if (move.x > 0 && transform.position.x < xClamp) {
                transform.position += move * movementSpeed * Time.deltaTime;
                playerShip.transform.rotation = Quaternion.Slerp(playerShip.transform.rotation, Quaternion.Euler(0, 180, 25), Time.deltaTime * leanSpeed);
            }
        }

        //Touch control
        if (touchControls) {
            //gyroControls = false;

            if (Input.touchCount == 1) {
                //Clamping
                Vector3 pos = transform.position;
                pos.x = Mathf.Clamp(pos.x, -xClamp, xClamp);
                transform.position = pos;

                Touch touch = Input.touches[0];

                //Left
                if (touch.position.x < Screen.width / 2 && transform.position.x > -xClamp) {
                    print("touching left");
                    //Movement
                    transform.Translate(Vector3.left * Time.deltaTime * movementSpeed);

                    //Rotation
                    if (touch.phase == TouchPhase.Stationary) {
                        playerShip.transform.rotation = Quaternion.Slerp(playerShip.transform.rotation, Quaternion.Euler(0, 180, -25), Time.deltaTime * leanSpeed);
                    }
                }

                //Right
                if (touch.position.x > Screen.width / 2 && transform.position.x < xClamp) {
                    print("touching right");
                    //Movement
                    //smoothInput = Mathf.Lerp(smoothInput, 1f, Time.deltaTime * sensitivity);
                    transform.Translate(-Vector3.left * Time.deltaTime * movementSpeed);

                    //Rotation
                    if (touch.phase == TouchPhase.Stationary) {
                        playerShip.transform.rotation = Quaternion.Slerp(playerShip.transform.rotation, Quaternion.Euler(0, 180, 25), Time.deltaTime * leanSpeed);
                    }
                }
            }
            else {
                //Turn the ship back to its original rotation, when no touch is detected
                playerShip.transform.rotation = Quaternion.Slerp(playerShip.transform.rotation, Quaternion.Euler(0, 180, 0), Time.deltaTime * leanSpeed);
            }
        }

#if UNITY_EDITOR
        if(Input.GetKeyDown("space") && !superShieldUp && !rechargeSuperShield && superShieldUsable)
            StartCoroutine("SuperShieldV2");
#endif

        //Super shield gesture
        if (Input.touchCount > 0) {
            Touch touch = Input.touches[0];

            if (touch.phase == TouchPhase.Began) {
                superShieldGestureStartPos = touch.position;
            }

            else if (touch.phase == TouchPhase.Ended) {
                float swipeDistVertical = (new Vector3(0, touch.position.y, 0) - new Vector3(0, superShieldGestureStartPos.y, 0)).magnitude;

                if (swipeDistVertical > superShieldMinSwipeDistance) {
                    float swipeValue = Mathf.Sign(touch.position.y - superShieldGestureStartPos.y);

                    //Up Swipe
                    if (swipeValue > 0 && !superShieldUp && !rechargeSuperShield && superShieldUsable) {
                        //Activate Super Shield
                        StartCoroutine("SuperShieldV2");
                        //Debug.Log("Swiped up");
                    }
                }
            }
        }

        if(rechargeSuperShield && superShieldUsable)
        {
            superShiedSlider.fillAmount += 0.05f * modSuperShieldCooldownMultiplier * Time.deltaTime;
            //Debug.Log("SupershieldRechargeRate" + (0.05f * modSuperShieldCooldownMultiplier).ToString());
            if(superShiedSlider.fillAmount >= 1.0f) {
                rechargeSuperShield = false;
            }
        }

        //Change camera offsetZ
        if (changeCameraOffset) {
            camController.offsetZ = Mathf.Lerp(camController.offsetZ, -6.5f, Time.deltaTime);
        }
        else {
            if (camController.offsetZ < -4.7f) {
                camController.offsetZ = Mathf.Lerp(camController.offsetZ, -4.5f, Time.deltaTime);
            }
        }
    }

    void RegenShield ()
    {
        // If player is not dead, does not have blocked regen nor is not full shield.. Regen
        if (shieldSlider.fillAmount != 1 && shieldRegenActive && shieldActive)//&& shieldSlider.fillAmount != 0
        {
            shieldSlider.fillAmount += (defaultShieldRegen * modShieldRegenMultiplier)/modShieldAmountMultiplier;
        }
    }

    void CheckForDistanceTravelledAchievements()
    {
        if (!travelDoneAchievementsChecked)
        {
            //Add the distance travelled to achievement
            PlayerPrefs.SetFloat("Achievement_DistanceTravelledInTotal", PlayerPrefs.GetFloat("Achievement_DistanceTravelledInTotal") + distanceTraveled);

            //Distance travelled achievement
            if (!PlayerPrefsX.GetBool("Achievement_TravelAchievementsGranted"))
            {
                //Debug.Log(PlayerPrefs.GetFloat("Achievement_DistanceTravelledInTotal") + " and travelled this round " + distanceTraveled);

                if (PlayerPrefs.GetFloat("Achievement_DistanceTravelledInTotal") >= 49999)
                {
#if UNITY_ANDROID
                    GPManager.Travelled50kUnitsTotal();
#endif

#if UNITY_IOS
                    GameCenterManager.SubmitAchievement(100f, "Achievement_50kUnits", true);
#endif
                }

                if (PlayerPrefs.GetFloat("Achievement_DistanceTravelledInTotal") >= 99999)
                {
#if UNITY_ANDROID
                    GPManager.Travelled100kUnitsTotal();
#endif

#if UNITY_IOS
                    GameCenterManager.SubmitAchievement(100f, "Achievement_100kUnits", true);
#endif
                }

                if (PlayerPrefs.GetFloat("Achievement_DistanceTravelledInTotal") >= 499999)
                {
#if UNITY_ANDROID
                    GPManager.Travelled500kUnitsTotal();
#endif

#if UNITY_IOS
                    GameCenterManager.SubmitAchievement(100f, "Achievement_500kUnits", true);
#endif
                }

                if (PlayerPrefs.GetFloat("Achievement_DistanceTravelledInTotal") >= 999999)
                {
#if UNITY_ANDROID
                    GPManager.Travelled1mUnitsTotal();
#endif

#if UNITY_IOS
                    GameCenterManager.SubmitAchievement(100f, "Achievement_1MUnits", true);
#endif
                }

                if (PlayerPrefs.GetFloat("Achievement_DistanceTravelledInTotal") >= 9999999)
                {
#if UNITY_ANDROID
                    GPManager.Travelled10mUnitsTotal();
#endif

#if UNITY_IOS
                    GameCenterManager.SubmitAchievement(100f, "Achievement_10MUnits", true);
#endif
                }

                travelDoneAchievementsChecked = true;
            }
        }
    }

    void BackToNormalSpeed() {
        for (int i = 0; i < speedEffects.Length; i++) {
            speedEffects[i].startSpeed = 50f;
        }

        changeCameraOffset = false;
        forwardMovementSpeed = lastForwardSpeed;
    }

    void Immortalize() {
        immortality = false;
    }

    void ReturnExtraScoreMultiplier() {
        extraScoreMultiplier = 0;
    }

    void OnTriggerEnter(Collider other) {

        if (other.gameObject.tag == "Asteroid" && immortality)
        {
            RaycastHit hit;
            hitPos = other.transform.position;
            //if (Physics.Raycast(transform.position, (other.gameObject.transform.position - transform.position), out hit, layerMask))
            if (Physics.Linecast(transform.position, other.gameObject.transform.position, out hit, layerMask))
            {
                //Debug.Log(hit.transform);
                hitPos = hit.point;
            }

            GameObject go = Instantiate(asteroidDestroyParticle, hitPos, Quaternion.identity) as GameObject;
            go.transform.parent = transform;
            //Sound effect
            audio.pitch = Random.Range(0.8f, 1f);
            audio.PlayOneShot(collisionSound, 0.5f);
            //Visual effect
            camController.impact = 0.1f;
            //Disable the asteroid
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "Asteroid" && !immortality)
        {
            if (ironmanModeOn && !superShieldUp && !rechargeSuperShield && superShieldUsable)
            {
                StartCoroutine("SuperShieldV2");
                RaycastHit hit;
                hitPos = other.transform.position;
                //if (Physics.Raycast(transform.position, (other.gameObject.transform.position - transform.position), out hit, layerMask))
                if (Physics.Linecast(transform.position, other.gameObject.transform.position, out hit, layerMask))
                {
                    //Debug.Log(hit.transform);
                    hitPos = hit.point;
                }

                GameObject go = Instantiate(asteroidDestroyParticle, hitPos, Quaternion.identity) as GameObject;
                go.transform.parent = transform;
                //Sound effect
                audio.pitch = Random.Range(0.8f, 1f);
                audio.PlayOneShot(collisionSound, 0.5f);
                //Visual effect
                camController.impact = 0.1f;
                //Disable the asteroid
                other.gameObject.SetActive(false);
            }

            else if (shieldSlider.fillAmount > asteroidDmg && shieldActive)
            {
                //In Competitive Mode
                //analytics.OnAsteroidHit();

                //Sound effect
                audio.pitch = Random.Range(0.8f, 1f);
                audio.PlayOneShot(collisionSound, 0.5f);
                //Visual effect
                camController.impact = 0.1f;
                gridCamController.impact = 0.1f;

                RaycastHit hit;
                hitPos = other.transform.position;
                //if(Physics.Raycast(transform.position, (other.gameObject.transform.position - transform.position), out hit, layerMask))
                if (Physics.Linecast(transform.position, other.gameObject.transform.position, out hit, layerMask))
                {
                    //Debug.Log(hit.transform);
                    hitPos = hit.point;
                }

                //Instantiate asteroid destroy particle
                GameObject go = Instantiate(asteroidDestroyParticle, hitPos, Quaternion.identity) as GameObject;

                //Debug.LogError("pause");
                //Vector3 closestPoint = other.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                //GameObject go = Instantiate(asteroidDestroyParticle, closestPoint, Quaternion.identity) as GameObject;

                go.transform.parent = transform;

                //When hitting an asteroid take 4th of your shield away
                shieldSlider.fillAmount -= 1f * asteroidDmg;

                //Reduce players current speed
                speedTime -= speedTime * 0.06f;//speedTime / 100*25 as default

                // Start temp shield if not already active
                if (immortality == false)
                    StartCoroutine("AsteroidHitTempShield");

                //Reduce the score multiplier
                scoreMultipler = scoreMultipler * (0.5f * modScoreMultiplierDecrease);
                if (scoreMultipler < 1)
                    scoreMultipler = 1;
                if (sBuffed == true)
                {
                    superShiedSlider.fillAmount = 0;
                    CancelInvoke("UpdatePickupTimerFill");
                }
                scoreBuff = 0f;
                sBuffed = false;

                //Set scoreMultiplier text
                if (scoreMultipler == 1 || scoreMultipler == 2 || scoreMultipler == 3 ||
                   scoreMultipler == 4 || scoreMultipler == 5 || scoreMultipler == 6)
                    scoreMultiplierText.text = "X" + scoreMultipler.ToString("F0");
                else
                    scoreMultiplierText.text = "X" + scoreMultipler.ToString("F1");

                //Achievement
                //Hit 50 asteroids 
#if UNITY_ANDROID
                GPManager.HitAsteroids();
#endif

#if UNITY_IOS
                    GameCenterManager.SubmitAchievement(2f, "Achievement_Hit50Asteroids", true);
                //GameCenterManager.ShowGmaeKitNotification("Hit 50 asteroids!", "");
#endif
                //Hit an asteroid
                if (!PlayerPrefsX.GetBool("HitAsteroid"))
                {
#if UNITY_ANDROID
                    GPManager.HitAsteroid();
#endif

#if UNITY_IOS
                    GameCenterManager.SubmitAchievement(100f, "Achievement_HitAsteroid", true);
                    //GameCenterManager.ShowGmaeKitNotification("Hit An Asteroid", "");
#endif
                }

                //Vibrate device
                if (DataRepo.canVibrate)
                {
                    Handheld.Vibrate();
                }

                other.gameObject.SetActive(false);
            }
            else
            {
                ManagerScript.gameOver = true;
            }
        }

        if (other.gameObject.tag == "DeathWall")
        {
            shieldSlider.fillAmount = 0;
            ManagerScript.gameOver = true;
        }

        if (other.gameObject.tag == "ShieldBoost") {
            audio.PlayOneShot(collectableSound);

            shieldSlider.fillAmount += 1f / 4;
            // new "hit asteroid" effect
            StartCoroutine("ShieldHit");
            StartCoroutine(pickUpSystem.GetComponent<PickUpSpawner>().PickedUp());
        }

        if (other.gameObject.tag == "ScoreMultiplier") {
            audio.PlayOneShot(collectableSound);

            //IF cur scoremultiplier is equal or less than 8, then just add 2 into it
            if (scoreMultipler <= maxMultiplier - scoreBuffAmount)
            {
                scoreMultipler += scoreBuffAmount;
                if (scoreMultipler > 10)
                    scoreMultipler = 10;
            }
            //Else run coroutine where player gets + 2 buff for x seconds
            else if (scoreMultipler > maxMultiplier - scoreBuffAmount)
            {
                StartCoroutine("ScoreBuff");
            }

            //other.gameObject.SetActive(false);
            StartCoroutine(pickUpSystem.GetComponent<PickUpSpawner>().PickedUp());
        }

        if (other.gameObject.tag == "SpeedBoost")
        {
            //Sound effect
            audio.PlayOneShot(collectableSound);
            StartCoroutine("SuperShield");

            //other.gameObject.SetActive(false);
            StartCoroutine(pickUpSystem.GetComponent<PickUpSpawner>().PickedUp());
        }

        // When picking up the Gold pickup, add gold to players dataRepo. Save is called at the end of the run!
        if (other.gameObject.tag == "GoldCurrencyPickUp")
        {
            int trueGoldAmount = (goldPickUpAmount * (int)modGoldCollectMultiplier);

            audio.PlayOneShot(collectableSound);
            savingSystem.goldCurrency += trueGoldAmount;
            manager.goldFromPickUps += trueGoldAmount;
            StartCoroutine(pickUpSystem.GetComponent<PickUpSpawner>().PickedUp());

            // Sort the pickUpText and its fading
            pickUpText.text = "<color=#ffc100ff>+ " + trueGoldAmount + " P</color>";
            pickUpText.CrossFadeAlpha(1, 0, true);
            pickUpText.CrossFadeAlpha(0, 3, true);
        }

        // When picking up the Silver pickup, add silver to players dataRepo. Save is called at the end of the run!
        if (other.gameObject.tag == "SilverCurrencyPickUp")
        {
            int trueSilverAmount = (silverPickUpAmount * (int)modSilverCollectMultiplier);

            audio.PlayOneShot(collectableSound);
            savingSystem.silverCurrency += trueSilverAmount;
            manager.silverFromPickUps += trueSilverAmount;
            StartCoroutine(pickUpSystem.GetComponent<PickUpSpawner>().PickedUp());

            // Sort the pickUpText and its fading
            pickUpText.text = "<color=silver>+ " + trueSilverAmount + " C</color>";
            pickUpText.CrossFadeAlpha(1, 0, true);
            pickUpText.CrossFadeAlpha(0, 3, true);
        }
    }

    IEnumerator ScoreBuff()
    {
        //Convert, Round and Set the score increment
        double trueScoreBuff = 0;
        sBuffed = true;
        trueScoreBuff = scoreMultipler + scoreBuffAmount;
        trueScoreBuff = System.Math.Round(trueScoreBuff, 1);
        scoreBuff = (float)trueScoreBuff; //scoreBuffAmount
        SetAndRunPickupTimer();
        //Wait for pickUp duration to end
        yield return new WaitForSeconds(5);
        if (trueMultiplier >= 10)
            scoreMultipler = 10f;
        if (trueMultiplier < 10)
            scoreMultipler = trueMultiplier;
        scoreBuff = 0f;
        sBuffed = false;
        yield return true;
    }

    void SetAndRunPickupTimer()
    {
        superShiedSlider.fillAmount = 1f;
        InvokeRepeating("UpdatePickupTimerFill", 0f, pickUpFillDeductSmoothness);
    }

    void UpdatePickupTimerFill()
    {
        if (superShiedSlider.fillAmount > 0)
        {
            superShiedSlider.fillAmount -= pickUpFillDeduct;
        }
        else if (superShiedSlider.fillAmount <= 0)
        {
            superShiedSlider.fillAmount = 0;
            CancelInvoke("UpdatePickupTimerFill");
        }
    }

    IEnumerator AsteroidHitTempShield()
    {
        blueLongShield.SetActive(true);
        immortality = true;
        yield return new WaitForSeconds(0.5f);
        blueLongShield.SetActive(false);
        // If player also has superShield boost on, dont disable immortality just yet
        if (superShieldUp == false)
            immortality = false;
    }

    IEnumerator ShieldHit()
    {
        blueShield.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        blueShield.SetActive(false);
    }

    IEnumerator SuperShield()
    {
        //SetAndRunPickupTimer();
        superShiedSlider.fillAmount = 0;
        rechargeSuperShield = true;
        immortality = true;
        superShieldUp = true;

        greenShield.SetActive(true);
        
        yield return new WaitForSeconds((5 * modSuperShieldDurationMultiplier));
        superShieldUp = false;
        greenShield.SetActive(false);
        immortality = false;
        yield return true;
    }

    IEnumerator SuperShieldV2 ()
    {
        //SuperShield enabled, start depleting shield and enable immortality
        InvokeRepeating("DepleteShield", 0, 0.1f);
        immortality = true;
        superShieldUp = true;
        greenShield.SetActive(true);

        // The supershield is depleted, start charging and return vulnerability
        yield return new WaitForSeconds((5 * modSuperShieldDurationMultiplier));
        CancelInvoke("DepleteShield");
        rechargeSuperShield = true;
        immortality = false;
        superShieldUp = false;
        greenShield.SetActive(false);
        yield return true;
    }

    void DepleteShield ()
    {
        if(shieldDepleteRate == 0)
            shieldDepleteRate = (1 / (5 * modSuperShieldDurationMultiplier)) * 0.1f;

        superShiedSlider.fillAmount -= shieldDepleteRate;
    }

    IEnumerator SlowMotion()
    {   //Check if SlowMo is already running, if not; Start SlowMo
        if (slowmoAlreadyRunning == false)
        {
            durationOnPickUp = pickupDuration;
            slowmoAlreadyRunning = true;

            //Pickup variables
            float progress = 0;
            float increment = SpeedChangeSmoothness / SpeedChangeInSecs;
            float slowDown = 1 - slowingAmount;
            timeScale = 1;

            //Vignette variables
            if (vignette == null)
                vignette = GameObject.Find("Main Camera").GetComponent<VignetteAndChromaticAberration>();

            vignette.enabled = true;
            float curVignette;

            //Lerp timeScale down for slowMotion effect
            while (progress < 1)
            {
                timeScale = Mathf.Lerp(1, slowDown, progress);
                progress += increment;

                curVignette = Mathf.Lerp(0, vignetteAmountMax, progress);
                vignette.intensity = curVignette;
                yield return new WaitForSeconds(SpeedChangeSmoothness);
            }

            //Make sure theres no decimal errors
            timeScale = slowDown;
            curVignette = vignetteAmountMax;

            //Wait for pickUp duration to end
            yield return new WaitForSeconds(durationOnPickUp);
            pickUpEnded = true;

            //Lerp timeScale back to 1 for normal game speed
            while (pickUpEnded == true && progress >= 0)
            {
                timeScale = Mathf.Lerp(1, slowDown, progress);
                progress -= increment;

                curVignette = Mathf.Lerp(0, vignetteAmountMax, progress);
                vignette.intensity = curVignette;
                yield return new WaitForSeconds(SpeedChangeSmoothness);
            }

            //Set back to default
            timeScale = 1;
            slowmoAlreadyRunning = false;
            vignette.enabled = false;

            yield return true;
        }
        else
        {
            //If theres slowMo already in action, just add duration
            durationOnPickUp = durationOnPickUp + pickupDuration;
            yield return true;
        }
    }
    void GetUiComponents()
    {
        if (shieldSlider == null)
            shieldSlider = GameObject.Find("ShieldBar").GetComponent<Image>();
        if (currentScoreText == null)
            currentScoreText = GameObject.Find("CurrentScoreText").GetComponent<Text>();
        if (scoreMultiplierText == null)
            scoreMultiplierText = GameObject.Find("ScoreMultiplier").GetComponent<Text>();
        /*if (pickUpTimerFill == null)
            pickUpTimerFill = GameObject.Find("PickUpTimer").GetComponent<Image>();
        pickUpTimerFill.fillAmount = 0f;*/
        if (speedText == null)
            speedText = GameObject.Find("SpeedometerSpeedText").GetComponent<Text>();
        /*if (speedometerFill == null)
            speedometerFill = GameObject.Find("SpeedometerMeterFill").GetComponent<Image>();
        if (distanceTraveledText == null)
            distanceTraveledText = GameObject.Find("TraveledDistanceText").GetComponent<Text>();
        if (timeText== null)
            timeText = GameObject.Find("TimeText").GetComponent<Text>();*/

        if (speedEffects.Length == 0 || speedEffects[0] == null || speedEffects[1] == null)
        {
            speedEffects = new ParticleSystem[2];
            speedEffects[0] = GameObject.Find("SpeedEffectHoriz").GetComponent<ParticleSystem>();
            speedEffects[1] = GameObject.Find("SpeedEffectHoriz (1)").GetComponent<ParticleSystem>();
        }
    }

    void SetModuleAdjustedValues()
    {
        // Get and figure out what stats are being changed here at start
 //       Debug.Log(savingSystem.cachedModules[savingSystem.curModule].name);

        // Set effects into a list and iterate them through
        List<SavingSystemV3.moduleType> effectsList = new List<SavingSystemV3.moduleType>();

        var firstEffect = savingSystem.cachedModules[savingSystem.curModule].modEffectType1;
        effectsList.Add(firstEffect);

        var secondEffect = savingSystem.cachedModules[savingSystem.curModule].modEffectType2;
        effectsList.Add(secondEffect);

        var thirdEffect = savingSystem.cachedModules[savingSystem.curModule].modEffectType3;
        effectsList.Add(thirdEffect);

        var fourthEffect = savingSystem.cachedModules[savingSystem.curModule].modEffectType4;
        effectsList.Add(fourthEffect);

        // Check and set the found effects into action(except bools)
        for (int i = 0; i < effectsList.Count; i++)
        {
            // Set adjusted sideways movement speed, is replacing the default value and using it on update
            if (effectsList[i] == SavingSystemV3.moduleType.strafeSpeed)
            {
                if (i == 0)
                    movementSpeed = movementSpeed * savingSystem.cachedModules[savingSystem.curModule].effect1AmountPercent;
                if (i == 1)
                    movementSpeed = movementSpeed * savingSystem.cachedModules[savingSystem.curModule].effect2AmountPercent;
                if (i == 2)
                    movementSpeed = movementSpeed * savingSystem.cachedModules[savingSystem.curModule].effect3AmountPercent;
                if (i == 3)
                    movementSpeed = movementSpeed * savingSystem.cachedModules[savingSystem.curModule].effect4AmountPercent;

                //Debug.Log("movementSpeed" + movementSpeed);
            }

            // Acceleration = modSpeedMultiplier, is being calculated in update
            if (effectsList[i] == SavingSystemV3.moduleType.acceleration)
            {
                if (i == 0)
                    modSpeedMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect1AmountPercent;
                if (i == 1)
                    modSpeedMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect2AmountPercent;
                if (i == 2)
                    modSpeedMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect3AmountPercent;
                if (i == 3)
                    modSpeedMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect4AmountPercent;

                //Debug.Log("modSpeedMultiplier" + modSpeedMultiplier);
            }

            // Shield Regenerate = modShieldRegenMultiplier
            if (effectsList[i] == SavingSystemV3.moduleType.shieldRegen)
            {
                if (i == 0)
                    modShieldRegenMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect1AmountPercent;
                if (i == 1)
                    modShieldRegenMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect2AmountPercent;
                if (i == 2)
                    modShieldRegenMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect3AmountPercent;
                if (i == 3)
                    modShieldRegenMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect4AmountPercent;

                //Debug.Log("mod regen multiplier" + modShieldRegenMultiplier);
            }

            // Shield Amount = modShieldAmountMultiplier, Käytännössä + shield on sama kuin -dmg asteroideista. Eli se lasketaan täällä
            // ja asetetaan asteroidDmg var:n jotta peli tietää millon oikeasti gameover ja ottaa läpyä
            if (effectsList[i] == SavingSystemV3.moduleType.shieldAmount)
            {
                if (i == 0)
                    asteroidDmg = asteroidDmg / savingSystem.cachedModules[savingSystem.curModule].effect1AmountPercent;
                if (i == 1)
                    asteroidDmg = asteroidDmg / savingSystem.cachedModules[savingSystem.curModule].effect2AmountPercent;
                if (i == 2)
                    asteroidDmg = asteroidDmg / savingSystem.cachedModules[savingSystem.curModule].effect3AmountPercent;
                if (i == 3)
                    asteroidDmg = asteroidDmg / savingSystem.cachedModules[savingSystem.curModule].effect4AmountPercent;

                //Debug.Log("Asteroid dmg" + asteroidDmg);
            }

            // Silver collect rate = modSilverCollectMultiplier // Note that this and the one below work both in the same script, adjust from just one effect
            if (effectsList[i] == SavingSystemV3.moduleType.silverCollectRate)
            {
                if (i == 0)
                    modSilverCollectMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect1AmountPercent;
                if (i == 1)
                    modSilverCollectMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect2AmountPercent;
                if (i == 2)
                    modSilverCollectMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect3AmountPercent;
                if (i == 3)
                    modSilverCollectMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect4AmountPercent;

                //Debug.Log("silverSpawnChance  after silverColRate" + pickUpSystem.GetComponent<PickUpSpawner>().silverSpawnChance);
            }

            // Gold collect rate = modGoldCollectMultiplier // Note that this and the one above work both in the same script, adjust from just one effect
            if (effectsList[i] == SavingSystemV3.moduleType.goldCollectRate)
            {
                if (i == 0)
                    modGoldCollectMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect1AmountPercent;
                if (i == 1)
                    modGoldCollectMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect2AmountPercent;
                if (i == 2)
                    modGoldCollectMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect3AmountPercent;
                if (i == 3)
                    modGoldCollectMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect4AmountPercent;

                //Debug.Log("silverSpawnChance after goldColRate" + pickUpSystem.GetComponent<PickUpSpawner>().silverSpawnChance);
            }

            // Pickup spawn rate = modPickUpSpawnRateMultiplier
            if (effectsList[i] == SavingSystemV3.moduleType.pickUpSpawnRate)
            {
                if (i == 0)
                    pickUpSystem.GetComponent<PickUpSpawner>().timeBetweenPickups = pickUpSystem.GetComponent<PickUpSpawner>().timeBetweenPickups / savingSystem.cachedModules[savingSystem.curModule].effect1AmountPercent;
                if (i == 1)
                    pickUpSystem.GetComponent<PickUpSpawner>().timeBetweenPickups = pickUpSystem.GetComponent<PickUpSpawner>().timeBetweenPickups / savingSystem.cachedModules[savingSystem.curModule].effect2AmountPercent;
                if (i == 2)
                    pickUpSystem.GetComponent<PickUpSpawner>().timeBetweenPickups = pickUpSystem.GetComponent<PickUpSpawner>().timeBetweenPickups / savingSystem.cachedModules[savingSystem.curModule].effect3AmountPercent;
                if (i == 3)
                    pickUpSystem.GetComponent<PickUpSpawner>().timeBetweenPickups = pickUpSystem.GetComponent<PickUpSpawner>().timeBetweenPickups / savingSystem.cachedModules[savingSystem.curModule].effect4AmountPercent;

                //Debug.Log("timeBetweenPickups" + pickUpSystem.GetComponent<PickUpSpawner>().timeBetweenPickups);
            }

            // SuperShield Cooldown = modSuperShieldCooldownMultiplier
            if (effectsList[i] == SavingSystemV3.moduleType.superShieldCooldown)
            {
                if (i == 0)
                    modSuperShieldCooldownMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect1AmountPercent;
                if (i == 1)
                    modSuperShieldCooldownMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect2AmountPercent;
                if (i == 2)
                    modSuperShieldCooldownMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect3AmountPercent;
                if (i == 3)
                    modSuperShieldCooldownMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect4AmountPercent;

                //Debug.Log("modSuperShieldRegenMultiplier" + modSuperShieldCooldownMultiplier);
            }

            // SuperShield duration = modSuperShieldDurationMultiplier
            if (effectsList[i] == SavingSystemV3.moduleType.superShieldDuration)
            {
                if (i == 0)
                    modSuperShieldDurationMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect1AmountPercent;
                if (i == 1)
                    modSuperShieldDurationMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect2AmountPercent;
                if (i == 2)
                    modSuperShieldDurationMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect3AmountPercent;
                if (i == 3)
                    modSuperShieldDurationMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect4AmountPercent;

                shieldDepleteRate = (1 / (5 * modSuperShieldDurationMultiplier)) * 0.1f;
                //Debug.Log("modSuperShieldDurationMultiplier" + modSuperShieldDurationMultiplier);

                // Set the animation on greenshield to be same duration as the supershield is
                greenShield.GetComponent<Animator>().speed = 1 / modSuperShieldDurationMultiplier;
            }

            /*// Asteroid spawn rate = modAsteroidSpawnRateMultiplier
            if (effectsList[i] == SavingSystemV3.moduleType.asteroidSpawnRate)
            {
                SpawnerV2 spawner = GameObject.Find("Spawner").GetComponent<SpawnerV2>();

                if (i == 0)
                    spawner.spawnInterval = spawner.spawnInterval * -savingSystem.cachedModules[savingSystem.curModule].effect1AmountPercent;
                if (i == 1)
                    spawner.spawnInterval = spawner.spawnInterval * -savingSystem.cachedModules[savingSystem.curModule].effect2AmountPercent;
                if (i == 2)
                    spawner.spawnInterval = spawner.spawnInterval * -savingSystem.cachedModules[savingSystem.curModule].effect3AmountPercent;
                if (i == 3)
                    spawner.spawnInterval = spawner.spawnInterval * -savingSystem.cachedModules[savingSystem.curModule].effect4AmountPercent;

                Debug.Log("spawnInterval" + spawner.spawnInterval);
            }*/

            // scoreModifier
            if (effectsList[i] == SavingSystemV3.moduleType.scoreModifier)
            {
                if (i == 0)
                    modScoreMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect1AmountPercent;
                if (i == 1)
                    modScoreMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect2AmountPercent;
                if (i == 2)
                    modScoreMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect3AmountPercent;
                if (i == 3)
                    modScoreMultiplier = savingSystem.cachedModules[savingSystem.curModule].effect4AmountPercent;

                //Debug.Log("modScoreMultiplier" + modScoreMultiplier);
            }

            // scoreClimbRate
            if (effectsList[i] == SavingSystemV3.moduleType.scoreClimbRate)
            {
                if (i == 0)
                    modScoreClimbRate = savingSystem.cachedModules[savingSystem.curModule].effect1AmountPercent;
                if (i == 1)
                    modScoreClimbRate = savingSystem.cachedModules[savingSystem.curModule].effect2AmountPercent;
                if (i == 2)
                    modScoreClimbRate = savingSystem.cachedModules[savingSystem.curModule].effect3AmountPercent;
                if (i == 3)
                    modScoreClimbRate = savingSystem.cachedModules[savingSystem.curModule].effect4AmountPercent;

                //Debug.Log("scoreClimbRate" + modScoreClimbRate);
            }

            // scoreMultiplierDecrease
            if (effectsList[i] == SavingSystemV3.moduleType.scoreMultiplierDecrease)
            {
                if (i == 0)
                    modScoreMultiplierDecrease = savingSystem.cachedModules[savingSystem.curModule].effect1AmountPercent;
                if (i == 1)
                    modScoreMultiplierDecrease = savingSystem.cachedModules[savingSystem.curModule].effect2AmountPercent;
                if (i == 2)
                    modScoreMultiplierDecrease = savingSystem.cachedModules[savingSystem.curModule].effect3AmountPercent;
                if (i == 3)
                    modScoreMultiplierDecrease = savingSystem.cachedModules[savingSystem.curModule].effect4AmountPercent;

                //Debug.Log("modScoreMultiplierDecrease" + modScoreMultiplierDecrease);
            }
        }

        // Check the mod if it enabled/disables super shield or shield regen
        superShieldUsable = savingSystem.cachedModules[savingSystem.curModule].superShieldUsable;
        // If superShield is disabled, set it to 0 and not to regen
        if (!superShieldUsable)
            superShiedSlider.fillAmount = 0;

        shieldRegenActive = savingSystem.cachedModules[savingSystem.curModule].shieldRegenActive;

        shieldActive = savingSystem.cachedModules[savingSystem.curModule].shieldActive;
        //If shield is disabled, set it to 0 and to not regen(regen checks this itself)
        if (!shieldActive)
        {
            shieldSlider.fillAmount = 0;
            ironmanModeOn = true;
        }

        // Check the mod if it disables silver or gold spawning
        pickUpSystem.GetComponent<PickUpSpawner>().spawningSilver = savingSystem.cachedModules[savingSystem.curModule].spawningSilver;
        if (savingSystem.cachedModules[savingSystem.curModule].spawningSilver == false)
            pickUpSystem.GetComponent<PickUpSpawner>().timeBetweenPickups = 5;

        pickUpSystem.GetComponent<PickUpSpawner>().spawningGold = savingSystem.cachedModules[savingSystem.curModule].spawningGold;
    }
}

