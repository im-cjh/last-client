using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance = null;
    [SerializeField] private TextMeshProUGUI scoreText;
    private int curScore = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public int GetScore()
    {
        return curScore;
    }

    public void AddScore(int score)
    {
        curScore += score;
        scoreText.text = $"{curScore}";
    }
}
