using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardView : MonoBehaviour,IPointerClickHandler
{
    [SerializeField]
    private Image cardImage;

    public CardData Data {get; private set;}

public void SetUp(CardData data)
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

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
    
}