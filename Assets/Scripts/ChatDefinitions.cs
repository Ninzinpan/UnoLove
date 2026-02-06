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
[CreateAssetMenu(fileName = "NewTopic", menuName = "ChatSystem/TopicScenario")]
public class TopicScenario : ScriptableObject
{
    public string TopicId;
    public List<ScenarioStep> Steps = new List<ScenarioStep>();
    
    [Header("ブレイク時のランダム会話リスト")]
    // 文字列ではなく、1ステップ分のシナリオとして定義することで、表情などもつけられる
    public List<ScenarioStep> BreakSteps = new List<ScenarioStep>();
    private void OnValidate()
    {
        // 通常の会話リストのチェック
        ValidateList(Steps);
        // ブレイク用リストのチェック
        ValidateList(BreakSteps);
    }

private void ValidateList(List<ScenarioStep> targetSteps)
    {
        if (targetSteps == null) return;
        foreach (var step in targetSteps)
        {
            if (step.Branches == null) continue;

            foreach (var branch in step.Branches)
            {
                // 1. 色の修正: アルファ値(透明度)が0なら、設定漏れとみなして赤(初期値)にする
                if (branch.TextColor.a == 0f)
                {
                    branch.TextColor = Color.black; // ここをお好みの色(Color.whiteなど)に変更可
                }

                // 2. TargetColorの修正:
                // 「テキストが空」かつ「TargetColorがRed(Enumの0番目)」の場合、
                // 新規作成されたばかりとみなして Any に書き換える
                // ※「あえてRedで、テキストも空にしたい」場合は手動で戻す必要がありますが、稀なケースと想定
                if (string.IsNullOrEmpty(branch.Text) && branch.TargetColor == CardColor.Red)
                {
                    branch.TargetColor = CardColor.Any;
                }
            }
        }

    }

}


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