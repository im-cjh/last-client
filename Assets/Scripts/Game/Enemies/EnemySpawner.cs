using Protocol;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    /*---------------------------------------------
        [��� ����]
---------------------------------------------*/
    public static EnemySpawner instance;
    private Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>();


    void Awake()
    {
        instance = this;
    }


    /*---------------------------------------------
        [������ �ε� �� ���]
    ---------------------------------------------*/
    async void Start()
    {
        // �ʿ��� �����յ��� �ε� �� ���
        await RegisterPrefab("Prefab/Enemy/Robot1");
        await RegisterPrefab("Prefab/Enemy/Robot2");
        await RegisterPrefab("Prefab/Enemy/Robot3");
        await RegisterPrefab("Prefab/Enemy/Robot4");
        await RegisterPrefab("Prefab/Enemy/Robot5");
    }

    /*---------------------------------------------
        [������ ���]
        -�������� Addressables���� �ε��ϰ� Dictionary�� ����մϴ�.
    ---------------------------------------------*/
    private async Task RegisterPrefab(string key)
    {
        // Ű�� ����ȭ (��: Prefab/Enemy/Robot1 -> Robot1)
        string shortKey = ExtractShortKey(key);

        // �̹� ��ϵ� �������� ����
        if (prefabMap.ContainsKey(shortKey))
        {
            Debug.LogWarning($"Prefab '{shortKey}' is already registered.");
            return;
        }

        // ������ �ε�
        GameObject prefab = await AssetManager.LoadAsset<GameObject>(key);

        if (prefab != null)
        {
            prefabMap[shortKey] = prefab;
            //Debug.Log($"Prefab '{shortKey}' loaded and registered.");
        }
        else
        {
            Debug.LogError($"Failed to load prefab: {key}");
        }
    }
    /*---------------------------------------------
      [������ Ű ����]
   ---------------------------------------------*/
    private string ExtractShortKey(string key)
    {
        // �����÷� �и��Ͽ� ������ �κи� ��ȯ
        return key.Substring(key.LastIndexOf('/') + 1);
    }


    /*---------------------------------------------
        [���� ����]
        -��ϵ� �������� ����Ͽ� ���͸� �����մϴ�.
    ---------------------------------------------*/

    public void SpawnMonster(string prefabId, PosInfo pos)
    {
        if (prefabMap.TryGetValue(prefabId, out GameObject prefab))
        {
            // 2D ���ӿ����� rotation �⺻������ Quaternion.identity ���
            Instantiate(prefab, new Vector2(pos.X, pos.Y), Quaternion.identity);
            // Debug.Log($"Monster spawned: {prefabId} at {pos}");
        }
        else
        {
            Debug.LogError($"Prefab not found: {prefabId}");
        }
    }
}
