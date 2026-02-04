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
    protected Action<BaseCardView> onClickAction;


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
        }
        if (cardButton != null && onClick != null)
        {
            onClickAction = onClick;
            cardButton.onClick.AddListener(() => onClickAction?.Invoke(this));
            
        }
        else
        {
            Debug.LogWarning("CardView:カードのクリックアクションが設定されていません。");
        }

    }

    public void Delite()
    {
        
        Destroy(gameObject);
    }

}