using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; // AudioMixer 사용

public class BattleSettingManager : MonoBehaviour
{
    public GameObject settingsPanel;

    public Slider backgroundMusicSlider; // 배경음악 슬라이더
    public AudioSource backgroundMusic; // 배경음악 AudioSource

    public Slider effectSoundSlider; // 효과음 슬라이더
    public AudioMixer audioMixer; // Audio Mixer

    public Button exitButton; // 게임 종료 버튼

    void Start()
    {
        // 배경음악 슬라이더 초기값 설정
        if (backgroundMusic != null && backgroundMusicSlider != null)
        {
            backgroundMusicSlider.value = backgroundMusic.volume;
            backgroundMusicSlider.onValueChanged.AddListener(SetBackgroundMusicVolume);
        }

        // 효과음 슬라이더 초기값 설정
        if (effectSoundSlider != null)
        {
            float currentVolume;
            audioMixer.GetFloat("EffectVolume", out currentVolume);
            effectSoundSlider.value = Mathf.Pow(10, currentVolume / 20f); // dB 값을 0~1 범위로 변환
            effectSoundSlider.onValueChanged.AddListener(SetEffectSoundVolume);

            if (!audioMixer.GetFloat("EffectVolume", out currentVolume))
            {
                Debug.LogError("EffectVolume parameter not found in Audio Mixer!");
            }
            else
            {
                Debug.Log($"Initial EffectVolume: {currentVolume} dB");
            }
        }

        // 게임 종료 버튼
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

    // 효과음 볼륨 조정 (Audio Mixer 사용)
    public void SetEffectSoundVolume(float volume)
    {
        float dB = (volume <= 0.0001f) ? -80f : Mathf.Log10(volume) * 20; // 0~1 값을 dB로 변환
        audioMixer.SetFloat("EffectVolume", dB);

        // Loop 중인 AudioSource들을 강제로 갱신
        foreach (AudioSource source in FindObjectsOfType<AudioSource>())
        {
            if (source.isPlaying && source.loop)
            {
                source.Pause();  // 재생 일시정지
                source.Play();   // 다시 재생
            }
        }
    }

    // 게임 종료 기능
    public void ExitGame()
    {
        Application.Quit();
    }
}
