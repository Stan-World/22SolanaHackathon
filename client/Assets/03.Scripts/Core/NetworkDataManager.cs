using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkDataManager
{

    private Dictionary<string, Action<string>> responseHandler = new Dictionary<string, Action<string>>();
    private Dictionary<string, List<Action>> responseHandleListener = new Dictionary<string, List<Action>>();
    public NetworkDataManager()
    {
        responseHandler.Add(CMD.login, DoLoginHandler);
        responseHandler.Add(CMD.fandom, DoFandomHandler);
        responseHandler.Add(CMD.fandomStake, DoFandomStakeHandler);
        responseHandler.Add(CMD.buyLightStick, DoBuyLightStickHandler);
        responseHandler.Add(CMD.fandomShop, DoFandomShopHandler);
        responseHandler.Add(CMD.purchaseFandomShopItem, DoPurchaseFandomShopItemHandler);
        responseHandler.Add(CMD.fandomLevels, DoFandomLevelTableHandler);
        responseHandler.Add(CMD.userUseStanGem, DoUserUseStanGemHandler);

    }

    internal void AddListener(string myCMD, Action myListener)
    {
        List<Action> listeners = null;
        if (false == responseHandleListener.TryGetValue(myCMD, out listeners))
        {
            listeners = new List<Action>();
            responseHandleListener.Add(myCMD, listeners);
        }

        listeners.Add(myListener);
    }

    internal void RemoveListener(string myCMD, Action myListener)
    {
        List<Action> listeners = null;
        if (true == responseHandleListener.TryGetValue(myCMD, out listeners))
        {           
            listeners.Remove(myListener);
        }
    }

    private void DoActionListener(string myCMD)
    {
        List<Action> listeners = null;
        if (true == responseHandleListener.TryGetValue(myCMD, out listeners))
        {
            Action[] executeListeners = listeners.ToArray();

            for (int i = 0; i < executeListeners.Length; i++)
            {
                Action action = executeListeners[i];
                action();
            }
        }
    }


    internal void ResponseData(string myCmd, string myResponseRawData)
    {
        DoResponseData(myCmd, myResponseRawData);
    }

    private void DoResponseData(string myCmd, string myResponseRawData)
    {
        Action<string> handler = null;
        if (true == responseHandler.TryGetValue(myCmd, out handler))
        {
            handler(myResponseRawData);
        }       
    }

    #region CMD Handlers
    private void DoLoginHandler(string myResponseRawData)
    {
        
        SCLogin loginData = JsonUtility.FromJson<SCLogin>(myResponseRawData);

        Debug.Log(loginData.ToData());

        Core.System.DataManager.UpdateData(loginData);

        DoActionListener(CMD.login);

        
    }

    private void DoFandomHandler(string myResponseRawData)
    {
        //Debug.Log(myResponseRawData);

        SCFandomInfo fandomInfoData = JsonUtility.FromJson<SCFandomInfo>(myResponseRawData);
        Core.System.DataManager.UpdateData(fandomInfoData);

        DoActionListener(CMD.fandom);
    }

    private void DoFandomStakeHandler(string myResponseRawData)
    {                
        SCFandomStake fandomStakeData = JsonUtility.FromJson<SCFandomStake>(myResponseRawData);
        Core.System.DataManager.UpdateData(fandomStakeData);

        DoActionListener(CMD.fandomStake);
    }

    

    private void DoBuyLightStickHandler(string myResponseRawData)
    {

        Debug.Log($"DoBuyLightStickHandler > {myResponseRawData}");
        SCBuyLightStickItem data = JsonUtility.FromJson<SCBuyLightStickItem>(myResponseRawData);
        Core.System.DataManager.UpdateData(data);

        DoActionListener(CMD.buyLightStick);
    }


    private void DoFandomShopHandler(string myResponseRawData)
    {
        SCFandomShopData data = JsonUtility.FromJson<SCFandomShopData>(myResponseRawData);
        Core.System.DataManager.UpdateData(data);

        DoActionListener(CMD.fandomShop);
    }

    private void DoPurchaseFandomShopItemHandler(string myResponseRawData)
    {
        SCPurchaseFandomShopItemData data = JsonUtility.FromJson<SCPurchaseFandomShopItemData>(myResponseRawData);
        Core.System.DataManager.UpdateData(data);

        DoActionListener(CMD.purchaseFandomShopItem);
    }

    private void DoFandomLevelTableHandler(string myResponseRawData)
    {        
        SCFandomLevelsData data = JsonUtility.FromJson<SCFandomLevelsData>(myResponseRawData);
        Core.System.DataManager.UpdateData(data);

        DoActionListener(CMD.fandomLevels);
    }


    private void DoUserUseStanGemHandler(string myResponseRawData)
    {
        Debug.Log($"DoUserUseStanGemHandler > {myResponseRawData}");

        SCUserUseStanGemData data = JsonUtility.FromJson<SCUserUseStanGemData>(myResponseRawData);
        Core.System.DataManager.UpdateData(data);

        DoActionListener(CMD.userUseStanGem);
    }


    #endregion
}
