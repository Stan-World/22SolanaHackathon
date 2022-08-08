using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class EnterStreamingPartyGroup : MonoBehaviour
{
    private const int WaitingSecTime = 1000 * 2;

    [SerializeField]
    private Text userName;
    [SerializeField]
    private Text userCount;

    private void Start()
    {
        DoInit();
        DoLoadFandomInfo();
    }

    private void DoInit()
    {
        userName.text = $"Hello {Core.System.DataManager.User.name}!";
        DoInitUserCount("---");
    }

    private void DoLoadFandomInfo()
    {
        Core.System.NetworkDataManager.AddListener(CMD.fandom, DoOnLoadFandomInfo);
        Core.System.NetworkManager.Fandom();
    }

    private void DoOnLoadFandomInfo()
    {
        Debug.Log($"-------------- DoOnLoadFandomInfo");
        Core.System.NetworkDataManager.RemoveListener(CMD.fandom, DoOnLoadFandomInfo);

        DoInitUserCount(Core.System.DataManager.FandomInfo.GetOnlineUserCountString());

        DoLoadFandomShop();
    }

    private void DoLoadFandomShop()
    {
        Core.System.NetworkDataManager.AddListener(CMD.fandomShop, DoOnLoadFandomShop);
        Core.System.NetworkManager.FandomShop();
    }

    private void DoOnLoadFandomShop()
    {
        Core.System.NetworkDataManager.RemoveListener(CMD.fandomShop, DoOnLoadFandomShop);

        DoLoadFandomLevelTable();
    }

    private void DoLoadFandomLevelTable()
    {

        Core.System.NetworkDataManager.AddListener(CMD.fandomLevels, DoOnLoadFandomLevelTable);
        Core.System.NetworkManager.FandomLevelTable();

    }

    private void DoOnLoadFandomLevelTable()
    {
        Core.System.NetworkDataManager.RemoveListener(CMD.fandomLevels, DoOnLoadFandomLevelTable);
        DoNextScreen();
    }

    private void DoInitUserCount(string myUserCountString)
    {
        userCount.text = myUserCountString;
    }

    private async void DoNextScreen()
    {
        await Task.Delay(WaitingSecTime);

        Core.System.GameController.SetScreen(ScreenTag.StreamingPartyGroup);
    }
}
