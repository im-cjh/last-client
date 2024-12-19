using System.Net.Http;
using System.Text;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LoginManager : MonoBehaviour
{
    [Header("# SignIn")]
    public TMP_Text SignInErrorText;

    public TMP_InputField SignInEmailField;
    public TMP_InputField SignInPwdField;
    public Button PostSignInButton;

    [Header("# SignUp")]
    public TMP_Text SignUpErrorText;
    public TMP_InputField SignUpEmailField;
    public TMP_InputField SignUpPwdField;
    public TMP_InputField SignUpNameField;
    public Button PostSignUpButton;


    [Header("# UI References")]
    public GameObject SignUpPanel;
    public GameObject SignInPanel;

    private void Start()
    {
        PostSignInButton.onClick.AddListener(SignIn);
        PostSignUpButton.onClick.AddListener(SignUp);
    }


    public async void SignIn()
    {
        Debug.Log("ㅇㅇ로그인");
        //string url = "http://ec2-13-125-207-67.ap-northeast-2.compute.amazonaws.com:4000/api/sign/signin";
        string url = "http://localhost:4000/api/sign/signin";
        string json = JsonConvert.SerializeObject(new { email = SignInEmailField.text, password = SignInPwdField.text });

        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(3); // Timeout 설정

                HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                string jsonString = await response.Content.ReadAsStringAsync();
                JObject jsonObj = JObject.Parse(jsonString);

                if (response.IsSuccessStatusCode)
                {

                    string token = jsonObj["token"].ToString();
                    string userId = jsonObj["userId"].ToString();
                    string nickname = jsonObj["nickname"].ToString();

                    PlayerInfoManager.instance.userId = userId;
                    PlayerInfoManager.instance.nickname = nickname;
                    PlayerInfoManager.instance.token = token;

                    NetworkManager.instance.ConnectToGatewayServer();

                    SceneChanger.ChangeScene(SceneChanger.SceneType.Lobby);
                }
                else
                {
                    string errorMessage = jsonObj["message"].ToString();
                    SignInErrorText.text = errorMessage;
                    SignInErrorText.color = Color.red;
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Debug.LogError("HTTP 요청 예외 발생: " + ex.Message);
            //messageManager.ShowMessage("네트워크 연결을 확인해주세요.");
        }
        catch (Exception ex)
        {
            Debug.Log("예외 발생: " + ex);
            //messageManager.ShowMessage("네트워크 연결에 실패했습니다.");
        }
    }

    public async void SignUp()
    {
        // HTTP POST 요청을 보낼 엔드포인트 URL
        //string url = "http://ec2-13-125-207-67.ap-northeast-2.compute.amazonaws.com:4000/api/sign/signup";
        string url = "http://localhost:4000/api/sign/signup";

        // 사용자 입력 데이터를 JSON 형식으로 직렬화
        string json = JsonConvert.SerializeObject(new
        {
            email = SignUpEmailField.text,
            password = SignUpPwdField.text,
            nickname = SignUpNameField.text
        });
        Debug.Log(json);

        // HttpClient 인스턴스 생성
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(5); // 시간 제한 설정
            try
            {
                // HTTP POST 요청을 만들고 전송
                HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                string jsonString = await response.Content.ReadAsStringAsync();
                JObject jsonObj = JObject.Parse(jsonString);

                if (response.IsSuccessStatusCode)
                {
                    Debug.Log("회원가입 성공");
                    SignUpErrorText.text = "회원가입이 완료되었습니다. 로그인 해주세요";
                    SignUpErrorText.color = Color.blue;

                }
                else
                {
                    // 요청이 실패한 경우
                    string errorMessage = jsonObj["message"].ToString();
                    SignUpErrorText.text = errorMessage;
                    SignUpErrorText.color = Color.red;
                }
            }
            catch (Exception e)
            {
                // 오류 처리
                Debug.Log("확인해주세요");
                Debug.Log(e.Message);


                //           messageManager.ShowMessage("네트워크를 확인해주세요");

            }
        }
    }

    public void ShowSignInPanel()
    {
        SignUpPanel.SetActive(false); //패널 활성화
        SignInPanel.SetActive(true);

        Debug.Log(PostSignUpButton);
    }

    public void ShowSignUpPanel()
    {
        SignUpPanel.SetActive(true); //패널 활성화
        SignInPanel.SetActive(false);
    }


    private void HideSignUpPanel()
    {
        SignUpPanel.SetActive(false); // 패널 비활성화
        SignInPanel.SetActive(true);
    }
}
