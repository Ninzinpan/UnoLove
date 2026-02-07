using UnityEngine;
using System;

// タイミング定義
public enum StoryTrigger
{
    None,
    GameStart,      // ゲーム開始時
    TurnStart,      // ターン開始時
    CardPlayed,     // カードを出してスコア計算した後
    ComboBreak,     // コンボ中断時
    GameClear,      // 勝利時
    GameOver        // 敗北時
}

// イベント識別子 (条件分岐用)
public enum StoryEventID
{
    None,
    // --- 導入 ---
    Intro_Welcome,       // ゲーム開始時の挨拶
    
    // --- チュートリアル/進行 ---
    First_Turn_Start,    // 最初のターン
    Combo_Reach_5,       // 5コンボ達成
    Score_Reach_1000,    // スコア1000点突破

    // --- 終了系 ---
    Game_Victory,        // クリア
    Game_Defeat          // 敗北
}

// ゲーム状況のスナップショット
public class GameStateSnapshot
{
    public int CurrentScore;
    public int CurrentCombo;
    public int TurnCount;
    public int RemainingTurns;
    public string CurrentTopicId; // CardTypeを文字列化して保持
    public bool IsComboActive;

    public GameStateSnapshot(int score, int combo, int turn, int remain, string topicId, bool isActive)
    {
        CurrentScore = score;
        CurrentCombo = combo;
        TurnCount = turn;
        RemainingTurns = remain;
        CurrentTopicId = topicId;
        IsComboActive = isActive;
    }
}

// Inspector設定用データ
[Serializable]
public class StoryEventItem
{
    public string Note; // メモ用
    public StoryEventID ID;
    public StoryTrigger Trigger;
    public TopicScenario Scenario;
    public bool IsOneTimeOnly = true;

    [NonSerialized] public bool HasPlayed = false;
}