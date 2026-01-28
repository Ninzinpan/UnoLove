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
        await MainGameloop();
    }

private async Task MainGameloop()
    
    {
       player.InitializeDuelist();
        opponent.InitializeDuelist();
        await player.DrawCardtoHandWithAnimation(player.InitialHandCount);
        await opponent.DrawCardtoHandWithAnimation(opponent.InitialHandCount);

        while (!ifGameEnd){
        await TurnSequence(player, WhoseTurn.Player);
        await TurnSequence(opponent, WhoseTurn.Opponent);

    }
    }
private async Task TurnSequence(DuelistManager duelist, WhoseTurn turn)
    {
        Debug.Log($"{turn}のターンが始まりました。{turnCount}ターン目");
        currentTurn = turn;
        await duelist.DrawCardtoHandWithAnimation(1);
        SetPlayerInputEnabled(turn == WhoseTurn.Player);
        turnCount++;
        check_game_end();
        Debug.Log($"{turn}のターンが終了しました。");




        
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
            Debug.Log($"プレイヤーの入力を{(ennabled ? "有効" : "無効")}にしました。");
        }
        else
        {
            Debug.LogWarning("CanvasGroupが割り当てられていません。");
        }
    }
}
