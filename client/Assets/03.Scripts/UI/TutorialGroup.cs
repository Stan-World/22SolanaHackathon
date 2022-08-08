using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public class TutorialGroup : MonoBehaviour
{
    [SerializeField]
    private Image arrow;

    private Vector2 firstPos;
    private Vector2 endPos;
    private void Start()
    {
        firstPos = arrow.rectTransform.anchoredPosition;
        endPos = firstPos + new Vector2(100f, -100f);
        DoArrow();
        DoWaiting();
    }


    private void DoArrow()
    {
        Sequence sequence = DOTween.Sequence();
        
        sequence.Append(arrow.rectTransform.DOAnchorPos(endPos, 1f));
        sequence.Append(arrow.rectTransform.DOAnchorPos(firstPos, 1f));
        sequence.SetLoops(-1);
    }

    private async void DoWaiting()
    {
        await Task.Delay(1000 * 5);

        this.gameObject.SetActive(false);

        Core.System.WindowManager.BeginLightStickActiveTimer();
    }

   
}
