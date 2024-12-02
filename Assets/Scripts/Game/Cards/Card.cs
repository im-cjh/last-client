using UnityEngine;
public class Card : MonoBehaviour
{
    public string prefabId;
    public bool isTowerCard = true;

    public string GetPrefabId()
    {
        return prefabId;
    }
}
