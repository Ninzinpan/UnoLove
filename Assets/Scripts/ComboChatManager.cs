using UnityEngine;
using System.Collections.Generic;

public class ComboChatManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ChatSequencer sequencer;

    [Header("Data Settings")]
    // ShapeごとのシナリオアセットをInspectorで設定
    [SerializeField] private TopicScenario circleTopic;
    [SerializeField] private TopicScenario squareTopic;
    [SerializeField] private TopicScenario triangleTopic;

    // 実行時の状態管理
    private Dictionary<CardType, TopicState> topicStates = new Dictionary<CardType, TopicState>();

    void Start()
    {
        // 状態の初期化
        if (circleTopic != null) topicStates[CardType.Circle] = new TopicState(circleTopic);
        if (squareTopic != null) topicStates[CardType.Square] = new TopicState(squareTopic);
        if (triangleTopic != null) topicStates[CardType.Triangle] = new TopicState(triangleTopic);
    }

  

    // --- 外部呼び出し: カードが出された時 ---
    public void OnCardPlayed(CardType playedShape, CardColor playedColor)
    {
        // 1. 対応する話題を取得
        if (!topicStates.ContainsKey(playedShape)) return;
        TopicState state = topicStates[playedShape];

        // 2. 終了判定
        if (state.CurrentIndex >= state.Scenario.Steps.Count)
        {
            // 話題切れの場合の処理（ループするか、黙るか）
            return; 
        }

        // 3. 現在のステップ取得
        ScenarioStep currentStep = state.Scenario.Steps[state.CurrentIndex];

        // 4. 分岐解決 (コンテキストカラーを使用)
        // プレイヤーの場合は自分の出した色、相手の場合は直前のコンテキスト色を使う
        CardColor searchKey = currentStep.IsPlayer ? playedColor : state.LastContextColor;
        
        // 初手がいきなり相手の場合のケア
        if (!currentStep.IsPlayer && searchKey == CardColor.None) searchKey = CardColor.Any;

        DialogueBranch branch = FindBranch(currentStep, searchKey);

        // 5. コンテキスト更新 (今回出した色を記憶)
        state.LastContextColor = playedColor;

        // 6. 再生用データ作成 (Sequencer用)
        ChatSequenceData data = new ChatSequenceData
        {
            Text = branch.Text,
            IsPlayer = currentStep.IsPlayer,
            TextColor = branch.TextColor,
            Face = branch.Face,
            
            // コンボ会話なので、基本はデータの設定に従う（主にAutoDelayで進む想定）
            WaitForInput = currentStep.WaitForInput,
            AutoDelay = currentStep.AutoDelay
        };

        // 7. Sequencerへ依頼 (Fire and Forget)
        var sequence = new List<ChatSequenceData> { data };
        _ = sequencer.PlaySequence(sequence);

        // 8. インデックス進行
        state.CurrentIndex++;
    }

    // --- 外部呼び出し: コンボ中断時 ---
    public void OnComboBreak(CardType currentShape)
    {
        if (!topicStates.ContainsKey(currentShape)) return;
        TopicState state = topicStates[currentShape];

        if (state.Scenario.BreakSteps.Count == 0) return;

        // ランダムにブレイク会話を取得
        ScenarioStep breakStep = state.Scenario.BreakSteps[Random.Range(0, state.Scenario.BreakSteps.Count)];
        
        // ブレイク会話は基本分岐なし(Any)とする
        DialogueBranch branch = FindBranch(breakStep, CardColor.Any);

        ChatSequenceData data = new ChatSequenceData
        {
            Text = branch.Text,
            IsPlayer = breakStep.IsPlayer,
            TextColor = branch.TextColor,
            Face = branch.Face,
            WaitForInput = breakStep.WaitForInput,
            AutoDelay = breakStep.AutoDelay
        };

        _ = sequencer.PlaySequence(new List<ChatSequenceData> { data });
        
        // ※仕様通りインデックスはリセットしない
        // 文脈だけリセット
        state.LastContextColor = CardColor.None;
    }

    // --- ヘルパー: 分岐検索 ---
    private DialogueBranch FindBranch(ScenarioStep step, CardColor colorKey)
    {
        // 完全一致 -> Any -> 先頭 の順で検索
        var match = step.Branches.Find(b => b.TargetColor == colorKey);
        if (match != null) return match;

        var any = step.Branches.Find(b => b.TargetColor == CardColor.Any);
        if (any != null) return any;

        if (step.Branches.Count > 0) return step.Branches[0]; // エラー回避のフォールバック

        return new DialogueBranch { Text = "..." }; // 完全なエラー
    }
}