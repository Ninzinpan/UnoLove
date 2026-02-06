using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class ChatSequencer : MonoBehaviour
{
    [SerializeField] private ChatWindowView view;

    private bool isPlaying = false;
    private TaskCompletionSource<bool> clickWaiter;

    void Start()
    {
        // Viewのクリックイベントを購読
        if (view != null)
        {
            view.OnScreenClick += OnViewClicked;
        }
    }

    private void OnViewClicked()
    {
        // 待機中のタスクがあれば完了させる
        clickWaiter?.TrySetResult(true);
    }

    /// <summary>
    /// メッセージリストを順番に再生する
    /// </summary>
    public async Task PlaySequence(List<ChatSequenceData> sequence)
    {
        // ※必要であればここでキューイング処理を入れるが、今回は即時再生
        if (isPlaying) 
        {
            // 割り込み時の挙動（今回は前のを待つ簡易実装）
            // 実装によっては前の会話を強制終了させてもよい
            while(isPlaying) await Task.Delay(100);
        }

        isPlaying = true;

        try
        {
            foreach (var data in sequence)
            {
                // 1. 表示
                view.AddMessage(data);

                // 2. 待機制御
                if (data.WaitForInput)
                {
                    // クリック待ち
                    clickWaiter = new TaskCompletionSource<bool>();
                    await clickWaiter.Task;
                    Debug.Log("通過テスト");

                }
                else
                {
                    // 自動送り（秒数待機）
                    // 0秒なら待たない
                    if (data.AutoDelay > 0)
                    {
                        await Task.Delay((int)(data.AutoDelay * 1000));
                    }
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogWarning($"例外処理が発生しました{e}");
        }
        finally
        {
            isPlaying = false;
        }
    }
}