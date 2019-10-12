using UnityEngine;
using System.Collections.Generic;
using IAPC.Android.Core;
using IAPC.Android.Util;

public class IAPCrossAndroidExample : MonoBehaviour
{
    private float cursorY;
    private float cursorYInterval = 55;
    public string resultStr;

    private void Awake()
    {
        IAPCross.Instance.IsDebugEnable = true;
        IAPCross.Instance.FakeResultInEditor = IAPCross.FakeResults.Success;
        IAPCross.Instance.OnConnectCompleteEvent += OnConnectComplete;
        IAPCross.Instance.OnInventoryLoadCompleteEvent += OnInventoryLoadComplete;
        IAPCross.Instance.OnPurchaseCompleteEvent += OnPurchaseComplete;
        IAPCross.Instance.OnConsumeCompleteEvent += OnConsumeComplete;
    }

    private void OnGUI()
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
    }

    private void OnApplicationQuit()
    {
        IAPCross.Instance.Dispose();
    }

    private void OnConsumeComplete(IAPCrossResult result, Purchase purchase)
    {
        if (result.IsSuccess)
        {
            resultStr = string.Format("Consume success. Sku:{0} Orjinal Json:{1} ", purchase.ProductId, purchase.OriginalJson);
        }
        else
        {
            resultStr = "Consume failed : " + result.Message;
        }
    }

    private void OnPurchaseComplete(IAPCrossResult result, Purchase purchase)
    {
        if (result.IsSuccess)
        {
            resultStr = string.Format("purchase success. Sku:{0} Token:{1} ", purchase.ProductId, purchase.Token);
        }
        else
        {
            resultStr = "purchase failed : " + result.Message;
        }
    }

    private void OnInventoryLoadComplete(IAPCrossResult result, List<Purchase> purchases, List<SkuDetail> products)
    {
        if (result.IsSuccess)
        {
            resultStr = "Inventory load success.\nSku Details :";

            foreach (var item in products)
            {
                resultStr += string.Format("\nSKU: {0} ItemType: {1} Price:{2}", item.ProductId, item.ItemType, item.Price);
            }

            resultStr += "\nPurchases :";

            foreach (var item in purchases)
            {
                resultStr += string.Format("\nSKU: {0} ItemType: {1} Purchase Local Time:{2}", item.ProductId, item.ItemType, item.PurchaseTimeByLocalDateTime);
            }
        }
        else
        {
            resultStr = "Inventory load failed : " + result.Message;
        }
    }

    private void OnConnectComplete(IAPCrossResult result)
    {
        resultStr = result.IsSuccess ? "Connection Success" : string.Format("Connection failed : {0} ", result.Message);
    }
}
