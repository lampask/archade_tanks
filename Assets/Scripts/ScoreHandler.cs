using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ScoreHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;
    private int player1Score = 0;
    private int player2Score = 0;
   

    private void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            addScoreToPlayerOne(0);
            addScoreToPlayerTwo(0);
        }
    }

    public void addScoreToPlayerOne(int score) {
        player1Score += score;
        player1ScoreText.text = player1Score.ToString();
    }

    public void addScoreToPlayerTwo(int score) {
        player2Score += score;
        player2ScoreText.text = player2Score.ToString();
    }

}
