using UnityEngine;
using System.Collections.Generic;
using System;

// ※ CardColor は CardData.cs 側の定義 (Red, Green, Blue, Yiellow, None, Any) を使用します
// Viewに渡すためのデータ構造
[Serializable]
public class ChatMessage
{
    public string MessageText;
    public bool IsPlayer;      // Speakerオブジェクトの代わりにboolで判定
    public Color TextColor;
    public ExpressionType Face;
}
public enum ExpressionType
{
    Normal,
    Smile,
    Surprise,
    Shameful,
    Angry,
    Sad
}

// --- Data Classes ---

[Serializable]
public class DialogueBranch
{
    [Tooltip("この分岐を選択する条件となる色。\nCardColor.Any を指定すると、他の色（Red/Green/Blue）に該当しなかった場合の「デフォルト」として機能します")]
    public CardColor TargetColor;
    
    [TextArea(2, 5)]
    public string Text;
    
    [Tooltip("UIの文字色")]
    public Color TextColor = Color.white;
    
    [Tooltip("相手(Heroine)の場合の表情指定。Playerの場合は無視されます")]
    public ExpressionType Face = ExpressionType.Normal;
}

[Serializable]
public class ScenarioStep
{
    [Header("Basic Settings")]
    public bool IsPlayer; // True:プレイヤーのターン, False:相手のターン

    [Header("Flow Settings")]
    [Tooltip("Trueの場合、メッセージ表示後にカード入力を待たずに自動で次へ進みます")]
    public bool AutoAdvance = false;
    
    [Tooltip("AutoAdvanceがTrueの場合の待機時間(秒)")]
    public float AutoDelay = 1.0f;

    [Header("Branches")]
    public List<DialogueBranch> Branches = new List<DialogueBranch>();
}

// --- ScriptableObject ---

[CreateAssetMenu(fileName = "NewTopic", menuName = "ChatSystem/TopicScenario")]
public class TopicScenario : ScriptableObject
{
    public string TopicId;
    public List<ScenarioStep> Steps = new List<ScenarioStep>();
    
    [Header("コンボ中断時の汎用メッセージ")]
    public List<ScenarioStep> BreakMessages = new List<ScenarioStep>();
}