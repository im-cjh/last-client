using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using System.Collections;
using TMPro;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private Button abilityButton;
    [SerializeField] private Image cooldownImage;
    private float cooldown = 3f;
    private bool isCooldown = false;

    void Start()
    {
        abilityButton.onClick.AddListener(SendUseAbilityRequest);

        cooldownImage.fillAmount = 0;
    }

    void Update()
    {
        if (!isCooldown && IsMouseOverButton())
        {
            abilityButton.transform.DOScale(1.1f, 0.2f);
        }
        else
        {
            abilityButton.transform.DOScale(1f, 0.2f);
        }
    }

    private void SendUseAbilityRequest()
    {
        if (isCooldown) return;

        Debug.Log("Ability 사용");
        Protocol.C2G_PlayerUseAbilityRequest pkt = new Protocol.C2G_PlayerUseAbilityRequest
        {
            Position = new Protocol.PosInfo
            {
                Uuid = PlayerInfoManager.instance.userId,

            },
            PrefabId = PlayerInfoManager.instance.prefabId,

            RoomId = PlayerInfoManager.instance.roomId,
        };

        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2G_PlayerUseAbilityRequest, PlayerInfoManager.instance.GetNextSequence());
        NetworkManager.instance.SendPacket(sendBuffer);

        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        isCooldown = true;

        float remainingTime = cooldown;
        cooldownImage.fillAmount = 1;

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            cooldownImage.fillAmount = remainingTime / cooldown;
            yield return null;
        }

        cooldownImage.fillAmount = 0;
        isCooldown = false;
    }

    bool IsMouseOverButton()
    {
        RectTransform rectTransform = abilityButton.GetComponent<RectTransform>();
        Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);
        return rectTransform.rect.Contains(localMousePosition);
    }
}
