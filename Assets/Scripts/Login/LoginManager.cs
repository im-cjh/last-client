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
    public Text SignInID_text;
    public Text SignInPwd_text;
    public InputField SignInEmailField;
    public InputField SignInPwdField;
    public Button PostSignInButton;

    [Header("# SignUp")]
    private InputField SignUpEmailField;
    private InputField SignUpPwdField;
    private InputField SignUpNameField;
    private Button PostSignUpButton;
    

    [Header("# UI References")]
    public Button EnableSignUpButton;
    public Button DisableSignUpButton;
    public GameObject SignUpPanel;
    public GameObject SignInPanel;
    public Text MessageText;
    


    private void Start()
    {
        EnableSignUpButton.onClick.AddListener(ShowSignInPanel);
        DisableSignUpButton.onClick.AddListener(HideSignUpPanel);
        PostSignInButton.onClick.AddListener(SignIn);
    }


    public async void SignIn()
    {
        Debug.Log("ㅇㅇ로그인");
        string url = "http://localhost:4000/api/sign/signin";
        string json = JsonConvert.SerializeObject(new { email = SignInEmailField.text, password = SignInPwdField.text });

        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(3); // Timeout 설정

                HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JObject jsonObj = JObject.Parse(jsonString);

                    //User.Instance.userName = jsonObj["name"].ToString();
                    //User.Instance.id = Convert.ToInt32(jsonObj["id"].ToString());

                    // 추가 작업 수행
                    //LobbySession.Instance.Connect("127.0.0.1", 7777);

                    //Protocol.C2SLoginSuccess pkt = new Protocol.C2SLoginSuccess();
                    //pkt.UserName = jsonObj["name"].ToString();
                    //pkt.UserID = Convert.ToInt32(jsonObj["id"].ToString());

                    //byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.LOGIN_SUCCESS);
                    //await LobbySession.Instance.Send(sendBuffer);

                    SceneChanger.ChangeScene(SceneChanger.SceneType.Lobby);
                }
                else
                {
                    Debug.LogWarning("로그인 실패: " + response.StatusCode);
                    SignInID_text.text = "이메일 - 유효하지 않은 아이디 또는 비밀번호입니다.";
                    SignInID_text.color = Color.red;
                    SignInPwd_text.text = "비밀번호 - 유효하지 않은 아이디 또는 비밀번호입니다.";
                    SignInPwd_text.color = Color.red;
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
            Debug.Log("예외 발생: " + ex.Message);
            //messageManager.ShowMessage("네트워크 연결에 실패했습니다.");
        }
    }

    public async void SignUp()
    {

        Debug.Log("ㅇㅇ 나임");


        // HTTP POST 요청을 보낼 엔드포인트 URL
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

                // 응답 메시지를 확인
                if (response.IsSuccessStatusCode)
                {
                    var rc = await response.Content.ReadAsStringAsync();
                    // 성공적으로 요청이 완료되었을 때
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JObject jsonObj = JObject.Parse(jsonString);

                    // 회원가입 성공 메시지를 표시
                    Debug.Log("회원가입 성공");
                    //messageManager.ShowMessage("회원가입이 완료되었습니다. 로그인 해주세요");
                    MessageText.text = "회원가입 성공";
                    MessageText.color = Color.blue;

                }
                else
                {
                    // 요청이 실패한 경우
                    MessageText.text = "유효하지 않은 이메일/비밀번호입니다.";
                    MessageText.color = Color.red;
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

    private void ShowSignInPanel()
    {
        SignUpPanel.SetActive(true); //패널 활성화
        SignInPanel.SetActive(false);

        SignUpEmailField = Utilities.FindAndAssign<InputField>("Canvas/SignUpPanel/InputGroup/ID");
        SignUpPwdField = Utilities.FindAndAssign<InputField>("Canvas/SignUpPanel/InputGroup/PASSWORD");
        SignUpNameField = Utilities.FindAndAssign<InputField>("Canvas/SignUpPanel/InputGroup/NICKNAME");
        PostSignUpButton = Utilities.FindAndAssign<Button>("Canvas/SignUpPanel/SignUpBtn");

        PostSignUpButton.onClick.AddListener(SignUp);
        Debug.Log(PostSignUpButton);
    }

    
    private void HideSignUpPanel()
    {
        SignUpPanel.SetActive(false); // 패널 비활성화
        SignInPanel.SetActive(true);
    }
}
