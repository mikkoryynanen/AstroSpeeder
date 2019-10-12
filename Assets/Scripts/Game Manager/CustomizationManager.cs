using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class CustomizationManager : MonoBehaviour
{
    public GooglePlayManager GPManager;
    public GooglePlayCloudSave GPCloudSave;
    public RectTransform menuWindows;

    public Text walletSilver;
    public Text walletGold;

    [Header("Customize")]
    public GameObject[] customizePanels;
    public GameObject thrusterColorPanelGo;
    public GameObject[] thrusters;

    [Header("Customize menu")]
    public Text currentSilverCurrency;
    public Text currentGoldCurrency;
    public Text currentGoldCurrency2;
    public Text costOfCurrentItemText;
    public Text firstTotalCostOfCustomization;
    public Text secondTotalCostOfCustomization;
    public Image[] customizeMenuSelectionButtons;
    public Sprite customizeMenuSelectionButtonSelected;
    public Sprite customizeMenuSelectionButtonNotSelected;
    public GameObject topUI;
    public GameObject playerShip;
    public GameObject CustomizeWindowCarage;
    public Button buyButton;
    public Button leftButton;
    public Button rightButton;
    public GameObject moduleTooltip;
    public Text moduleTooltipText;

    public Text helperText;
    public Text typeText;
    public Text midText;

    public Text buyWindowSilverCostText;
    public Text buyWindowGoldCostText;
    int totalCostOfCustomizationInSilver;
    int totalCostOfCustomizationInGold;

    public GameObject BuyWindow;
    public Text BuyWindowHeader;

    public GameObject unlockThrusterWindow;
    public Text unlockThrusterWindowHeader;
    public Text unlockThrusterSilverCostText;
    public Text unlockThrusterGoldCostText;
    public Button unlockBuyButton;
    public Text unlockBuyButtonText;

    public int materialSelected;
    public int thrusterTypeSelected;
    public int thrusterColorSelected;
    public int lightSelected;
    public int moduleSelected;

    public int startingMaterial;
    public int startingThruster;
    public int startingThrusterColor;
    public int startingLight;
    public int startingModule;

    private SampleButton lastMatBtn;
    private SampleButton lastThrusterBtn;
    private SampleButton lastLightBtn;
    private SampleButton lastModuleBtn;
    public HeaderButton lastHeaderButton;
    public SampleButton lastButton;

    public SampleButtonCurrencyButton lastMatPriceButton;
    public SampleButtonCurrencyButton lastThrusterPriceButton;
    public SampleButtonCurrencyButton lastLightPriceButton;
    public SampleButtonCurrencyButton lastModulePriceButton;

    public Sprite selectedButton;
    public Sprite unselectedButton;

    public Sprite topFolderSprite;
    public Sprite midFolderSprite;
    public Sprite bottomFolderSprite;
    public Transform thrusterColorPanel;

    public Color isNotOwnedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color ownedColor = new Color(1f, 1f, 1f, 1f);

    public Color silverCostColor;
    public Color goldCostColor;
    public Color defaultStringColor;

    public List<SampleButton> materialButtons = new List<SampleButton>();
    public List<SampleButton> thrusterButtons = new List<SampleButton>();
    public List<HeaderButton> thrusterHeaderButtons = new List<HeaderButton>();
    public List<SampleButton> lightButtons = new List<SampleButton>();
    public List<SampleButton> moduleButtons = new List<SampleButton>();

    public List<int> thrusterStartIndexes = new List<int>();
    public List<int> thrusterEndIndexes = new List<int>();

    //    public List<SampleButton> usableThrusterColors = new List<SampleButton>();

    public int currentWindow = 0;

    private SavingSystemV3 savingSystem;
    private Renderer playerRenderer;

    public int materialSilverCost;
    public int materialGoldCost;
    public int thrusterSilverCost;
    public int thrusterGoldCost;
    public int lightSilverCost;
    public int lightGoldCost;
    public int moduleSilverCost;
    public int moduleGoldCost;

    private bool isSilverCost;

    // Cost text strings
    public Text costText;
    private string costDescString;
    private string costCurItemString;
    private string costMidString;
    private string costFirstCostString;
    private string costSecondCostString;
    private string costHelperString;

    [Header("Audio Clips")]
    AudioSource audio;
    public AudioClip buttonPressSound;
    public AudioClip shipSelectionSound;
    public AudioClip backSound;


    void Start()
    {
        audio = GetComponent<AudioSource>();

        if (savingSystem == null)
            savingSystem = GameObject.FindWithTag("DataRepo").GetComponent<SavingSystemV3>();

        SetCostsString();

        //Update the wallet
        UpdateWallet();
    }

    public void UpdateWallet()
    {
        currentSilverCurrency.text = "C: " + savingSystem.silverCurrency;
        currentGoldCurrency.text = "P: " + savingSystem.goldCurrency;
        currentGoldCurrency2.text = "P: " + savingSystem.goldCurrency;
        walletSilver.text = "C: " + savingSystem.silverCurrency;
        walletGold.text = "P: " + savingSystem.goldCurrency;
    }

/*    public void CustomizeMenuOpen()
    {
        //Sound effect
 //       audio.PlayOneShot(buttonPressSound);

        topUI.SetActive(false);
        menuWindows.localPosition = new Vector3(-1150f * MainMenuManager.currentMenu, -866, 0);

        savingSystem.PopulateLists();
        SetButtonsReferences();
        UpdateWallet();
        GetThrusterButtonIndex();
        GetStartingSelection();
        SetOpenedThrusterTypesOnTop();
        SetTheOpenedButtonsOnTop();

        ResetBuyingValues();

        // Make sure correct window and arrows are enabled
        currentWindow = 0;
        GetWindow();
    }*/

    public void CustomizeMenuOpen(int windowNmbr)
    {
        //Sound effect
        //       audio.PlayOneShot(buttonPressSound);

        topUI.SetActive(false);
        menuWindows.localPosition = new Vector3(-1150f * MainMenuManager.currentMenu, -866, 0);

        savingSystem.PopulateLists();
        SetButtonsReferences();
        UpdateWallet();
        GetThrusterButtonIndex();
        GetStartingSelection();
        SetOpenedThrusterTypesOnTop();
        SetTheOpenedButtonsOnTop();

        ResetBuyingValues();

        // Make sure correct window and arrows are enabled
        currentWindow = windowNmbr;
        GetWindow();
    }

    public void CustomizeMenuClose()
    {
        //Sound effect
        audio.PlayOneShot(backSound);
        ResetSelection();

        ResetButtonSelections();

        //Reset customization back to paintjob if reEntered
        customizeMenuSelectionButtons[0].sprite = customizeMenuSelectionButtonSelected;
        customizeMenuSelectionButtons[1].sprite = customizeMenuSelectionButtonNotSelected;
        customizeMenuSelectionButtons[2].sprite = customizeMenuSelectionButtonNotSelected;
        customizePanels[0].SetActive(true);
        customizePanels[1].SetActive(false);
        customizePanels[2].SetActive(false);
        thrusterColorPanelGo.SetActive(false);

        topUI.SetActive(true);
        menuWindows.localPosition = new Vector3(-1150f * MainMenuManager.currentMenu, -40, 0);
    }

    void ResetButtonSelections()
    {
        if (lastMatPriceButton != null)
            lastMatPriceButton.CancelSelection();
        if (lastThrusterPriceButton != null)
            lastThrusterPriceButton.CancelSelection();
        if (lastLightPriceButton != null)
            lastLightPriceButton.CancelSelection();
        if (lastModulePriceButton != null)
            lastModulePriceButton.CancelSelection();
    }

    void ResetBuyingValues()
    {
        //To reset cost values if reEntering shop after browsing
        materialSilverCost = 0;
        thrusterSilverCost = 0;
        lightSilverCost = 0;
        moduleSilverCost = 0;
        materialGoldCost = 0;
        thrusterGoldCost = 0;
        lightGoldCost = 0;
        moduleGoldCost = 0;

        /*
        costOfCurrentItemText.text = "Unlocked";
        costOfCurrentItemText.color = defaultStringColor;
        midText.text = "";
        firstTotalCostOfCustomization.text = "";
        secondTotalCostOfCustomization.text = "";
        helperText.text = "";
        */

        costCurItemString = "Unlocked";
        costMidString = "";
        costFirstCostString = "";
        costSecondCostString = "";
        costHelperString = "";

        costText.text = costDescString + " " + costCurItemString + "\n" + costMidString + " " + costFirstCostString + costHelperString + costSecondCostString;

    }

    public void ResetToStartingEquipment()
    {
        //Set starting equipment back
        if (playerRenderer == null)
            playerRenderer = GameObject.Find("PlayerShipMesh").GetComponent<Renderer>();

        playerRenderer.material = savingSystem.cachedMaterials[startingMaterial].material;

        thrusterTypeSelected = startingThruster;
        SetThruster(startingThrusterColor);

        /*      float h = savingSystem.emissionColorsCache[startingLight].HHue;
                float s = savingSystem.emissionColorsCache[startingLight].SSaturation;
                float v = savingSystem.emissionColorsCache[startingLight].VBrightness;
                savingSystem.SetEmission(h, s, v);*/

        playerRenderer.material.SetColor("_EmissionColor", savingSystem.cachedThrusterColors[startingThrusterColor].thrusterColor);

        SetModule(startingModule);
    }

    public void CustomizeMenuLeft()
    {
        currentWindow++;
        GetWindow();
    }

    void GetWindow()
    {
        if (currentWindow == 0)
        {
            leftButton.interactable = true;
            rightButton.interactable = false;

            PaintjobSelection();
        }

        if (currentWindow == 1)
        {
            leftButton.interactable = true;
            rightButton.interactable = true;

            ThrusterSelection();
        }

        if (currentWindow == 2)
        {
            leftButton.interactable = false;
            rightButton.interactable = true;

            LightsSelection();
        }
    }

    // Tap/click to move between menus inside customization menu
    public void ClickCustomizationButton(GameObject go)
    {
        audio.PlayOneShot(buttonPressSound);

        if (go.name == "PaintjobButton")
        {
            currentWindow = 0;
        }

        if (go.name == "ThrustersBUtton")
        {
            currentWindow = 1;
        }

        if (go.name == "LightsButton")
        {
            currentWindow = 2;
        }
        GetWindow();
    }

    public void CustomizeMeuRight()
    {
        currentWindow--;
        GetWindow();
    }

    public void OnBuyButtonPress()
    {
        savingSystem.CheckForSkinsAchievement();

        audio.PlayOneShot(buttonPressSound);
        //If you're purchasing something, then show confirm, else apply selection
        if (totalCostOfCustomizationInGold > 0 || totalCostOfCustomizationInSilver > 0)
        {
            BuyWindow.SetActive(true);

            buyWindowSilverCostText.text = "0C";
            buyWindowGoldCostText.text = "0P";

            if (totalCostOfCustomizationInSilver > 0)
            {
                buyWindowSilverCostText.text = totalCostOfCustomizationInSilver.ToString() + "C";
            }
            if (totalCostOfCustomizationInGold > 0)
            {
                buyWindowGoldCostText.text = totalCostOfCustomizationInGold.ToString() + "P";
            }
        }
        else
        {
            BuyAndApplyCustomization();
        }
        ResetButtonSelections();
    }

    public void BuyAndApplyCustomization()
    {
        BuyWindow.SetActive(false);
        audio.PlayOneShot(buttonPressSound);

        savingSystem.silverCurrency -= totalCostOfCustomizationInSilver;
        savingSystem.goldCurrency -= totalCostOfCustomizationInGold;

        //Add to achievement
        PlayerPrefs.SetFloat("Achievement_SilverSpended", PlayerPrefs.GetFloat("Achievement_SilverSpended") + totalCostOfCustomizationInSilver);

        //Check for Achievements
        if (!PlayerPrefsX.GetBool("Achievement_SilverSpender") && PlayerPrefs.GetFloat("Achievement_SilverSpended") >= 2500)
        {
            GPManager.SilverSpender();
        }

        UpdateWallet();
        ResetSelection();

        //Save customization options
        SaveCustomization();
        //Save to cloud 
        //GPCloudSave.Save();

        savingSystem.CheckForSkinsAchievement();

        //Reset values
        totalCostOfCustomizationInSilver = 0;
        totalCostOfCustomizationInGold = 0;

        moduleTooltip.SetActive(false);
        CustomizeMenuOpen(currentWindow);
    }

    void SaveCustomization()
    {
        // Materials
        savingSystem.SetAndOpenMaterial(materialSelected);
        SetSelectedMat(materialButtons[materialSelected].GetComponent<SampleButton>());
        materialButtons[materialSelected].isOwned = true;

        // Thruster
        savingSystem.SetAndOpenThruster(thrusterTypeSelected, thrusterColorSelected);
        lastThrusterBtn.isOwned = true;

        // Lights
        /*L     savingSystem.SetAndOpenLight(lightSelected);
                SetSelectedLight(lightButtons[lightSelected].GetComponent<SampleButton>());
                lightButtons[lightSelected].isOwned = true;*/

        // Modules
        savingSystem.SetAndOpenModule(moduleSelected);
        SetSelectedModule(moduleButtons[moduleSelected].GetComponent<SampleButton>());
        moduleButtons[savingSystem.cachedModules[moduleSelected].orderInShop].isOwned = true;
        savingSystem.curModule = moduleSelected;

        startingMaterial = materialSelected;
        startingThruster = thrusterTypeSelected;
        startingThrusterColor = thrusterColorSelected;
        startingLight = lightSelected;
        startingModule = moduleSelected;

        savingSystem.SaveData();

        // Resize the button you just bought and hide its costs
            if(lastButton != null)
                lastButton.UpdateButtons();

        // Make sure they all should be getting hidden after purchase
        if (lastMatBtn != null)
            lastMatBtn.UpdateButtons();
        if (lastThrusterBtn != null)
            lastThrusterBtn.UpdateButtons();
        if (lastLightBtn != null)
            lastLightBtn.UpdateButtons();
        if (lastModuleBtn != null)
            lastModuleBtn.UpdateButtons();
    }

    public void UpdateTotalCost()
    {
        //Add up all the costs together
        //        totalCostOfCustomizationInSilver = materialSilverCost + thrusterSilverCost + lightSilverCost;
        //        totalCostOfCustomizationInGold = materialGoldCost + thrusterGoldCost + lightGoldCost;

        totalCostOfCustomizationInSilver = materialSilverCost + thrusterSilverCost + moduleSilverCost;
        totalCostOfCustomizationInGold = materialGoldCost + thrusterGoldCost + moduleGoldCost;

        if (totalCostOfCustomizationInSilver == 0 && totalCostOfCustomizationInGold == 0)
        {
            buyButton.interactable = true;
            buyButton.GetComponentInChildren<Text>().text = "Apply";
        }

        else if (savingSystem.silverCurrency >= totalCostOfCustomizationInSilver && savingSystem.goldCurrency >= totalCostOfCustomizationInGold)
        {
            buyButton.interactable = true;
            buyButton.GetComponentInChildren<Text>().text = "Buy";
        }
        else
        {
            buyButton.interactable = false;
            buyButton.GetComponentInChildren<Text>().text = "Can't afford";
        }
        //UpdateCostsString();
        SetCostsString();
    }

    //Ship material selections
    void PaintjobSelection()
    {
        currentWindow = 0;

        //Sound effect
        audio.PlayOneShot(buttonPressSound);

        UpdateTotalCost();

        customizeMenuSelectionButtons[0].sprite = customizeMenuSelectionButtonSelected;
        customizeMenuSelectionButtons[1].sprite = customizeMenuSelectionButtonNotSelected;
        customizeMenuSelectionButtons[2].sprite = customizeMenuSelectionButtonNotSelected;

        customizePanels[0].SetActive(true);
        customizePanels[1].SetActive(false);
        customizePanels[2].SetActive(false);
        thrusterColorPanelGo.SetActive(false);
        moduleTooltip.SetActive(false);
    }

    void ThrusterSelection()
    {
        //Sound effect
        audio.PlayOneShot(buttonPressSound);

        UpdateTotalCost();

        customizeMenuSelectionButtons[0].sprite = customizeMenuSelectionButtonNotSelected;
        customizeMenuSelectionButtons[1].sprite = customizeMenuSelectionButtonSelected;
        customizeMenuSelectionButtons[2].sprite = customizeMenuSelectionButtonNotSelected;

        customizePanels[0].SetActive(false);
        customizePanels[1].SetActive(true);
        customizePanels[2].SetActive(false);
        thrusterColorPanelGo.SetActive(true);

        //        if (thrusterTypeSelected != 0 || thrusterTypeSelected != 1 || thrusterTypeSelected != 2 || thrusterTypeSelected != 3 || thrusterTypeSelected != 4)
        if (thrusterTypeSelected > 4)
        {
            moduleTooltip.SetActive(true);
            moduleTooltipText.text = "This thruster is unique and is not affected by the custom color";
        }
        else
            moduleTooltip.SetActive(false);

        //Debug.Log("thrusterTypeSelected " + thrusterTypeSelected.ToString());
    }

    void LightsSelection()
    {
        //Sound effect
        audio.PlayOneShot(buttonPressSound);

        UpdateTotalCost();

        customizeMenuSelectionButtons[0].sprite = customizeMenuSelectionButtonNotSelected;
        customizeMenuSelectionButtons[1].sprite = customizeMenuSelectionButtonNotSelected;
        customizeMenuSelectionButtons[2].sprite = customizeMenuSelectionButtonSelected;

        customizePanels[0].SetActive(false);
        customizePanels[1].SetActive(false);
        customizePanels[2].SetActive(true);
        thrusterColorPanelGo.SetActive(false);
        moduleTooltip.SetActive(true);
        moduleTooltipText.text = savingSystem.cachedModules[moduleSelected].description;
    }

    public void SetThruster(int it)
    {
        // Disable thrusters
        for (int i = 0; i < thrusters.Length; i++)
        {
            if (thrusters[i].activeInHierarchy)
            {
                thrusters[i].SetActive(false);
            }
        }
        // Enable the thruster needed
        thrusters[thrusterTypeSelected].SetActive(true);

        if (playerRenderer == null)
            playerRenderer = GameObject.Find("PlayerShipMesh").GetComponent<Renderer>();

        // Check if thruster is NOT unique, then set the color
        if (thrusterTypeSelected <= 4)
        {
            // Use the default color of each particle system if "default"
            if (it == 0)
                thrusters[thrusterTypeSelected].GetComponent<ParticleSystem>().startColor = savingSystem.thrusterDefaultColors[thrusterTypeSelected];
            else
                thrusters[thrusterTypeSelected].GetComponent<ParticleSystem>().startColor = savingSystem.cachedThrusterColors[it].thrusterColor;

            // Also set emissions with same color
            playerRenderer.material.SetColor("_EmissionColor", savingSystem.cachedThrusterColors[it].thrusterColor);
        }

        // If is "unique" thruster, still do set the emissive of the material
        else
        {
            playerRenderer.material.SetColor("_EmissionColor", savingSystem.cachedThrusterColors[thrusterColorSelected].thrusterColor);
        }
    }

    public void SetModule(int id)
    {
        moduleTooltipText.text = savingSystem.cachedModules[id].description;
    }

    public void SetSelectedMat(SampleButton btn)
    {
        if (lastMatBtn != null)
        {
            lastMatBtn.selected = false;
        }

        btn.selected = true;
        lastMatBtn = btn;

        foreach (SampleButton mbtn in materialButtons)
        {
            if (!mbtn.isOwned)
            {
                mbtn.GetComponent<Image>().color = isNotOwnedColor;
                mbtn.CancelSelection();
            }
            if (mbtn.selected)
            {
                mbtn.GetComponent<Image>().sprite = selectedButton;
                mbtn.GetComponent<Image>().color = ownedColor;
                mbtn.ItemSelected();
            }
            else if (!mbtn.selected && mbtn.isOwned)
            {
                mbtn.GetComponent<Image>().sprite = unselectedButton;
                mbtn.GetComponent<Image>().color = ownedColor;
                mbtn.CancelSelection();
            }
        }
        //SetTheOpenedButtonsOnTop();
    }

    public void SetSelectedThruster(SampleButton btn)
    {
        if (lastThrusterBtn != null)
        {
            lastThrusterBtn.selected = false;
            lastThrusterBtn.CancelSelection();
        }

        btn.selected = true;
        lastThrusterBtn = btn;

        foreach (SampleButton mbtn in thrusterButtons)
        {
            if (!mbtn.isOwned)
            {
                mbtn.GetComponent<Image>().color = isNotOwnedColor;
                mbtn.CancelSelection();
            }
            if (mbtn.selected)
            {
                mbtn.GetComponent<Image>().sprite = selectedButton;
                mbtn.GetComponent<Image>().color = ownedColor;
                mbtn.ItemSelected();
            }
            else if (!mbtn.selected && mbtn.isOwned)
            {
                mbtn.GetComponent<Image>().sprite = unselectedButton;
                mbtn.GetComponent<Image>().color = ownedColor;
                mbtn.CancelSelection();
            }
        }
        //SetTheOpenedButtonsOnTop();
    }

    public void SetSelectedLight(SampleButton btn)
    {
        if (lastLightBtn != null)
        {
            lastLightBtn.selected = false;
        }

        btn.selected = true;
        lastLightBtn = btn;

        foreach (SampleButton mbtn in lightButtons)
        {
            if (!mbtn.isOwned)
            {
                mbtn.GetComponent<Image>().color = isNotOwnedColor;
                mbtn.CancelSelection();
            }
            if (mbtn.selected)
            {
                mbtn.GetComponent<Image>().sprite = selectedButton;
                mbtn.GetComponent<Image>().color = ownedColor;
                mbtn.ItemSelected();
            }
            else if (!mbtn.selected && mbtn.isOwned)
            {
                mbtn.GetComponent<Image>().sprite = unselectedButton;
                mbtn.GetComponent<Image>().color = ownedColor;
                mbtn.CancelSelection();
            }
        }
        //SetTheOpenedButtonsOnTop();
    }

    public void SetSelectedModule(SampleButton btn)
    {
        if (lastModuleBtn != null)
        {
            lastModuleBtn.selected = false;
        }

        btn.selected = true;
        lastModuleBtn = btn;

        foreach (SampleButton mbtn in moduleButtons)
        {
            if (!mbtn.isOwned)
            {
                mbtn.GetComponent<Image>().color = isNotOwnedColor;
                mbtn.CancelSelection();
            }
            if (mbtn.selected)
            {
                mbtn.GetComponent<Image>().sprite = selectedButton;
                mbtn.GetComponent<Image>().color = ownedColor;
                mbtn.ItemSelected();
            }
            else if (!mbtn.selected && mbtn.isOwned)
            {
                mbtn.GetComponent<Image>().sprite = unselectedButton;
                mbtn.GetComponent<Image>().color = ownedColor;
                mbtn.CancelSelection();
            }
        }
    }

    public void SetSelectedThrusterHeader(HeaderButton btn)
    {
        if (lastHeaderButton != null)
        {
            lastHeaderButton.selected = false;
        }

        btn.selected = true;
        lastHeaderButton = btn;

        foreach (HeaderButton mbtn in thrusterHeaderButtons)
        {
            if (!mbtn.isOwned)
            {
                mbtn.GetComponent<Image>().color = isNotOwnedColor;
                mbtn.CancelSelection();
            }
            if (mbtn.selected)
            {
                mbtn.GetComponent<Image>().sprite = selectedButton;
                mbtn.GetComponent<Image>().color = ownedColor;
                mbtn.ItemSelected();
            }
            else if (!mbtn.selected && mbtn.isOwned)
            {
                mbtn.GetComponent<Image>().sprite = unselectedButton;
                mbtn.GetComponent<Image>().color = ownedColor;
                mbtn.CancelSelection();
            }
        }
        //SetTheOpenedButtonsOnTop(); // No need to change order for these. 
        SetFoldersOpen();
    }

    public void ClickOnThrusterHeader(int headerReference, HeaderButton btn, bool isSilverButton)
    {
        //If the selected thruster is actually unlocked, then open it up, select default and expand folder
        if (lastHeaderButton != null)
            lastHeaderButton.selected = false;

        isSilverCost = isSilverButton;
        // As this should select already opened thruster, set thruster costs to 0
        thrusterSilverCost = 0;
        thrusterGoldCost = 0;

        thrusterTypeSelected = headerReference;
        lastHeaderButton = btn;

        // If the thruster type is already unlocked
        if (savingSystem.cachedThrusters[headerReference].thrustersByColor[0].isOwned == true)
        {
            SetSelectedThrusterHeader(btn);
            unlockThrusterWindow.SetActive(false);
        }

        // if the thruster type is still locked
        else
        {
            SetSelectedThrusterHeader(btn);
            unlockThrusterWindow.SetActive(true);
            unlockThrusterWindowHeader.text = "Confirm unlock : " + savingSystem.cachedThrusters[headerReference].name + "?";

            // Silver costs
            if (isSilverButton)
            {
                if (savingSystem.cachedThrusters[headerReference].silverCost != 0)
                {
                    unlockBuyButtonText.text = savingSystem.cachedThrusters[headerReference].silverCost + "C";
                    unlockBuyButtonText.color = silverCostColor;
                }

                // If cant afford in silver
                if (savingSystem.silverCurrency < savingSystem.cachedThrusters[headerReference].silverCost)
                {
                    unlockBuyButton.interactable = false;
                    //unlockBuyButtonText.text = "Can't afford";
                }

                // If can afford
                else
                {
                    unlockBuyButton.interactable = true;
                    //unlockBuyButtonText.text = "Confirm";
                }
            }

            // Gold costs
            if (!isSilverButton)
            {
                if (savingSystem.cachedThrusters[headerReference].goldCost != 0)
                {
                    unlockBuyButtonText.text = savingSystem.cachedThrusters[headerReference].goldCost + "P";
                    unlockBuyButtonText.color = goldCostColor;
                }

                // If cant afford in silver
                if (savingSystem.goldCurrency < savingSystem.cachedThrusters[headerReference].goldCost)
                {
                    unlockBuyButton.interactable = false;
                    //unlockBuyButtonText.text = "Can't afford";
                }

                // If can afford
                else
                {
                    unlockBuyButton.interactable = true;
                    //unlockBuyButtonText.text = "Confirm";
                }
            }

            // If the header button pressed is unique, show tooltip with notification info
            if (headerReference > 4)
            {
                moduleTooltip.SetActive(true);
                moduleTooltipText.text = "This thruster is unique and is not affected by the custom color";
            }
            // if the button is "generic" one with several colorings, hide info
            else if (headerReference <= 4)
            {
                moduleTooltip.SetActive(false);
            }
        }

        // Check if the thruster type has enough colors to choose from
        if (savingSystem.cachedThrusters[headerReference].thrustersByColor.Count >= thrusterColorSelected)
        {
            // If selected color is also open in this thruster type, set it
            if (thrusterButtons[thrusterColorSelected].isOwned)
            {
                SetSelectedThruster(thrusterButtons[thrusterColorSelected]);
                SetThruster(thrusterButtons[thrusterColorSelected].thrusterColorReference);
                thrusterColorSelected = thrusterButtons[thrusterColorSelected].GetComponent<SampleButton>().thrusterColorReference;
            }

            // If the chosen one isnt open in this thruster type, then set default
            else
            {
                /*
                SetSelectedThruster(thrusterButtons[0]);
                SetThruster(thrusterButtons[0].thrusterColorReference);
                thrusterColorSelected = thrusterButtons[0].GetComponent<SampleButton>().thrusterColorReference;
                */

                SetSelectedThruster(thrusterButtons[thrusterColorSelected]);
                SetThruster(thrusterButtons[thrusterColorSelected].thrusterColorReference);
                thrusterColorSelected = thrusterButtons[thrusterColorSelected].GetComponent<SampleButton>().thrusterColorReference;

                lastThrusterBtn.GetComponent<Button>().onClick.Invoke();
      //          lastThrusterBtn.checkIfPurchaseableWithSilver();
/*                if(lastThrusterBtn.transform.childCount > 2)
                {
                    GameObject silverBtn = lastThrusterBtn.transform.GetChild(2).gameObject;
                    lastThrusterBtn.GetComponent<SampleButton>().checkIfPurchaseableWithSilver();
                    //lastThrusterBtn.GetComponent<SampleButton>().OnSilverGoldClickEvent(true);
                    silverBtn.GetComponent<SampleButtonCurrencyButton>().SetPriceButtonSelected();
                    Debug.Log(lastThrusterBtn.transform.GetChild(2).name);
                }*/
            }
        }

        // If isnt, select the default one
        else
        //if (!usableThrusterColors[thrusterColorSelected].isOwned)
        {
            SetSelectedThruster(thrusterButtons[thrusterColorSelected]);
            SetThruster(thrusterButtons[0].thrusterColorReference);
            thrusterColorSelected = thrusterButtons[thrusterColorSelected].GetComponent<SampleButton>().thrusterColorReference;
        }
        UpdateTotalCost();
    }

    public void UnlockThruster()
    {
        if (isSilverCost)
            savingSystem.silverCurrency -= savingSystem.cachedThrusters[thrusterTypeSelected].silverCost;
        else if (!isSilverCost)
            savingSystem.goldCurrency -= savingSystem.cachedThrusters[thrusterTypeSelected].goldCost;

        lastThrusterPriceButton.CancelSelection();
        audio.PlayOneShot(buttonPressSound);

        //Add to achievement
        PlayerPrefs.SetFloat("Achievement_SilverSpended", PlayerPrefs.GetFloat("Achievement_SilverSpended") + totalCostOfCustomizationInSilver);
        //Check for Achievements
        if (!PlayerPrefsX.GetBool("Achievement_SilverSpender") && PlayerPrefs.GetFloat("Achievement_SilverSpended") >= 2500)
            GPManager.SilverSpender();

        ClickOnThrusterHeader(thrusterTypeSelected, lastHeaderButton, true);
        UpdateWallet();

        lastHeaderButton.GetComponent<HeaderButton>().isOwned = true;

        savingSystem.UnlockThrusterType(thrusterTypeSelected);
        savingSystem.SaveData();

        unlockThrusterWindow.SetActive(false);
        //thrusterButtons[thrusterStartIndexes[thrusterTypeSelected]].isOwned = true; this only needed if theres thruster unique colors
        SetSelectedThrusterHeader(lastHeaderButton);

        if (lastHeaderButton != null)
            lastHeaderButton.UpdateButtons();

        SetOpenedThrusterTypesOnTop();
    }


    public void CancelThrusterUnlock()
    {
        thrusterTypeSelected = startingThruster;
        SetSelectedThrusterHeader(thrusterHeaderButtons[thrusterTypeSelected]);
        SetThruster(startingThrusterColor);

        // Cancel the selection effect aswell
        if (lastThrusterPriceButton != null)
            lastThrusterPriceButton.CancelSelection();

        //thrusterTypeSelected = lastHeaderButton.GetComponent<HeaderButton>().headerReference; //lastHeaderButton gets set
        //SetThruster(lastButton.GetComponent<SampleButton>().thrusterColorReference);

        unlockThrusterWindow.SetActive(false);
    }

    void ResetSelection()
    {
        if (lastMatBtn != null)
            lastMatBtn.CancelSelection();
        if (lastThrusterBtn != null)
            lastThrusterBtn.CancelSelection();
        if (lastLightBtn != null)
            lastLightBtn.CancelSelection();
        if (lastModuleBtn != null)
            lastModuleBtn.CancelSelection();
    }

    void SetButtonsReferences()
    {
        //Make a lists of material buttons
        List<GameObject> tempMatGoList = savingSystem.materialButtons;
        materialButtons = new List<SampleButton>();

        foreach (GameObject go in tempMatGoList)
            materialButtons.Add(go.GetComponent<SampleButton>());

        // Make list of thruster header buttons
        List<GameObject> tempThrusterHeaderList = savingSystem.thrusterHeaderButtons;
        thrusterHeaderButtons = new List<HeaderButton>();

        foreach (GameObject go in tempThrusterHeaderList)
            thrusterHeaderButtons.Add(go.GetComponent<HeaderButton>());

        // Make list of thruster buttons
        List<GameObject> tempThrusterList = savingSystem.thrusterButtons;
        thrusterButtons = new List<SampleButton>();

        foreach (GameObject go in tempThrusterList)
            thrusterButtons.Add(go.GetComponent<SampleButton>());

        /*L        // Make list of light buttons
                List<GameObject> tempLightList = savingSystem.lightButtons;
                lightButtons = new List<SampleButton>();

                foreach (GameObject go in tempLightList)
                    lightButtons.Add(go.GetComponent<SampleButton>());*/

        // Make list of module buttons
        List<GameObject> tempModuleList = savingSystem.moduleButtons;
        moduleButtons = new List<SampleButton>();

        foreach (GameObject go in tempModuleList)
            moduleButtons.Add(go.GetComponent<SampleButton>());
    }

    void GetStartingSelection()
    {
        //Set the current equipment to appear as "Selected" in the customization
        startingMaterial = savingSystem.curSkin;
        startingThruster = savingSystem.curThrusters;
        startingThrusterColor = savingSystem.curThrusterColor;
        startingLight = savingSystem.curEmission;
        startingModule = savingSystem.curModule;

        //Set as selectedLight to avoid nullRef
        materialSelected = startingMaterial;
        lightSelected = startingLight;
        thrusterTypeSelected = startingThruster;
        thrusterColorSelected = startingThrusterColor;
        moduleSelected = startingModule;

        SetSelectedMat(materialButtons[startingMaterial]);
        SetSelectedThrusterHeader(thrusterHeaderButtons[startingThruster]);
        //      SetSelectedThruster(thrusterButtons[thrusterStartIndexes[thrusterTypeSelected] + thrusterColorSelected]);
        SetSelectedThruster(thrusterButtons[startingThrusterColor]);
        //L     SetSelectedLight(lightButtons[startingLight]);
        SetSelectedModule(moduleButtons[savingSystem.cachedModules[startingModule].orderInShop]);
        moduleTooltipText.text = savingSystem.cachedModules[startingModule].description;
    }

    void SetOpenedThrusterTypesOnTop()
    {
        // Thruster types
        foreach (HeaderButton btn in thrusterHeaderButtons)
        {
            if (btn.isOwned)
                btn.gameObject.transform.SetAsFirstSibling();
        }

        foreach (HeaderButton btn in thrusterHeaderButtons)
        {
            if (btn.selected)
                btn.gameObject.transform.SetAsFirstSibling();
        }
    }

    void SetTheOpenedButtonsOnTop()
    {
        // Material Buttons, get all opened on top
        foreach (SampleButton btn in materialButtons)
        {
            if (btn.isOwned)
            {
                btn.gameObject.transform.SetAsFirstSibling();
            }
        }
        // After that, set the selected one to be first one on top
        foreach (SampleButton btn in materialButtons)
        {
            if (btn.selected)
            {
                btn.gameObject.transform.SetAsFirstSibling();
            }
        }

        // Thrusters Colors
        /*
        for (int i = thrusterStartIndexes[thrusterTypeSelected]; i < thrusterEndIndexes[thrusterTypeSelected] + 1; i++)
        {
            if (thrusterButtons[i].GetComponent<SampleButton>().isOwned)
            {
                thrusterButtons[i].transform.SetSiblingIndex(thrusterHeaderButtons[thrusterTypeSelected].gameObject.transform.GetSiblingIndex());
            }
        }

        for (int i = thrusterStartIndexes[thrusterTypeSelected]; i < thrusterEndIndexes[thrusterTypeSelected] + 1; i++)
        {
            if (thrusterButtons[i].GetComponent<SampleButton>().selected)
            {
                thrusterButtons[i].transform.SetSiblingIndex(thrusterHeaderButtons[thrusterTypeSelected].gameObject.transform.GetSiblingIndex());
            }
        }*/

        for (int i = 0; i < thrusterButtons.Count; i++)
        {
            if (thrusterButtons[i].GetComponent<SampleButton>().isOwned)
            {
                thrusterButtons[i].transform.SetSiblingIndex(thrusterHeaderButtons[thrusterTypeSelected].gameObject.transform.GetSiblingIndex());
            }
        }
        for (int i = 0; i < thrusterButtons.Count; i++)
        {
            if (thrusterButtons[i].GetComponent<SampleButton>().selected)
            {
                thrusterButtons[i].transform.SetSiblingIndex(thrusterHeaderButtons[thrusterTypeSelected].gameObject.transform.GetSiblingIndex());
            }
        }

        // Emissive Lights
        foreach (SampleButton btn in lightButtons)
        {
            if (btn.isOwned)
            {
                btn.gameObject.transform.SetAsFirstSibling();
            }
        }

        foreach (SampleButton btn in lightButtons)
        {
            if (btn.selected)
            {
                btn.gameObject.transform.SetAsFirstSibling();
            }
        }

        // Modules
        foreach (SampleButton btn in moduleButtons)
        {
            if (btn.isOwned)
            {
                btn.gameObject.transform.SetAsFirstSibling();
            }
        }
        foreach (SampleButton btn in moduleButtons)
        {
            if (btn.selected)
            {
                btn.gameObject.transform.SetAsFirstSibling();
            }
        }
        //CheckAndSetButtonFolders(); Disabled as moved to own panel
    }

    void SetFoldersOpen()
    {/*
        usableThrusterColors = new List<SampleButton>();
        //foreach (HeaderButton btn in thrusterHeaderButtons)
        for (int curHeaderIndex = 0; curHeaderIndex < thrusterHeaderButtons.Count; curHeaderIndex++)
        {
            if (thrusterHeaderButtons[curHeaderIndex].selected == true)
            {
                // Enable the children, for small, enable 22 first ones in thrusterButtons, for long, 23-44 etc.
                for (int i = thrusterStartIndexes[curHeaderIndex]; i <= thrusterEndIndexes[curHeaderIndex]; i++)
                {
                    thrusterButtons[i].gameObject.SetActive(true);
                    usableThrusterColors.Add(thrusterButtons[i]);
                }
            }

            else
            {
                // Disable the other children
                for (int i = thrusterStartIndexes[curHeaderIndex]; i <= thrusterEndIndexes[curHeaderIndex]; i++)
                {
                    //Debug.Log(savingSystem.cachedThrusters[i].thrustersByColor.Count + "disabling children of " + curHeaderIndex);
                    thrusterButtons[i].gameObject.SetActive(false);
                }
            }
        }*/
     //    SetTheOpenedButtonsOnTop();
     //CheckAndSetButtonFolders(); Disabled as moved to own panel
    }

    void GetThrusterButtonIndex()
    {
        // Get the FIRST and LAST indexes for each thruster type to simplify other scripts
        int lastIndex = 0;

        for (int i = 0; i < savingSystem.cachedThrusters.Count; i++)
        {
            int firstIndexOfThruster = lastIndex;
            lastIndex = lastIndex + savingSystem.cachedThrusters[i].thrustersByColor.Count;
            thrusterStartIndexes.Add(firstIndexOfThruster);
            thrusterEndIndexes.Add(lastIndex - 1);
        }
    }

    void SetCostsString()
    {
        //  When in "PaintJob" customization
        if (currentWindow == 0)
        {
            costCurItemString = materialSilverCost.ToString();
            costDescString = "Current Paint:";

            // If the cost of current item is 0c and 0p. Its Opened
            if (materialSilverCost == 0 && materialGoldCost == 0)
            {
                costCurItemString = "Unlocked";
                //costOfCurrentItemText.color = defaultStringColor;
            }

            // If current Item does cost, set the cost texts
            if (materialSilverCost != 0 || materialGoldCost != 0)
            {
                if (materialSilverCost != 0)
                {
                    costCurItemString = "<color=silver>" + materialSilverCost + "C </color>";
                    //costOfCurrentItemText.color = silverCostColor;
                }
                if (materialGoldCost != 0)
                {
                    costCurItemString = "<color=#ffc100ff>" + materialGoldCost + "P </color>";
                    //costOfCurrentItemText.color = goldCostColor;
                }
            }

            // If there is no "Total cost" needed, hide total cost texts
            if ((totalCostOfCustomizationInSilver == 0 && totalCostOfCustomizationInGold == 0) || materialSilverCost == totalCostOfCustomizationInSilver)
            {
                costMidString = "";
                costFirstCostString = "";
                costSecondCostString = "";
                costHelperString = "";
            }

            // If total cost is needed, then set the texts
            if ((materialSilverCost != totalCostOfCustomizationInSilver) || (materialGoldCost != totalCostOfCustomizationInGold))
            {
                costMidString = "total:";

                // If does cost silver but not gold, hide second cost texts
                if (totalCostOfCustomizationInSilver != 0 && totalCostOfCustomizationInGold == 0)
                {
                    costFirstCostString = "<color=silver>" + totalCostOfCustomizationInSilver + "C </color>";
                    //firstTotalCostOfCustomization.color = silverCostColor;

                    costHelperString = "";
                    costSecondCostString = "";
                }

                // If does cost gold but not silver, hide second cost texts
                if (totalCostOfCustomizationInSilver == 0 && totalCostOfCustomizationInGold != 0)
                {
                    costFirstCostString = "<color=#ffc100ff>" + totalCostOfCustomizationInGold + "P </color>";
                    //firstTotalCostOfCustomization.color = goldCostColor;

                    costHelperString = "";
                    costSecondCostString = "";
                }

                //If does cost both currencies, set the texts for both costs
                else if (totalCostOfCustomizationInSilver != 0 && totalCostOfCustomizationInGold != 0)
                {
                    costFirstCostString = "<color=silver>" + totalCostOfCustomizationInSilver + "C </color>";
                    //firstTotalCostOfCustomization.color = silverCostColor;

                    costHelperString = "& ";

                    costSecondCostString = "<color=#ffc100ff>" + totalCostOfCustomizationInGold + "P </color>";
                    //secondTotalCostOfCustomization.color = goldCostColor;
                }
            }
        }

        //  When in "Thrusters" customization
        if (currentWindow == 1)
        {
            costCurItemString = thrusterSilverCost.ToString();
            costDescString = "Current Thruster:";

            // If the cost of current item is 0c and 0p. Its Opened
            if (thrusterSilverCost == 0 && thrusterGoldCost == 0)
            {
                if(lastHeaderButton.isOwned)
                    costCurItemString = "Unlocked";
                else
                    costCurItemString = "Locked";
                //costOfCurrentItemText.color = defaultStringColor;
            }

            // If current Item does cost, set the cost texts
            if (thrusterSilverCost != 0 || thrusterGoldCost != 0)
            {
                if (thrusterSilverCost != 0)
                {
                    costCurItemString = "<color=silver>" + thrusterSilverCost + "C </color>";
                    //costOfCurrentItemText.color = silverCostColor;
                }
                if (thrusterGoldCost != 0)
                {
                    costCurItemString = "<color=#ffc100ff>" + thrusterGoldCost + "P </color>";
                    //costOfCurrentItemText.color = goldCostColor;
                }
            }

            // If there is no "Total cost" needed, hide total cost texts
            if ((totalCostOfCustomizationInSilver == 0 && totalCostOfCustomizationInGold == 0) || thrusterSilverCost == totalCostOfCustomizationInSilver)
            {
                costMidString = "";
                costFirstCostString = "";
                costSecondCostString = "";
                costHelperString = "";
            }

            // If total cost is needed, then set the texts
            if ((thrusterSilverCost != totalCostOfCustomizationInSilver) || (thrusterGoldCost != totalCostOfCustomizationInGold))
            {
                costMidString = "total:";

                // If does cost silver but not gold, hide second cost texts
                if (totalCostOfCustomizationInSilver != 0 && totalCostOfCustomizationInGold == 0)
                {
                    costFirstCostString = "<color=silver>" + totalCostOfCustomizationInSilver + "C </color>";
                    //firstTotalCostOfCustomization.color = silverCostColor;

                    costHelperString = "";
                    costSecondCostString = "";
                }

                // If does cost gold but not silver, hide second cost texts
                if (totalCostOfCustomizationInSilver == 0 && totalCostOfCustomizationInGold != 0)
                {
                    costFirstCostString = "<color=#ffc100ff>" + totalCostOfCustomizationInGold + "P </color>";
                    //firstTotalCostOfCustomization.color = goldCostColor;

                    costHelperString = "";
                    costSecondCostString = "";
                }

                //If does cost both currencies, set the texts for both costs
                else if (totalCostOfCustomizationInSilver != 0 && totalCostOfCustomizationInGold != 0)
                {
                    costFirstCostString = "<color=silver>" + totalCostOfCustomizationInSilver + "C </color>";
                    //firstTotalCostOfCustomization.color = silverCostColor;

                    costHelperString = "& ";

                    costSecondCostString = "<color=#ffc100ff>" + totalCostOfCustomizationInGold + "P </color>";
                    //secondTotalCostOfCustomization.color = goldCostColor;
                }
            }
        }

        //  When in "Modules" customization (the light one can be found as commented on bottom of the script)
        if (currentWindow == 2)
        {
            costCurItemString = moduleSilverCost.ToString();
            costDescString = "Current Module:";

            // If the cost of current item is 0c and 0p. Its Opened
            if (moduleSilverCost == 0 && moduleGoldCost == 0)
            {
                costCurItemString = "Unlocked";
                //costOfCurrentItemText.color = defaultStringColor;
            }

            // If current Item does cost, set the cost texts
            if (moduleSilverCost != 0 || moduleGoldCost != 0)
            {
                if (moduleSilverCost != 0)
                {
                    costCurItemString = "<color=silver>" + moduleSilverCost + "C </color>";
                    //costOfCurrentItemText.color = silverCostColor;
                }
                if (moduleGoldCost != 0)
                {
                    costCurItemString = "<color=#ffc100ff>" + moduleGoldCost + "P </color>";
                    //costOfCurrentItemText.color = goldCostColor;
                }
            }

            // If there is no "Total cost" needed, hide total cost texts
            if ((totalCostOfCustomizationInSilver == 0 && totalCostOfCustomizationInGold == 0) || moduleSilverCost == totalCostOfCustomizationInSilver)
            {
                costMidString = "";
                costFirstCostString = "";
                costSecondCostString = "";
                costHelperString = "";
            }

            // If total cost is needed, then set the texts
            if ((moduleSilverCost != totalCostOfCustomizationInSilver) || (moduleGoldCost != totalCostOfCustomizationInGold))
            {
                costMidString = "total:";

                // If does cost silver but not gold, hide second cost texts
                if (totalCostOfCustomizationInSilver != 0 && totalCostOfCustomizationInGold == 0)
                {
                    costFirstCostString = "<color=silver>" + totalCostOfCustomizationInSilver + "C </color>";
                    //firstTotalCostOfCustomization.color = silverCostColor;

                    costHelperString = "";
                    costSecondCostString = "";
                }

                // If does cost gold but not silver, hide second cost texts
                if (totalCostOfCustomizationInSilver == 0 && totalCostOfCustomizationInGold != 0)
                {
                    costFirstCostString = "<color=#ffc100ff>" + totalCostOfCustomizationInGold + "P </color>";
                    //firstTotalCostOfCustomization.color = goldCostColor;

                    costHelperString = "";
                    costSecondCostString = "";
                }

                //If does cost both currencies, set the texts for both costs
                else if (totalCostOfCustomizationInSilver != 0 && totalCostOfCustomizationInGold != 0)
                {
                    costFirstCostString = "<color=silver>" +  totalCostOfCustomizationInSilver + "C </color>";
                    //firstTotalCostOfCustomization.color = silverCostColor;

                    costHelperString = "& ";

                    costSecondCostString = "<color=#ffc100ff>" + totalCostOfCustomizationInGold + "P </color>";
                    //secondTotalCostOfCustomization.color = goldCostColor;
                }
            }
        }

        //costText.text = costDescString + " " + costCurItemString + "\n" + costMidString + " " + costFirstCostString + costHelperString + costSecondCostString;
        costText.text = costDescString + " " + costCurItemString + "\n" + costMidString + " " + costFirstCostString + costHelperString + costSecondCostString;
    }
    /* Color hex values
    gold = #ffc100ff
    */
    public void GiveCashForDebugging()
    {
        savingSystem.silverCurrency += 1000;
        savingSystem.goldCurrency += 1000;
        Debug.Log("Giving cash");
    }
    /*
    public void SetPriceButtonSelected (SampleButtonCurrencyButton curPriceButton)
    {
        lastPriceButton.CancelSelection();
        curPriceButton.ItemSelected();

        lastPriceButton = curPriceButton;
    }*/

    /*  Works, disabled as system changed to be setting buttons in other panel
    void CheckAndSetButtonFolders()
    {
        // Get the first index of cur thrusterType + curHeader + other possible headers thatll come
        int firstButtonIndex = thrusterStartIndexes[thrusterTypeSelected] + 1 + thrusterTypeSelected;

        //Image topButtonImage = thrusterButtons[firstButtonIndex].GetComponent<Image>();
        Image topButtonImage = thrusterContentpanel.GetChild(firstButtonIndex).GetComponent<Image>();
        topButtonImage.sprite = topFolderSprite;

        // Get the bottom button and set its sprite
        //int lastButtonIndex = (thrusterHeaderButtons[thrusterTypeSelected + 1].transform.GetSiblingIndex()) - 1;
        int lastButtonIndex = thrusterEndIndexes[thrusterTypeSelected] + 1 + thrusterTypeSelected;

        //Image bottomButtonImage = thrusterButtons[lastButtonIndex].GetComponent<Image>();
        Image bottomButtonImage = thrusterContentpanel.GetChild(lastButtonIndex).GetComponent<Image>();
        bottomButtonImage.sprite = bottomFolderSprite;

        // Get the buttons between top and bottom index, and loop setting sprites on them
        for (int i = firstButtonIndex + 1; i < lastButtonIndex; i++)
        {
            Image midButtonImage = thrusterContentpanel.GetChild(i).GetComponent<Image>();
            midButtonImage.sprite = midFolderSprite;
        }
    }*/

    /*
         void SetCostsString()
    {
        //  When in "PaintJob" customization
        if (currentWindow == 0)
        {
            costOfCurrentItemText.text = materialSilverCost.ToString();
            typeText.text = "Current Paint:";

            // If the cost of current item is 0c and 0p. Its Opened
            if (materialSilverCost == 0 && materialGoldCost == 0)
            {
                costOfCurrentItemText.text = "Unlocked";
                costOfCurrentItemText.color = defaultStringColor;
            }

            // If current Item does cost, set the cost texts
            if (materialSilverCost != 0 || materialGoldCost != 0)
            {
                if (materialSilverCost != 0)
                {
                    costOfCurrentItemText.text = materialSilverCost + "C";
                    costOfCurrentItemText.color = silverCostColor;
                }
                if (materialGoldCost != 0)
                {
                    costOfCurrentItemText.text = materialGoldCost + "P";
                    costOfCurrentItemText.color = goldCostColor;
                }
            }

            // If there is no "Total cost" needed, hide total cost texts
            if ((totalCostOfCustomizationInSilver == 0 && totalCostOfCustomizationInGold == 0) || materialSilverCost == totalCostOfCustomizationInSilver)
            {
                midText.text = "";
                firstTotalCostOfCustomization.text = "";
                secondTotalCostOfCustomization.text = "";
                helperText.text = "";
            }

            // If total cost is needed, then set the texts
            if ((materialSilverCost != totalCostOfCustomizationInSilver) || (materialGoldCost != totalCostOfCustomizationInGold))
            {
                midText.text = "total:";

                // If does cost silver but not gold, hide second cost texts
                if (totalCostOfCustomizationInSilver != 0 && totalCostOfCustomizationInGold == 0)
                {
                    firstTotalCostOfCustomization.text = totalCostOfCustomizationInSilver + "C";
                    firstTotalCostOfCustomization.color = silverCostColor;

                    helperText.text = "";
                    secondTotalCostOfCustomization.text = "";
                }

                // If does cost gold but not silver, hide second cost texts
                if (totalCostOfCustomizationInSilver == 0 && totalCostOfCustomizationInGold != 0)
                {
                    firstTotalCostOfCustomization.text = totalCostOfCustomizationInGold + "P";
                    firstTotalCostOfCustomization.color = goldCostColor;

                    helperText.text = "";
                    secondTotalCostOfCustomization.text = "";
                }

                //If does cost both currencies, set the texts for both costs
                else if (totalCostOfCustomizationInSilver != 0 && totalCostOfCustomizationInGold != 0)
                {
                    firstTotalCostOfCustomization.text = totalCostOfCustomizationInSilver + "C";
                    firstTotalCostOfCustomization.color = silverCostColor;

                    helperText.text = "&";

                    secondTotalCostOfCustomization.text = totalCostOfCustomizationInGold + "P";
                    secondTotalCostOfCustomization.color = goldCostColor;
                }
            }
        }

        //  When in "Thrusters" customization
        if (currentWindow == 1)
        {
            costOfCurrentItemText.text = thrusterSilverCost.ToString();
            typeText.text = "Current Thruster:";

            // If the cost of current item is 0c and 0p. Its Opened
            if (thrusterSilverCost == 0 && thrusterGoldCost == 0)
            {
                costOfCurrentItemText.text = "Unlocked";
                costOfCurrentItemText.color = defaultStringColor;
            }

            // If current Item does cost, set the cost texts
            if (thrusterSilverCost != 0 || thrusterGoldCost != 0)
            {
                if (thrusterSilverCost != 0)
                {
                    costOfCurrentItemText.text = thrusterSilverCost + "C";
                    costOfCurrentItemText.color = silverCostColor;
                }
                if (thrusterGoldCost != 0)
                {
                    costOfCurrentItemText.text = thrusterGoldCost + "P";
                    costOfCurrentItemText.color = goldCostColor;
                }
            }

            // If there is no "Total cost" needed, hide total cost texts
            if ((totalCostOfCustomizationInSilver == 0 && totalCostOfCustomizationInGold == 0) || thrusterSilverCost == totalCostOfCustomizationInSilver)
            {
                midText.text = "";
                firstTotalCostOfCustomization.text = "";
                secondTotalCostOfCustomization.text = "";
                helperText.text = "";
            }

            // If total cost is needed, then set the texts
            if ((thrusterSilverCost != totalCostOfCustomizationInSilver) || (thrusterGoldCost != totalCostOfCustomizationInGold))
            {
                midText.text = "total:";

                // If does cost silver but not gold, hide second cost texts
                if (totalCostOfCustomizationInSilver != 0 && totalCostOfCustomizationInGold == 0)
                {
                    firstTotalCostOfCustomization.text = totalCostOfCustomizationInSilver + "C";
                    firstTotalCostOfCustomization.color = silverCostColor;

                    helperText.text = "";
                    secondTotalCostOfCustomization.text = "";
                }

                // If does cost gold but not silver, hide second cost texts
                if (totalCostOfCustomizationInSilver == 0 && totalCostOfCustomizationInGold != 0)
                {
                    firstTotalCostOfCustomization.text = totalCostOfCustomizationInGold + "P";
                    firstTotalCostOfCustomization.color = goldCostColor;

                    helperText.text = "";
                    secondTotalCostOfCustomization.text = "";
                }

                //If does cost both currencies, set the texts for both costs
                else if (totalCostOfCustomizationInSilver != 0 && totalCostOfCustomizationInGold != 0)
                {
                    firstTotalCostOfCustomization.text = totalCostOfCustomizationInSilver + "C";
                    firstTotalCostOfCustomization.color = silverCostColor;

                    helperText.text = "&";

                    secondTotalCostOfCustomization.text = totalCostOfCustomizationInGold + "P";
                    secondTotalCostOfCustomization.color = goldCostColor;
                }
            }
        }

        //  When in "Modules" customization (the light one can be found as commented on bottom of the script)
        if (currentWindow == 2)
        {
            costOfCurrentItemText.text = moduleSilverCost.ToString();
            typeText.text = "Current Module:";

            // If the cost of current item is 0c and 0p. Its Opened
            if (moduleSilverCost == 0 && moduleGoldCost == 0)
            {
                costOfCurrentItemText.text = "Unlocked";
                costOfCurrentItemText.color = defaultStringColor;
            }

            // If current Item does cost, set the cost texts
            if (moduleSilverCost != 0 || moduleGoldCost != 0)
            {
                if (moduleSilverCost != 0)
                {
                    costOfCurrentItemText.text = moduleSilverCost + "C";
                    costOfCurrentItemText.color = silverCostColor;
                }
                if (moduleGoldCost != 0)
                {
                    costOfCurrentItemText.text = moduleGoldCost + "P";
                    costOfCurrentItemText.color = goldCostColor;
                }
            }

            // If there is no "Total cost" needed, hide total cost texts
            if ((totalCostOfCustomizationInSilver == 0 && totalCostOfCustomizationInGold == 0) || moduleSilverCost == totalCostOfCustomizationInSilver)
            {
                midText.text = "";
                firstTotalCostOfCustomization.text = "";
                secondTotalCostOfCustomization.text = "";
                helperText.text = "";
            }

            // If total cost is needed, then set the texts
            if ((moduleSilverCost != totalCostOfCustomizationInSilver) || (moduleGoldCost != totalCostOfCustomizationInGold))
            {
                midText.text = "total:";

                // If does cost silver but not gold, hide second cost texts
                if (totalCostOfCustomizationInSilver != 0 && totalCostOfCustomizationInGold == 0)
                {
                    firstTotalCostOfCustomization.text = totalCostOfCustomizationInSilver + "C";
                    firstTotalCostOfCustomization.color = silverCostColor;

                    helperText.text = "";
                    secondTotalCostOfCustomization.text = "";
                }

                // If does cost gold but not silver, hide second cost texts
                if (totalCostOfCustomizationInSilver == 0 && totalCostOfCustomizationInGold != 0)
                {
                    firstTotalCostOfCustomization.text = totalCostOfCustomizationInGold + "P";
                    firstTotalCostOfCustomization.color = goldCostColor;

                    helperText.text = "";
                    secondTotalCostOfCustomization.text = "";
                }

                //If does cost both currencies, set the texts for both costs
                else if (totalCostOfCustomizationInSilver != 0 && totalCostOfCustomizationInGold != 0)
                {
                    firstTotalCostOfCustomization.text = totalCostOfCustomizationInSilver + "C";
                    firstTotalCostOfCustomization.color = silverCostColor;

                    helperText.text = "&";

                    secondTotalCostOfCustomization.text = totalCostOfCustomizationInGold + "P";
                    secondTotalCostOfCustomization.color = goldCostColor;
                }
            }
        }
    }
    */
}
