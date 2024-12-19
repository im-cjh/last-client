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
    [SerializeField] private TMP_Text no;         // 슬롯 번호
    [SerializeField] private TMP_Text nickname;   // 사용자 닉네임
    [SerializeField] private Image character;     // 캐릭터 이미지
    private UserData userData;
    public GameObject readyObject;

    // 슬롯 데이터를 설정
    public void SetItem(UserData pUserData)
    {
        
        userData = pUserData;
        if (userData != null)
        {
            nickname.text = userData.Name; // 사용자 이름 설정
            OnChangeCharacter(userData.PrefabId); // 캐릭터 이미지 변경
            gameObject.SetActive(true);
        }
        else
        {
            ClearItem();
        }
    }

    // 캐릭터 이미지 변경
    public async void OnChangeCharacter(string characterType)
    {
        if (!string.IsNullOrEmpty(characterType))
        {
            // 리소스 로드 후 캐릭터 이미지 설정
            character.sprite = await AssetManager.LoadAsset<Sprite>("Thumbnail/"+characterType+".png", eAddressableType.Thumbnail);

            Color color = character.color;
            color.a = 1;
            character.color = color;
        }
        else
        {
            Debug.Log("리소스 로드 실패" + characterType);
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

    public void EnableReadyObject()
    {
        readyObject.SetActive(true);
    }

    public void DisableReadyObject()
    {
        readyObject.SetActive(false);
    }
}
