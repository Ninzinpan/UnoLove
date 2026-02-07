using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "HeroineProfile", menuName = "ChatSystem/HeroineProfile")]
public class HeroineProfile : ScriptableObject
{
    [System.Serializable]
    public class ExpressionData
    {
        public ExpressionType Type;
        public Sprite Sprite;
    }

    [Header("表情と画像のペア設定")]
    [SerializeField]
    private List<ExpressionData> expressions = new List<ExpressionData>();

    /// <summary>
    /// ExpressionTypeに応じた画像を返す。見つからなければNormalを返す。
    /// </summary>
    public Sprite GetSprite(ExpressionType type)
    {
        // 指定されたタイプを検索
        var data = expressions.FirstOrDefault(x => x.Type == type);
        if (data != null && data.Sprite != null)
        {
            return data.Sprite;
        }
        
        // 見つからない場合はNormalをフォールバックとして使用
        var normal = expressions.FirstOrDefault(x => x.Type == ExpressionType.Normal);
        return normal != null ? normal.Sprite : null;
    }
}