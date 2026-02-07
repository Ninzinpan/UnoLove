using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations; // リスト検索用に追加

// 話題ごとのステータスを管理するクラス（Inspectorで見れるようにSerializableを付与）
[System.Serializable]
public class TopicStatus
{
    public CardType Type;      // 話題の種類 (Triangle, Square, Circleなど)
    public int Level = 1;      // 現在のレベル
    public int CurrentExp = 0; // 現在の経験値
    public int NextLevelExp = 100; // 次のレベルに必要な経験値
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
    private int finalScore;

    public int CurrentScore => currentScore;
    public int FinalScore => finalScore;
    public int CurrentComboCount => currentComboCount;

    // --- 追加部分: 話題レベル管理用 ---
    [Header("Topic System")]
    [SerializeField]
    // ここにTriangle, Square, Circle用の3つの要素をInspectorで追加してください
    private List<TopicStatus> topicStatuses = new List<TopicStatus>();

    // 現在進行中の話題データへの参照
    private TopicStatus currentTopicStatus = null;

        private CardData firstCard = null;

    // --------------------------------

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Initialieze()
    {
        currentScore = 0;
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
            
            // --- 追加: 最初の1枚が出された時、話題をセットする ---
            // 場の最初のカードのマークを「現在の話題」として登録
            SetPlayedTopic(fielddatas[0].Type);
            AddScore(finalScore);
            return;
            // ------------------------------------------------
        }
      
        var currentdata = fielddatas[fielddatas.Count -1 ];
        var furtherdata = fielddatas[fielddatas.Count -2];
            
        if ((currentdata.Color == furtherdata.Color) || (currentdata.Type == furtherdata.Type))
            {
                currentComboCount += 1;
                var finaldoubleScore = baseScorePerCard + (baseScorePerCard * comboMagnificatioin * currentComboCount);
                finalScore = (int)finaldoubleScore;

                // --- 追加: コンボ成立時、話題の経験値を加算 ---
                // 例として、獲得スコアの10%分の経験値が入るなどの仕様にできます
                // 今回は単純に固定値(例:10)またはスコア依存で加算します
                AddEXP(10); 
                // ------------------------------------------
            }
        else
            {
                currentComboCount = 0;
                finalScore = baseScorePerCard;
                
                // --- 追加: コンボが途切れた(=新しい話題) ---
                SetPlayedTopic(currentdata.Type);
                // ----------------------------------------
            }
        
        Debug.Log($"スコアが{finalScore}ポイント加算されます。現在のコンボ:{currentComboCount}");
        AddScore(finalScore);
        return;

    }

    private void AddScore(int n)
    {
        currentScore += finalScore;
    }
    public void OnComboBreak()
    {
        
        firstCard = null;
        currentComboCount = 0;

    }

    public void SetFirstCard(CardData card)
    {
        if (card = null)
        {
            Debug.LogWarning("ファーストカードをリセットできません。");
        }
        firstCard = null;

    }

    // --- 追加メソッド: 現在の話題をセットする ---
    public void SetPlayedTopic(CardType type)
    {
        // リストから合致するTypeのステータスを探す
        // using System.Linq; が必要
        currentTopicStatus = topicStatuses.Find(x => x.Type == type);

        if (currentTopicStatus != null)
        {
            Debug.Log($"話題が「{currentTopicStatus.Type}」(Lv.{currentTopicStatus.Level}) に設定されました。");
        }
        else
        {
            // Inspectorで設定し忘れている場合などの対策
            Debug.LogWarning($"Type: {type} に対応するTopicStatusが見つかりませんでした。リストを確認してください。");
        }


    }
    public void ResetCurrentTopic()
       {
           currentTopicStatus = null;
           Debug.Log("現在の話題がリセットされました。");
       }

    // --- 追加メソッド: 経験値を加算する ---
    public void AddEXP(int amount)
    {
        if (currentTopicStatus == null) return;

        currentTopicStatus.CurrentExp += amount;
        // Debug.Log($"話題[{currentTopicStatus.Type}] Exp +{amount} (Total: {currentTopicStatus.CurrentExp}/{currentTopicStatus.NextLevelExp})");

        // レベルアップ判定
        if (currentTopicStatus.CurrentExp >= currentTopicStatus.NextLevelExp)
        {
            currentTopicStatus.Level++;
            
            // 経験値のリセットまたは繰り越し
            // ここでは単純に0リセットにするか、余剰分を持ち越すか選べます。今回は繰り越しで実装します。
            currentTopicStatus.CurrentExp -= currentTopicStatus.NextLevelExp;

            // 必要経験値を増やす（例: レベルごとに +50 難しくなる）
            currentTopicStatus.NextLevelExp += 50; 

            Debug.Log($"🎉 LEVEL UP! 話題[{currentTopicStatus.Type}] が Lv.{currentTopicStatus.Level} になりました！");
        }
    }
}