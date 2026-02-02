using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;

class HandManager : MonoBehaviour
{
    [SerializeField]
    private List<BaseCardView> hand;
    [SerializeField]
    private GameObject cardPrehub;
    [SerializeField]
    private Transform handArea;
    public List<BaseCardView> Hand => hand;
    void Awake()
    {
        hand = new List<BaseCardView>();
    }

    public async Task AddCard(CardData card, Action<BaseCardView> onClick,bool animation =false)
    {
        if (card == null)
        {
            Debug.LogWarning("HandManager:追加しようとしたカードデータがnullです。");
            return;
        }
        GameObject cardView = Instantiate(cardPrehub,handArea);
        BaseCardView view = cardView.GetComponent<BaseCardView>();
        if (view == null)
        {
            Debug.LogError("HandManager:CardViewコンポーネントが見つかりません。");
            return;
        }
        view.SetUp(card, onClick);
        hand.Add(view);
        if (animation)
        {
            await Task.Delay(500); // 少し待ってからログを出す
        }
        Debug.Log($"カードを手札に追加しました: {card.name}");
    }

    public bool RemoveAllCards()
    {
        if (hand.Count > 0)
        {
            foreach (var card in hand)
            {
                card.Delite();
            }

            
            hand.Clear();
            Debug.Log("手札の全てのカードを削除しました");
        }

        
        else
        {
            Debug.LogWarning("手札にカードがありません。削除できません。");
        }
        
        foreach (Transform child in handArea)
            {
                Destroy(child.gameObject);

                Debug.Log("handareaのカードオブジェクトを全て削除しました");
            }
        return true;
    }
public bool RemoveCard(BaseCardView card)
    {
        if (card == null)
        {
            Debug.LogWarning("HandManager:削除しようとしたカードがnullです。");
            return false;
        }
        hand.Remove(card);
        card.Delite();

        return true;
        

    }
}