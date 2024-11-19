using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Protocol;
using System.Resources;

public class ItemRoomSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text no;         // ���� ��ȣ
    [SerializeField] private TMP_Text nickname;   // ����� �г���
    [SerializeField] private Image character;     // ĳ���� �̹���
    private UserData userData;

    // ���� �����͸� ����
    public void SetItem(UserData pUserData)
    {
        Debug.Log(pUserData);
        userData = pUserData;
        if (userData != null)
        {
            nickname.text = userData.Name; // ����� �̸� ����
            OnChangeCharacter(userData.Character.CharacterType.ToString()); // ĳ���� �̹��� ����
            gameObject.SetActive(true);
        }
        else
        {
            ClearItem();
        }
    }

    // ĳ���� �̹��� ����
    public async void OnChangeCharacter(string characterType)
    {
        if (!string.IsNullOrEmpty(characterType))
        {
            // ���ҽ� �ε� �� ĳ���� �̹��� ����
            character.sprite = await AssetManager.LoadAsset<Sprite>("Thumbnail/"+characterType+".png", eAddressableType.Thumbnail);
        }
    }
    public void ClearItem()
    {
        userData = null;
        nickname.text = "Empty";
        character.sprite = null;
        gameObject.SetActive(false);
    }

    public bool IsEmpty()
    {
        return userData == null;
    }

    public bool HasUser(string userId)
    {
        return userData != null && userData.Id == userId;
    }
}
