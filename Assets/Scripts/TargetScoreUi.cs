using System;
using Mono.Cecil.Cil;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class TargetScoreUi : MonoBehaviour
{
    [SerializeField]
    ScoreManager scoreManager;
    [SerializeField]
    TurnManager turnManager;
    [SerializeField]
    TextMeshProUGUI text;
        [SerializeField]
    TextMeshProUGUI gameEndtext;
        private int totalTurn;


    private int totalScore;

    private int targetScore;
    private int limitTurn;
    private string gameEndMessage = "";



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        TurnManager.OnGameFinished += ShowGameEndMessage;
    }

    // Update is called once per frame
    void Update()
    {
        totalTurn = turnManager.TurnCount;
        limitTurn = turnManager.LimitTurn;
        int time_left = Math.Max(0,limitTurn-totalTurn);
        totalScore= scoreManager.CurrentScore; 
        targetScore = turnManager.TargetScore;
        int goalScore_left = Math.Max(0,targetScore - totalScore);
        text.text = $"Turn Left:{time_left}\nScore Left:{goalScore_left}";



    }
    void ShowGameEndMessage(TurnManager.GameEndState action)
    {
        if (action == TurnManager.GameEndState.Victory)
        {
            gameEndMessage = "<color=#FF0000>You reached the target Score! SUGOI</color>";
            gameEndtext.text = gameEndMessage;
        }
        if (action == TurnManager.GameEndState.Lose)
        {
                gameEndMessage = "<color=#FF0000>Time UP! ZAKOGA</color>";
                gameEndtext.text = gameEndMessage;


            
        }
    }
}
