using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance = null;
    [SerializeField] private TextMeshProUGUI scoreText;
    private int curScore = 0;

    [SerializeField] private TextMeshProUGUI waveText;
    private int curWave = 1;

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

    public void AddWave()
    {
        curWave++;
        waveText.text = $"{curWave} Wave";
    }

    public int GetWave()
    {
        return curWave;
    }
}
