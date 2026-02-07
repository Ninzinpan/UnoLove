using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class StoryManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ChatSequencer sequencer;

    [Header("Test Data")]
    [Tooltip("テスト再生したいシナリオをここに登録してください")]
    [SerializeField] private List<TopicScenario> debugScenarios = new List<TopicScenario>();

    [Header("Debug")]
    [Tooltip("再生時に強制的にクリック待ちにするか")]
    [SerializeField] private bool forceWaitForInput = true;

    [SerializeField] private List<StoryEventItem> storyEvents = new List<StoryEventItem>();

public async Task CheckAndPlay(StoryTrigger trigger, GameStateSnapshot snapshot)
    {
        foreach (var evt in storyEvents)
        {
            // 1. タイミング違い、再生済みはスキップ
            if (evt.Trigger != trigger) continue;
            if (evt.IsOneTimeOnly && evt.HasPlayed) continue;

            // 2. IDごとの条件判定
            if (CheckSpecificCondition(evt.ID, snapshot))
            {
                // 条件一致
                evt.HasPlayed = true;
                
                // ※TopicScenarioからChatSequenceDataへの変換は
                // 以前作成した変換ロジック(ConvertScenarioなど)を使ってください。
                // ここでは仮に sequencer.PlaySequence(evt.Scenario) としておきます。
                // 実装に合わせて調整してください。
                await ConvertScenario(evt.Scenario);
            }
        }
    }

    private bool CheckSpecificCondition(StoryEventID id, GameStateSnapshot data)
    {
        switch (id)
        {
            case StoryEventID.Intro_Welcome:
                return true; // 無条件

            case StoryEventID.First_Turn_Start:
                return data.TurnCount == 1;

            case StoryEventID.Score_Reach_1000:
                return data.CurrentScore >= 1000;

            case StoryEventID.Combo_Reach_5:
                return data.CurrentCombo >= 5;

            case StoryEventID.Game_Victory:
                return true; // トリガーがGameClearの時点で確定なのでtrue

            case StoryEventID.Game_Defeat:
                return true;

            default:
                return false;
        }
    }

    private async Task PlayScenario(TopicScenario scenario)
    {
        if (scenario == null) return;
        
        // ★重要: ここで TopicScenario -> List<ChatSequenceData> の変換を行う
        // 以前のStoryManagerテストコードにある変換処理をここに移設してください
        // 今回は簡略化のためダミーウェイトのみ記述します
        Debug.Log($"[Story] Play: {scenario.name}");
        await Task.Delay(100); 
    }

    public async Task ConvertScenario(TopicScenario scenario)
    {
        List<ChatSequenceData> masterSequence = new List<ChatSequenceData>();
        foreach (var step in scenario.Steps)
            {
                // ストーリーモードなので、分岐は基本「Any」または「先頭」を採用
                DialogueBranch branch = FindStoryBranch(step);



                if (branch == null) continue;

                // シーケンサー用のデータに変換
                ChatSequenceData data = new ChatSequenceData
                {
                    Text = branch.Text,
                    IsPlayer = step.IsPlayer,
                    TextColor = branch.TextColor,
                    Face = branch.Face,
                    
                    // ストーリー用設定（強制クリック待ち、またはデータ通りの遅延）
                    WaitForInput = step.WaitForInput,
                    AutoDelay = step.AutoDelay
                };

                masterSequence.Add(data);
            }
        Debug.Log($"[StoryManager] Sending {masterSequence.Count} messages to Sequencer.");
        await sequencer.PlaySequence(masterSequence);

    }
    public async Task TestPlayAll()
    {
        if (sequencer == null)
        {
            Debug.LogError("ChatSequencer is not assigned!");
            return;
        }

        if (debugScenarios.Count == 0)
        {
            Debug.LogWarning("No scenarios assigned to debugScenarios list.");
            return;
        }

        // 1. 再生用データのリストを作成
        List<ChatSequenceData> masterSequence = new List<ChatSequenceData>();

        // 2. 登録された全シナリオをループ
        foreach (var topic in debugScenarios)
        {
            if (topic == null) continue;

            // 各シナリオ内のステップを変換して追加
            foreach (var step in topic.Steps)
            {
                // ストーリーモードなので、分岐は基本「Any」または「先頭」を採用
                DialogueBranch branch = FindStoryBranch(step);

                if (branch == null) continue;

                // シーケンサー用のデータに変換
                ChatSequenceData data = new ChatSequenceData
                {
                    Text = branch.Text,
                    IsPlayer = step.IsPlayer,
                    TextColor = branch.TextColor,
                    Face = branch.Face,
                    
                    // ストーリー用設定（強制クリック待ち、またはデータ通りの遅延）
                    WaitForInput = step.WaitForInput,
                    AutoDelay = step.AutoDelay
                };

                masterSequence.Add(data);
            }
        }

        // 3. シーケンサーに再生依頼 (Fire and Forget)
        Debug.Log($"[StoryManager] Sending {masterSequence.Count} messages to Sequencer.");
        await sequencer.PlaySequence(masterSequence);
    }

    // ストーリー用の分岐解決ロジック（基本はAnyか先頭を選ぶ）
    private DialogueBranch FindStoryBranch(ScenarioStep step)
    {
        if (step.Branches == null || step.Branches.Count == 0) return null;

        // 1. Anyを探す
        var anyMatch = step.Branches.Find(b => b.TargetColor == CardColor.Any);
        if (anyMatch != null) return anyMatch;

        // 2. なければ先頭を使う（一本道とみなす）
        return step.Branches[0];
    }
}