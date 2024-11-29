using UnityEngine;

public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager instance = null;
    private bool isPlacementActive = false;
    public string towerPrefabId;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SetPlacementState(bool state, string prefabId)
    {
        isPlacementActive = state;
        towerPrefabId = prefabId;
        Debug.Log("타워 설치 상태: " + state);
    }

    public string GetTowerPrefabId()
    {
        return towerPrefabId;
    }

    public bool IsPlacementActive()
    {
        return isPlacementActive;
    }
}
