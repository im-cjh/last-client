using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{  
    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public static void ChangeLobbyScene()
    {
        SceneManager.LoadScene("LobbyScene"); 
    }


    public static void ChangeGameScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
