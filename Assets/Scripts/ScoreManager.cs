using System.Collections.Generic;

using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private double comboMagnificatioin = 0.2;
    [SerializeField]
    private int baseScorePerCard = 100;
    [SerializeField]
    private int currentScore = 0;
    [SerializeField]
    private int currentComboCount = 0;
    [SerializeField]
    private int finalScore;
    
    public int CurrentScore => currentScore;

    public int FinalScore => finalScore;
    public int CurrentComboCount => currentComboCount;
    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CalculateScore(List<CardData> fielddatas)
    {
        finalScore = 0;
        if (fielddatas == null|| fielddatas.Count == 0)
        {
            Debug.LogWarning("dataのリストが空です。");
            return;
            
        
        }
        if (fielddatas.Count == 1)
        {
            currentComboCount = 0;
            finalScore = baseScorePerCard;
        }
        else {
        
        var currentdata = fielddatas[fielddatas.Count -1 ];
        var furtherdata = fielddatas[fielddatas.Count -2];
        if ((currentdata.Color == furtherdata.Color)  ||(currentdata.Type == furtherdata.Type))
        {
            currentComboCount += 1;
            var finaldoubleScore = baseScorePerCard + (baseScorePerCard*  comboMagnificatioin *currentComboCount);
            finalScore = (int)finaldoubleScore;
        }
        else{
            currentComboCount = 0;
            finalScore = baseScorePerCard;
        }
        }
            Debug.Log($"スコアが{finalScore}ポイント加算されます。現在のコンボ:{currentComboCount}");
            currentScore  += finalScore;

   
    
    }
}

