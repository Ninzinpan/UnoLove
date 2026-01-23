using UnityEngine;
using System.Collections.Generic;

class  DuelistManager : MonoBehaviour
{
    [SerializeField]
    private DeckManager deckManager;

    [SerializeField]
    private HandManager handManager;

    [SerializeField]
    private BaseDeckData baseDeckData;

[Header("Duelist Settings")]
    [SerializeField]
    private int initialHandCount = 3;

    public List<CardData> StartingDeck{get; private set;} = new List<CardData>();

    private void Awake()
    {
        if (baseDeckData == null)
        {
            Debug.LogError("BaseDeckDataが割り当てられていません。");
            return;
        }
        else{
        StartingDeck = new List<CardData>(baseDeckData.BaseDeckCards);
        }
        if (deckManager != null)
        {
            if (StartingDeck.Count == 0)
            {
                Debug.LogWarning("StartingDeckが空です。BaseDeckDataからカードを取得できませんでした。");
                return;
            }
            else{

            deckManager.SetDeck(StartingDeck);
            deckManager.ShuffleDeck();
            }
        }
        else
        {
            Debug.LogError("DeckManagerが割り当てられていません。");
            return;
        }
    }
    private void Start()
    {
        DrawCardtoHand(initialHandCount);
    }

    private void DrawCardtoHand(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CardData drawnCard = deckManager.DrawCard();
            if (drawnCard != null)
            {
                handManager.AddCard(drawnCard);
            }
        }
    }
    private void DiscardCardFromHand(CardData card)
    {
        if (handManager.RemoveCard(card))
        {
            deckManager.DiscardCard(card); 

        }

    }
}