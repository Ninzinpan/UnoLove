using UnityEngine;

[CreateAssetMenu(fileName = "NewSpeaker", menuName = "ChatSystem/Speaker")]
public class ChatSpeaker : ScriptableObject
{
    [Header("基本情報")]
    public string CharacterName; // 名前（表示用）
    public Sprite Icon;          // アイコン画像

    [Header("UI設定")]
    public bool IsPlayer;        // trueなら右側(緑)、falseなら左側(白)に表示
    public Color ThemeColor = Color.white; // 吹き出しの色などを変えたい場合用
}