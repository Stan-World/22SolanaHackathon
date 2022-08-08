using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StreamingPartyUpdater
{
    private bool loopFlag = false;
    private bool flagStop = false;
    private const int LoopTimeOutSec = 1000 * 1;

    internal void Begin()
    {
        loopFlag = true;
        flagStop = false;
        Core.System.NetworkDataManager.AddListener(CMD.fandom, DoOnLoadFandomInfo);
        DoLoopUpdate();
    }

    internal void Stop()
    {
        loopFlag = false;
        flagStop = true;
        Core.System.NetworkDataManager.RemoveListener(CMD.fandom, DoOnLoadFandomInfo);
    }

    private void DoOnLoadFandomInfo()
    {
        GameObject streamingPartyGroupObj = GameObject.FindGameObjectWithTag(ScreenTag.StreamingPartyGroup);

        StreamingPartyForVuplexGroup streamingPartyGroup = streamingPartyGroupObj.GetComponent<StreamingPartyForVuplexGroup>();

        streamingPartyGroup.UpdateFandomInfo();
    }

    private async void DoLoopUpdate()
    {
        while (loopFlag)
        {
            await Task.Delay(LoopTimeOutSec);
            if (true == flagStop) break;
            DoUpdate();
        }
    }

    private void DoUpdate()
    {

        NetworkItem item = new NetworkItem(CMD.fandom);
        item.Post();
    }
}
