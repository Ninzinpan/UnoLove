using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Threading.Tasks;

public class FieldCardView : BaseCardView
{
    
 


    public void UpdateFieldCard(CardData data)
    {
        if (data != null)
        {
            Data = data;
        if (cardImage != null && data.CardImage != null)
        {
            cardImage.sprite = data.CardImage;
            Debug.Log($"フィールドカードの画像を更新しました: {data.name}");
        }        }
    }





}