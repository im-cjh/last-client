using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor.VersionControl;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private GameObject chatPanel;
    [SerializeField] private RectTransform chatPanelTranfrom;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text chatDisplay;
    [SerializeField] private Button chatButton;
    private bool isChatVisible = false;
    public static ChatManager instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        chatPanel.SetActive(false);
        chatPanelTranfrom.localScale = Vector3.zero;

        chatButton.onClick.AddListener(ToggleChat);
    }

    void Update()
    {
        if (isChatVisible && Input.GetKeyDown(KeyCode.Return))
        {
            SendMessageRequest();
        }
    }

    private void ToggleChat()
    {
        if (!isChatVisible)
        {
            OpenChat();
        }
        else
        {
            CloseChat();
        }
    }

    private void SendMessageRequest()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            Protocol.C2G_ChatMessageRequest pkt = new Protocol.C2G_ChatMessageRequest
            {
                Message = inputField.text,
                RoomId = PlayerInfoManager.instance.roomId,
            };

            byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2G_ChatMessageRequest, PlayerInfoManager.instance.GetNextSequence());
            NetworkManager.instance.SendPacket(sendBuffer);
        }
    }

    public void AddMessageOnDisPlay(string nickname, string message)
    {
        chatDisplay.text += $"{nickname}: {message}\n";
    }

    private void OpenChat()
    {
        chatPanel.SetActive(true);
        chatPanelTranfrom.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        inputField.text = string.Empty;
        inputField.Select();
        isChatVisible = true;
    }

    private void CloseChat()
    {
        chatPanelTranfrom.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() => chatPanel.SetActive(false));
        isChatVisible = false;
    }
}
