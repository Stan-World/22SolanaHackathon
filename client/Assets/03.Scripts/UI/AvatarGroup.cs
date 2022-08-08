using stanworld;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarGroup : MonoBehaviour
{
    [SerializeField]
    private GameObject[] prefabAvatars;

    private Dictionary<string, NPCController> userAvatars = new Dictionary<string, NPCController>();


    internal void CreateAvatar(User myUser)
    {
        NPCController userAvatar = null;
        if (false == userAvatars.TryGetValue(myUser.uid, out userAvatar))
        {
            if (true == myUser.online)
            {                
                userAvatar = DoCreateAvatar(myUser);
                userAvatar.ForceRandomAnimation();

                userAvatars.Add(myUser.uid, userAvatar);
            }
        }
        else
        {
            userAvatar.gameObject.SetActive(true);            
        }
    }

    internal void UpdateAvatar(User myUser)
    {
        NPCController userAvatar = null;
        if (true == userAvatars.TryGetValue(myUser.uid, out userAvatar))
        {
            if (true == myUser.online && true == userAvatar.gameObject.activeSelf) return;
            else if (false == myUser.online && false == userAvatar.gameObject.activeSelf) return;

            userAvatar.gameObject.SetActive(myUser.online);
        }
        else
        {
            CreateAvatar(myUser);
        }
    }


    private NPCController DoCreateAvatar(User myUser)
    {
        int index = myUser.index % 8;
        GameObject avatarObj = GameObject.Instantiate(prefabAvatars[index], this.transform);
        avatarObj.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        UnityEngine.Random.InitState(myUser.index);
        float xPos = UnityEngine.Random.Range(-2f, 2f);
        float yPos = UnityEngine.Random.Range(0f, 0.2f);
        avatarObj.transform.localPosition = new Vector3(xPos, yPos, 0f);
        avatarObj.transform.localScale = Vector3.one * 0.7f;

        NPCController controller = avatarObj.GetComponent<NPCController>();
        controller.SetName(myUser.name);

        return controller;
    }
    
}
