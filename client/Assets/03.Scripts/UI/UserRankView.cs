using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserRankView : MonoBehaviour
{
    [SerializeField]
    private Text rankText;

    [SerializeField]
    private Text userNameText;

    [SerializeField]
    private Text powerText;

    internal void Init(int myRank, User myUser)
    {
        rankText.text = $"{myRank}";
        userNameText.text = $"{myUser.name}";
        powerText.text = $"{myUser.power}";
    }
}
