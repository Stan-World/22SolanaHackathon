using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StakingWindow : MonoBehaviour
{
    [SerializeField]
    private Text userStanGem;
    [SerializeField]
    private Text userStakeStanGem;
    [SerializeField]
    private InputField stakingAmount;
    private void Start()
    {
        
    }

    private void OnEnable()
    {
        DoInit();
    }

    private void DoInit()
    {
        stakingAmount.textComponent.text = string.Empty;
        stakingAmount.text = string.Empty;
        userStakeStanGem.text = $"{Core.System.DataManager.User.stake}";
        userStanGem.text = $"{Core.System.DataManager.User.stanGem}";
    }
    public void OnClickOkStakeButton()
    {
        string stakingAmountDataStrig = stakingAmount.text.Trim();
        if (string.IsNullOrEmpty(stakingAmountDataStrig) 
            || string.IsNullOrWhiteSpace(stakingAmountDataStrig) )
        {
            DoShowInputAmountError("stakingAmount value is wrong. check your input value! [001]");
            
            return;
        }

        int amount = 0;
        if (false == int.TryParse(stakingAmountDataStrig, out amount))
        {
            DoShowInputAmountError("stakingAmount value is wrong. check your input value! [002]");
            return;
        }

        if (0 >= amount)
        {
            DoShowInputAmountError($"Please Input Amount more than zero!");
            return;
        }

        if (Core.System.DataManager.User.stanGem < amount)
        {
            DoShowInputAmountError($"Please Need StanGem! Your StanGame is {Core.System.DataManager.User.stanGem} and staking amount is {amount}");
            return;
        }

        Core.System.NetworkManager.StakingStanGem(amount);

        DoClose();
    }

    private void DoShowInputAmountError(string myMessage)
    {
        Debug.LogError(myMessage);
        stakingAmount.ActivateInputField();
    }

    public void OnClickCloseButton()
    {
        DoClose();
    }

    private void DoClose()
    {
        this.gameObject.SetActive(false);
    }
}
