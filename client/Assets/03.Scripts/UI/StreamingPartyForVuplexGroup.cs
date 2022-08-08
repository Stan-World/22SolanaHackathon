using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YoutubeControllerForVuplex;
using DG.Tweening;

public class StreamingPartyForVuplexGroup : MonoBehaviour, IYoutubeControllerForVuplex
{
    [SerializeField]
    private YoutubeControllerForVuplex.YoutubeController youtubeController;

    [SerializeField]
    private Text videoTitle;
    [SerializeField]
    private TMP_Text viewCount;
    [SerializeField]
    private Text videoTotalPlayTime;
    [SerializeField]
    private Text videoCurrentTime;
    [SerializeField]
    private TMP_Text userCount;
    [SerializeField]
    private TMP_Text vitaPoint;
    [SerializeField]
    private TMP_Text vitaGold;
    [SerializeField]
    private TMP_Text stanGem;
    [SerializeField]
    private TMP_Text fandomLevel;
    [SerializeField]
    private TMP_Text fandomName;

    [SerializeField]
    private TMP_Text userName;
    [SerializeField]
    private TMP_Text userStanGem;

    [SerializeField]
    private Image fandomStanGemIcon;


    [SerializeField]
    private UserPropertyInfo userPropertyInfo;

    [SerializeField]
    private StakingWindow stakingWindow;
    [SerializeField]
    private YoutubeViewCountWindow youtubeViewCountWindow;
    [SerializeField]
    private UserLightStickItemWindow userLightStickItemWindow;
    [SerializeField]
    private NoticeWindow noticeWindow;
    [SerializeField]
    private LevelUpWindow levelUpWindow;
    [SerializeField]
    private FandomShopWindow fandomShopWindow;
    [SerializeField]
    private RankWindow rankWindow;

    [SerializeField]
    private AvatarGroup avatarGroup;
    [SerializeField]
    private Button userItemButton;
    [SerializeField]
    private Text lightStickLevelText;

    [SerializeField]
    private Button stakingButton;
    [SerializeField]
    private Slider lightStickActiveTimer;
    [SerializeField]
    private Image lightStickExclamationMark;
    [SerializeField]
    private Image lightStickActiveTimerIcon;


    [SerializeField]
    private RawImage fandomStanGemAmountBar;
    private float maxFandomStanGemAmountBarWidth;

    [SerializeField]
    private ParticleSystem levelUpEventParticle;

    [SerializeField]
    private Coffee.UIExtensions.UIParticle uiParticle;

    [SerializeField]
    private Image stanGemIconForStaking;
    [SerializeField]
    private Transform targetStanGemIconForStaking;

    [SerializeField]
    private Image screenMask;

    [SerializeField]
    private TutorialGroup tutorialGroup;

    private DateTime beginDateTime;

    private YoutubeAPIManager youtubeapi;

    private void Start()
    {
        ObjectPool.CreatePool(stanGemIconForStaking.gameObject, 5);

        youtubeapi = this.GetComponent<YoutubeAPIManager>();
        youtubeController.Init(this);

        Core.System.NetworkDataManager.AddListener(CMD.fandomStake, DoOnFandomStake);
        Core.System.NetworkDataManager.AddListener(CMD.buyLightStick, DoOnBuyLightStick);
        Core.System.NetworkDataManager.AddListener(CMD.userUseStanGem, DoOnUserUseStanGem);

        maxFandomStanGemAmountBarWidth = fandomStanGemAmountBar.rectTransform.sizeDelta.x;
        DoInitUI();

        
        tutorialGroup.gameObject.SetActive(!Core.System.DataManager.User.lightStick);
    }

    private void DoInitUI()
    {
        DoUpdateUserCount();

        string initString = "---";
        videoTitle.text = initString;
        videoCurrentTime.text = initString;
        videoTotalPlayTime.text = initString;
        viewCount.text = $"---";

        //vitaPoint.text = initString;
        //vitaGold.text = initString;
        //stanGem.text = initString;
        //fandomLevel.text = initString;



        fandomName.text = Core.System.DataManager.Fandom.name;
        userName.text = Core.System.DataManager.User.name;
        userPropertyInfo.UpdateInfo(Core.System.DataManager.User);
        DoUpdateUserStanGem();
        DoCreateAvatars();

        DoUpdateFandomInfo();
        DoUpdateFandomStanGem();

        // 구매버튼.
        userItemButton.interactable = false; // Core.System.DataManager.User.lightStick;
        lightStickExclamationMark.gameObject.SetActive(false);

        stakingButton.interactable = Core.System.DataManager.User.lightStick;
        

        DoUpdateLightStickLevel();

        DoCheckLightStickActiveTimer();

        
    }

    private void DoUpdateFandomStanGemAmountBar()
    {
        float ratio = 1f;
        int currentFandomLevel = Core.System.DataManager.Fandom.level;
        bool isMaxFandomLevel = Core.System.DataManager.IsMaxFandomLevel(currentFandomLevel);
        if (false == isMaxFandomLevel)
        {
            FandomLevel fandomLevelData = Core.System.DataManager.GetFandomLevelData(currentFandomLevel);

            ratio = Core.System.DataManager.Fandom.stanGem / (float)fandomLevelData.maxGem;
             
        }
       
        Vector2 size = fandomStanGemAmountBar.rectTransform.sizeDelta;
        size.x = maxFandomStanGemAmountBarWidth * ratio;
        fandomStanGemAmountBar.rectTransform.sizeDelta = size;
    }

    private void DoCheckLightStickActiveTimer()
    {
        if (true == Core.System.DataManager.User.lightStick)
        {
            DoEnableLightStickUseButton();
            return;
        }

        
    }

    internal void BeginLightStickActiveTimer()
    {
        DoBeginLightStickActiveTimer();
    }

    private void DoBeginLightStickActiveTimer()
    {
        lightStickActiveTimer.DOValue(0f, UserItem.ActiveSecTime).OnComplete(() =>
        {
            DoEnableLightStickUseButton();
        });

        lightStickActiveTimerIcon.rectTransform.DOShakePosition(1f, 5f).SetLoops(-1);
    }

    private void DoEnableLightStickUseButton()
    {
        userItemButton.interactable = true;
        lightStickExclamationMark.gameObject.SetActive(true);
        lightStickActiveTimer.gameObject.SetActive(false);
    }



    private void DoUpdateLightStickLevel()
    {
        lightStickLevelText.text = $"Lv.{Core.System.DataManager.UserItem.level}";
    }


    private void DoOnBuyLightStick()
    {        
        Core.System.NetworkDataManager.RemoveListener(CMD.buyLightStick, DoOnBuyLightStick);
        stakingButton.interactable = Core.System.DataManager.User.lightStick;
    }

    private void DoOnUserUseStanGem()
    {
        DoUpdateUserStanGem();
    }

    private void DoCreateAvatars()
    {
        Dictionary<string, User>.Enumerator it = Core.System.DataManager.FandomInfo.userList.GetEnumerator();

        while(it.MoveNext())
        {
            avatarGroup.CreateAvatar(it.Current.Value);
        }
    }

    private void DoUpdateAvatars()
    {
        Dictionary<string, User>.Enumerator it = Core.System.DataManager.FandomInfo.userList.GetEnumerator();

        while (it.MoveNext())
        {
            avatarGroup.UpdateAvatar(it.Current.Value);
        }
    }

    private void DoPlayCurrentVideo()
    {
        string youtubeURL = Core.System.DataManager.GetCurrentStreamingURL();
        string youtubeID = youtubeController.GetYoutubeID(youtubeURL);
        //Debug.Log($"DoPlayVideo > {youtubeID}");
        int currentPlaySecTime = Core.System.DataManager.StreamingParty.GetCurrentPlaySecTime();
        youtubeController.Set(youtubeID, currentPlaySecTime);

        beginDateTime = DateTime.Now;
    }

    private void DoCheckStreamingParty()
    {
        Core.System.NetworkDataManager.AddListener(CMD.fandom, DoOnLoadFandomInfo);
        Core.System.NetworkManager.Fandom();
    }

    private void DoOnLoadFandomInfo()
    {
        Core.System.NetworkDataManager.RemoveListener(CMD.fandom, DoOnLoadFandomInfo);

        DoUpdateViewCount();
        DoPlayCurrentVideo();
        UpdateFandomInfo();
        
        Core.System.NetworkManager.BeginStreamingPartyUpdater();
    }

    private void DoOnFandomStake()
    {
        DoUpdateFandomInfo();

        DoLevelUpEvent();
    }

    private void DoLevelUpEvent()
    {
        if (true == Core.System.DataManager.Fandom.IsLevelUp())
        {
            levelUpWindow.gameObject.SetActive(true);
            uiParticle.Play();
        }
    }


    private void DoStakingEvent()
    {
        GameObject stanGemIcon = stanGemIconForStaking.gameObject.Spawn(stanGemIconForStaking.transform.parent);
        stanGemIcon.SetActive(true);
        stanGemIcon.transform.localScale = Vector3.one;
        stanGemIcon.transform.position = stanGemIconForStaking.transform.position;
        stanGemIcon.transform.DOJump(targetStanGemIconForStaking.position, 2.5f, 1, 1f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                stanGemIcon.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => { stanGemIcon.Recycle(); });
                DoUpdateFandomStanGem();
                DoShakeFandomStanGem();
            });
    }

    private void DoShakeFandomStanGem()
    {
        fandomStanGemIcon.rectTransform.DOShakePosition(1f, 4f);
    }

    private void DoUpdateUserCount()
    {
        userCount.text = Core.System.DataManager.FandomInfo.GetOnlineUserCountString();
    }

    private void DoUpdateVideoCurrentTime(int myCurrentTime)
    {
        videoCurrentTime.text = $"{DoFormatTime(myCurrentTime)}";
    }

    private void DoUpdateTotalVideoCurrentTime(int myTotalPlaySecTime)
    {
        videoTotalPlayTime.text = $"{DoFormatTime(myTotalPlaySecTime)}";
    }

    private void DoUpdateViewCount()
    {
        int viewCountValue = Core.System.DataManager.StreamingParty.GetCurrentVideoViewCount();
        viewCount.text = $"{viewCountValue}";
    }

    internal void UpdateFandomInfo()
    {
        DoUpdateAvatars();
        DoUpdateUserCount();

        DoUpdateFandomInfo();
        DoUpdateFandomStanGem();
        youtubeController.GetCurrentTime();
    }

    private void DoUpdateFandomInfo()
    {
        vitaPoint.text = $"{Core.System.DataManager.Fandom.vitaPoint:#,0}";
        vitaGold.text = $"{Core.System.DataManager.Fandom.vitaGold:#,0}";

        DoUpdateUserStanGem();

    }

    private void DoUpdateFandomStanGem()
    {
        stanGem.text = DoGetFandomStanGemText();
        fandomLevel.text = $"Lv.{Core.System.DataManager.Fandom.level}";
        DoUpdateFandomStanGemAmountBar();

    }

    private string DoGetFandomStanGemText()
    {
        int currentFandomLevel = Core.System.DataManager.Fandom.level;
        bool isMaxFandomLevel = Core.System.DataManager.IsMaxFandomLevel(currentFandomLevel);
        if (false == isMaxFandomLevel)
        {
            FandomLevel fandomLevelData = Core.System.DataManager.GetFandomLevelData(currentFandomLevel);
            return $"{Core.System.DataManager.Fandom.stanGem:#,0}/{fandomLevelData.maxGem:#,0}";
        }
        else
        {
            return $"{Core.System.DataManager.Fandom.stanGem:#,0}";
        }
    }

    private void DoUpdateUserStanGem()
    {
        userStanGem.text = $"x{Core.System.DataManager.User.stanGem:#,0}";
    }


    public void OnClickYoutubeViewCountButton()
    {
        string youtubeURL = Core.System.DataManager.GetCurrentStreamingURL();
        string youtubeID = youtubeController.GetYoutubeID(youtubeURL);
        DoRequestViewCount(youtubeID);
        
    }

    private void DoRequestViewCount(string myYoutubeID)
    {
        youtubeapi.GetVideoData(myYoutubeID, DoOnFinishLoadingData);
    }

    private void DoOnFinishLoadingData(YoutubeData result)
    {
        Debug.Log($"Views: {result.statistics.viewCount}");
        int totalViewCount = int.Parse(result.statistics.viewCount.Trim());
        Core.System.DataManager.StreamingParty.UpdateTotalViewCount(totalViewCount);
        youtubeViewCountWindow.gameObject.SetActive(true);
    }

    public void OnClickStakeButton()
    {
        
        int amount = 1;
        if (Core.System.DataManager.User.stanGem < amount)
        {
            Debug.Log($"Please Need StanGem! Your StanGame is {Core.System.DataManager.User.stanGem} and staking amount is {amount}");
            return;
        }

        Core.System.NetworkManager.StakingStanGem(amount);
        DoStakingEvent();
    }

    public void OnClickFandomShop()
    {
        fandomShopWindow.gameObject.SetActive(true);
    }

    private string DoFormatTime(int time)
    {
        int hours = time / 3600;
        int minutes = (time % 3600) / 60;
        int seconds = (time % 3600) % 60;
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

    public void OnClickUserItem()
    {
        userLightStickItemWindow.gameObject.SetActive(true);
        
    }

    public void OnClickUserRankButton()
    {
        rankWindow.gameObject.SetActive(true);
    }
    internal void ShowNotice()
    {
        noticeWindow.gameObject.SetActive(true);
    }

    #region IYoutubeControllerForVuplex
    void IYoutubeControllerForVuplex.OnInitWebViewControll()
    {
        DoUpdateViewCount();
        DoPlayCurrentVideo();        
        Core.System.NetworkManager.BeginStreamingPartyUpdater();

    }

    void IYoutubeControllerForVuplex.OnPlayerReady(string myState)
    {
        Debug.Log($"OnPlayerReady > {myState}");
        youtubeController.ForceClick();
        TimeSpan timeSpan = DateTime.Now - beginDateTime;

        if (1 <= timeSpan.TotalSeconds)
        {
            int currentPlaySecTime = Core.System.DataManager.StreamingParty.GetCurrentPlaySecTime();
            int currentSeekSecTime = currentPlaySecTime + (int)timeSpan.TotalSeconds;
            youtubeController.Seek(currentSeekSecTime);
        }


        screenMask.gameObject.SetActive(false);

    }
    void IYoutubeControllerForVuplex.OnGetTitle(string myTitle)
    {
        videoTitle.text = myTitle;
    }    

    void IYoutubeControllerForVuplex.OnError(string myErrorLog)
    {
        Debug.LogError($"IYoutubeControllerForVuplex.OnError > {myErrorLog}");
    }

    void IYoutubeControllerForVuplex.OnVideoEnded()
    {
        Debug.Log($"OnVideoEnded");
        Core.System.NetworkManager.StopStreamingPartyUpdater();
        DoCheckStreamingParty();
        
    }

    void IYoutubeControllerForVuplex.OnPlayerStateChange(string myState)
    {
        Debug.Log($"OnPlayerStateChange > {myState}");
    }

    void IYoutubeControllerForVuplex.OnIsMute(bool myIsMute)
    {

    }

    void IYoutubeControllerForVuplex.OnGetVolume(int myVolume)
    {

    }

    void IYoutubeControllerForVuplex.OnGetPlayerState(int state)
    {

    }
    void IYoutubeControllerForVuplex.OnGetTotalPlaySecTime(int myTotalPlaySecTime)
    {
        DoUpdateTotalVideoCurrentTime(myTotalPlaySecTime);        
    }

    void IYoutubeControllerForVuplex.OnGetCurrentTime(int myCurrentTime)
    {
        DoUpdateVideoCurrentTime(myCurrentTime);

        
    }

    #endregion
}
