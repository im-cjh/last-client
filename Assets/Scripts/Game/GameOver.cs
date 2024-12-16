using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public static bool isGameOver { get; private set; }
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private Button lobbyButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (isGameOver)
        {
            Invoke("ShowGameOver", 1f);
        }

        lobbyButton.onClick.AddListener(BackToLobby);
    }

    private void ShowGameOver()
    {
        gameOverPanel.SetActive(true);

        scoreText.text = $": {ScoreManager.instance.GetScore()}";
        waveText.text = $"Wave: {ScoreManager.instance.GetWave()}";

        Time.timeScale = 0f;
    }

    private void BackToLobby()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LobbyScene");
    }
}
