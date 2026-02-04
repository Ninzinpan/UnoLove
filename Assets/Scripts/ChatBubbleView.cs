using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProを使用する場合

public class ChatBubbleView : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image iconImage;       // 顔アイコン
    [SerializeField] private TextMeshProUGUI messageText; // 本文
    [SerializeField] private Image bubbleBackground; // 吹き出しの背景画像
    [SerializeField] private HorizontalLayoutGroup layoutGroup; // 配置制御用

    [Header("Design Settings")]
    [SerializeField] private Color playerBubbleColor = new Color(0.8f, 1f, 0.8f); // プレイヤーの色(薄緑)
    [SerializeField] private Color opponentBubbleColor = Color.white;             // 相手の色(白)

    public void SetUp(ChatMessage message)
    {
        // 1. テキスト設定
        messageText.text = message.MessageText;

        // 2. アイコン設定
        if (message.Speaker != null && message.Speaker.Icon != null)
        {
            iconImage.sprite = message.Speaker.Icon;
            iconImage.gameObject.SetActive(true);
        }
        else
        {
            iconImage.gameObject.SetActive(false);
        }

        // 3. 配置と色の切り替え
        bool isPlayer = (message.Speaker != null && message.Speaker.IsPlayer);
        UpdateLayout(isPlayer);

        // テーマカラーの適用（Speakerデータに色設定があれば優先、なければデフォルト）
        if (message.Speaker != null && message.Speaker.ThemeColor != Color.white)
        {
             bubbleBackground.color = message.Speaker.ThemeColor;
        }
    }

    private void UpdateLayout(bool isPlayer)
    {
        if (isPlayer)
        {
            // プレイヤー（右側）
            // レイアウトを「右詰め」にする
            layoutGroup.childAlignment = TextAnchor.MiddleRight;
            layoutGroup.reverseArrangement = true; // アイコンを右、吹き出しを左にするために順序反転
            bubbleBackground.color = playerBubbleColor;
        }
        else
        {
            // 相手（左側）
            // レイアウトを「左詰め」にする
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            layoutGroup.reverseArrangement = false; // アイコン左、吹き出し右
            bubbleBackground.color = opponentBubbleColor;
        }
    }
}