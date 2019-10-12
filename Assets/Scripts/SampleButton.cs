using UnityEngine;
using UnityEngine.UI;

public class SampleButton : MonoBehaviour
{
    public Button button;
    public Text nameLabel;
    public int silverCost;
    public int goldCost;
    public bool isOwned;
    public bool selected;

    public string name;
    public string type;
    public int matReference;
    public int thrusterTypeReference;
    public int thrusterColorReference;
    public int lightReference;
    public int moduleReference;

    public SavingSystemV3 savingsystem;
    public SampleButton thisBtn;
    private CustomizationManager cm;
    private Renderer playerRenderer;

    [Header("Floating Effect variables")]
    public RectTransform myTransform;//
    private float value;//              Vars for floating selection
    private bool goingDown;//

    [Header("Price Button variables")]
    public Text silverText;
    public Text goldText;
    public SampleButtonCurrencyButton silverButton;
    public SampleButtonCurrencyButton goldButton;

    void Start ()
    {
        UpdateButtons();
    }

    // Float the selected buttons text
    void IsSelected ()
    {
        if (myTransform.localScale.x <= 1)
        {
            value = 1;
            goingDown = false;
        }
        if(myTransform.localScale.x >= 1.05f)
        {
            value = 1.05f;
            goingDown = true;
        }
        if(!goingDown)
            value += 0.01f;

        if(goingDown)
            value -= 0.01f;

        myTransform.localScale = new Vector3(value, value, value);
    }

    // Cancel the float effect
    public void CancelSelection ()
    {
        CancelInvoke("IsSelected");
        myTransform.localScale = new Vector3(1, 1, 1);
    }

    // Start the float effect
    public void ItemSelected ()
    {
        CancelInvoke("IsSelected");
        InvokeRepeating("IsSelected", 0f, 0.1f);
    }

    // When click on the button; make it float, check the type and run functions accordingly
    public void OnClickEvent ()
    {
        if (cm == null)
            cm = GameObject.Find("MainMenuCanvas").GetComponent<CustomizationManager>();

        // Reset the selection so the floating speed wont be multiplied
        CancelSelection();

        if (thisBtn == null)
            thisBtn = gameObject.GetComponent<SampleButton>();

        cm.lastButton = thisBtn;

        if (type == "material")
        {
            if (playerRenderer == null)
                playerRenderer = GameObject.Find("PlayerShipMesh").GetComponent<Renderer>();

            playerRenderer.material = savingsystem.cachedMaterials[matReference].material;

            cm.materialSelected = matReference;
            cm.SetSelectedMat(thisBtn);

            // Also sets lights here to fix the light from changing with material
            playerRenderer.material.SetColor("_EmissionColor", savingsystem.cachedThrusterColors[cm.thrusterColorSelected].thrusterColor);

            /*float h = savingsystem.emissionColorsCache[cm.lightSelected].HHue;
            float s = savingsystem.emissionColorsCache[cm.lightSelected].SSaturation;
            float v = savingsystem.emissionColorsCache[cm.lightSelected].VBrightness;

            savingsystem.SetEmission(h, s, v);*/
        }

        if(type == "thruster")
        {
            if (playerRenderer == null)
                playerRenderer = GameObject.Find("PlayerShipMesh").GetComponent<Renderer>();

            //cm.thrusterTypeSelected = thrusterTypeReference;
            cm.thrusterColorSelected = thrusterColorReference;
            cm.SetSelectedThruster(thisBtn);

            cm.SetThruster(thrusterColorReference);

            // 1.2> Also set the light with the thruster color in COLOR, not hsv
            // Use the same color of a thruster on the emission lights
            playerRenderer.material.SetColor("_EmissionColor", savingsystem.cachedThrusterColors[thrusterColorReference].thrusterColor);

            cm.lightSelected = lightReference;//
            cm.SetSelectedLight(thisBtn);//
        }

/*      if(type == "light")
        {
            float h = savingsystem.emissionColorsCache[lightReference].HHue;
            float s = savingsystem.emissionColorsCache[lightReference].SSaturation;
            float v = savingsystem.emissionColorsCache[lightReference].VBrightness;
            
            savingsystem.SetEmission(h, s, v);

            cm.lightSelected = lightReference;
            cm.SetSelectedLight(thisBtn);
        }*/

        if(type == "module")
        {
            cm.moduleSelected = moduleReference;
            cm.SetSelectedModule(thisBtn);
            cm.SetModule(moduleReference);
        }
        cm.UpdateTotalCost();
    }

    // Called on click to check if there is silver cost to show
    public void checkIfPurchaseableWithSilver ()
    {
        if (silverCost == 0)
            OnSilverGoldClickEvent(false);
        else
            OnSilverGoldClickEvent(true);
    }

    // Even when clicked on the "mainbutton", this selects the possible silver cost
    public void OnSilverGoldClickEvent(bool isSilverButton)
    {
        if (cm == null)
            cm = GameObject.Find("MainMenuCanvas").GetComponent<CustomizationManager>();

        if (type == "material")
        {
            cm.materialSilverCost = 0;
            cm.materialGoldCost = 0;

            if (!isOwned && !cm.unlockThrusterWindow.activeInHierarchy)
            {
                if (isSilverButton)
                    cm.materialSilverCost = silverCost;
                if (!isSilverButton)
                    cm.materialGoldCost = goldCost;
            }
        }

        if (type == "thruster")
        {
            cm.thrusterSilverCost = 0;
            cm.thrusterGoldCost = 0;

            if (!isOwned && !cm.unlockThrusterWindow.activeInHierarchy)
            {
                if (isSilverButton)
                    cm.thrusterSilverCost = silverCost;
                if (!isSilverButton)
                    cm.thrusterGoldCost = goldCost;
            }
        }

/*      if (type == "light")
        {
            cm.lightSilverCost = 0;
            cm.lightGoldCost = 0;

            if (!isOwned && !cm.unlockThrusterWindow.activeInHierarchy)
            {
                if (isSilverButton)
                    cm.lightSilverCost = silverCost;
                if (!isSilverButton)
                    cm.lightGoldCost = goldCost;
            }
        }*/

        if (type == "module")
        {
            cm.moduleSilverCost = 0;
            cm.moduleGoldCost = 0;

            if(!isOwned && !cm.unlockThrusterWindow.activeInHierarchy)
            {
                if (isSilverButton)
                    cm.moduleSilverCost = silverCost;
                if (!isSilverButton)
                    cm.moduleGoldCost = goldCost;
            }
        }
        //cm.UpdateTotalCost();
        OnClickEvent();
        UpdateButtons();
    }

    // Check and set the buttons, if they still need to be shown etc.
    public void UpdateButtons ()
    {
        silverText.text = silverCost.ToString() + "C";
        goldText.text = goldCost.ToString() + "P";

        // If button is owned, then costs are no longer needed, also check if its sample button OR slimmer version for thruster colors
        if ((isOwned && gameObject.tag == "Untagged") || (name == "Default" && gameObject.tag == "Untagged"))
        {
            // Get the button text RectTransform and resize it
            RectTransform childText = transform.GetChild(0).gameObject.GetComponent<RectTransform>();
            LayoutElement layOutElement = GetComponent<LayoutElement>();
            layOutElement.minHeight = 100;

            // Fix the size of the text
            // Left - Bottom
            childText.offsetMax = new Vector2(-22f, -25.9f);
            // Right - Top
            childText.offsetMin = new Vector2(22f, 25.9f);

            silverButton.gameObject.SetActive(false);
            goldButton.gameObject.SetActive(false);
            //gameObject.SetActive(false);
        }

        // If the button is thruster color buttons
        if ((isOwned && gameObject.tag == "SlimThrusterColorButton") || (name == "Default" && gameObject.tag == "SlimThrusterColorButton"))
        {
            // Get the button text RectTransform and resize it
            RectTransform childText = transform.GetChild(0).gameObject.GetComponent<RectTransform>();
            LayoutElement layOutElement = GetComponent<LayoutElement>();
            layOutElement.minHeight = 50;

            // Fix the size of the text
            // Left - Bottom
            childText.offsetMax = new Vector2(-1, -1);
            // Right - Top
            childText.offsetMin = new Vector2(1, 1);

            // And for the slim version setup the anchors
            childText.anchorMin = new Vector2(0.054f, 0.1533333f);
            childText.anchorMax = new Vector2(0.946f, 0.8280001f);

            silverButton.gameObject.SetActive(false);
            goldButton.gameObject.SetActive(false);
        }

        // If there is no value to be show, disable the button
        if (silverCost == 0 && silverButton.isSilverButton)
            silverButton.gameObject.SetActive(false);

        if (goldCost == 0 && !goldButton.isSilverButton)
            goldButton.gameObject.SetActive(false);
    }
}
