using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneChanger : MonoBehaviour
{
    public enum SceneType
    {
        Lobby, // 로비 씬
        Game,   // 게임 씬
        TestGame
    }

    public static event Action OnSceneLoaded; // 씬 로드 완료 시 실행될 이벤트

    private void Start()
    {
        DontDestroyOnLoad(this); // 씬 전환 시 파괴되지 않음
    }

    // 씬 전환 메서드
    public static void ChangeScene(SceneType sceneType)
    {
        string sceneName = GetSceneName(sceneType);
        Debug.Log($"씬 전환 요청: {sceneName}");
        SceneManager.LoadScene(sceneName);
        SceneManager.sceneLoaded -= OnSceneLoadedHandler; // 중복 제거
        SceneManager.sceneLoaded += OnSceneLoadedHandler;
    }

    // 열거형을 기반으로 씬 이름 반환
    private static string GetSceneName(SceneType sceneType)
    {
        return sceneType switch
        {
            SceneType.Lobby => "LobbyScene",
            SceneType.Game => "GameScene",
            SceneType.TestGame => "TestGameScene",
            _ => throw new ArgumentException("잘못된 씬 타입입니다."),
        };
    }

    // 씬 로드 완료 시 호출
    private static void OnSceneLoadedHandler(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"씬 로드 완료: {scene.name}");

        // 씬 로드 완료 이벤트 호출
        OnSceneLoaded?.Invoke();

        // 이벤트 핸들러 제거 (중복 호출 방지)
        SceneManager.sceneLoaded -= OnSceneLoadedHandler;
    }
}
