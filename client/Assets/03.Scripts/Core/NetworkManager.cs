using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class CMD
{
    public const string login = "login";
    public const string ping = "ping";
    public const string fandom = "fandom";
    public const string fandomStake = "fandom/stake";
    public const string logout = "login/off";
    public const string buyLightStick = "user/light-stick";
    public const string fandomShop = "fandom/shop";
    public const string purchaseFandomShopItem = "fandom/purchase";
    public const string fandomLevels = "fandom/levels";
    public const string userUseStanGem = "user/use-gem";
}

[Serializable]
public class BasePacket
{
    public string cmd;
}

[Serializable]
public class CSPing : BasePacket
{    
}

[Serializable]
public class SCPing : BasePacket
{
}

[Serializable]
public class CSLogin : BasePacket
{    
    public string uid;
    public string name;
}

[Serializable]
public class SCLogin : BasePacket
{
    public string uid;
    public string name;
    public int index;
    public int vitaPoint;
    public int stanGem;
    public int stake;
    public int power;
    public bool online;
    public bool lightStick;
    public SCFandom fandom;

    public string ToData()
    {
        return $"{cmd} | {uid} | {index} | {stanGem} | {power} | {fandom.ToData()}";
    }
}

[Serializable]
public class SCFandom : BasePacket
{
    public int id;
    public string name;
    public int adjust;
    public int vitaPoint;
    public int vitaGold;
    public int stanGem;
    public int level;

    public string ToData()
    {
        return $"{id} | {name} | {vitaPoint} | {vitaGold} | {stanGem} | {level}";
    }
}


[SerializeField]
public class CSFandomInfo : BasePacket
{
}

[Serializable]
public class SCFandomInfoUser
{
    public string uid;
    public string name;
    public int index;
    public int vitaPoint;
    public int stanGem;
    public int stake;
    public int power;
    public bool online;
    public bool lightStick;

    public string ToData()
    {
        return $"{uid} {name} {stanGem} {power} {online}";
    }
}

[Serializable]
public class SCFandomInfoParty
{
    public int id;
    public int streamingIndex;
    public int currentTime;
    public List<StreamingData> streamingList;

    public string ToData()
    {
        return $"{id} {streamingIndex} {currentTime}";
    }
}

[Serializable]
public class StreamingData
{
    public string url;
    public int runTime;
    public int viewCount;

    public string ToData()
    {
        return $"{url} {runTime} {viewCount}";
    }

    internal void Update(StreamingData myStreamingInfo)
    {
        url = myStreamingInfo.url;
        runTime = myStreamingInfo.runTime;
        viewCount = myStreamingInfo.viewCount;
    }
}

[Serializable]
public class SCFandomInfo : BasePacket
{
    public int id;
    public string name;
    public int adjust;
    public int vitaPoint;
    public int vitaGold;
    public int stanGem;
    public int level;
    public List<SCFandomInfoUser> users;
    public SCFandomInfoParty party;

    public string ToData()
    {
        return $"{id} | {name} | {vitaPoint} | {vitaGold} | {stanGem} | {level}";
    }
}

[Serializable]
public class SCFandomStake : BasePacket
{
    public string uid;
    public string name;
    public int index;
    public int vitaPoint;
    public int stanGem;
    public int stake;
    public int power;
    public bool online;
    public bool lightStick;
    public SCFandom fandom;

    public string ToData()
    {
        return $"{cmd} | {uid} | {stanGem} | {power} | {fandom.ToData()}";
    }
}

[Serializable]
public class SCBuyLightStickItem : BasePacket
{
    public bool lightStick;

    public string ToData()
    {
        return $"{cmd} | {lightStick}";
    }
}

[Serializable]
public class FandomShopItem
{
    public string id;
    public string name;
    public int price;
}

[Serializable]
public class SCFandomShopData : BasePacket
{
    public List<FandomShopItem> shop;
}

[Serializable]
public class SCPurchaseFandomShopItemData : BasePacket
{
    public List<FandomShopItem> items;
}

[Serializable]
public class SCFandomLevelsData : BasePacket
{
    public List<FandomLevel> table;
}

[Serializable]
public class FandomLevel
{
    public int level;
    public int minGem;
    public int maxGem;
    public int vitaPointRatio;
    public int vitaGoldRatio;
    public int stanGemRatio;
    public int time;
    public int vitaPointRate;
}

[Serializable]
public class SCUserUseStanGemData : BasePacket
{
    public string uid;
    public string name;
    public int index;
    public int vitaPoint;
    public int stanGem;
    public int stake;
    public int power;
    public bool online;
    public bool lightStick;
}

public class NetworkItem
{
    private string cmd;
    private string sendJsonString;
    private Dictionary<string, string> postData;
    private UnityWebRequest request;
    private UnityWebRequestAsyncOperation reqAsyncOp;
    public NetworkItem(string myCMD, Dictionary<string, string> myPostData)
    {
        cmd = myCMD;
        postData = myPostData;
    }

    public NetworkItem(string myCMD)
    {
        cmd = myCMD;
        postData = new Dictionary<string, string>(); ;
        AddPostData("cmd", cmd);
    }

    internal void Post()
    {
        DoPost();

        Core.System.NetworkManager.CheckTimeOutPost();
    }

    internal void AddPostData(string myKey, string myValue)
    {
        postData.Add(myKey, myValue);
    }

    internal void AddPostData(string myKey, int myValue)
    {
        postData.Add(myKey, $"{myValue}");
    }

    private void DoPost()
    {
        string url = $"YOUR SERVER ADDRESS/{cmd}";
        
        request = UnityWebRequest.Post(url, postData);        
        //request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");        
        reqAsyncOp = request.SendWebRequest();

        DoAysncRequest();
    }

    private async void DoAysncRequest()
    {        
        while (!reqAsyncOp.isDone)
        {
            await Task.Delay(1);
        }
        DoResult();
    }

    private void DoResponse(string myResponseRawData)
    {
        Core.System.NetworkDataManager.ResponseData(cmd, myResponseRawData);
    }

    private void DoResult()
    {

        //Debug.Log($"{request.responseCode} - { request.downloadHandler.text}");

        switch (request.responseCode)
        {
            case 200:
                {
                    DoResponseResult();
                }
                break;
            case 401:
                {
                    // TODO try to login again!
                }
                break;
            default:
                {
                    // TODO Try to handle a error
                    
                }
                break;
        }
        
    }

    private void DoResponseResult()
    {
        switch (request.result)
        {
            case UnityWebRequest.Result.Success:
                {
                    DoResponse(request.downloadHandler.text);
                }
                break;

            case UnityWebRequest.Result.InProgress:
                {

                }
                break;
            //
            // Summary:
            //     Failed to communicate with the server. For example, the request couldn't connect
            //     or it could not establish a secure channel.
            case UnityWebRequest.Result.ConnectionError:
            //
            // Summary:
            //     The server returned an error response. The request succeeded in communicating
            //     with the server, but received an error as defined by the connection protocol.
            case UnityWebRequest.Result.ProtocolError:
            //
            // Summary:
            //     Error processing data. The request succeeded in communicating with the server,
            //     but encountered an error when processing the received data. For example, the
            //     data was corrupted or not in the correct format.
            case UnityWebRequest.Result.DataProcessingError:
                {
                    Debug.LogError($"UnityWebRequest Error > {request.result}");
                }
                break;
        }
    }
    
}



public class NetworkManager
{
    private const string BaseURL = "http://";
    private Ping ping = new Ping();
    private StreamingPartyUpdater streamingPartyUpdater = new StreamingPartyUpdater();
    
    
    
    public NetworkManager()
    {
        DoInit();
    }

    private void DoInit()
    {

    }

    internal void Login(string myNickName)
    {
        ping.Begin();

        NetworkItem item = new NetworkItem(CMD.login);
        string uid = SystemInfo.deviceUniqueIdentifier;

#if UNITY_EDITOR
        uid = "RenTest001";
#endif
        item.AddPostData("uid", uid);
        item.AddPostData("name", myNickName);
        item.Post();
    }

    internal void Fandom()
    {
        NetworkItem item = new NetworkItem(CMD.fandom);
        item.Post();
    }

    internal void FandomShop()
    {
        NetworkItem item = new NetworkItem(CMD.fandomShop);
        item.Post();
    }

    internal void Quit()
    {
        DoLogOut();
        DoRelease();
    }

    private void DoLogOut()
    {        
        if (true == Core.System.DataManager.User.online)
        {
            NetworkItem item = new NetworkItem(CMD.logout);
            item.AddPostData("uid", Core.System.DataManager.User.uid);
            item.Post();
        }
    }

    private void DoRelease()
    {
        if (null != ping)
        {
            ping.Stop();
        }

        ping = null;

        if (null != streamingPartyUpdater)
        {
            StopStreamingPartyUpdater();
        }

        streamingPartyUpdater = null;
    }

    

    internal void BeginStreamingPartyUpdater()
    {
        streamingPartyUpdater.Begin();
    }

    internal void StopStreamingPartyUpdater()
    {        
        streamingPartyUpdater.Stop();        
    }

    internal void StakingStanGem(int myAmount)
    {        
        NetworkItem item = new NetworkItem(CMD.fandomStake);        
        item.AddPostData("amount", myAmount);
        item.Post();
    }

    internal void CheckTimeOutPost()
    {
        if (null != ping)
        {
            ping.CheckTimeOutPost();
        }
        
    }

    internal void BuyLightStick()
    {
        NetworkItem item = new NetworkItem(CMD.buyLightStick);        
        item.Post();
    }

    internal void PurchaseFandomShopItem(FandomShopItem myFandomShopItem)
    {
        NetworkItem item = new NetworkItem(CMD.purchaseFandomShopItem);
        item.AddPostData("itemId", myFandomShopItem.id);
        item.Post();
    }

    internal void FandomLevelTable()
    {
        NetworkItem item = new NetworkItem(CMD.fandomLevels);        
        item.Post();
    }

    internal void UserUseStanGem(int myUseStanGemAmount)
    {
        NetworkItem item = new NetworkItem(CMD.userUseStanGem);
        item.AddPostData("amount", myUseStanGemAmount);
        item.Post();
    }
}
