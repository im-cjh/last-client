using UnityEngine;

public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager instance = null;
    private bool isPlacementActive = false;
    private string towerPrefabId;
    private string cardId;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SetPlacementState(bool state, string prefabId, string uuid)
    {
        isPlacementActive = state;
        towerPrefabId = prefabId;
        cardId = uuid;
        Debug.Log("타워 설치 상태: " + state);
    }

    public string GetTowerPrefabId()
    {
        return towerPrefabId;
    }

    public string GetCardId()
    {
        return cardId;
    }

    public bool IsPlacementActive()
    {
        return isPlacementActive;
    }
}
