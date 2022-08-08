using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Ping
{
    private bool loopPing = false;
    private bool flagStop = false;
    private const int PingTimeOutSec = 1000 * 4;
    private DateTime timeOutPost = DateTime.Now;
    private bool breakLoopTimeOut = false;
    internal void Begin()
    {
        breakLoopTimeOut = false;
        DoLoopTimeOut();
    }
    private async void DoLoopTimeOut()
    {
        while (true)
        {
            await Task.Delay(10);
            if (true == breakLoopTimeOut)
            {
                break;
            }
            TimeSpan elaspedTime = DateTime.Now - timeOutPost;

            if (PingTimeOutSec <= elaspedTime.TotalMilliseconds)
            {
                DoPing();
            }
        }
    }

    private void DoBegin()
    {        
        loopPing = true;
        flagStop = false;

        DoLoopPing();
    }

    internal void Stop()
    {
        breakLoopTimeOut = true;

        loopPing = false;
        flagStop = true;        

        DoLoopPing();
    }

    private async void DoLoopPing()
    {
        while (loopPing)
        {
            await Task.Delay(PingTimeOutSec);
            if (true == flagStop) break;
            DoPing();
        }
    }

    private void DoPing()
    {
        NetworkItem item = new NetworkItem(CMD.ping);        
        item.Post();
    }

    internal void CheckTimeOutPost()
    {        
        timeOutPost = DateTime.Now;
    }
}
