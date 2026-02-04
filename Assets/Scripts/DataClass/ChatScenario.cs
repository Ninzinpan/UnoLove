using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewScenario", menuName = "ChatSystem/Scenario")]
public class ChatScenario : ScriptableObject
{
    [Header("シナリオ設定")]
    public string ScenarioID; // 管理用ID（例: "topic_love_lv1"）
    
    [Header("会話内容")]
    public List<ChatMessage> Messages = new List<ChatMessage>();
}

// 1行分のメッセージデータ
// クラスの中に定義するか、別ファイルにしてもOKですが、今回はセットで扱います
[System.Serializable]
public class ChatMessage
{
    [Header("誰が話すか")]
    public ChatSpeaker Speaker;

    [Header("内容")]
    [TextArea(3, 5)] // Inspectorで複数行入力できるようにする
    public string MessageText;

    [Header("演出")]
    [Tooltip("このメッセージを表示した後の待機時間（秒）")]
    public float Duration; // 次のメッセージが出るまでの間隔

    public AudioClip VoiceClip;   // ボイスがあれば設定
}