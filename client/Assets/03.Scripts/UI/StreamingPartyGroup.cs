using LightShaft.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StreamingPartyGroup : MonoBehaviour
{
    [SerializeField]
    private YoutubePlayer player;
    [SerializeField]
    private Text videoTitle;
    [SerializeField]
    private Text currentVideoTime;
    [SerializeField]
    private Text totalVideoTime;
    private void Start()
    {
        player._events.OnVideoReadyToStart.AddListener(DoOnVideoReadyToStart);
        player._events.OnVideoFinished.AddListener(DoOnVideoFinished);
        //player.autoPlayOnStart = true;
        //PlayCustomPlaylist();
        PreLoadTheVideoOnly();
        
    }

    private void DoOnVideoReadyToStart()
    {
        Debug.Log($"DoOnVideoReadyToStart > {player.GetVideoTitle()}");
        videoTitle.text = player.GetVideoTitle();
        player.Play();
    }

    private void DoOnVideoFinished()
    {
        Debug.Log($"DoOnVideoFinished");
        player.Stop();
        PreLoadTheVideoOnly();
    }

    private void Update()
    {       
        currentVideoTime.text = DoFormatTime(Mathf.RoundToInt(player.currentVideoDuration));
        totalVideoTime.text = DoFormatTime(Mathf.RoundToInt(player.totalVideoDuration));
    }

    private string DoFormatTime(int myTime)
    {
        int hours = myTime / 3600;
        int minutes = (myTime % 3600) / 60;
        int seconds = (myTime % 3600) % 60;
        if (hours == 0 && minutes != 0)
        {
            return minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        else if (hours == 0 && minutes == 0)
        {
            return "00:" + seconds.ToString("00");
        }
        else
        {
            return hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }

    public void PreLoadTheVideoOnly()
    {
        //Good for when you want to keep showing the thumbnail to play the video later.
        string youtubeURL = Core.System.DataManager.GetStreamingURL();
        player.PreLoadVideo(youtubeURL);
    }


    //public void PlayFromUrlField()
    //{
    //    //Simple call to start playing
    //    player.Play(urlInput.text);
    //}

    //public void PlayFromUrlFieldStartingAt()
    //{
    //    //Simple call to start playing starting from second. in this case 10 seconds
    //    player.Play(urlInput.text, 10);
    //}

}
