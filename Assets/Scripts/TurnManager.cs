using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Rendering;

public class TurnManager : MonoBehaviour
{
    [SerializeField]
    private DuelistManager player;
        [SerializeField]
    private DuelistManager opponent;
    [SerializeField]
    private CanvasGroup canvasGroup;

[SerializeField]
private int turnCount = 0;
[SerializeField]
private int limitTurn = 5;

    private WhoseTurn currentTurn;
    private TurnPhase currentPhase;
    private bool ifGameEnd = false;

    public WhoseTurn CurrentTurn => currentTurn;
    public TurnPhase CurrentPhase {get; private set;}
    private TaskCompletionSource<CardView> _tcs;

public enum WhoseTurn
    {
        Player,
        Opponent
    }
    public enum TurnPhase
    {
        Init,
        Draw,
        Select,
        Play,
        End

    }
    
    async void Start()
    {
        player.OnCardPlayed = (card) => {_tcs?.TrySetResult(card);};
        await MainGameloop();
    }

private async Task MainGameloop()
    
    {
       player.InitializeDuelist();
        opponent.InitializeDuelist();
        await player.DrawCardtoHand(player.InitialHandCount);
        await opponent.DrawCardtoHand(opponent.InitialHandCount);
        
        while (!ifGameEnd){
        await TurnSequence(player, WhoseTurn.Player);
        await TurnSequence(opponent, WhoseTurn.Opponent);

    }
    }
private async Task TurnSequence(DuelistManager duelist, WhoseTurn turn)
    {
        Debug.Log($"{turn}のターンが始まりました。{turnCount}ターン目");
        await DrawPhase(duelist, turn);
        await SelectPhase(duelist, turn);
        
        turnCount++;
        check_game_end();
        Debug.Log($"{turn}のターンが終了しました。");




        
    }
    private async Task DrawPhase(DuelistManager duelist, WhoseTurn turn)
    {
        currentTurn = turn;
        currentPhase = TurnPhase.Draw;
        Debug.Log($"{turn}のドローフェイズが始まりました。");
        await duelist.DrawCardtoHand(1);
    }

    private async Task SelectPhase(DuelistManager duelist, WhoseTurn turn)
    {
        currentTurn = turn;
        currentPhase = TurnPhase.Select;
        Debug.Log($"{turn}のセレクトフェイズが始まりました。");
        SetPlayerInputEnabled(turn == WhoseTurn.Player);
        _tcs = new TaskCompletionSource<CardView>();
        CardView selectedCard = await _tcs.Task;
        _tcs = null;
        Debug.Log($"{turn}が{selectedCard.Data.name}カードを選択しました: {selectedCard.Data.name}");

        SetPlayerInputEnabled(false);


    }



    // Update is called once per frame


    private void check_game_end()
    {
        if (turnCount >= limitTurn)
        {
            ifGameEnd = true;
            Debug.Log("ゲーム終了条件を満たしました。ゲームを終了します。");
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
