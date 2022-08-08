using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StreamingParty
{
    private Dictionary<int, StreamingData> videoList = new Dictionary<int, StreamingData>();
    private int id;
    private int videoIndex = -1;
    private int videoCurrentPlaySecTime = 0;

    private int totalViewCount = 0;
    private int partyViewCount = 0;
    public StreamingParty()
    {        
    }


    internal void UpdateTotalViewCount(int myTotalViewCount)
    {
        totalViewCount = myTotalViewCount;

        DoUpdatePartyViewCount();
    }

    private void DoUpdatePartyViewCount()
    {
        partyViewCount = videoList[videoIndex].viewCount;        
    }

    internal int GetTotalViewCount()
    {
        return totalViewCount;
    }

    internal int GetPartyViewCount()
    {
        return partyViewCount;
    }

    internal int GetCurrentPlaySecTime()
    {
        return videoCurrentPlaySecTime;
    }

    internal int GetCurrentVideoViewCount()
    {
        return videoList[videoIndex].viewCount;
    }

    internal string GetCurrentStreamingURL()
    {
        return videoList[videoIndex].url;
    }

    internal string GetStreamingURL()
    {
        int index = videoIndex + 1;
        if (videoList.Count <= index)
        {
            index = 0;
        }
        videoIndex = index;

        return videoList[videoIndex].url;
    }

    internal void Update(SCFandomInfoParty myParty)
    {
        id = myParty.id;
        videoIndex = myParty.streamingIndex;
        videoCurrentPlaySecTime = myParty.currentTime;
        for (int i = 0; i < myParty.streamingList.Count; i++)
        {
            StreamingData streamingInfo = myParty.streamingList[i];
            StreamingData streamingData = null;
            if (false == videoList.TryGetValue(i, out streamingData))
            {
                streamingData = new StreamingData();
                videoList.Add(i, streamingData);
            }

            streamingData.Update(streamingInfo);
        }   
    }
}
