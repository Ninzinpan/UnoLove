using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ChatController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private ChatWindowView chatWindowView;

    [Header("Settings")]
    [SerializeField, Tooltip("文字数に関わらず確保する最低待機時間(秒)")]
    private float baseDuration = 0.8f;
    [SerializeField, Tooltip("1文字あたりの待機時間(秒)")]
    private float durationPerChar = 0.1f;

    // 順番待ちの予約リスト
    // シナリオデータと、その完了を通知するためのTCS(TaskCompletionSource)をペアで管理
    private Queue<(ChatScenario scenario, TaskCompletionSource<bool> tcs)> requestQueue 
        = new Queue<(ChatScenario, TaskCompletionSource<bool>)>();

    private bool isPlaying = false;

    /// <summary>
    /// 外部から会話をリクエストするメソッド。
    /// 会話が終了するまで待機可能な Task を返します。
    /// </summary>
    public Task RequestChat(ChatScenario scenario)
    {
        if (scenario == null) return Task.CompletedTask;

        // 完了通知用のTCSを作成
        var tcs = new TaskCompletionSource<bool>();
        
        // キューに追加
        requestQueue.Enqueue((scenario, tcs));

        // 再生試行（もし再生中でなければ開始）
        TryPlayNext();

        return tcs.Task;
    }

    // キューを確認して次を再生するループ
    private async void TryPlayNext()
    {
        // すでに再生中、またはキューが空なら何もしない
        if (isPlaying || requestQueue.Count == 0) return;

        isPlaying = true;

        // キューから次のリクエストを取り出す
        var (currentScenario, currentTcs) = requestQueue.Dequeue();

  
            // --- 再生開始 ---
            if (chatWindowView != null)
            {
                chatWindowView.SetWindowActive(true);
            }

            // メッセージを1つずつ処理
            foreach (var message in currentScenario.Messages)
            {
                if (chatWindowView != null)
                {
                    chatWindowView.AddMessage(message);
                    chatWindowView.AutoScroll();
                }

                // 待機時間を計算して待つ
                float waitTime = CalculateDuration(message);
                
                // ミリ秒に変換して待機
                await Task.Delay((int)(waitTime * 1000));
            }

            // --- 再生終了 ---
            // 少し余韻を持たせる
            await Task.Delay(1000);

            if (chatWindowView != null)
            {
                // ここでウィンドウを消さない、
                // 今回は連続再生を考慮して、キューが空の時だけ消すなどの調整も可能
            
                if (requestQueue.Count == 0)
                {
                    chatWindowView.SetWindowActive(false);
                }
            }

            // 呼び出し元に「終わったよ」と報告
            currentTcs.TrySetResult(true);

            isPlaying = false;
            // 次の予約があれば連続して再生へ
            TryPlayNext();

    }

    // 待機時間を計算するロジック
    private float CalculateDuration(ChatMessage message)
    {
        // もしデータ側で明示的に指定されている(0より大きい)場合はそれを優先
        // (特定の間を作りたい時などのオーバーライド用)
        if (message.Duration > 0.1f)
        {
            return message.Duration;
        }

        // テキストがない場合はベース時間のみ
        if (string.IsNullOrEmpty(message.MessageText))
        {
            return baseDuration;
        }

        // 計算式: 基本時間 + (文字数 * 係数)
        // 例: 10文字の場合 -> 0.8 + (10 * 0.1) = 1.8秒
        return baseDuration + (message.MessageText.Length * durationPerChar);
    }
}