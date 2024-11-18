using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ItemRoom : MonoBehaviour // UIListItem Á¦°Å
{
    [SerializeField] private TMP_Text roomNo;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text count;

    public UnityAction<int> callback;
    public Protocol.RoomData roomData;

    public void SetItem(Protocol.RoomData roomData, UnityAction<int> callback)
    {
        gameObject.SetActive(true);
        this.roomData = roomData;
        this.callback = callback;
        roomNo.text = roomData.Id.ToString();
        title.text = roomData.Name;
        count.text = string.Format("({0}/{1})", roomData.Users.Count, roomData.MaxUserNum);
    }

    public void OnClickItem()
    {
        callback?.Invoke(roomData.Id);
    }
}
