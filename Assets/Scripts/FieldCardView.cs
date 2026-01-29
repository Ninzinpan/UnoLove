using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Threading.Tasks;

public class FieldCardView : MonoBehaviour
{
    [SerializeField]
    private Image cardImage;
    [SerializeField]
    private Button cardButton;

    public CardData Data {get; private set;}
    private Action<FieldCardView> onClickAction;


public void SetUp(CardData data, Action<FieldCardView> onClick)
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

    public void UpdateImage(CardData data)
    {
        if (cardImage != null && data.CardImage != null)
        {
            cardImage.sprite = data.CardImage;
            Debug.Log($"フィールドカードの画像を更新しました: {data.name}");
        }
    }

    public void Delite()
    {
        
        Destroy(gameObject);
    }

}