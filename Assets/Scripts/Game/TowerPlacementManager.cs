using UnityEngine;

public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager instance = null;
    private bool isPlacementActive = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public void SetPlacementState(bool state)
    {
        isPlacementActive = state;
        Debug.Log("타워 설치 상태: " + state);
    }

    public bool IsPlacementActive()
    {
        return isPlacementActive;
    }
}
