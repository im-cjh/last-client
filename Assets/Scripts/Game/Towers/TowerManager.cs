using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager instance;
    private Dictionary<string, Tower> towers = new Dictionary<string, Tower>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void AddTower(string uuid, Tower tower)
    {
        if (!towers.ContainsKey(uuid))
        {
            towers.Add(uuid, tower);
            Debug.Log("타워 추가: uuid: " + uuid);
        }
    }

    public void RemoveTower(string uuid)
    {
        if (towers.ContainsKey(uuid))
        {
            towers.Remove(uuid);
            Debug.Log("타워 제거: uuid: " + uuid);
        }
    }

    public Tower GetTowerByUuid(string uuid)
    {
        if (towers.ContainsKey(uuid))
        {
            return towers[uuid];
        }

        return null;
    }
}
