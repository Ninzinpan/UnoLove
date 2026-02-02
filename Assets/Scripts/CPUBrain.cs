using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.VisualScripting;
public class CPUBrain
{

public async Task<CardView> SelectCard( List<CardView> cpuHand,List<CardView> playerHand,FieldCardView fieldCard)
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
             if(card.Data.Color == fieldCard.Data.Color || card.Data.Type == fieldCard.Data.Type)
            {
                Debug.Log($"CPUがカードを決定しました: {card.Data.name}");
                return card;
            }


        }
        int index = Random.Range(0, cpuHand.Count);
        Debug.Log($"CPUがランダムにカードを選択しました: {cpuHand[index]}");
        return cpuHand[index];

    }
public async Task<TurnManager.IfSelectContinue> DefineIfContinue( List<CardView> myHand,List<CardView> otherHand,CardView fieldCard)
    {
        return TurnManager.IfSelectContinue.Continue;
    }
    public bool IfCardMatch(CardView playCard, CardView nextCard)
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



    

