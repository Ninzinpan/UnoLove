using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

class HandManager : MonoBehaviour
{
    [SerializeField]
    private List<CardData> hand;
    [SerializeField]
    private GameObject cardPrehub;
    [SerializeField]
    private Transform handArea;
    public List<CardData> Hand => hand;
    void Awake()
    {
        hand = new List<CardData>();
    }

    public void AddCard(CardData card)
    {
        hand.Add(card);
        GameObject cardView = Instantiate(cardPrehub,handArea);
        CardView view = cardView.GetComponent<CardView>();
        view.SetUp(card);
        Debug.Log($"カードを手札に追加しました: {card.name}");
    }
public async Task AddCardWithAnimation(CardData card)
    {
        hand.Add(card);
        GameObject cardView = Instantiate(cardPrehub,handArea);
        CardView view = cardView.GetComponent<CardView>();
        view.SetUp(card);
        await Task.Delay(500); // 少し待ってからログを出す
        Debug.Log($"カードを手札に追加しました: {card.name}");
    }
    public bool RemoveAllCards()
    {
        if (hand.Count > 0)
        {
            hand.Clear();
            Debug.Log("手札の全てのカードを削除しました");
            return true;
        }
        else
        {
            Debug.LogWarning("手札にカードがありません。削除できません。");
            return false;
        }
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