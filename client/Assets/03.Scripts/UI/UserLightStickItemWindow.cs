using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserLightStickItemWindow : MonoBehaviour
{
    [SerializeField]
    private Button buyItemButton;
    [SerializeField]
    private Text buyItemLabel;
    [SerializeField]
    private Image itemIamge;

    private void OnEnable()
    {
        buyItemButton.gameObject.SetActive(!Core.System.DataManager.User.lightStick);

        DoUpdateItemImage();

        bool enableBuy = !Core.System.DataManager.UserItem.isActive;
        buyItemButton.interactable = enableBuy;

        buyItemLabel.text = $"{Core.System.DataManager.UserItem.needStanGemAmount}";
        
    }

    private void DoUpdateItemImage()
    {
        Color itemStateColor = Color.white;
        itemStateColor.a = (true == Core.System.DataManager.User.lightStick) ? 1f : 0.5f;
        itemIamge.color = itemStateColor;
    }

    public void OnClickPickItem()
    {
        if (Core.System.DataManager.UserItem.needStanGemAmount > Core.System.DataManager.User.stanGem)
        {
            string message = $"Need more StanGem!";
            Core.System.WindowManager.ShowNotice(message);
            return;
        }

        buyItemButton.interactable = false;
        Core.System.NetworkDataManager.AddListener(CMD.buyLightStick, DoOnBuyLightStick);
        Core.System.NetworkManager.BuyLightStick();
    }

    private void DoOnBuyLightStick()
    {        
        Core.System.NetworkDataManager.RemoveListener(CMD.buyLightStick, DoOnBuyLightStick);

        buyItemButton.gameObject.SetActive(false);
        DoUpdateItemImage();

        DoUseStanGem();
    }

    private void DoUseStanGem()
    {
        Core.System.NetworkDataManager.AddListener(CMD.userUseStanGem, DoOnUserUseStanGem);
        Core.System.NetworkManager.UserUseStanGem(Core.System.DataManager.UserItem.needStanGemAmount);
    }

    private void DoOnUserUseStanGem()
    {
        Core.System.NetworkDataManager.RemoveListener(CMD.userUseStanGem, DoOnUserUseStanGem);

        string message = $"Congratulation! Light Stick Level is 2. Let's Stake";
        Core.System.WindowManager.ShowNotice(message);
    }

    public void OnClickClose()
    {
        this.gameObject.SetActive(false);
    }
}
