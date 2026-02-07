using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ComboChatManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ChatSequencer sequencer;

    [Header("Session Data")]
    [Tooltip("セッションごとのプロファイルを順に登録")]
    [SerializeField] private List<SessionScenarioProfile> sessionProfiles = new List<SessionScenarioProfile>();

    // 実行時の状態管理
    private Dictionary<CardType, TopicState> currentTopicStates = new Dictionary<CardType, TopicState>();
    
    private int currentSessionIndex = 0;
    private CardColor lockedColor = CardColor.None; // コンボ中に固定される色

    public int CurrentSessionIndex => currentSessionIndex;

    // 現在のセッション情報を取得するプロパティ
    public SessionScenarioProfile CurrentProfile
    {
        get
        {
            if (sessionProfiles == null || sessionProfiles.Count == 0) return null;
            // インデックスが範囲外なら最後のものを使う（ループ回避）
            int safeIndex = Mathf.Clamp(currentSessionIndex, 0, sessionProfiles.Count - 1);
            return sessionProfiles[safeIndex];
        }
    }

    void Start()
    {
        InitializeSession();
    }

    // セッション開始・切り替え時の初期化
    private void InitializeSession()
    {
        currentTopicStates.Clear();
        lockedColor = CardColor.None;

        var profile = CurrentProfile;
        if (profile == null) return;

        // 現在のプロファイルから各Shapeのシナリオをロード
        if (profile.CircleScenario != null) 
            currentTopicStates[CardType.Circle] = new TopicState(profile.CircleScenario);
        
        if (profile.SquareScenario != null) 
            currentTopicStates[CardType.Square] = new TopicState(profile.SquareScenario);
        
        if (profile.TriangleScenario != null) 
            currentTopicStates[CardType.Triangle] = new TopicState(profile.TriangleScenario);
            
        Debug.Log($"Session {currentSessionIndex + 1} Initialized. Bonus Color: {profile.BonusColor}");
    }

    // --- 外部呼び出し: カードが出された時 ---
    public async Task OnCardPlayed(CardType playedShape, CardColor playedColor)
    {
        // 1. 対応する話題を取得
        if (!currentTopicStates.ContainsKey(playedShape)) return;
        TopicState state = currentTopicStates[playedShape];

        // 2. 終了判定 (会話切れ)
        if (state.CurrentIndex >= state.Scenario.Steps.Count)
        {
            return; 
        }

        // 3. カラーロック判定 (ここが重要！)
        // まだロックされていない（コンボ初手）なら、今の色でロックする
        if (lockedColor == CardColor.None)
        {
            lockedColor = playedColor;
            Debug.Log($"Color Locked: {lockedColor}");
        }

        // 4. 現在のステップ取得
        ScenarioStep currentStep = state.Scenario.Steps[state.CurrentIndex];

        // 5. 分岐解決
        // プレイヤーの場合： 「今回出した色」ではなく「ロックされた色」を使って検索する
        // 相手の場合： 文脈（LastContextColor）を使うが、今回はロック色＝文脈となるのでロック色を使う
        CardColor searchKey = currentStep.IsPlayer ? lockedColor : state.LastContextColor;
        
        // 初手がいきなり相手、かつロック前の場合のケア
        if (!currentStep.IsPlayer && searchKey == CardColor.None) 
        {
             // ロック色がまだないならAnyで探すしかない
             searchKey = CardColor.Any;
        }
        else if (!currentStep.IsPlayer)
        {
            // 相手番でも基本はロック色に従う
            searchKey = lockedColor;
        }

        DialogueBranch branch = FindBranch(currentStep, searchKey);

        // 6. コンテキスト更新
        state.LastContextColor = lockedColor; // 常にロック色で上書き

        // 7. 再生用データ作成
        ChatSequenceData data = new ChatSequenceData
        {
            Text = branch.Text,
            IsPlayer = currentStep.IsPlayer,
            TextColor = branch.TextColor,
            Face = branch.Face,
            WaitForInput = currentStep.WaitForInput,
            AutoDelay = currentStep.AutoDelay
        };

        // 8. Sequencerへ依頼
        await sequencer.PlaySequence(new List<ChatSequenceData> { data });

        // 9. インデックス進行
        state.CurrentIndex++;
    }

    // --- 外部呼び出し: コンボ中断時 ---
    // TurnManagerから呼ばれる。セッションを進める処理もここで行うか、別途メソッドを用意するか。
    // ここでは「ブレイク会話の再生」を行い、その後にTurnManagerがAdvanceSessionを呼ぶ想定にします。
    public async Task OnComboBreak(CardType currentShape, WhoseTurn turn)
    {
        // ブレイク時の会話再生（既存ロジック）
        if (currentTopicStates.ContainsKey(currentShape))
        {
            TopicState state = currentTopicStates[currentShape];
            if (state.Scenario.BreakSteps.Count > 0)
            {
                ScenarioStep breakStep = state.Scenario.BreakSteps[Random.Range(0, state.Scenario.BreakSteps.Count)];
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
                await sequencer.PlaySequence(new List<ChatSequenceData> { data });
                
                // ヒロインがブレイクした場合はインデックスを戻す等の処理
                if (turn == WhoseTurn.Opponent && state.CurrentIndex > 0)
                {
                    state.CurrentIndex--;
                }
            }
        }
    }

    // --- セッション進行（TurnManagerから呼ぶ） ---
    public void AdvanceSession()
    {
        currentSessionIndex++;
        // 次のセッションデータをロードしてリセット
        InitializeSession();
    }

    // --- ヘルパー: 分岐検索 ---
    private DialogueBranch FindBranch(ScenarioStep step, CardColor colorKey)
    {
        var match = step.Branches.Find(b => b.TargetColor == colorKey);
        if (match != null) return match;

        var any = step.Branches.Find(b => b.TargetColor == CardColor.Any);
        if (any != null) return any;

        if (step.Branches.Count > 0) return step.Branches[0];

        return new DialogueBranch { Text = "..." };
    }
}