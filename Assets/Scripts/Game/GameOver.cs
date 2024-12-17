using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GameOver : MonoBehaviour
{
    public static bool isGameOver;

    public static GameOver instance;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private Button lobbyButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        //ShowGameOver();
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        //gameOverPanel.transform.localScale = Vector3.zero;
        //gameOverPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        scoreText.text = $": {ScoreManager.instance.GetScore()}";
        waveText.text = $"Wave: {ScoreManager.instance.GetWave()}";
        lobbyButton.onClick.AddListener(BackToLobby);
        Time.timeScale = 0f;
    }

    private void BackToLobby()
    {
        Time.timeScale = 1f;
        SceneChanger.ChangeScene(SceneChanger.SceneType.Lobby);
        gameOverPanel.SetActive(false);
    }
}
