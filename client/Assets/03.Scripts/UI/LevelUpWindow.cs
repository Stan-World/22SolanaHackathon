using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelUpWindow : MonoBehaviour
{
    [SerializeField]
    private Image levelUpImage;
    [SerializeField]
    private Text title;

    private float originPosY;
    private void Awake()
    {
        originPosY = levelUpImage.rectTransform.anchoredPosition.y;
    }
    private void OnEnable()
    {
        title.text = $"Level {Core.System.DataManager.Fandom.level}.";
        DoShowForDuration();
    }

    private void DoShowForDuration()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(levelUpImage.rectTransform.DOAnchorPosY(0f, 1f).SetEase(Ease.InSine));
        sequence.AppendInterval(1f);
        sequence.Append(levelUpImage.rectTransform.DOAnchorPosY(originPosY, 2).SetEase(Ease.OutSine));
        sequence.AppendCallback(DoClose);

    }

    private void DoClose()
    {
        this.gameObject.SetActive(false);
    }
}
