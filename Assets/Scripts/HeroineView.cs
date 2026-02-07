using UnityEngine;
using UnityEngine.UI;

public class HeroineView : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image heroineImage;

    [Header("Data")]
    [SerializeField] private HeroineProfile profile;

    public void ChangeExpression(ExpressionType type)
    {
        if (profile == null)
        {
            Debug.LogWarning("HeroineView: Profileが設定されていません。");
            return;
        }

        Sprite sprite = profile.GetSprite(type);
        if (sprite != null)
        {
            heroineImage.sprite = sprite;
            
            // 画像サイズをSpriteに合わせて自動調整したい場合は以下を有効化
            // heroineImage.SetNativeSize(); 
        }
    }
}