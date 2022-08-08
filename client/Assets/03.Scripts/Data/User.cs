using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User
{
    public string uid;
    public string name;
    public int index;
    public int vitaPoint;
    public int stake;
    public int stanGem;
    public int power;
    public bool online;
    public bool lightStick;
    
    public string ToData()
    {
        return $"{uid} {name} {index} {vitaPoint} {stanGem} {power} {online} {lightStick}";
    }

    internal void Update(SCFandomInfoUser mySCFandomInfoUser)
    {
        uid = mySCFandomInfoUser.uid;
        name = mySCFandomInfoUser.name;
        index = mySCFandomInfoUser.index;
        vitaPoint = mySCFandomInfoUser.vitaPoint;
        stanGem = mySCFandomInfoUser.stanGem;
        stake = mySCFandomInfoUser.stake;
        power = mySCFandomInfoUser.power;
        online = mySCFandomInfoUser.online;
        lightStick = mySCFandomInfoUser.lightStick;
    }

    internal void Update(SCUserUseStanGemData mySCUserUseStanGemData)
    {
        uid = mySCUserUseStanGemData.uid;
        name = mySCUserUseStanGemData.name;
        index = mySCUserUseStanGemData.index;
        vitaPoint = mySCUserUseStanGemData.vitaPoint;
        stanGem = mySCUserUseStanGemData.stanGem;
        stake = mySCUserUseStanGemData.stake;
        power = mySCUserUseStanGemData.power;
        online = mySCUserUseStanGemData.online;
        lightStick = mySCUserUseStanGemData.lightStick;
    }


    
    internal bool CheckLightStick()
    {
        return lightStick;
    }

    internal bool ItMe(SCFandomInfoUser myUser)
    {
        return uid.Equals(myUser.uid);
    }
}
