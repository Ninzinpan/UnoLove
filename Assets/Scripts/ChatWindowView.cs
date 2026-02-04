using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatWindowView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject chatBubblePrefab; // ChatBubbleViewがついたプレハブ
    [SerializeField] private Transform contentArea;       // ScrollViewのContent
    [SerializeField] private ScrollRect scrollRect;       // スクロール制御用

    // 外部から呼ばれる: メッセージを追加
    public void AddMessage(ChatMessage message)
    {
        // プレハブを生成
        GameObject obj = Instantiate(chatBubblePrefab, contentArea);
        
        // セットアップ
        ChatBubbleView bubble = obj.GetComponent<ChatBubbleView>();
        if (bubble != null)
        {
            bubble.SetUp(message);
        }
    }

    // 外部から呼ばれる: 自動スクロール
    public void AutoScroll()
    {
        // UIのレイアウト更新を待ってからスクロールするためにコルーチンを使用
        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        // 1フレーム待って、UIのサイズ計算が終わるのを待つ
        yield return new WaitForEndOfFrame();

        // Canvasの更新を強制（念のため）
        Canvas.ForceUpdateCanvases();

        // 一番下（0）へスクロール
        scrollRect.verticalNormalizedPosition = 0f;
    }

    // ウィンドウの表示切り替え
    public void SetWindowActive(bool isActive)
    {
        gameObject.SetActive(isActive);
        
        // 非表示にする時に中身をクリアするかは仕様次第（今回は残す）
        // if (!isActive) ClearMessages(); 
    }
}