using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class TopicStatus
{
    public CardType Type;      
    public int Level = 1;      
    public int CurrentExp = 0; 
    public int NextLevelExp = 100; 
}

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private double comboMagnificatioin = 0.2;
    [SerializeField]
    private int baseScorePerCard = 100;
    [SerializeField]
    private int currentScore = 0;
    [SerializeField]
    private int currentComboCount = 0;
    [SerializeField]
    private int finalScore; // 直近の計算結果保持用

    public int CurrentScore => currentScore;
    public int CurrentComboCount => currentComboCount;

    [Header("Topic System")]
    [SerializeField]
    private List<TopicStatus> topicStatuses = new List<TopicStatus>();

    private TopicStatus currentTopicStatus = null;
    private CardData firstCard = null;

    public void Initialieze()
    {
        currentComboCount = 0;
        finalScore = 0;
        ResetCurrentTopic();
        firstCard = null;
    }

    public void CalculateScore(List<CardData> fielddatas)
    {
        finalScore = 0;
        if (fielddatas == null|| fielddatas.Count == 0)
        {
            Debug.LogWarning("dataのリストが空です。");
            return;
        }
        if (fielddatas.Count == 1)
        {
            firstCard = fielddatas[0];
            currentComboCount = 0;
            finalScore = baseScorePerCard;
            
            SetPlayedTopic(fielddatas[0].Type);
            AddScore(finalScore);
            return;
        }
      
        var currentdata = fielddatas[fielddatas.Count -1 ];
        var furtherdata = fielddatas[fielddatas.Count -2];
            
        if ((currentdata.Color == furtherdata.Color) || (currentdata.Type == furtherdata.Type))
        {
            currentComboCount += 1;
            var finaldoubleScore = baseScorePerCard + (baseScorePerCard * comboMagnificatioin * currentComboCount);
            finalScore = (int)finaldoubleScore;

            AddEXP(10); 
        }
        else
        {
            currentComboCount = 0;
            finalScore = baseScorePerCard;
            SetPlayedTopic(currentdata.Type);
        }
        
        Debug.Log($"スコアが{finalScore}ポイント加算されます。現在のコンボ:{currentComboCount}");
        AddScore(finalScore);
    }

    // ★修正: 外部から任意の値を加算できるように変更
    public void AddScore(int amount)
    {
        currentScore += amount;
    }

    public void OnComboBreak()
    {
        firstCard = null;
        currentComboCount = 0;
    }

    public void SetFirstCard(CardData card)
    {
        firstCard = null; 
    }

    public void SetPlayedTopic(CardType type)
    {
        currentTopicStatus = topicStatuses.Find(x => x.Type == type);
        if (currentTopicStatus != null)
        {
            // Debug.Log($"話題設定: {currentTopicStatus.Type}");
        }
    }

    public void ResetCurrentTopic()
    {
        currentTopicStatus = null;
    }

    public void AddEXP(int amount)
    {
        if (currentTopicStatus == null) return;

        currentTopicStatus.CurrentExp += amount;
        if (currentTopicStatus.CurrentExp >= currentTopicStatus.NextLevelExp)
        {
            currentTopicStatus.Level++;
            currentTopicStatus.CurrentExp -= currentTopicStatus.NextLevelExp;
            currentTopicStatus.NextLevelExp += 50; 
            Debug.Log($"🎉 LEVEL UP! [{currentTopicStatus.Type}] Lv.{currentTopicStatus.Level}");
        }
    }
}