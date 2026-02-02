using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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

    public int InitialHandCount => initialHandCount;

    public HandManager HandManager => handManager;

    public List<CardData> StartingDeck{get; private set;} = new List<CardData>();
    public  Action<BaseCardView> OnCardPlayed;
    private CPUBrain cPUBrain;

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
            }
        }
        else
        {
            Debug.LogError("DeckManagerが割り当てられていません。");
            return;
        }
    }
    private  void Start()
    {
    }

    public void InitializeDuelist()
    {
        Debug.Log("デュエリストを初期化します。");
        handManager.RemoveAllCards();
        deckManager.CleanDiscardPile();
        if (StartingDeck == null)
        {
            Debug.LogError("StartingDeckが設定されていません。");
            return;
        }
        else if (StartingDeck.Count == 0)
        {
            Debug.LogWarning("StartingDeckが空です。カードを引けません。");
            return;
        }
        deckManager.SetDeck(StartingDeck);
        deckManager.ShuffleDeck();
    }



    public async Task DrawCardtoHand(int count )
    {
        for (int i = 0; i < count; i++)
        {
            CardData drawnCard = deckManager.DrawCard();
            if (drawnCard != null)
            {
                await handManager.AddCard(drawnCard, HandleCardPlayed, true);
            }
        }
    }
private void HandleCardPlayed(BaseCardView card)
    {
        OnCardPlayed?.Invoke(card);
    }
    
    
    public void DiscardCardFromHand(BaseCardView card)
    {
        if (handManager.RemoveCard(card))
        {
            deckManager.DiscardCard(card.Data); 

        }

    }
    public async Task<BaseCardView> CPUSelectCard(List<BaseCardView> playerhand ,BaseCardView fieldCard)
    {
        if (cPUBrain == null)
        {
            cPUBrain = new CPUBrain();
        }
        var cpuHand = handManager.Hand;
        var selectedCard = await cPUBrain.SelectCard(cpuHand, playerhand, fieldCard);
        return selectedCard;
    }
    public async Task<bool> GetIfSessionfinished()
    {
        return true;
    }
        public async Task<TurnManager.IfSelectContinue> CheckifComboEnd(List<BaseCardView> myHand ,List<BaseCardView> otherHand ,BaseCardView fieldCard)
    {
        
        return TurnManager.IfSelectContinue.Continue;
    }

}