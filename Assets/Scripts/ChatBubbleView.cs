using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatBubbleView : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI messageText; 
    [SerializeField] private Image bubbleBackground; 
    [SerializeField] private HorizontalLayoutGroup layoutGroup;

    [Header("Design Settings")]
    [SerializeField] private Color playerBubbleColor = new Color(0.8f, 1f, 0.8f);
    [SerializeField] private Color opponentBubbleColor = Color.white;

    public void SetUp(ChatMessage message)
    {
        // 1. テキスト設定
        messageText.text = message.MessageText;
        messageText.color = message.TextColor; // 文字色適用

        // 2. 配置と色の切り替え (IsPlayerフラグを使用)
        UpdateLayout(message.IsPlayer);
        
        // 3. 相手の表情変更 (Face) の処理が必要ならここに記述
        // 例: GameManager.Instance.SetHeroineFace(message.Face);
    }

    private void UpdateLayout(bool isPlayer)
    {
        if (isPlayer)
        {
            // プレイヤー（右側）
            layoutGroup.childAlignment = TextAnchor.MiddleRight;
            bubbleBackground.color = playerBubbleColor;
        }
        else
        {
            // 相手（左側）
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            bubbleBackground.color = opponentBubbleColor;
        }
    }
}