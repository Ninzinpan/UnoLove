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