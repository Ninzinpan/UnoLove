using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Threading.Tasks;

public class ChatWindowView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject chatBubblePrefab; 
    [SerializeField] private Transform contentArea;       
    [SerializeField] private ScrollRect scrollRect;       

    // クリックされたことを通知するイベント
    public event Action OnScreenClick;

    public void AddMessage(ChatSequenceData data)
    {
        GameObject obj = Instantiate(chatBubblePrefab, contentArea);
        
        // 最新を一番上に表示
        obj.transform.SetAsFirstSibling();
        
        ChatBubbleView bubble = obj.GetComponent<ChatBubbleView>();
        if (bubble != null)
        {
            // 変更点: 変換せずに data をそのまま渡す
            bubble.SetUp(data);
        }
        AutoScroll();

    }

    // UIの透明ボタンなどから呼ぶ
    public void OnChatWindowClicked()
    {
        Debug.Log("スクリーンボタンが押されました。");
        OnScreenClick?.Invoke();
    }

    public void AutoScroll()
    {
        StartCoroutine(ScrollToTop());
    }

    private IEnumerator ScrollToTop()
    {
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();
        // 上方向へスクロール
        scrollRect.verticalNormalizedPosition = 1f; 
    }
}