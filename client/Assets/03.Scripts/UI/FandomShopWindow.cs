using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class FandomShopWindow : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPrefab;

    private void Awake()
    {
        int itemCount = Core.System.DataManager.FandomShopItemList.Count;
        ObjectPool.CreatePool(itemPrefab, itemCount);
    }
    private void OnEnable()
    {
        DoInit();
    }

    private void OnDisable()
    {
        itemPrefab.RecycleAll();
    }

    private void DoInit()
    {
        for (int i = 0; i < Core.System.DataManager.FandomShopItemList.Count; i++)
        {
            FandomShopItem item = Core.System.DataManager.FandomShopItemList[i];

            GameObject itemObj = itemPrefab.Spawn(itemPrefab.transform.parent);
            itemObj.SetActive(true);
            itemObj.transform.localScale = Vector3.one;
            FandomShopItemView shopItemView = itemObj.GetComponent<FandomShopItemView>();

            shopItemView.Init(item);
        }
    }

    public void OnClickClose()
    {
        this.gameObject.SetActive(false);
    }
}
