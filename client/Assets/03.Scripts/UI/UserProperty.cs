using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserProperty : MonoBehaviour
{
    [SerializeField]
    private Text titleLabel;
    
    private Image barImage;

    private float maxHeight;
    private void Awake()
    {
        barImage = this.GetComponent<Image>();
        maxHeight = barImage.rectTransform.sizeDelta.y;
    }
   
    internal void Init(string myTitle)
    {
        titleLabel.text = myTitle;
    }

    internal void UpdateRatio(float myRatio)
    {
        Vector2 size = barImage.rectTransform.sizeDelta;
        size.y = maxHeight * myRatio;
        barImage.rectTransform.sizeDelta = size;
    }
}
