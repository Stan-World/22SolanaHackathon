using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FandomShopItemView : MonoBehaviour
{
    [SerializeField]
    private Text name;
    [SerializeField]
    private Text price;
    [SerializeField]
    private Image itemImage;


    private FandomShopItem itemData;

    internal void Init(FandomShopItem myItem)
    {
        itemData = myItem;
        DoInit();
    }


    private void DoInit()
    {
        name.text = itemData.name;
        price.text = $"{itemData.price:#,0}";
        itemImage.sprite = Core.System.ResourceManager.GetSprite($"FandomShopProduct.{itemData.id}");
    }

    public void OnClickBuy()
    {
        if (Core.System.DataManager.Fandom.vitaGold < itemData.price) return;

        Core.System.NetworkDataManager.AddListener(CMD.purchaseFandomShopItem, DoOnPurchaseFandomShopItem);
        Core.System.NetworkManager.PurchaseFandomShopItem(itemData);
    }

    private void DoOnPurchaseFandomShopItem()
    {
        Core.System.NetworkDataManager.RemoveListener(CMD.purchaseFandomShopItem, DoOnPurchaseFandomShopItem);

        string purchaseItemText = $"Finished purchasing {itemData.name}!";
        Core.System.WindowManager.ShowNotice(purchaseItemText);
    }
}
