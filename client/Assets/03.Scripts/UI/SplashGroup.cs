using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SplashGroup : MonoBehaviour
{
    private const int SplashWaitingSecTime = 1000 * 5;
    private void Start()
    {
        DoNextScreen();
    }

    private async void DoNextScreen()
    {
        await Task.Delay(SplashWaitingSecTime);        

        Core.System.GameController.SetScreen(ScreenTag.LoginGroup);
    }

    
}
