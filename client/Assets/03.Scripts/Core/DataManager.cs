using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCache
{
    public const string KeyUserName = "UserName";    

    internal void Save(string myKey, string myData)
    {
        PlayerPrefs.SetString(myKey, myData);
    }


    internal string LoadString(string myKey)
    {
        return PlayerPrefs.GetString(myKey, string.Empty);
    }


    internal void Save(string myKey, int myData)
    {
        PlayerPrefs.SetInt(myKey, myData);
    }

    internal int LoadInt(string myKey, int myDefault = 0)
    {
        return PlayerPrefs.GetInt(myKey, myDefault);
    }
}




public class DataManager
{
    private LocalCache localCache = new LocalCache();
    public User User { get; } = new User();
    public UserItem UserItem { get; } = new UserItem();
    public Fandom Fandom { get; } = new Fandom();
    public FandomInfo FandomInfo { get; } = new FandomInfo();
    public StreamingParty StreamingParty { get; } = new StreamingParty();

    private Dictionary<string, Action<BasePacket>> updateDataHandlers = new Dictionary<string, Action<BasePacket>>();
    public List<FandomShopItem> FandomShopItemList = new List<FandomShopItem>();

    private Dictionary<int, FandomLevel> fandomLevelTable = new Dictionary<int, FandomLevel>();

    public DataManager()
    {
        DoInit();
        DoInitUpdateHandlers();        
    }

    private void DoInitUpdateHandlers()
    {
        updateDataHandlers.Add(CMD.login, DoUpdateLoginData);
        updateDataHandlers.Add(CMD.fandom, DoUpdateFandomInfoData);
        updateDataHandlers.Add(CMD.fandomStake, DoUpdateFandomStakeData);
        updateDataHandlers.Add(CMD.buyLightStick, DoUpdateBuyLightStickData);
        updateDataHandlers.Add(CMD.fandomShop, DoUpdateFandomShopData);
        updateDataHandlers.Add(CMD.purchaseFandomShopItem, DoUpdatePurchaseFandomShopItemData);
        updateDataHandlers.Add(CMD.fandomLevels, DoUpdateFandomLevelsData);
        updateDataHandlers.Add(CMD.userUseStanGem, DoUpdateUserUseStanGemData);
    }

    private void DoInit()
    {
        User.name = localCache.LoadString(LocalCache.KeyUserName);        
    }

    internal string GetUserName()
    {
        return User.name;
    }

    internal void UpdateData(BasePacket myNetworkData)
    {
        Action<BasePacket> updateHandler = null;
        if (true == updateDataHandlers.TryGetValue(myNetworkData.cmd, out updateHandler))
        {
            updateHandler(myNetworkData);
        }
    }

    internal void UpdateLoginData()
    {
        if (true == User.lightStick)
        {
            UserItem.SetLevel(2);
        }
    }

    #region Update Handler
    private void DoUpdateLoginData(BasePacket myNetworkData)
    {
        SCLogin loginData = myNetworkData as SCLogin;

        User.uid = loginData.uid;
        User.name = loginData.name;
        User.vitaPoint = loginData.vitaPoint;
        User.stake = loginData.stake;
        User.stanGem = loginData.stanGem;
        User.power = loginData.power;
        User.online = loginData.online;
        User.lightStick = loginData.lightStick;

        Debug.Log(User.ToData());

        Fandom.id = loginData.fandom.id;
        Fandom.name = loginData.fandom.name;
        Fandom.adjust = loginData.fandom.adjust;
        Fandom.vitaPoint = loginData.fandom.vitaPoint;
        Fandom.vitaGold = loginData.fandom.vitaGold;
        Fandom.stanGem = loginData.fandom.stanGem;
        Fandom.level = loginData.fandom.level;

        Debug.Log(Fandom.ToData());

        localCache.Save(LocalCache.KeyUserName, User.name);
    }
    private void DoUpdateFandomInfoData(BasePacket myNetworkData)
    {
        SCFandomInfo fandomInfoData = myNetworkData as SCFandomInfo;
        //Debug.Log(fandomInfoData.name);

        Fandom.adjust = fandomInfoData.adjust;
        Fandom.vitaPoint = fandomInfoData.vitaPoint;
        Fandom.vitaGold = fandomInfoData.vitaGold;
        Fandom.stanGem = fandomInfoData.stanGem;
        Fandom.level = fandomInfoData.level;

        
        for (int i = 0; i < fandomInfoData.users.Count; i++)
        {
            SCFandomInfoUser user = fandomInfoData.users[i];            
            FandomInfo.UpdateUser(user);

            if (User.ItMe(user))
            {
                User.Update(user);
            }
        }

        FandomInfo.UpdateOnlineUser();

        StreamingParty.Update(fandomInfoData.party);
    }

    private void DoUpdateFandomStakeData(BasePacket myNetworkData)
    {
        SCFandomStake fandomStakeData = myNetworkData as SCFandomStake;

        User.uid = fandomStakeData.uid;
        User.name = fandomStakeData.name;
        User.stanGem = fandomStakeData.stanGem;
        User.vitaPoint = fandomStakeData.vitaPoint;
        User.stake = fandomStakeData.stake;
        User.power = fandomStakeData.power;
        User.online = fandomStakeData.online;

        //Debug.Log(User.ToData());

        Fandom.id = fandomStakeData.fandom.id;
        Fandom.name = fandomStakeData.fandom.name;
        Fandom.vitaPoint = fandomStakeData.fandom.vitaPoint;
        Fandom.vitaGold = fandomStakeData.fandom.vitaGold;
        Fandom.stanGem = fandomStakeData.fandom.stanGem;

        int currentLevel = Fandom.level;
        
        Fandom.level = fandomStakeData.fandom.level;

        Fandom.SetLevelUp(currentLevel < Fandom.level);

        //Debug.Log(Fandom.ToData());

    }
    private void DoUpdateBuyLightStickData(BasePacket myNetworkData)
    {
        SCBuyLightStickItem lightStickItemData = myNetworkData as SCBuyLightStickItem;
        User.lightStick = lightStickItemData.lightStick;
    }

    private void DoUpdateFandomShopData(BasePacket myNetworkData)
    {
        SCFandomShopData fandomShopData = myNetworkData as SCFandomShopData;
        for (int i = 0; i < fandomShopData.shop.Count; i++)
        {
            FandomShopItemList.Add(fandomShopData.shop[i]);
        }        
    }

    private void DoUpdatePurchaseFandomShopItemData(BasePacket myNetworkData)
    {
        SCPurchaseFandomShopItemData purchaseItemData = myNetworkData as SCPurchaseFandomShopItemData;

    }

    private void DoUpdateFandomLevelsData(BasePacket myNetworkData)
    {
        SCFandomLevelsData fandomLevelDatas = myNetworkData as SCFandomLevelsData;

        for (int i = 0; i < fandomLevelDatas.table.Count; i++)
        {
            FandomLevel fandomLevel = fandomLevelDatas.table[i];
            fandomLevelTable.Add(fandomLevel.level, fandomLevel);
        }
    }

    private void DoUpdateUserUseStanGemData(BasePacket myNetworkData)
    {
        SCUserUseStanGemData fandomLevelDatas = myNetworkData as SCUserUseStanGemData;

        User.Update(fandomLevelDatas);

    }
        

    #endregion


        internal bool IsMaxFandomLevel(int myFandomLevel)
    {
        FandomLevel fandomLevel = null;
        if (false == fandomLevelTable.TryGetValue(myFandomLevel, out fandomLevel))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    internal FandomLevel GetFandomLevelData(int myFandomLevel)
    {
        return fandomLevelTable[myFandomLevel];
    }
    internal string GetStreamingURL()
    {
        return StreamingParty.GetStreamingURL();
    }

    internal string GetCurrentStreamingURL()
    {
        return StreamingParty.GetCurrentStreamingURL();
    }

    

}
