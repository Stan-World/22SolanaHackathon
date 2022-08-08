using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginGroup : MonoBehaviour
{
    [SerializeField]
    private Button loginButton;
    [SerializeField]
    private InputField nickNameText;
    private void Start()
    {
        loginButton.interactable = true;

        Core.System.NetworkDataManager.AddListener(CMD.login, DoOnLogin);

        DoInit();
    }

    private void DoInit()
    {        
        nickNameText.text = Core.System.DataManager.GetUserName();
    }

    private void DoOnLogin()
    {        
        Core.System.NetworkDataManager.RemoveListener(CMD.login, DoOnLogin);
        Core.System.DataManager.UpdateLoginData();
        Core.System.GameController.SetScreen(ScreenTag.EnterStreamingPartyGroup);
    }

    public void OnClickLoginButton()
    {
        string nickName = nickNameText.text.Trim();
        if (string.IsNullOrEmpty(nickName) || string.IsNullOrWhiteSpace(nickName))
        {
            Debug.LogError($"Your Nick Name is not acceptable! Check Nick Name!");
            return;
        }
        Core.System.NetworkManager.Login(nickName);
    }
}
