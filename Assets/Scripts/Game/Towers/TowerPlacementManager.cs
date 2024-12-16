using UnityEngine;
using Protocol;
using System.Collections.Generic;

public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager instance = null;
    private Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>(); // 설치할 타워 프리팹
    private bool isTowerActive = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    async void Start()
    {
        await Utilities.RegisterPrefab("Prefab/Towers/BasicTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Towers/BuffTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Towers/IceTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Towers/MissileTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Towers/StrongTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Towers/TankTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Towers/ThunderTower", prefabMap);
    }

    public void BuildTower(string ownerId, TowerData towerData, int maxHp)
    {
        Vector3 cellCenterWorld = new Vector3(towerData.TowerPos.X, towerData.TowerPos.Y);

        GameObject newTower = Instantiate(prefabMap[towerData.PrefabId], cellCenterWorld, Quaternion.identity);
        Debug.Log($"타워가 {cellCenterWorld} 위치에 설치되었습니다.");
        
        Tower towerScript = newTower.GetComponent<Tower>();
        if (towerScript != null)
        {
            HpBar hpBar = towerScript.GetComponentInChildren<HpBar>();
            if (hpBar != null)
            {
                hpBar.SetMaxHp(maxHp);
            }
            else
            {
                Debug.LogWarning("HpBar를 찾을 수 없습니다.");
            }
            towerScript.SetTowerId(towerData.TowerPos.Uuid);
            TowerManager.instance.AddTower(towerData.TowerPos.Uuid, towerScript);
        }

        

        // 로컬 플레이어의 타워 설치 모드 비활성화
        CharacterManager.instance.GetCharacter(ownerId).isTowerActive = false;
        isTowerActive = false;
    }

    public void SetTowerActive(bool state)
    {
        isTowerActive = state;
    }

    public bool GetTowerActive()
    {
        return isTowerActive;
    }
}
