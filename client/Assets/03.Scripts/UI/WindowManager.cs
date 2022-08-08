using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tag
{
    public const string StreamingPartyGroup = "StreamingPartyGroup";
}
public class WindowManager
{
    private StreamingPartyForVuplexGroup streamingParty;

    private string noticeTitle;
    internal void SetNoticeWindowData(string myTitle)
    {
        noticeTitle = myTitle;
    }

    internal string GetNoticeWindowData()
    {
        return noticeTitle;
    }

    internal void ShowNotice(string myTitle)
    {
        SetNoticeWindowData(myTitle);

        if (null == streamingParty)
        {
            GameObject streamingPartyGroupObj = GameObject.FindGameObjectWithTag(Tag.StreamingPartyGroup);
            streamingParty = streamingPartyGroupObj.GetComponent<StreamingPartyForVuplexGroup>();
        }

        streamingParty.ShowNotice();
    }

    internal void BeginLightStickActiveTimer()
    {
        if (null == streamingParty)
        {
            GameObject streamingPartyGroupObj = GameObject.FindGameObjectWithTag(Tag.StreamingPartyGroup);
            streamingParty = streamingPartyGroupObj.GetComponent<StreamingPartyForVuplexGroup>();
        }

        streamingParty.BeginLightStickActiveTimer();
    }
}
