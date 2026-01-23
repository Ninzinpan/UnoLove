using UnityEngine;
using System.Collections.Generic;

class HandManager : MonoBehaviour
{
    [SerializeField]
    private List<CardData> hand = new List<CardData>();

public void AddCard(CardData card)
    {
        hand.Add(card);
        Debug.Log($"カードを手札に追加しました: {card.name}");
    }
}