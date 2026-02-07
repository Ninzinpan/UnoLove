
using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "NewTopic", menuName = "ChatSystem/TopicScenario")]
public class TopicScenario : ScriptableObject
{
    public string TopicId;
    public List<ScenarioStep> Steps = new List<ScenarioStep>();
    
    [Header("ブレイク時のランダム会話リスト")]
    // 文字列ではなく、1ステップ分のシナリオとして定義することで、表情などもつけられる
    public List<ScenarioStep> BreakSteps = new List<ScenarioStep>();

    
    private void OnValidate()
    {
        // 通常の会話リストのチェック
        ValidateList(Steps);
        // ブレイク用リストのチェック
        ValidateList(BreakSteps);
    }
    
    



private void ValidateList(List<ScenarioStep> targetSteps)
    {
        if (targetSteps == null) return;
        foreach (var step in targetSteps)
        {
            if (step == null) continue;
            if (step.Branches == null) continue;

            foreach (var branch in step.Branches)
            {
                if (branch == null) continue;
                // 1. 色の修正: アルファ値(透明度)が0なら、設定漏れとみなして赤(初期値)にする
                if (branch.TextColor.a == 0f)
                {
                    branch.TextColor = Color.black; // ここをお好みの色(Color.whiteなど)に変更可
                }

                // 2. TargetColorの修正:
                // 「テキストが空」かつ「TargetColorがRed(Enumの0番目)」の場合、
                // 新規作成されたばかりとみなして Any に書き換える
                // ※「あえてRedで、テキストも空にしたい」場合は手動で戻す必要がありますが、稀なケースと想定
                /*
                if (string.IsNullOrEmpty(branch.Text) && branch.TargetColor == CardColor.Red)
                {
                    branch.TargetColor = CardColor.Any;
                }
                */
            }
        }

    }

}