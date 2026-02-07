using UnityEngine;

[CreateAssetMenu(fileName = "NewSessionProfile", menuName = "ChatSystem/SessionProfile")]
public class SessionScenarioProfile : ScriptableObject
{
    [Header("このセッションで使用する話題シナリオ")]
    public TopicScenario CircleScenario;
    public TopicScenario SquareScenario;
    public TopicScenario TriangleScenario;

    [Header("ボーナス設定")]
    [Tooltip("この色がコンボの最後に出されたらボーナス")]
    public CardColor BonusColor;
    
    [Tooltip("ボーナス達成時に再生するストーリーイベントID")]
    public StoryEventID BonusEventID;
}