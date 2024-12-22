using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbySettingManager : MonoBehaviour
{
    public GameObject settingsPanel;

    public Slider backgroundMusicSlider; // 배경음악 슬라이더
    public AudioSource backgroundMusic; // 배경음악 AudioSource
    public Button logoutButton; // 로그아웃 버튼
    public Button exitButton; // 게임 종료 버튼

    void Start()
    {
        // 슬라이더 초기값 설정
        if (backgroundMusic != null && backgroundMusicSlider != null)
        {
            backgroundMusicSlider.value = backgroundMusic.volume;
            backgroundMusicSlider.onValueChanged.AddListener(SetBackgroundMusicVolume);
        }

        // 버튼 이벤트 연결
        if (logoutButton != null)
        {
            logoutButton.onClick.AddListener(Logout);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowSettingPanel();
        }
    }

    void ShowSettingPanel()
    {
        bool isActive = settingsPanel.activeSelf;
        settingsPanel.SetActive(!isActive);
    }

    // 배경음악 볼륨 조정
    public void SetBackgroundMusicVolume(float volume)
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = volume;
        }
    }

    // 로그아웃 기능
    public void Logout()
    {
        PlayerInfoManager.instance.Logout();
        NetworkManager.instance.Logout();
        SceneManager.LoadScene("LoginScene");
    }

    // 게임 종료 기능
    public void ExitGame()
    {
        Application.Quit();
    }
}
