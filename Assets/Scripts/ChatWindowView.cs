using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatWindowView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject chatBubblePrefab; 
    [SerializeField] private Transform contentArea;       
    [SerializeField] private ScrollRect scrollRect;       

    public void AddMessage(ChatMessage message)
    {
        // プレハブを生成
        GameObject obj = Instantiate(chatBubblePrefab, contentArea);
        
        // ★変更点: 最新のメッセージをリストの一番上（ヒエラルキーの先頭）に移動させる
        obj.transform.SetAsFirstSibling();
        
        // セットアップ
        ChatBubbleView bubble = obj.GetComponent<ChatBubbleView>();
        if (bubble != null)
        {
            bubble.SetUp(message);
        }

        // 追加したら自動スクロール
        AutoScroll();
    }

    public void AutoScroll()
    {
        StartCoroutine(ScrollToTop());
    }

    private IEnumerator ScrollToTop()
    {
        // レイアウト更新待ち
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();

        // ★変更点: 一番上（1.0）へスクロール
        scrollRect.verticalNormalizedPosition = 1f;
    }

    public void SetWindowActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}