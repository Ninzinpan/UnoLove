using Mono.Cecil.Cil;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreUi : MonoBehaviour
{
    [SerializeField]
    ScoreManager scoreManager;
    [SerializeField]
    TurnManager turnManager;
    [SerializeField]
    TextMeshProUGUI text;
    private string whoseTurntext;
        private int totalTurn;


    private int totalScore;
    private int UI_currentScore;

    private int comboCount;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        whoseTurntext = (turnManager.CurrentTurn ==  WhoseTurn.Player ) ? "Player" : "Opponent"  ;
        totalTurn = turnManager.TurnCount;
        UI_currentScore = scoreManager.FinalScore;
        totalScore= scoreManager.CurrentScore; 
        comboCount = scoreManager.CurrentComboCount;

        text.text = $"TurnPlayer:{whoseTurntext}\nTurn:{totalTurn}\nTotalScore:{totalScore}\nCurrent Score:{UI_currentScore}\nCombo count:{comboCount}";

    }
}
