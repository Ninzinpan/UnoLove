using Mono.Cecil.Cil;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
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
        UI_currentScore = 0;
        totalScore= scoreManager.CurrentScore; 
        comboCount = scoreManager.CurrentComboCount;
        string  forbitten_color = "";
        switch (totalTurn % 4)
        {
            case 0 :
            forbitten_color = "<color=#ff0000>Red</color>";
            break;
            case 1 :
                forbitten_color = "<color=#0000ff>Blue</color>";
                    break;
            case 2 :
                forbitten_color = "<color=#008000>Green</color>";
                break;
            case 3 :
                forbitten_color = "<color=#ffff00>Yellow</color>";
                break; 






        }

        text.text = $"TurnPlayer:{whoseTurntext}\nTurn:{totalTurn}\nTotalScore:{totalScore}\nCurrent Score:{UI_currentScore}\nCombo count:{comboCount}\n{forbitten_color}";

    }
}
