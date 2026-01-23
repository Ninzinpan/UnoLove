using UnityEngine;
using System.Collections.Generic;

class HandManager : MonoBehaviour
{
    [SerializeField]
    private List<CardData> hand = new List<CardData>();
    [SerializeField]
    private GameObject cardPrehub;
    [SerializeField]
    private Transform handArea;
public void AddCard(CardData card)
    {
        hand.Add(card);
        GameObject cardView = Instantiate(cardPrehub,handArea);
        CardView view = cardView.GetComponent<CardView>();
        view.SetUp(card);
        Debug.Log($"カードを手札に追加しました: {card.name}");
    }
public bool RemoveCard(CardData card)
    {
        if (hand.Remove(card))
        {
            Debug.Log($"カードを手札から削除しました: {card.name}");
            return true;
        }
        else
        {
            Debug.LogWarning($"手札にカードが見つかりません: {card.name}");
            return false;
        }
    }
}