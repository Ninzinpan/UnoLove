using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.VisualScripting;
public class CPUBrain
{

public async Task<BaseCardView> SelectCard( List<BaseCardView> cpuHand,List<BaseCardView> playerHand,BaseCardView fieldCard)
    {
        await Task.Delay(1000);
        if (cpuHand.Count == 0)
        {
            Debug.Log("CPUの手札にカードがありません。");
            return null;
        }
        if (fieldCard == null)
        {
            int index2 = Random.Range(0, cpuHand.Count);
            Debug.Log($"フィールドカードがないため、CPUがランダムにカードを選択しました: {cpuHand[index2]}");
            return cpuHand[index2];
        }   
        foreach (var card in cpuHand)
        {
             if(IfCardMatch(card,fieldCard))
            {
                Debug.Log($"CPUがカードを決定しました: {card.Data.name}");
                return card;
            }


        }
        int index = Random.Range(0, cpuHand.Count);
        Debug.Log($"CPUがランダムにカードを選択しました: {cpuHand[index]}");
        return cpuHand[index];

    }
public async Task<TurnManager.IfSelectContinue> DefineIfContinue( List<BaseCardView> myHand,List<BaseCardView> otherHand,BaseCardView fieldCard)
    {
        if (myHand.Count == 0 || otherHand.Count == 0)
        {
            Debug.Log("手札にカードがありません");
            return TurnManager.IfSelectContinue.Continue;
        }
        foreach(var mycard in myHand)
        {
            foreach(var othercard in otherHand)
            {
                if(IfCardMatch(mycard, othercard))
                {
                    Debug.Log("マッチするカードがあります。");
                    return TurnManager.IfSelectContinue.Continue;
                }
            }
        
        }
        Debug.Log("マッチするカードが見つかりませんでした。");

        return TurnManager.IfSelectContinue.Finish;
    }

    public int CountMatchNumber (BaseCardView card, List<CardView> hand)
    {
        var count = 0;
        foreach (var a_card in hand)
        {
            if (IfCardMatch(card, a_card)){
                count++;
                
            }
        }
        return count;
    }
    public bool IfCardMatch(BaseCardView playCard, BaseCardView nextCard)
    {
        if (playCard == null || nextCard == null)
        {
            Debug.LogWarning("カードが設定されていません。");
            return false;
        }
        if (playCard.Data.Color == nextCard.Data.Color || playCard.Data.Type == nextCard.Data.Type){
            return true;
            
        }
        return false;
    }


    }



    

