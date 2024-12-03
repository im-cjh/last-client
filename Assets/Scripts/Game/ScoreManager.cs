using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance = null;
    [SerializeField] private TextMeshProUGUI scoreText;
    private int score = 0;
    [SerializeField] private int scorePerCard = 5;
    [SerializeField] private HandManager handManager;
    private int nextScore;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        nextScore = scorePerCard;
    }

    public int GetScore()
    {
        return score;
    }

    public void AddScore()
    {
        score++;
        scoreText.text = $"{score}";

        if (score >= nextScore)
        {
            GiveCard();
            nextScore += scorePerCard;
        }
    }

    private void GiveCard()
    {
        // handManager.AddCard();
    }
}
