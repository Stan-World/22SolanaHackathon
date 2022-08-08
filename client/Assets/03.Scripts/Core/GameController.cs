using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScreenTag
{
    public const string SplashGroup = "SplashGroup";
    public const string LoginGroup = "LoginGroup";
    public const string EnterStreamingPartyGroup = "EnterStreamingPartyGroup";
    public const string StreamingPartyGroup = "StreamingPartyGroup";
}


public class GameController
{

    private Dictionary<string, GameObject> screenContainer;


    private string currentScreen = ScreenTag.SplashGroup;
    internal void Init(Dictionary<string, GameObject> myScreens)
    {
        screenContainer = myScreens;
        DoInit();

        DoInitScreen(currentScreen);
    }

    internal void SetScreen(string myScreenTag)
    {
        DoInitScreen(myScreenTag);
    }

    private void DoInit()
    {
        

    }

    private void DoInitScreen(string myScreenTag)
    {
        Dictionary<string, GameObject>.Enumerator it = screenContainer.GetEnumerator();

        while(it.MoveNext())
        {
            bool active = it.Current.Key.Equals(myScreenTag);            
            it.Current.Value.SetActive(active);
        }
    }
}
