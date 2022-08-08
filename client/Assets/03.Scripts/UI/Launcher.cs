using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField]
    private GameObject splashGroupObj;
    [SerializeField]
    private GameObject loginGroupObj;
    [SerializeField]
    private GameObject enterStreamingPartyGroupObj;
    [SerializeField]
    private GameObject streamingPartyGroupObj;

    private void Awake()
    {
        Core.System.Init();
    }
    private void Start()
    {
        Dictionary<string, GameObject> screenContainer = new Dictionary<string, GameObject>();
        screenContainer.Add(ScreenTag.SplashGroup, splashGroupObj);
        screenContainer.Add(ScreenTag.LoginGroup, loginGroupObj);
        screenContainer.Add(ScreenTag.EnterStreamingPartyGroup, enterStreamingPartyGroupObj);
        screenContainer.Add(ScreenTag.StreamingPartyGroup, streamingPartyGroupObj);
        Core.System.GameController.Init(screenContainer);    
    }




    private void OnApplicationQuit()
    {
        Core.System.Quit();
    }

}
