using UnityEngine;
public class Card : MonoBehaviour
{
    public string cardId;
    public string prefabId;
    public bool isTowerCard = true;

    public string GetPrefabId()
    {
        return prefabId;
    }

    public void SetCardId(string uuid)
    {
        cardId = uuid;
    }

    public string GetCardId()
    {
        return cardId;
    }
}
