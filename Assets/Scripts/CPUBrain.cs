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
             if(CheckIfCardMatch(card,fieldCard))
            {
                Debug.Log($"CPUがカードを決定しました: {card.Data.name}");
                return card;
            }


        }
        int index = Random.Range(0, cpuHand.Count);
        Debug.Log($"CPUがランダムにカードを選択しました: {cpuHand[index]}");
        return cpuHand[index];

    }
public async Task<TurnManager.SelectContinueState> DefineIfContinue( List<BaseCardView> myHand,BaseCardView fieldCard)
    {
        if (myHand.Count == 0)
        {
            Debug.Log("手札にカードがありません");
            return TurnManager.SelectContinueState.Finish;
        }
        if (fieldCard == null)
        {
            Debug.LogWarning("fielidCardを参照できません");
            return TurnManager.SelectContinueState.Eroor;
        }
        if (fieldCard.Data== null)
        {
            Debug.Log("最初のfieldCardです。");
            return TurnManager.SelectContinueState.Continue;
        }
        foreach(var mycard in myHand)
        {

            if (CheckIfCardMatch(mycard, fieldCard))
            {
                return TurnManager.SelectContinueState.Continue;
            }
            
            
     
        
        }
        Debug.Log("マッチするカードが見つかりませんでした。");

        return TurnManager.SelectContinueState.Finish;
    }

    public int CountMatchNumber (BaseCardView card, List<CardView> hand)
    {
        var count = 0;
        foreach (var a_card in hand)
        {
            if (CheckIfCardMatch(card, a_card)){
                count++;
                
            }
        }
        return count;
    }
    public bool CheckIfCardMatch(BaseCardView playCard, BaseCardView nextCard)
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



    

