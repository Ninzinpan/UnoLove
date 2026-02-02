using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public abstract class BaseCardView : MonoBehaviour
{
    [SerializeField]
    protected Image cardImage;
    [SerializeField]
    protected Button cardButton;

    public CardData Data {get; protected set;}
    private Action<CardView> onClickAction;


public void SetUp(CardData data, Action<BaseCardView> onClick)
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