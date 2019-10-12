using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SampleButtonCurrencyButton : MonoBehaviour
{
    public bool selected;
    public bool isSilverButton;

    [Header("Floating Effect variables")]
    public RectTransform myTransform;//
    private float value;//              Vars for floating selection
    private bool goingDown;//
    private CustomizationManager cm;

    public Sprite selectedButton;
    public Sprite unselectedButton;

    public Color notSelectedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color selectedColor = new Color(1f, 1f, 1f, 1f);

    private Image myImage;

    void Start ()
    {
        myImage = GetComponent<Image>();
    }

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

    public void CancelSelection()
    {
        CancelInvoke("IsSelected");
        myTransform.localScale = new Vector3(1, 1, 1);

        if(myImage == null)
            myImage = GetComponent<Image>();

        myImage.sprite = unselectedButton;
        myImage.color = notSelectedColor;
    }

    public void ItemSelected()
    {
        CancelInvoke("IsSelected");
        InvokeRepeating("IsSelected", 0f, 0.1f);

        if (myImage == null)
            myImage = GetComponent<Image>();

        myImage.sprite = selectedButton;
        myImage.color = selectedColor;
    }

    public void SetPriceButtonSelected ()
    {
        if (cm == null)
            cm = GameObject.Find("MainMenuCanvas").GetComponent<CustomizationManager>();

        // When on paintjob page
        if(cm.currentWindow == 0)
        {
            if (cm.lastMatPriceButton != null)
                cm.lastMatPriceButton.CancelSelection();

            cm.lastMatPriceButton = GetComponent<SampleButtonCurrencyButton>();
        }

        // When on thrusters page
        if (cm.currentWindow == 1)
        {
            if (cm.lastThrusterPriceButton != null)
                cm.lastThrusterPriceButton.CancelSelection();

            cm.lastThrusterPriceButton = GetComponent<SampleButtonCurrencyButton>();
        }

        // When on lights page
        if (cm.currentWindow == 2)
        {
            if (cm.lastLightPriceButton != null)
                cm.lastLightPriceButton.CancelSelection();

            cm.lastLightPriceButton = GetComponent<SampleButtonCurrencyButton>();
        }
        ItemSelected();
    }
}
