using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Rendering;

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

    private TaskCompletionSource<CardView> _tcs;
    private CardView _selectedCard;


    public enum TurnPhase
    {
        Init,
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
    
    async void Start()
    {
        player.OnCardPlayed = (card) => {_tcs?.TrySetResult(card);};
        await MainGameloop();
    }

private async Task MainGameloop()
    
    {
        gameEndState = GameEndState.Continue;
       player.InitializeDuelist();
        opponent.InitializeDuelist();
        await player.DrawCardtoHand(player.InitialHandCount);
        await opponent.DrawCardtoHand(opponent.InitialHandCount);
        
        while (true){
        await TurnSequence(player, WhoseTurn.Player);
        if (gameEndState != GameEndState.Continue)
            {
                break;
            }
        await TurnSequence(opponent, WhoseTurn.Opponent);
        if (gameEndState != GameEndState.Continue)
            {
                break;
            }

    }
    }
private async Task TurnSequence(DuelistManager duelist, WhoseTurn turn)
    {
        Debug.Log($"{turn}のターンが始まりました。{turnCount}ターン目");

        await SelectPhase(duelist, turn);
        await PlayPhase(duelist, turn);
        await CalculatePhase(duelist, turn);
        await DrawPhase(duelist, turn);


        
        check_game_end();
        turnCount++;

        Debug.Log($"{turn}のターンが終了しました。");




        
    }
    private async Task DrawPhase(DuelistManager duelist, WhoseTurn turn)
    {
        currentTurn = turn;
        currentPhase = TurnPhase.Draw;
        Debug.Log($"{turn}のドローフェイズが始まりました。");
        await duelist.DrawCardtoHand(1);
        await Task.Delay(50); // 少し待つ
    }

    private async Task SelectPhase(DuelistManager duelist, WhoseTurn turn)
    {
        currentTurn = turn;
        currentPhase = TurnPhase.Select;
        Debug.Log($"{turn}のセレクトフェイズが始まりました。");
        SetPlayerInputEnabled(turn == WhoseTurn.Player);
        if (turn == WhoseTurn.Player){
        _tcs = new TaskCompletionSource<CardView>();
        _selectedCard = await _tcs.Task;
        }
        else
        {
            var playerHandManager = player.HandManager;
            _selectedCard = await duelist.CPUSelectCard(playerHandManager.Hand,  fieldManager.CurrentFieldCardView);
        }
        _tcs = null;
        if (_selectedCard == null)
        {
            Debug.LogWarning($"{turn}がカードを選択できませんでした。");
            return;
        }
        Debug.Log($"{turn}がカードを選択しました: {_selectedCard.Data.name}");

        SetPlayerInputEnabled(false);
        await Task.Delay(500); // 少し待つ


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
    }



    // Update is called once per frame


    private void check_game_end()
    {
        if (turnCount >= limitTurn)
        {
            gameEndState = GameEndState.Lose;
            Debug.Log("ターンリミットです。ゲームを終了します。");
            OnGameFinished?.Invoke(gameEndState);
            return;
        }
        if (scoreManager.CurrentScore >= targetScore)
        {
            gameEndState = GameEndState.Victory;
            Debug.Log("目標スコアを達成しました。ゲームを終了します。");
            OnGameFinished?.Invoke(gameEndState);

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
}
