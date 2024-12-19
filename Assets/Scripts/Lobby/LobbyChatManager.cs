using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class LobbyChatManager : MonoBehaviour
{
    public static bool isChatting { get; private set; }
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private ScrollRect scrollRect;
    public static LobbyChatManager instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        isChatting = inputField.isFocused;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isChatting)
            {
                inputField.Select();
            }

            if (!string.IsNullOrEmpty(inputField.text))
            {
                SendLobbyMessageRequest();

                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;

                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }


    private void SendLobbyMessageRequest()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            Protocol.C2G_ChatMessageRequest pkt = new Protocol.C2G_ChatMessageRequest
            {
                Message = inputField.text,
                RoomId = PlayerInfoManager.instance.roomId,
                IsLobbyChat = true,
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
}
