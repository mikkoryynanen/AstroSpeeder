using UnityEngine;
using UnityEngine.UI;

public class HeaderButton : MonoBehaviour
{
    public Button button;
    public Text nameLabel;
    public int silverCost;
    public int goldCost;
    public bool isOwned;
    public bool selected;

    //public string type;
    public int headerReference;

    public HeaderButton thisBtn;
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

    void Start()
    {
        UpdateButtons();
    }

    // Float the selected buttons text
    void IsSelected()
    {
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
            value += 0.01f;

        if (goingDown)
            value -= 0.01f;

        myTransform.localScale = new Vector3(value, value, value);
    }
    
    // Cancel the float effect
    public void CancelSelection()
    {
        CancelInvoke("IsSelected");
        myTransform.localScale = new Vector3(1, 1, 1);
    }

    // Start the float effect
    public void ItemSelected()
    {
        CancelInvoke("IsSelected");
        InvokeRepeating("IsSelected", 0f, 0.1f);
    }

    // When click on the button, set floating:s and send CM the click data
    public void OnClickEvent()
    {
        if (cm == null)
            cm = GameObject.Find("MainMenuCanvas").GetComponent<CustomizationManager>();

        CancelSelection();

        if (thisBtn == null)
            thisBtn = gameObject.GetComponent<HeaderButton>();

        cm.ClickOnThrusterHeader(headerReference, thisBtn, true);
    }

    // Called on click to check if there is silver cost to show
    public void checkIfPurchaseableWithSilver()
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

        CancelSelection();

        OnClickEvent();
        UpdateButtons();

        if (thisBtn == null)
            thisBtn = gameObject.GetComponent<HeaderButton>();

        cm.ClickOnThrusterHeader(headerReference, thisBtn, isSilverButton);
    }

    public void UpdateButtons()
    {
        silverText.text = silverCost.ToString() + "C";
        goldText.text = goldCost.ToString() + "P";

        // If button is owned, then costs are no longer needed
        if (isOwned)
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
        }

        // If there is no value to be show, disable the button
        if (silverCost == 0 && silverButton.isSilverButton)
            silverButton.gameObject.SetActive(false);

        if (goldCost == 0 && !goldButton.isSilverButton)
            goldButton.gameObject.SetActive(false);
    }
}
