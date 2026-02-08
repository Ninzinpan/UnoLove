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



  public async Task DrawCardtoHand(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // まず普通に引いてみる
            CardData drawnCard = deckManager.DrawCard();

            // 山札が空だった場合（nullが返ってきた場合）
            if (drawnCard == null)
            {
                Debug.Log("山札が空になりました。捨て札を回収してリシャッフルします。");
                
                // ★追加: 捨て札を山札に戻す処理を呼ぶ
                deckManager.RefillDeckFromDiscard(); 
                
                // もう一度引いてみる
                drawnCard = deckManager.DrawCard();
            }

            // カードが取得できていれば手札に加える
            if (drawnCard != null)
            {
                await handManager.AddCard(drawnCard, HandleCardPlayed, true);
            }
            else
            {
                // リシャッフルしてもカードがない場合（捨て札も空など）
                Debug.LogWarning("補充できるカードがありませんでした（ドロー中断）。");
                break; 
            }
        }
    }
    private void HandleCardPlayed(BaseCardView card)
    {
        OnCardPlayed?.Invoke(card);
    }
    
    public void DiscardAllCardFromHand()
    {
        if (handManager.Hand == null || handManager.Hand.Count < 0)
        {
            return;
        }
        while(HandManager.Hand.Count > 0)
        {
            DiscardCardFromHand(handManager.Hand[0]);
        }
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
    public async Task<TurnManager.SelectContinueState> CPUGetIfSessionContinued(BaseCardView fieldCard)
    {
        cPUBrain = new CPUBrain();
        var result = await cPUBrain.DefineIfContinue(handManager.Hand,fieldCard);
        return result;
    }
  

}