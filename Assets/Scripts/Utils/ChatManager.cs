using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ChatManager : MonoBehaviour
{
    public static bool isChatting { get; private set; }

    [SerializeField] private GameObject chatPanel;
    [SerializeField] private RectTransform chatPanelTranfrom;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private ScrollRect scrollRect;
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
        isChatting = inputField.isFocused;

        if (isChatVisible && Input.GetKeyDown(KeyCode.Return))
        {
            if (!isChatting)
            {
                inputField.Select();
            }

            if (!string.IsNullOrEmpty(inputField.text))
            {
                SendMessageRequest();

                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;

                EventSystem.current.SetSelectedGameObject(null);
            }
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
                IsLobbyChat = false,
            };

            byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2G_ChatMessageRequest, PlayerInfoManager.instance.GetNextSequence());
            NetworkManager.instance.SendPacket(sendBuffer);

            inputField.text = string.Empty;
        }
    }

    public void AddMessageOnDisPlay(string nickname, string message)
    {
        GameObject newMessage = Instantiate(messagePrefab, contentTransform);
        TMP_Text messageText = newMessage.GetComponent<TMP_Text>();
        messageText.text = $"{nickname}: {message}";
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
