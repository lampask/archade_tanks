using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ScoreHandler : MonoBehaviour
{
    public static ScoreHandler Instance;
    // Start is called before the first frame update
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;
    public int player1Score = 0;
    public int player2Score = 0;
   

    private void Awake()
    {
        if (Instance == null || Instance.Equals(null))
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        } 
        addScoreToPlayerOne(0);
        addScoreToPlayerTwo(0);
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
