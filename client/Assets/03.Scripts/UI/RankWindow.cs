using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankWindow : MonoBehaviour
{
    [SerializeField]
    private UserRankView viewPrefab;

    [SerializeField]
    private UserRankView myRankView;

    private const int MaxViewUserCount = 8;
    private void OnEnable()
    {
        DoInit();
    }

    private void OnDisable()
    {
        viewPrefab.RecycleAll();
    }

    private void DoInit()
    {
        viewPrefab.CreatePool<UserRankView>(10);

        List<User> allUsers = Core.System.DataManager.FandomInfo.GetAllUsers();       
        int viwwCount = (MaxViewUserCount > allUsers.Count) ? allUsers.Count : MaxViewUserCount;
        for (int i = 0; i < viwwCount; i++)
        {
            int rank = i + 1;
            User user = allUsers[i];
            UserRankView rankView = viewPrefab.Spawn<UserRankView>(viewPrefab.transform.parent);
            rankView.gameObject.transform.localScale = Vector3.one;
            rankView.Init(rank, user);
        }

        int meRank = 0;
        for (int i = 0; i < allUsers.Count; i++)
        {
            meRank = i + 1;
            User user = allUsers[i];
            if (user.index == Core.System.DataManager.User.index)
            {
                break;
            }
        }


        myRankView.Init(meRank, Core.System.DataManager.User);
    }

    public void OnClickClose()
    {
        this.gameObject.SetActive(false);
    }
}
