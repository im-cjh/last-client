using System.Net.Http;
using System.Text;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

enum eInputMode
{
    None = 0,
    SignIn = 1,
    SignUp = 2,
}

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

    private List<TMP_InputField> signInInputFields;
    private List<TMP_InputField> signUpInputFields;
    private eInputMode inputMode = eInputMode.None;

    private void Start()
    {
        PostSignInButton.onClick.AddListener(RegisterSignIn);
        PostSignUpButton.onClick.AddListener(RegisterSignUp);

        signInInputFields = new List<TMP_InputField>
        {
            SignInEmailField,
            SignInPwdField,
        };

        signUpInputFields = new List<TMP_InputField>
        {
            SignUpEmailField,
            SignUpPwdField,
            SignUpNameField
        };
    }

    private async void Update()
    {
        // Tab 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // 현재 활성화된 패널의 Input Field 리스트 가져오기
            List<TMP_InputField> activeInputFields;

            switch(inputMode)
            {
                case eInputMode.SignIn:
                    activeInputFields = signInInputFields;
                    break;
                case eInputMode.SignUp:
                    activeInputFields = signUpInputFields;
                    break;
                default:
                    return;
            }

            if (activeInputFields == null || activeInputFields.Count == 0)
                return;

            // 현재 선택된 UI 요소 가져오기
            GameObject current = EventSystem.current.currentSelectedGameObject;


            // 현재 활성화된 Input Field 내에서 포커스 전환
            for (int i = 0; i < activeInputFields.Count; i++)
            {
                if (current == activeInputFields[i].gameObject)
                {
                    // Shift+Tab이면 이전 Input Field로 이동
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        int previousIndex = (i - 1 + activeInputFields.Count) % activeInputFields.Count;
                        activeInputFields[previousIndex].Select();
                    }
                    // Tab이면 다음 Input Field로 이동
                    else
                    {
                        int nextIndex = (i + 1) % activeInputFields.Count;
                        activeInputFields[nextIndex].Select();
                    }
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) 
        {
            switch (inputMode)
            {
                case eInputMode.SignIn:
                    await SignIn();
                    break;
                case eInputMode.SignUp:
                    await SignUp();
                    break;
                default:
                    break;
            }
        }
    }


    public async Task SignIn()
    {
        Debug.Log("ㅇㅇ로그인");
        string url = "http://ec2-13-125-207-67.ap-northeast-2.compute.amazonaws.com:4000/api/sign/signin";

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
        }
        catch (Exception ex)
        {
            Debug.Log("예외 발생: " + ex);
        }
    }

    public async Task SignUp()
    {
        // HTTP POST 요청을 보낼 엔드포인트 URL
        string url = "http://ec2-13-125-207-67.ap-northeast-2.compute.amazonaws.com:4000/api/sign/signup";

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
        SignInPanel.SetActive(true);
        inputMode = eInputMode.SignIn;
        signInInputFields[0].Select();
    }

    public void ShowSignUpPanel()
    {
        SignUpPanel.SetActive(true); //패널 활성화
        inputMode = eInputMode.SignUp;
        signUpInputFields[0].Select();
    }


    public void HideSignInPanel()
    {
        SignInPanel.SetActive(false); // 패널 비활성화
        inputMode = eInputMode.None;
    }

    public void HideSignUpPanel()
    {
        SignUpPanel.SetActive(false); // 패널 비활성화
        inputMode = eInputMode.None;
    }

    public void RegisterSignIn()
    {
        _ = SignIn(); // 비동기 메서드 호출
    }

    public void RegisterSignUp()
    {
        _ = SignUp(); // 비동기 메서드 호출
    }
}
