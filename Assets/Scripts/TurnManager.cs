using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Mono.Cecil.Cil;

public enum WhoseTurn
    {
        Player,
        Opponent
    }

public class TurnManager : MonoBehaviour
{
    [SerializeField]
    private DuelistManager player;
        [SerializeField]
    private DuelistManager opponent;
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private FieldManager fieldManager;
        [SerializeField]
    private ScoreManager scoreManager;
    [SerializeField]
    private ChatController chatController;
    [SerializeField]
    private ChatScenario testScenario;

[SerializeField]
private int turnCount = 1;
[SerializeField]
private int limitTurn = 5;

[SerializeField]
private int targetScore = 500;

    private WhoseTurn currentTurn;
    private TurnPhase currentPhase;
    private GameEndState gameEndState;

    public WhoseTurn CurrentTurn => currentTurn;
    public int TurnCount => turnCount;
    public int LimitTurn => limitTurn;

    public int TargetScore => targetScore;
    public TurnPhase CurrentPhase {get; private set;}

    public static event System.Action<GameEndState> OnGameFinished;

    private TaskCompletionSource<BaseCardView> _tcs;
    private BaseCardView _selectedCard;


    public enum TurnPhase
    {
        Init,
        StandBy,
        Draw,
        Select,
        Play,
        End

    }

        public enum GameEndState{
        Continue,
        Victory,
        Lose
        
    }

    public enum SelectContinueState
    {
        Hold,
        Continue,
        Finish,
        Error
    }
    

    async void Start()
    {
        player.OnCardPlayed += OnPlayerPlayCard;
        await MainGameloop();
    }
private void OnPlayerPlayCard(BaseCardView card)
    {
        _tcs?.TrySetResult(card);
    }

private async Task MainGameloop()
    
    {
        gameEndState = GameEndState.Continue;
       player.InitializeDuelist();
        opponent.InitializeDuelist();
        fieldManager.Initialieze();
        Debug.Log("chatテスト。");
        await chatController.RequestChat(testScenario);
        Debug.Log("chatテスト終了。");



        while (true){
        Debug.Log("新たなセッションを開始します。");
        while (player.HandManager.Hand.Count < player.InitialHandCount)            
            {
                if (player.HandManager.Hand.Count >= player.InitialHandCount) break;
                await player.DrawCardtoHand(1);
            }
            while (opponent.HandManager.Hand.Count < opponent.InitialHandCount)            
            {
                if (opponent.HandManager.Hand.Count >= opponent.InitialHandCount) break;
                await opponent.DrawCardtoHand(1);
            }


        var ComboResult = await ComboLoop(player, opponent);
        
        check_game_end();
        fieldManager.ResetFieldCard();
        scoreManager.ResetCurrentTopic();

        if (gameEndState != GameEndState.Continue)
            {
                break;
            }

        await Task.Delay(100);
    }

    }
private async Task<(SelectContinueState selectContinueState,WhoseTurn turn)> ComboLoop(DuelistManager player,DuelistManager opponent)
    {
        while(true){

        if (await player.CPUGetIfSessionContinued(fieldManager.CurrentFieldCardView) == SelectContinueState.Finish)
            {
                Debug.Log("Playerの出せるカードがありません。コンボを終了します。");
                return (SelectContinueState.Finish,WhoseTurn.Player);
            }

        await TurnSequence(player, WhoseTurn.Player);

        
        if (gameEndState != GameEndState.Continue)
            {
                return (SelectContinueState.Finish,WhoseTurn.Player);
            }

        if (await opponent.CPUGetIfSessionContinued(fieldManager.CurrentFieldCardView) == SelectContinueState.Finish)
            {
                Debug.Log("Opponentの出せるカードがありません。コンボを終了します。");
                return (SelectContinueState.Finish,WhoseTurn.Opponent);
            }
        await TurnSequence(opponent, WhoseTurn.Opponent);
        if (gameEndState != GameEndState.Continue)
            {
                return (SelectContinueState.Finish,WhoseTurn.Player);
            }


        }
    }

private async Task TurnSequence(DuelistManager duelist, WhoseTurn turn)
    {
        Debug.Log($"{turn}のターンが始まりました。{turnCount}ターン目");
        await StandByPhase(duelist, turn);
 
        //await DrawPhase(duelist, turn);
                


        await SelectPhase(duelist, turn);
        await PlayPhase(duelist, turn);
        await CalculatePhase(duelist, turn);
        await EndPhases(duelist, turn);


        turnCount++;

        check_game_end();

        Debug.Log($"{turn}のターンが終了しました。");




        
    }

    private async Task StandByPhase(DuelistManager duelist, WhoseTurn turn)
    {
        currentTurn = turn;
        currentPhase = TurnPhase.StandBy;
        Debug.Log($"{turn}のスタンバイフェイズが始まりました。");
        
        await Task.Delay(50);

    }
    private async Task DrawPhase(DuelistManager duelist, WhoseTurn turn)
    {
        currentTurn = turn;
        currentPhase = TurnPhase.Draw;
        Debug.Log($"{turn}のドローフェイズが始まりました。");
        await duelist.DrawCardtoHand(duelist.InitialHandCount);
        
        await Task.Delay(50); // 少し待つ
    }

    private async Task SelectPhase(DuelistManager duelist, WhoseTurn turn)
    {
        currentTurn = turn;
        currentPhase = TurnPhase.Select;
        Debug.Log($"{turn}のセレクトフェイズが始まりました。");
        SetPlayerInputEnabled(turn == WhoseTurn.Player);
        if (turn == WhoseTurn.Player){

        _selectedCard = await PlayerSelectCard();


        _selectedCard = await _tcs.Task;
        
        }
        else
        {
            var playerHandManager = player.HandManager;
            _selectedCard = await duelist.CPUSelectCard(playerHandManager.Hand,  fieldManager.CurrentFieldCardView);
        }
        if (_selectedCard == null)
        {
            Debug.LogWarning($"{turn}がカードを選択できませんでした。");
            return;
        }
        Debug.Log($"{turn}がカードを選択しました: {_selectedCard.Data.name}");
        if (fieldManager.CurrentFieldCardView.Data.Type == CardType.None)
        {
            scoreManager.SetPlayedTopic(_selectedCard.Data.Type);
        }
        SetPlayerInputEnabled(false);
        await Task.Delay(500); // 少し待つ


    }
private async Task<BaseCardView> PlayerSelectCard()
    {
while (true)
            {
                // 1. 新しい「待ち受け」を作る
                _tcs = new TaskCompletionSource<BaseCardView>();

                // 2. プレイヤーの入力を待つ
                Debug.Log("カードを選択してください...");
                var tempSelectedCard = await _tcs.Task;

                // 3. ルール判定 (バリデーション)
                // フィールドのカード情報を取得 (初回などでnullの場合は通す、などの処理も可)
                var currentFieldCard = fieldManager.CurrentFieldCardView?.Data;
                var cPUBrain =new CPUBrain();
                if (currentFieldCard.Type == CardType.None && tempSelectedCard != null)
                {
                    // OKなら採用してループを抜ける
                    Debug.Log("有効な最初のカードが選択されました。");
                    return tempSelectedCard;
                }
                if (cPUBrain.CheckIfCardMatch(tempSelectedCard, fieldManager.CurrentFieldCardView))
            {
                    // OKなら採用してループを抜ける
                    Debug.Log("選択されたカードがフィールドカードとマッチしました。有効なカードです。");
                    return tempSelectedCard;
            }
                else
                {
                    // NGなら再度選択を促す
                    Debug.Log("選択されたカードがフィールドカードとマッチしません。再度選択を待ちます。");
                }
    }
    }



    private async Task PlayPhase(DuelistManager duelist, WhoseTurn turn)
    {
        currentTurn = turn;
        currentPhase = TurnPhase.Play;
        Debug.Log($"{turn}のプレイフェイズが始まりました。");
        if (_selectedCard != null)
        {
            Debug.Log($"{turn}がカードをプレイしました: {_selectedCard.Data.name}");
            if (fieldManager != null)
            {
                fieldManager.AddCard(_selectedCard.Data);
            }
            else
            {
                Debug.LogWarning("FieldManagerが割り当てられていません。");

        }
                    duelist.DiscardCardFromHand(_selectedCard);
                    _selectedCard = null;

        }
        else
        {
            Debug.LogWarning($"{turn}が選択したカードがありません。");
        }
        await Task.Delay(500); // 少し待つ
        
    }
    private async Task CalculatePhase(DuelistManager duelist, WhoseTurn turn)
    {
        Debug.Log($"{turn}のCalculatePhaseが始まりました");
        if (fieldManager == null)
        {
            Debug.LogWarning("fieldManagerが割り当てられていません");
            return;
        }
        var fieldCards = fieldManager.FieldCards;
        scoreManager.CalculateScore(fieldCards);
        await Task.Delay(500);
        return;

    }

    private async Task EndPhases(DuelistManager duelist, WhoseTurn turn)
    {
        Debug.Log($"{turn}のエンドフェイズがはじまりました。");
        
        await Task.Delay(500);
    }



    // Update is called once per frame


    private void check_game_end()
    {
        if (scoreManager.CurrentScore >= targetScore)
        {
            gameEndState = GameEndState.Victory;
            Debug.Log("目標スコアを達成しました。ゲームを終了します。");
            OnGameFinished?.Invoke(gameEndState);
            return;

        }
        
        if (turnCount >= limitTurn)
        {
            gameEndState = GameEndState.Lose;
            Debug.Log("ターンリミットです。ゲームを終了します。");
            OnGameFinished?.Invoke(gameEndState);
            return;
        }

    }
    void Update()
    {
        
    }
    
    void SetPlayerInputEnabled(bool ennabled)
    {
        if (canvasGroup != null)
        {
            canvasGroup.interactable = ennabled;
            canvasGroup.blocksRaycasts = ennabled;
            canvasGroup.alpha = ennabled ? 1f : 0.5f;
            Debug.Log($"プレイヤーの入力を{(ennabled ? "有効" : "無効")}にしました。");
        }
        else
        {
            Debug.LogWarning("CanvasGroupが割り当てられていません。");
        }
    }
    private void OnDestroy()
    {
        player.OnCardPlayed -= OnPlayerPlayCard;
    }
}
