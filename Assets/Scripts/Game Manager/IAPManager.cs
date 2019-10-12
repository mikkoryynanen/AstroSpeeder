using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//IAP
using IAPC.Android.Core;
using IAPC.Android.Util;
//Google play 
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class IAPManager : MonoBehaviour
{
    public Text shopDebug;
    public Text shopDebug2;

    //public GameObject buyButtons;
    //public GameObject redeemButtons;
    //public GooglePlayCloudSave GPCloudSave;

    private float cursorY;
    private float cursorYInterval = 55;
    public string resultStr;

    //public GameObject[] productRedeemTexts;
    //public GameObject[] otherButtonTexts;
    public Text[] productPriceTexts;
    public string[] productIds;
    public Animator[] productButtonsAnimators;
    public CustomizationManager customizationManager;
    public SavingSystemV3 savingSystem;
    public int selectedProductAmmount;
    public int redeemableProductIndex;

    private string googlePlayPublishKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAhliUC7zflQ0wnJGu48uM7LYHUXwQr7gEfvuhrg46KE9tXM0eDqldSVNsHFevLO9SZ0iHdqHSCmfHblOSr/AMppu+8mXxcQ1bKXqEmVld9OIdJcPhcvS3GlttFBHlRybG2vYyKUFa5YqzTla9pibGy5vB0QY4BrQNZdiDCkIEuDuw5miLS5oIICL1GT5ojvStw+v98Sjj9Y/dKl0VVcq75NqvYDkr5tCu4rswsQfFB3segrxwSY5gtNZujJZAYvEmwHLIqzhzwDNfCozX6XRAO6n6ZMGSFUaFMVCr1Qw0fmva+TTW/gVG0DpyPi4HJk3/ggxW79laHsogPsm6RjHWGQIDAQAB";

    bool bought100p = false;
    bool bought480p = false;
    bool bought1400p = false;
    bool bought3200p = false;
    bool bought9000p = false;
    bool bought20000p = false;
    bool inventoryLoaded = false;
    bool canBuy = true;


    private void Awake()
    {
        savingSystem = GameObject.FindWithTag("DataRepo").GetComponent<SavingSystemV3>();

        //In editor
#if UNITY_EDITOR
        IAPLogin();
        LoadInventory();
#endif

        IAPCross.Instance.IsDebugEnable = false;
        //        IAPCross.Instance.FakeResultInEditor = IAPCross.FakeResults.Success;
        IAPCross.Instance.OnConnectCompleteEvent += OnConnectComplete;
        IAPCross.Instance.OnInventoryLoadCompleteEvent += OnInventoryLoadComplete;
        IAPCross.Instance.OnPurchaseCompleteEvent += OnPurchaseComplete;
        IAPCross.Instance.OnConsumeCompleteEvent += OnConsumeComplete;
    }

    public void IAPLogin()
    {
        //login
        IAPCross.Instance.Connect(productIds, googlePlayPublishKey);
        LoadInventory();
    }

    void Update()
    {
        if (IAPCross.Instance.IsConnected && !inventoryLoaded)
        {
            IAPCross.Instance.LoadInventory();
        }
    }

    void LoadInventory()
    {
        IAPCross.Instance.LoadInventory();
    }

    private void OnInventoryLoadComplete(IAPCrossResult result, List<Purchase> purchases, List<SkuDetail> products)
    {
        if (result.IsSuccess)
        {
            //Check currency country and update cost texts accordingly
            for (int i = 0; i < productIds.Length; i++)
            {
                productPriceTexts[i].text = IAPCross.Instance.GetSkuDetail(productIds[i]).Price;
            }

            //Look for unconsumed products
            for (int i = 0; i < purchases.Count; i++)
            {
                IAPCross.Instance.Consume(purchases[i].ProductId);
            }
            inventoryLoaded = true;
        }

        else {
            //resultStr = "Inventory load failed : " + result.Message;
            //string5 = "Inventory load failed";
        }
    }

    private void OnPurchaseComplete(IAPCrossResult result, Purchase purchase)
    {
        if (result.IsSuccess)
        {
            if (redeemableProductIndex == 0)
                Redeem200p();

            if (redeemableProductIndex == 1)
                Redeem480p();

            if (redeemableProductIndex == 2)
                Redeem1400p();

            if (redeemableProductIndex == 3)
                Redeem3200p();

            if (redeemableProductIndex == 4)
                Redeem9000p();

            if (redeemableProductIndex == 5)
                Redeem20000p();
        }
        else
        {
            //resultStr = "purchase failed : " + result.Message;
            //string1 = "Purchase Failed";
        }
    }

    private void OnConsumeComplete(IAPCrossResult result, Purchase purchase)
    {
        if (result.IsSuccess)
        {
            ProductsConsumed();
            //canBuy = true;
            //string2 = "Consume Completed";
        }
        else
        {
            //resultStr = "Consume failed : " + result.Message;
            //string2 = "Consume Failed";
        }
    }

    void RebuyBoughtProducts(int ammount)
    {
        //Give player the bought ammount of gold currency
        savingSystem.goldCurrency += ammount;
        //Save data
        savingSystem.SaveData();
        //Update the wallet
        customizationManager.UpdateWallet();
    }

    void ProductsConsumed()
    {
        //Give player the bought ammount of gold currency
        savingSystem.goldCurrency += selectedProductAmmount;
        //Save data
        savingSystem.SaveData();
        //Update the wallet
        customizationManager.UpdateWallet();

        //Save the data to cloud
        //GPCloudSave.Save();
    }

    public void ProductAmmount(int productAmmount)
    {
        selectedProductAmmount = productAmmount;
    }

    public void Buy200Coins()
    {
        //    if (!bought100p) {
        IAPCross.Instance.Purchase(productIds[0], "DEVELOPER_PAYLOAD");
        //bought100p = true;
        redeemableProductIndex = 0;
        //canBuy = false;
    }

    public void Buy480Coins()
    {
        //    if (!bought480p) {
        IAPCross.Instance.Purchase(productIds[1], "DEVELOPER_PAYLOAD");
        //bought480p = true;
        redeemableProductIndex = 1;
        //canBuy = false;
    }

    public void Buy1400Coins()
    {
        //     if (!bought1400p) {
        IAPCross.Instance.Purchase(productIds[2], "DEVELOPER_PAYLOAD");
        //bought1400p = true;
        redeemableProductIndex = 2;
        //canBuy = false;
    }

    public void Buy3200Coins()
    {
        //    if (!bought3200p) {
        IAPCross.Instance.Purchase(productIds[3], "DEVELOPER_PAYLOAD");
        //bought3200p = true;
        redeemableProductIndex = 3;
        //canBuy = false;
    }

    public void Buy9000Coins()
    {
        //    if (!bought9000p) {
        IAPCross.Instance.Purchase(productIds[4], "DEVELOPER_PAYLOAD");
        //bought9000p = true;
        redeemableProductIndex = 4;
        //canBuy = false;
    }

    public void Buy20000Coins()
    {
        //    if (!bought20000p) {
        IAPCross.Instance.Purchase(productIds[5], "DEVELOPER_PAYLOAD");
        //bought20000p = true;
        redeemableProductIndex = 5;
        //canBuy = false;
    }

    public void Redeem200p()
    {
        IAPCross.Instance.Consume(productIds[0]);
        bought100p = false;
        //string4 = "Redeeming 100";
    }

    public void Redeem480p()
    {
        IAPCross.Instance.Consume(productIds[1]);
        bought480p = false;
        //string4 = "Redeeming 480";
    }

    public void Redeem1400p()
    {
        IAPCross.Instance.Consume(productIds[2]);
        bought1400p = false;
        //string4 = "Redeeming 1400";
    }

    public void Redeem3200p()
    {
        IAPCross.Instance.Consume(productIds[3]);
        bought3200p = false;
        //string4 = "Redeeming 3200";
    }

    public void Redeem9000p()
    {
        IAPCross.Instance.Consume(productIds[4]);
        bought9000p = false;
        //string4 = "Redeeming 9000";
    }

    public void Redeem20000p()
    {
        IAPCross.Instance.Consume(productIds[5]);
        bought20000p = false;
        //string4 = "Redeeming 20000";
    }

    /*private void OnGUI()
    {
        cursorY = 10;

        GUI.Label(new Rect(240, 10, Screen.width - 250, Screen.height - 20), resultStr);

        if (!IAPCross.Instance.IsConnected)
        {
            //Connect
            GUI.enabled = !IAPCross.Instance.IsConnectInProgress;
            if (GUI.Button(new Rect(10, cursorY, 220, 50), "Connect"))
            {
                resultStr = "Connecting...";

                IAPCross.Instance.Connect(new string[] { "unmanaged_0", "unmanaged_1", "unmanaged_2", "subscribe_test", "managed_test" }
                    , "PUBLIC_KEY");
            }
            cursorY += cursorYInterval;
        }
        else
        {
            //Load Inventory
            GUI.enabled = !IAPCross.Instance.IsInventoryLoadInProgress;
            if (GUI.Button(new Rect(10, cursorY, 220, 50), IAPCross.Instance.IsInventoryLoaded ? "Refresh Inventory" : "Load Inventory"))
            {
                resultStr = "Invertoy loading...";
                IAPCross.Instance.LoadInventory();
            }
            cursorY += cursorYInterval;

            //Unmanaged Product Purchase
            GUI.enabled = !IAPCross.Instance.IsPuchaseInProgress;
            if (GUI.Button(new Rect(10, cursorY, 220, 50), "Unmanaged Purchase"))
            {
                IAPCross.Instance.Purchase("spacecake.iap.diamond1", "DEVELOPER_PAYLOAD");
            }
            cursorY += cursorYInterval;

            //Unmanaged product Consume
            GUI.enabled = !IAPCross.Instance.IsConsumeInProgress;
            if (GUI.Button(new Rect(10, cursorY, 220, 50), "Unmanaged Consume"))
            {
                IAPCross.Instance.Consume("spacecake.iap.diamond1");
            }
            cursorY += cursorYInterval;

            //Managed Product Purchase
            GUI.enabled = !IAPCross.Instance.IsPuchaseInProgress;
            if (GUI.Button(new Rect(10, cursorY, 220, 50), "Managed Purchase"))
            {
                IAPCross.Instance.Purchase("managed_test", "DEVELOPER_PAYLOAD");
            }
            cursorY += cursorYInterval;

            //Managed Product Consume
            GUI.enabled = !IAPCross.Instance.IsConsumeInProgress;
            if (GUI.Button(new Rect(10, cursorY, 220, 50), "Managed Consume"))
            {
                IAPCross.Instance.Consume("managed_test");
            }
            cursorY += cursorYInterval;

            //Subscribe Prdouct Purchase
            GUI.enabled = !IAPCross.Instance.IsPuchaseInProgress;
            if (GUI.Button(new Rect(10, cursorY, 220, 50), "Subscribe Purchase"))
            {
                //Dont Forget you can not consume subscribe item
                IAPCross.Instance.Subscribe("subscribe_test", "DEVELOPER_PAYLOAD");
            }
            cursorY += cursorYInterval;

            GUI.enabled = true;
            //Check user subscribed
            if (GUI.Button(new Rect(10, cursorY, 220, 50), "Is Subscribed"))
            {
                resultStr = "IsProductSubscribed : " + IAPCross.Instance.IsProductSubscribed("subscribe_test");
            }
            cursorY += cursorYInterval;

            //Check user subscribed
            if (GUI.Button(new Rect(10, cursorY, 220, 50), "GetSkuDetail"))
            {
                SkuDetail detail = IAPCross.Instance.GetSkuDetail("subscribe_test");
                resultStr = string.Format("SKU: {0}\nItemType: {1}\nPrice:{2}\nTitle:{3}", detail.ProductId, detail.ItemType, detail.Price, detail.Title);
            }
            cursorY += cursorYInterval;

            //Check user subscribed
            if (GUI.Button(new Rect(10, cursorY, 220, 50), "GetPurchase"))
            {
                Purchase purchase = IAPCross.Instance.GetPurchase("subscribe_test");
                resultStr = string.Format("SKU: {0}\nItemType: {1}\nUTC Time:{2}\nToken:{3}", purchase.ProductId, purchase.ItemType, purchase.PurchaseTimeByUTCDateTime, purchase.Token);
            }
            cursorY += cursorYInterval;

            if (GUI.Button(new Rect(10, cursorY, 220, 50), "Dispose"))
            {
                IAPCross.Instance.Dispose();
            }
            cursorY += cursorYInterval;
        }
    }*/

    private void OnApplicationQuit()
    {
        IAPCross.Instance.Dispose();
    }

    private void OnConnectComplete(IAPCrossResult result)
    {
        resultStr = result.IsSuccess ? "Connection Success" : string.Format("Connection failed : {0} ", result.Message);
    }
    /*
    void OnGUI ()
    {
        GUILayout.Label(string1);
        GUILayout.Label(string2);
        GUILayout.Label(string3);
        GUILayout.Label(string4);
        GUILayout.Label(string5);
        GUILayout.Label(string6);
        GUILayout.Label(string7);

        string1 = IAPCross.Instance.GetSkuDetail(productIds[0]).OrginalJson;
        string2 = IAPCross.Instance.GetSkuDetail(productIds[1]).OrginalJson;
        string3 = IAPCross.Instance.GetSkuDetail(productIds[2]).OrginalJson;
        string4 = IAPCross.Instance.GetSkuDetail(productIds[3]).OrginalJson;
        string5 = IAPCross.Instance.GetSkuDetail(productIds[4]).OrginalJson;
        string6 = IAPCross.Instance.GetSkuDetail(productIds[5]).OrginalJson;
    }*/
}
