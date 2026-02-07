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
    [SerializeField] private Color playerDefaultBubbleColor = new Color(0.8f, 1f, 0.8f);
        [SerializeField] private Color playerRedBubbleColor = Color.red;
        [SerializeField] private Color playerGreenBubbleColor = Color.green;
        [SerializeField] private Color playerBlueBubbleColor = Color.blue;


    [SerializeField] private Color opponentBubbleColor = Color.white;

    // 変更点: 引数を ChatSequenceData に変更
    public void SetUp(ChatSequenceData data)
    {
        // 1. テキスト設定
        messageText.text = data.Text;
        messageText.color = data.TextColor; 

        // 2. 配置と色の切り替え
        UpdateLayout(data.IsPlayer);
        
        // 3. 表情の処理 (必要に応じて実装)
        // 例: data.Face を使って立ち絵の表情を変えるイベントを発火するなど
        // if (!data.IsPlayer) { ... }
    }

    private void UpdateLayout(bool isPlayer)
    {
        if (isPlayer)
        {
            // プレイヤー（右側）
            layoutGroup.childAlignment = TextAnchor.MiddleRight;
            // アイコン等の順序反転が必要な場合はここで reverseArrangement = true 等
            bubbleBackground.color = playerDefaultBubbleColor;
        }
        else
        {
            // 相手（左側）
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            bubbleBackground.color = opponentBubbleColor;
        }
    }
}