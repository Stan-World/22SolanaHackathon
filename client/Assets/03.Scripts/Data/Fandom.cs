using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fandom
{
    public int id;
    public string name;
    public int adjust;
    public int vitaPoint;
    public int vitaGold;
    public int stanGem;
    public int level;


    private bool isLevelUp = false;
    public void SetLevelUp(bool myIsLevelUp)
    {
        isLevelUp = myIsLevelUp;
    }

    public bool IsLevelUp()
    {
        return isLevelUp;
    }
    public string ToData()
    {
        return $"{id} {name} {adjust} {vitaPoint} {vitaGold} {stanGem} {level}";
    }
}

public class FandomInfo
{
    public Dictionary<string, User> userList = new Dictionary<string, User>();
    private List<User> allUsers = new List<User>();
    private List<User> onlineUserList = new List<User>();
    private List<User> offlineUserList = new List<User>();
    internal void UpdateUser(SCFandomInfoUser myCFandomInfoUser)
    {
        User user = null;
        if (false == userList.TryGetValue(myCFandomInfoUser.uid, out user))
        {
            user = new User();
            userList.Add(myCFandomInfoUser.uid, user);

            allUsers.Add(user);
        }

        user.Update(myCFandomInfoUser);        
    }

    internal void UpdateOnlineUser()
    {
        onlineUserList.Clear();
        offlineUserList.Clear();

        Dictionary<string, User>.Enumerator it = userList.GetEnumerator();
        while(it.MoveNext())
        {
            User user = it.Current.Value;

            if (user.online)
            {
                onlineUserList.Add(user);
            }
            else
            {
                offlineUserList.Add(user);
            }
        }

        onlineUserList.Sort(DoCompareDinosByPower);
        allUsers.Sort(DoCompareDinosByPower);
        //for (int i = 0; i < onlineUserList.Count; i++)
        //{
        //    Debug.Log(onlineUserList[i].ToData());
        //}
    }

    private static int DoCompareDinosByPower(User x, User y)
    {
        if (x == null)
        {
            if (y == null)
            {
                // If x is null and y is null, they're
                // equal.
                return 0;
            }
            else
            {
                // If x is null and y is not null, y
                // is greater.
                return 1;
            }
        }
        else
        {
            // If x is not null...
            //
            if (y == null)
            // ...and y is null, x is greater.
            {
                return -1;
            }
            else
            {
                // ...and y is not null, compare the
                // lengths of the two strings.
                //
                int retval = x.power.CompareTo(y.power);

                if (retval != 0)
                {
                    // If the strings are not of equal length,
                    // the longer string is greater.
                    //
                    return -retval;
                }
                else
                {
                    // If the strings are of equal length,
                    // sort them with ordinary string comparison.
                    //
                    return x.name.CompareTo(y.name);
                }
            }
        }
    }

    internal string GetOnlineUserCountString()
    {
        return $"{onlineUserList.Count}";
    }

    internal List<User> GetAllUsers()
    {
        return allUsers;
    }
}
