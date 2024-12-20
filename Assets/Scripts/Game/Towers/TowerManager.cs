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

        Debug.Log("해당하는 uuid의 타워를 찾을 수 없습니다: " + uuid);
        return null;
    }

}
