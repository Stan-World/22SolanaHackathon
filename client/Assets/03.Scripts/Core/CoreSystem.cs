using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Core
{
    public static CoreSystem System = new CoreSystem();
}
public class CoreSystem
{
    public DataManager DataManager = new DataManager();
    public GameController GameController = new GameController();
    public NetworkDataManager NetworkDataManager = new NetworkDataManager();
    public NetworkManager NetworkManager = new NetworkManager();
    public WindowManager WindowManager = new WindowManager();
    public ResourceManager ResourceManager = new ResourceManager();

    internal void Init()
    {
        DoInit();
    }

    private void DoInit()
    {
        
    }

    internal void Quit()
    {
        NetworkManager.Quit();
    }
}
