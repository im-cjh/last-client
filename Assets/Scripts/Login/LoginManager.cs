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
        Debug.Log("�����α���");
        string url = "http://localhost:4000/api/sign/signin";
        string json = JsonConvert.SerializeObject(new { email = SignInEmailField.text, password = SignInPwdField.text });

        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(3); // Timeout ����

                HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JObject jsonObj = JObject.Parse(jsonString);

                    //User.Instance.userName = jsonObj["name"].ToString();
                    //User.Instance.id = Convert.ToInt32(jsonObj["id"].ToString());

                    // �߰� �۾� ����
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
                    Debug.LogWarning("�α��� ����: " + response.StatusCode);
                    SignInID_text.text = "�̸��� - ��ȿ���� ���� ���̵� �Ǵ� ��й�ȣ�Դϴ�.";
                    SignInID_text.color = Color.red;
                    SignInPwd_text.text = "��й�ȣ - ��ȿ���� ���� ���̵� �Ǵ� ��й�ȣ�Դϴ�.";
                    SignInPwd_text.color = Color.red;
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Debug.LogError("HTTP ��û ���� �߻�: " + ex.Message);
            //messageManager.ShowMessage("��Ʈ��ũ ������ Ȯ�����ּ���.");
        }
        catch (Exception ex)
        {
            Debug.Log("���� �߻�: " + ex.Message);
            //messageManager.ShowMessage("��Ʈ��ũ ���ῡ �����߽��ϴ�.");
        }
    }

    public async void SignUp()
    {

        Debug.Log("���� ����");


        // HTTP POST ��û�� ���� ��������Ʈ URL
        string url = "http://localhost:4000/api/sign/signup";


        // ����� �Է� �����͸� JSON �������� ����ȭ
        string json = JsonConvert.SerializeObject(new
        {
            email = SignUpEmailField.text,
            password = SignUpPwdField.text,
            nickname = SignUpNameField.text
        });
        Debug.Log(json);

        // HttpClient �ν��Ͻ� ����
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(5); // �ð� ���� ����
            try
            {
                // HTTP POST ��û�� ����� ����
                HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                // ���� �޽����� Ȯ��
                if (response.IsSuccessStatusCode)
                {
                    var rc = await response.Content.ReadAsStringAsync();
                    // ���������� ��û�� �Ϸ�Ǿ��� ��
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JObject jsonObj = JObject.Parse(jsonString);

                    // ȸ������ ���� �޽����� ǥ��
                    Debug.Log("ȸ������ ����");
                    //messageManager.ShowMessage("ȸ�������� �Ϸ�Ǿ����ϴ�. �α��� ���ּ���");
                    MessageText.text = "ȸ������ ����";
                    MessageText.color = Color.blue;

                }
                else
                {
                    // ��û�� ������ ���
                    MessageText.text = "��ȿ���� ���� �̸���/��й�ȣ�Դϴ�.";
                    MessageText.color = Color.red;
                }
            }
            catch (Exception e)
            {
                // ���� ó��
                Debug.Log("Ȯ�����ּ���");
                Debug.Log(e.Message);


         //           messageManager.ShowMessage("��Ʈ��ũ�� Ȯ�����ּ���");
         
            }
        }
    }

    private void ShowSignInPanel()
    {
        SignUpPanel.SetActive(true); //�г� Ȱ��ȭ
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
        SignUpPanel.SetActive(false); // �г� ��Ȱ��ȭ
        SignInPanel.SetActive(true);
    }
}
