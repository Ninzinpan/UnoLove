using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.Linq;

public class CsvToTopicScenarioConverter
{
    // メニューバーに追加される項目
    [MenuItem("ChatSystem/Convert CSV to TopicScenario")]
    public static void ConvertCsv()
    {
        // 1. 選択されているファイルを取得
        UnityEngine.Object selectedObject = Selection.activeObject;
        string assetPath = AssetDatabase.GetAssetPath(selectedObject);

        // CSVファイルかどうかのチェック
        if (string.IsNullOrEmpty(assetPath) || !assetPath.EndsWith(".csv"))
        {
            Debug.LogError("[CSV Converter] Error: Please select a .csv file in the Project window.");
            return;
        }

        // 2. ファイル読み込みとセットアップ
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
        string csvContent = File.ReadAllText(fullPath);
        string fileName = Path.GetFileNameWithoutExtension(assetPath);

        // ScriptableObjectの生成
        TopicScenario scenario = ScriptableObject.CreateInstance<TopicScenario>();
        scenario.TopicId = fileName; // ファイル名をIDにする
        scenario.Steps = new List<ScenarioStep>();
        scenario.BreakSteps = new List<ScenarioStep>(); // 空リストで初期化

        // 3. CSVの解析開始
        // 改行コードで行に分割 (CRLF, LF対応)
        string[] lines = csvContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        if (lines.Length < 2)
        {
            Debug.LogError($"[CSV Converter] Error: File '{fileName}' is empty or has no data.");
            return;
        }

        // ヘッダー行の解析（列の場所を特定する）
        string headerLine = lines[0];
        Dictionary<string, int> headers = ParseHeaders(headerLine);

        // データ行のループ
        int successCount = 0;
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue; // 空行はスキップ

            try
            {
                // カンマ区切りの分解（ダブルクォート内のカンマを無視する正規表現）
                List<string> columns = ParseCsvLine(line);

                // Stepの生成と解析
                ScenarioStep step = ParseStep(columns, headers, fileName, i + 1);
                
                if (step != null)
                {
                    scenario.Steps.Add(step);
                    successCount++;
                }
            }
            catch (Exception e)
            {
                // 想定外のエラーキャッチ
                Debug.LogError($"[CSV Error] Unknown error at row {i + 1}: {e.Message}");
            }
        }

        // 4. アセットとして保存
        string dirPath = Path.GetDirectoryName(assetPath);
        string newAssetPath = Path.Combine(dirPath, $"{fileName}.asset");

        AssetDatabase.CreateAsset(scenario, newAssetPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"[CSV Converter] Success! Generated '{fileName}.asset' with {successCount} steps.");
    }

    // --- ヘルパーメソッド ---

    // ヘッダー行から "列名" -> "インデックス" の辞書を作る
    private static Dictionary<string, int> ParseHeaders(string line)
    {
        var headers = new Dictionary<string, int>();
        var cols = ParseCsvLine(line);
        for (int i = 0; i < cols.Count; i++)
        {
            // BOM(Byte Order Mark)除去と空白除去
            string key = cols[i].Trim().Trim('\uFEFF'); 
            headers[key] = i;
        }
        return headers;
    }

    // 行データの解析 (メインロジック)
    private static ScenarioStep ParseStep(List<string> columns, Dictionary<string, int> headers, string fileName, int rowNum)
    {
        ScenarioStep step = new ScenarioStep();
        step.Branches = new List<DialogueBranch>();

        // 1. IsPlayer (必須・厳格チェック)
        string isPlayerStr = GetColumnValue(columns, headers, "IsPlayer");
        if (string.IsNullOrEmpty(isPlayerStr))
        {
            Debug.LogError($"[CSV Error] File: {fileName} (Row {rowNum}) - 'IsPlayer' is empty. Step skipped.");
            return null; // 生成しない
        }

        isPlayerStr = isPlayerStr.ToLower();
        if (isPlayerStr == "t" || isPlayerStr == "true")
        {
            step.IsPlayer = true;
        }
        else if (isPlayerStr == "f" || isPlayerStr == "false")
        {
            step.IsPlayer = false;
        }
        else
        {
            Debug.LogError($"[CSV Error] File: {fileName} (Row {rowNum}) - Invalid 'IsPlayer' value: '{isPlayerStr}'. Must be 't' or 'f'. Step skipped.");
            return null; // 生成しない
        }

        // 2. Timing (デフォルト false)
        string timingStr = GetColumnValue(columns, headers, "Timing");
        step.WaitForInput = IsTrueString(timingStr);

        // 3. 各色のブランチ生成
        // (Any, Red, Green, Blue) の順でチェックして追加
        AddBranchIfTextExists(step, columns, headers, "Any", "Face_Any", CardColor.Any, fileName, rowNum);
        AddBranchIfTextExists(step, columns, headers, "Red", "Face_Red", CardColor.Red, fileName, rowNum);
        AddBranchIfTextExists(step, columns, headers, "Green", "Face_Green", CardColor.Green, fileName, rowNum);
        AddBranchIfTextExists(step, columns, headers, "Blue", "Face_Blue", CardColor.Blue, fileName, rowNum);

        // ブランチが1つもない場合（全部空欄の行）は警告を出してスキップ
        if (step.Branches.Count == 0)
        {
            Debug.LogWarning($"[CSV Warning] File: {fileName} (Row {rowNum}) - No text found in any color columns. Step skipped.");
            return null;
        }

        return step;
    }

    // テキストがあればブランチを作ってリストに追加する処理
    private static void AddBranchIfTextExists(ScenarioStep step, List<string> columns, Dictionary<string, int> headers, 
                                              string textColName, string faceColName, CardColor targetColor, 
                                              string fileName, int rowNum)
    {
        string text = GetColumnValue(columns, headers, textColName);
        
        // テキストがなければ何もしない
        if (string.IsNullOrEmpty(text)) return;

        // 改行コードの変換 (Excelセル内改行対応)
        text = text.Replace("\n", "\\n").Replace("\r", ""); // 必要に応じて調整

        DialogueBranch branch = new DialogueBranch();
        branch.TargetColor = targetColor;
        branch.Text = text;
        branch.TextColor = Color.black; // デフォルト黒

        // 表情の解析
        string faceStr = GetColumnValue(columns, headers, faceColName);
        branch.Face = ParseExpressionType(faceStr, fileName, rowNum);

        step.Branches.Add(branch);
    }

    // Enumのあいまい検索・補完ロジック
    private static ExpressionType ParseExpressionType(string rawInput, string fileName, int rowNum)
    {
        // 空欄ならNormal (Silent)
        if (string.IsNullOrWhiteSpace(rawInput)) return ExpressionType.Normal;

        string normalizedInput = rawInput.Trim().ToLower();

        // Enumの定義名を全検索
        foreach (string name in Enum.GetNames(typeof(ExpressionType)))
        {
            if (name.ToLower() == normalizedInput)
            {
                return (ExpressionType)Enum.Parse(typeof(ExpressionType), name);
            }
        }

        // 見つからなかった場合 -> 警告を出してNormal
        Debug.LogWarning($"[CSV Warning] File: {fileName} (Row {rowNum}) - Face type '{rawInput}' not found. Replaced with 'Normal'.");
        return ExpressionType.Normal;
    }

    // カラム取得の安全装置
    private static string GetColumnValue(List<string> columns, Dictionary<string, int> headers, string key)
    {
        if (headers.ContainsKey(key))
        {
            int index = headers[key];
            if (index < columns.Count)
            {
                return columns[index];
            }
        }
        return "";
    }

    // CSVの1行を正しく分割する正規表現 (カンマを含む文字列対応)
    private static List<string> ParseCsvLine(string line)
    {
        var list = new List<string>();
        // カンマで分割するが、ダブルクォートで囲まれたカンマは無視する正規表現
        string pattern = ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
        
        string[] fields = Regex.Split(line, pattern);

        foreach (string field in fields)
        {
            // 前後のダブルクォートを除去し、2重ダブルクォートを1つに戻す
            string value = field.TrimStart(' ').TrimEnd(' ');
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Substring(1, value.Length - 2);
            }
            value = value.Replace("\"\"", "\"");
            list.Add(value);
        }
        return list;
    }

    // "t" / "true" 判定
    private static bool IsTrueString(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;
        s = s.ToLower();
        return s == "t" || s == "true";
    }
}