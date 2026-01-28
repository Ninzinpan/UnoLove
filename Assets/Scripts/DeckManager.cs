using UnityEngine;
using System.Collections.Generic;


public class DeckManager : MonoBehaviour
{
    [Header("Deck Settings")]

    [Header("Game State")]
    [SerializeField]
    private List<CardData> drawPile = new List<CardData>();
    private List<CardData> discardPile = new List<CardData>();
    private void Awake()
    {
    }
    public void SetDeck(List<CardData> startingDeck)
    {
        drawPile = new List<CardData>(startingDeck);
    }


    public void ShuffleDeck()
    {
        for (int i = 0; i < drawPile.Count; i++)
        {
            CardData temp = drawPile[i];
            int randomIndex = Random.Range(i, drawPile.Count);
            drawPile[i] = drawPile[randomIndex];
            drawPile[randomIndex] = temp;
        }
        Debug.Log("デッキをシャッフルしました");
    }
    public CardData DrawCard()
    {
        
            if (drawPile.Count == 0)
            {
                Debug.LogWarning("山札が空です。カードを引けません。");
                return null;
            }
            CardData drawnCard = drawPile[0];
            drawPile.RemoveAt(0);

            Debug.Log($"カードを引きました: {drawnCard.name}");
            return drawnCard;
        
    }
    public void DiscardCard(CardData card)
    {
        discardPile.Add(card);
        Debug.Log($"カードを捨て札にしました: {card.name}");
    }
    public void CleanDiscardPile()
    {
        if (discardPile.Count == 0)
        {
            Debug.LogWarning("捨て札が空です。クリアするカードがありません。");
            return;
        }
    
        discardPile.Clear();
        Debug.Log("捨て札をクリアしました");
    }
}

