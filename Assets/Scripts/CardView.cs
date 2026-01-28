using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CardView : MonoBehaviour
{
    [SerializeField]
    private Image cardImage;
    [SerializeField]
    private Button cardButton;

    public CardData Data {get; private set;}
    private Action<CardView> onClickAction;


public void SetUp(CardData data, Action<CardView> onClick)
    {
        if (data == null)
        {
             Debug.LogWarning(" CardView:カードデータが渡されていません。");
             return;
        }
        Data = data;
        if (cardImage != null  && Data.CardImage != null)
        {
            cardImage.sprite = Data.CardImage;
            cardButton.onClick.AddListener(() => {onClick?.Invoke(this);});
        }

    }

    public void Delite()
    {
        
        Destroy(gameObject);
    }

}