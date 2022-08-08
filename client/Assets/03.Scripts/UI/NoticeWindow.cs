using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class NoticeWindow : MonoBehaviour
{
    [SerializeField]
    private Text title;       

    private void OnEnable()
    {
        title.text = Core.System.WindowManager.GetNoticeWindowData();

        DoShowForDuration();
    }

    private async void DoShowForDuration()
    {
        await Task.Delay(1000 * 2);

        DoClose();
    }

    private void DoClose()
    {
        this.gameObject.SetActive(false);
    }
}
