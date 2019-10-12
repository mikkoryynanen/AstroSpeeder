using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IOSManager : MonoBehaviour {

    public Text walletGoldText;
    public Button shopButton;
    public RectTransform shopWindow;
    public GameObject topUI;
    public GameObject restOfTheUI;

    private SavingSystemV3 savingSystem;

    private int purchasedItemIndex = -1;


    void OnLevelWasLoaded(int level) {
        if(level == 1 && !DataRepo.iosLoggedIn) {
            savingSystem = GameObject.FindWithTag("DataRepo").GetComponent<SavingSystemV3>();
            walletGoldText.text = "C: " + savingSystem.goldCurrency.ToString();

            GameCenterManager.Init();
            LoadIOSStore();
            DataRepo.iosLoggedIn = true;
        }
    }

    public void OpenAchievement() {
        GameCenterManager.ShowAchievements();
    }

    public void OpenLeaderboard() {
        GameCenterManager.ShowLeaderboard("Leaderboard_1");
    }

    public void ShopOpen() {
        topUI.SetActive(false);
        restOfTheUI.SetActive(false);
        shopWindow.localPosition = new Vector3(0, 0, 0);
    }

    public void ShopClose() {
        walletGoldText.text = "C: " + savingSystem.goldCurrency.ToString();

        topUI.SetActive(true);
        restOfTheUI.SetActive(true);
        shopWindow.localPosition = new Vector3(0, -866, 0);
    }

    //In app Purchases
    private void LoadIOSStore() {
        //Reset values
        purchasedItemIndex = -1;

        IOSInAppPurchaseManager.OnStoreKitInitComplete += IOSInAppPurchaseManager_OnStoreKitInitComplete;
        IOSInAppPurchaseManager.OnTransactionComplete += IOSInAppPurchaseManager_OnTransactionComplete;

        IOSInAppPurchaseManager.Instance.LoadStore();
    }

    private void IOSInAppPurchaseManager_OnTransactionComplete(IOSStoreKitResult response) {
        //When the transaction has been completed give player the corresponding ammount of premium currency
        if(response.State == InAppPurchaseState.Purchased) {
            Debug.Log("OnTransactionComplete: " + response.ProductIdentifier);

            if(purchasedItemIndex == 0) {
                savingSystem.goldCurrency += 200;
            }

            else if(purchasedItemIndex == 1) {
                savingSystem.goldCurrency += 480;
            }

            else if(purchasedItemIndex == 2) {
                savingSystem.goldCurrency += 1400;
            }

            else if(purchasedItemIndex == 3) {
                savingSystem.goldCurrency += 3200;
            }

            else if(purchasedItemIndex == 4) {
                savingSystem.goldCurrency += 9000;
            }

            else if(purchasedItemIndex == 5) {
                savingSystem.goldCurrency += 20000;
            }
        }
    }

    private void IOSInAppPurchaseManager_OnStoreKitInitComplete(ISN_Result result) {
        if(result.IsSucceeded) {
            Debug.Log("Inited successfully, Available products count: " + IOSInAppPurchaseManager.Instance.Products.Count.ToString());

            //make shop button interactable
            shopButton.interactable = true;

            //TODO set localiecd prices
        }
        else {
			Debug.Log("StoreKit Init Failed.  Error code: " + result.Error.Code + "\n" + "Error description:" + result.Error.Description);
		}
    }

    public void Buy200P() {
        if(IOSInAppPurchaseManager.Instance.IsStoreLoaded) {
            IOSInAppPurchaseManager.Instance.BuyProduct("premiumCurrency_200");

            purchasedItemIndex = 0;
        }
        else {
            LoadIOSStore();
        }
    }

    public void Buy480P() {
        if(IOSInAppPurchaseManager.Instance.IsStoreLoaded) {
            IOSInAppPurchaseManager.Instance.BuyProduct("premiumCurrency_480");

            purchasedItemIndex = 1;
        }
        else {
            LoadIOSStore();
        }
    }

    public void Buy1400P() {
        if(IOSInAppPurchaseManager.Instance.IsStoreLoaded) {
            IOSInAppPurchaseManager.Instance.BuyProduct("premiumCurrency_1400");

            purchasedItemIndex = 2;
        }
        else {
            LoadIOSStore();
        }
    }

    public void Buy3200P() {
        if(IOSInAppPurchaseManager.Instance.IsStoreLoaded) {
            IOSInAppPurchaseManager.Instance.BuyProduct("premiumCurrency_3200");

            purchasedItemIndex = 3;
        }
        else {
            LoadIOSStore();
        }
    }

    public void Buy9000P() {
        if(IOSInAppPurchaseManager.Instance.IsStoreLoaded) {
            IOSInAppPurchaseManager.Instance.BuyProduct("premiumCurrency_9000");

            purchasedItemIndex = 4;
        }
        else {
            LoadIOSStore();
        }
    }

    public void Buy20000P() {
        if(IOSInAppPurchaseManager.Instance.IsStoreLoaded) {
            IOSInAppPurchaseManager.Instance.BuyProduct("premiumCurrency_20k");

            purchasedItemIndex = 5;
        }
        else {
            LoadIOSStore();
        }
    }
}
