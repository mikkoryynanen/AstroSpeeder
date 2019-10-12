using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;

[System.Serializable]
public class Item {
    public string name;
    public int silverCost;
    public int goldCost;
    public bool isOnwed;
    public Button.ButtonClickedEvent onClick;
}

public class CreateScrollList : MonoBehaviour {

    public GameObject sampleButton;
    [Header("Add Skins/Thruster types here")]
    public List<Item> shipSkins;
    public List<Item> shipThrusters;
    public List<Item> shipLights;

    [Header("Do Not Touch!<<<<<<<<<<<")]
    public List<GameObject> shipSkinList;
    public List<GameObject> shipThrusterList;
    public List<GameObject> playerOwnedShipSkins;
    public List<GameObject> playerOwnedThrusters;
    [Header("Do Not Touch!>>>>>>>>>>>")]

    public Transform shipMaterialContentPanel;
    public Transform shipThrustersContentPanel;
    public Transform shipLightsContentPanel;

    void Start () {
        PopulateList ();
    }

    void PopulateList () {
        //Populate shipSkins
        foreach(var item in shipSkins) {
            GameObject newButton = Instantiate (sampleButton) as GameObject;
            SampleButton button = newButton.GetComponent <SampleButton> ();
            button.nameLabel.text = item.name;
            button.silverCost = item.silverCost;
            button.goldCost = item.goldCost;
            button.isOwned = item.isOnwed;
            button.button.onClick = item.onClick;
            //Parent under Skin  Content panel and make sure its scale is 1
            newButton.transform.SetParent (shipMaterialContentPanel);
            newButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            shipSkinList.Add(newButton);

            if (button.isOwned) {
                newButton.GetComponent<Button>().interactable = true;
                playerOwnedShipSkins.Add(newButton);
            }
            else {
                newButton.GetComponent<Button>().interactable = false;
            }
        }

        //Populate shipThrusters
        foreach (var item in shipThrusters) {
            GameObject newButton = Instantiate (sampleButton) as GameObject;
            SampleButton button = newButton.GetComponent <SampleButton> ();
            button.nameLabel.text = item.name;
            button.isOwned = item.isOnwed;
            button.button.onClick = item.onClick;
            newButton.transform.SetParent (shipThrustersContentPanel);
            newButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            if(button.isOwned) {
                newButton.GetComponent<Button>().interactable = true;
                //playerOwnedThrusters.Add(newButton);
            }
            else {
                newButton.GetComponent<Button>().interactable = false;
            }
        }

        //Populate shipLights
        /*foreach (var item in shipLights) {
            GameObject newButton = Instantiate (sampleButton) as GameObject;
            SampleButton button = newButton.GetComponent <SampleButton> ();
            button.nameLabel.text = item.name;
            button.button.onClick = item.onClick;
            newButton.transform.SetParent (shipLightsContentPanel);
            newButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }*/
    }

    public void RePopulateSkinList() {
        //Discard the old list
        foreach(var button in shipSkinList) {
            Destroy(button);
        }

        shipSkinList.Clear();
        playerOwnedShipSkins.Clear();

        foreach (var item in shipSkins) {
            GameObject newButton = Instantiate (sampleButton) as GameObject;
            SampleButton button = newButton.GetComponent <SampleButton> ();
            button.nameLabel.text = item.name;
            button.isOwned = item.isOnwed;
            button.button.onClick = item.onClick;
            newButton.transform.SetParent (shipMaterialContentPanel);
            newButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            shipSkinList.Add(newButton);

            if (button.isOwned) {
                newButton.GetComponent<Button>().interactable = true;
                playerOwnedShipSkins.Add(newButton);
            }
            else {
                newButton.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void RePopulateThrusterList() {
        //Discard the old list
        foreach(var button in shipThrusterList) {
            Destroy(button);
        }

        shipThrusterList.Clear();
        playerOwnedThrusters.Clear();

        foreach (var item in shipThrusters) {
            GameObject newButton = Instantiate (sampleButton) as GameObject;
            SampleButton button = newButton.GetComponent <SampleButton> ();
            button.nameLabel.text = item.name;
            button.isOwned = item.isOnwed;
            button.button.onClick = item.onClick;
            newButton.transform.SetParent (shipMaterialContentPanel);
            newButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            shipThrusterList.Add(newButton);

            if (button.isOwned) {
                newButton.GetComponent<Button>().interactable = true;
                playerOwnedThrusters.Add(newButton);
            }
            else {
                newButton.GetComponent<Button>().interactable = false;
            }
        }
    }
}
