using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserItem
{
    public int level { get; private set; } = 1;
    public const float ActiveSecTime = 10;
    public int needStanGemAmount { get; } = 50;
    public bool isActive { get; private set; } = false;

    internal void SetActive(bool myActive)
    {
        isActive = myActive;
    }

    internal void SetLevel(int myLevel)
    {
        level = myLevel;
    }

}
