using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YoutubeViewCountWindow : MonoBehaviour
{
    [SerializeField]
    private Text totalViewCount;
    [SerializeField]
    private Text partyViewCount;
    void Start()
    {
        totalViewCount.text = $"{Core.System.DataManager.StreamingParty.GetTotalViewCount():#,0}";
        partyViewCount.text = $"{Core.System.DataManager.StreamingParty.GetPartyViewCount():#,0}";
    }

    public void OnClickClose()
    {
        this.gameObject.SetActive(false);
    }
}
