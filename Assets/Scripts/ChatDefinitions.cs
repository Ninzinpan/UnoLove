using UnityEngine;
using System.Collections.Generic;
using System;

// --- Enums ---
// CardColor, CardTypeはCardData.csのものを使用

public enum ExpressionType
{
    Normal, Smile, Surprise, Shameful, Angry, Sad
}

// --- Data Classes ---

// 1つのセリフの分岐
[Serializable]
public class DialogueBranch
{
    [Tooltip("分岐条件。Anyはデフォルトとして機能")]
    public CardColor TargetColor = CardColor.Any;
    
    [TextArea(2, 5)]
    public string Text;
    public Color TextColor = Color.red;
    public ExpressionType Face = ExpressionType.Normal;
}

// 会話の1ステップ（タイミング情報含む）
[Serializable]
public class ScenarioStep
{
    [Header("Basic")]
    public bool IsPlayer;

    [Header("Timing")]
    [Tooltip("Trueなら、ユーザーが画面をクリックするまで待機する（ストーリーモード用）")]
    public bool WaitForInput = false;

    [Tooltip("WaitForInputがFalseの場合の、自動送りの待機時間")]
    public float AutoDelay = 1.0f;

    [Header("Content")]
    public List<DialogueBranch> Branches = new List<DialogueBranch>();
}

// シナリオ全体（1つの話題）



// 進行状況管理用クラス
[Serializable]
public class TopicState
{
    public TopicScenario Scenario;
    public int CurrentIndex = 0;
    public CardColor LastContextColor = CardColor.None;

    public TopicState(TopicScenario scenario)
    {
        Scenario = scenario;
        CurrentIndex = 0;
        LastContextColor = CardColor.None;
    }
}



// シーケンサーに渡す再生用データ（解決済みのメッセージ）
public class ChatSequenceData
{
    public string Text;
    public bool IsPlayer;
    public Color TextColor;
    public ExpressionType Face;
    
    // タイミング制御用
    public bool WaitForInput;
    public float AutoDelay;
}