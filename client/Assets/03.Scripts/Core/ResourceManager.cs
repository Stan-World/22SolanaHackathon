using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ResourceManager
{
    private SpriteAtlas fandomShopItemAtlas;
    public ResourceManager()
    {
        fandomShopItemAtlas = Resources.Load<SpriteAtlas>("SpriteAtlas/FandomShopItem");
    }
    internal Sprite GetSprite(string mySpriteName)
    {
        return fandomShopItemAtlas.GetSprite(mySpriteName);
    }
}
