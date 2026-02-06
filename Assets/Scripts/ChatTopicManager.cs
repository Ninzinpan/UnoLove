using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class ChatTopicManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ChatWindowView chatWindowView;

    [Header("Debug")]
    [SerializeField] private TopicScenario currentTopic;
    [SerializeField] private int currentStepIndex = 0;
    
    // CardData.csのCardColorを使用。初期値はNone
    [SerializeField] private CardColor lastContextColor = CardColor.None; 
    
    [SerializeField] private bool isProcessing = false;

    public TopicScenario CurrentTopic => currentTopic;

    // --- 初期化 ---
    public void InitializeTopic(TopicScenario topic)
    {
        currentTopic = topic;
        currentStepIndex = 0;
        lastContextColor = CardColor.None;
        isProcessing = false;
        
        Debug.Log($"Topic Initialized: {topic.TopicId}");
    }

    // --- 外部からのエントリーポイント: プレイヤーがカードを出した時 ---
    public async void OnCardPlayed(CardColor playedColor)
    {
        if (currentTopic == null) return;
        if (isProcessing) return;

        isProcessing = true;

        try
        {
            // 1. 終了判定
            if (currentStepIndex >= currentTopic.Steps.Count)
            {
                Debug.Log("Conversation Finished.");
                isProcessing = false;
                return;
            }

            ScenarioStep currentStep = currentTopic.Steps[currentStepIndex];

            // 2. Fail-Fast: プレイヤーのターンではないのにカードで進めようとした
            if (!currentStep.IsPlayer)
            {
                throw new Exception($"[ChatError] Step {currentStepIndex} is Opponent's turn (IsPlayer=false), but Player played a card. Scenario data structure mismatch.");
            }

            // 3. 分岐解決 (Playerなので、出したカードの色で判定)
            DialogueBranch selectedBranch = FindBranch(currentStep, playedColor);
            
            // 4. コンテキスト保存 (相手の返答用に色を記憶)
            // Any分岐に入ったとしても、「実際に出した色」を記憶する
            lastContextColor = playedColor;

            // 5. UI表示
            await DisplayMessage(currentStep, selectedBranch);

            // 6. インデックス進行
            currentStepIndex++;

            // 7. 自動進行チェック (相手のターンへ続く場合など)
            await CheckAndProcessAutoAdvance();

        }
        catch (Exception e)
        {
            Debug.LogError($"Chat System Critical Error: {e.Message}");
            #if UNITY_EDITOR
            Debug.Break();
            #endif
        }
        finally
        {
            isProcessing = false;
        }
    }

    // --- 自動進行ロジック ---
    private async Task CheckAndProcessAutoAdvance()
    {
        while (currentStepIndex < currentTopic.Steps.Count)
        {
            ScenarioStep nextStep = currentTopic.Steps[currentStepIndex];

            if (!nextStep.AutoAdvance)
            {
                break;
            }

            await Task.Delay((int)(nextStep.AutoDelay * 1000));

            DialogueBranch branch;
            if (nextStep.IsPlayer)
            {
                // 自動進行のPlayer(心の声等)
                branch = FindBranch(nextStep, lastContextColor);
            }
            else
            {
                // Opponentの場合、直前の色で解決
                if (lastContextColor == CardColor.None)
                {
                    // 初手がOpponentの場合の対策(Any)
                    branch = FindBranch(nextStep, CardColor.Any);
                }
                else
                {
                    branch = FindBranch(nextStep, lastContextColor);
                }
            }

            await DisplayMessage(nextStep, branch);
            currentStepIndex++;
        }
    }

    // --- 分岐検索ロジック ---
    private DialogueBranch FindBranch(ScenarioStep step, CardColor colorKey)
    {
        // 1. 完全一致を探す
        var exactMatch = step.Branches.Find(b => b.TargetColor == colorKey);
        if (exactMatch != null) return exactMatch;

        // 2. Any (CardData.cs定義のAny) を探す
        // シナリオデータのInspectorでは "Any" を設定してください
        var anyMatch = step.Branches.Find(b => b.TargetColor == CardColor.Any);
        if (anyMatch != null) return anyMatch;

        // 3. どちらもなければエラー
        throw new Exception($"[ChatError] No valid branch found for color '{colorKey}' at Step {currentStepIndex}. Please add a branch for this color or an 'Any' branch.");
    }

    // --- UI表示 ---
    private async Task DisplayMessage(ScenarioStep step, DialogueBranch branch)
    {
// Viewへ渡すデータを作成
        ChatMessage msg = new ChatMessage
        {
            MessageText = branch.Text,
            IsPlayer = step.IsPlayer,       // Stepの定義に従う
            TextColor = branch.TextColor,   // Branchの定義に従う
            Face = branch.Face              // 表情情報
        };

        Debug.Log($"Display: [{branch.Text}] IsPlayer:{step.IsPlayer} Color:{branch.TextColor}");
        
        if (chatWindowView != null)
        {
            // Viewのメソッドを呼ぶ
            chatWindowView.AddMessage(msg); 
        }

        // 演出用の待機（メッセージが出た瞬間の"間"）
        await Task.Delay(100);
    }

    // --- コンボ中断時 ---
    public void OnComboBroken()
    {
        if (currentTopic == null) return;

        if (currentTopic.BreakMessages.Count > 0)
        {
 
        }

        // 話題が途切れたのでNoneに戻す
        lastContextColor = CardColor.None;
    }
}