using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class StoryManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ChatSequencer sequencer;

    [Header("Debug")]
    [SerializeField] private List<TopicScenario> debugScenarios = new List<TopicScenario>();
    [SerializeField] private List<StoryEventItem> storyEvents = new List<StoryEventItem>();

    public async Task CheckAndPlay(StoryTrigger trigger, GameStateSnapshot snapshot)
    {
        foreach (var evt in storyEvents)
        {
            // 1. トリガーチェック & 再生済みチェック
            if (evt.Trigger != trigger) continue;
            if (evt.IsOneTimeOnly && evt.HasPlayed) continue;

            // 2. IDごとの条件判定 (Snapshotを使用)
            if (CheckSpecificCondition(evt.ID, snapshot))
            {
                Debug.Log($"[StoryManager] Event Triggered: {evt.ID}");
                evt.HasPlayed = true;
                await ConvertScenario(evt.Scenario);
            }
        }
    }

    private bool CheckSpecificCondition(StoryEventID id, GameStateSnapshot data)
    {
        float progress = data.TargetScore > 0 ? (float)data.CurrentScore / data.TargetScore : 0f;
        switch (id)
        {
            case StoryEventID.Intro_Welcome:
                return true; 

            case StoryEventID.First_Turn_Start:
                return data.TurnCount == 1;

            case StoryEventID.Score_Reach_1000:
                return data.CurrentScore >= 1000;

            case StoryEventID.Combo_Reach_5:
                return data.CurrentCombo >= 5;
            case StoryEventID.MainStory_Step1:
                return progress >= 0.2f ; // 目標の20%以上 (例: 500点なら100点)

            case StoryEventID.MainStory_Step2:
                return progress >= 0.4f  && data.CurrentSessionIndex >= 1; // 40%以上

            case StoryEventID.MainStory_Step3:
                return (progress >= 0.6f ) && data.CurrentSessionIndex >= 2; // 60%以上

            case StoryEventID.MainStory_Step4:
                return (progress >= 0.8f ) && data.CurrentSessionIndex >= 3; // 80%以上

            case StoryEventID.MainStory_Step5:
                return (progress >= 0.95f)  && data.CurrentSessionIndex >= 4; // ほぼクリア直前

            case StoryEventID.Game_Victory:
                return true; 

            case StoryEventID.Game_Defeat:
                return true;

            // ★追加: ボーナスイベント判定
            // Snapshotの情報 (IsBonusHit, SessionIndex) を元に判定する
            case StoryEventID.Bonus_Session1:
                return data.IsBonusHit && data.CurrentSessionIndex == 0; // 1回戦目

            case StoryEventID.Bonus_Session2:
                return data.IsBonusHit && data.CurrentSessionIndex == 1; // 2回戦目

            case StoryEventID.Bonus_Session3:
                return data.IsBonusHit && data.CurrentSessionIndex == 2;

            case StoryEventID.Bonus_Session4:
                return data.IsBonusHit && data.CurrentSessionIndex == 3;
            
            case StoryEventID.Bonus_Session5:
                return data.IsBonusHit && data.CurrentSessionIndex == 4;

            default:
                return false;
        }
    }

    // シナリオ再生処理 (Scenario -> SequencerData 変換)
    public async Task ConvertScenario(TopicScenario scenario)
    {
        if (scenario == null || sequencer == null) return;

        List<ChatSequenceData> masterSequence = new List<ChatSequenceData>();
        
        foreach (var step in scenario.Steps)
        {
            DialogueBranch branch = FindStoryBranch(step);
            if (branch == null) continue;

            ChatSequenceData data = new ChatSequenceData
            {
                Text = branch.Text,
                IsPlayer = step.IsPlayer,
                TextColor = branch.TextColor,
                Face = branch.Face,
                WaitForInput = step.WaitForInput,
                AutoDelay = step.AutoDelay
            };

            masterSequence.Add(data);
        }

        Debug.Log($"[StoryManager] Sending {masterSequence.Count} messages to Sequencer.");
        await sequencer.PlaySequence(masterSequence);
    }

    private DialogueBranch FindStoryBranch(ScenarioStep step)
    {
        if (step.Branches == null || step.Branches.Count == 0) return null;
        var anyMatch = step.Branches.Find(b => b.TargetColor == CardColor.Any);
        if (anyMatch != null) return anyMatch;
        return step.Branches[0];
    }
}