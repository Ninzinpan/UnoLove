using UnityEngine;
using System.Collections.Generic;


public class DeckManager : MonoBehaviour
{
    [Header("Deck Settings")]
    [SerializeField] HandManager handManager;
    [SerializeField] DuelistManager duelistManager;

    [Header("Game State")]
    [SerializeField]
    private List<CardData> drawPile = new List<CardData>();
    private List<CardData> discardPile = new List<CardData>();
    private void Awake()
    {
        InistializeDeck();
    }
    private void InistializeDeck()
    {
        drawPile = new List<CardData>(duelistManager.StartingDeck);
        ShuffleDeck();
        DrawCard(3);
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
    public void DrawCard(int count)
    {
        for (int i =0; i < count; i++)
        {
            if (drawPile.Count == 0)
            {
                Debug.LogWarning("山札が空です。カードを引けません。");
                return;
            }
            CardData drawnCard = drawPile[0];
            drawPile.RemoveAt(0);

            Debug.Log($"カードを引きました: {drawnCard.name}");
        }
    }
    public void DiscardCard(CardData card)
    {
        discardPile.Add(card);
        Debug.Log($"カードを捨て札にしました: {card.name}");
    }
}

