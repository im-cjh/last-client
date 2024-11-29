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
    [프리팹 로드 및 등록]
---------------------------------------------*/
    async void Start()
    {
        // 필요한 프리팹들을 로드 및 등록
        await Utilities.RegisterPrefab("Prefab/Enemy/Robot1", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Enemy/Robot1", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Enemy/Robot2", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Enemy/Robot3", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Enemy/Robot4", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Enemy/Robot5", prefabMap);
    }

   // /*---------------------------------------------
   //     [������ ���]
   //     -�������� Addressables���� �ε��ϰ� Dictionary�� ����մϴ�.
   // ---------------------------------------------*/
   // private async Task RegisterPrefab(string key)
   // {
   //     // Ű�� ����ȭ (��: Prefab/Enemy/Robot1 -> Robot1)
   //     string shortKey = ExtractShortKey(key);

   //     // �̹� ��ϵ� �������� ����
   //     if (prefabMap.ContainsKey(shortKey))
   //     {
   //         Debug.LogWarning($"Prefab '{shortKey}' is already registered.");
   //         return;
   //     }

   //     // ������ �ε�
   //     GameObject prefab = await AssetManager.LoadAsset<GameObject>(key);

   //     if (prefab != null)
   //     {
   //         prefabMap[shortKey] = prefab;
   //         //Debug.Log($"Prefab '{shortKey}' loaded and registered.");
   //     }
   //     else
   //     {
   //         Debug.LogError($"Failed to load prefab: {key}");
   //     }
   // }
   // /*---------------------------------------------
   //   [������ Ű ����]
   //---------------------------------------------*/
   // private string ExtractShortKey(string key)
   // {
   //     // �����÷� �и��Ͽ� ������ �κи� ��ȯ
   //     return key.Substring(key.LastIndexOf('/') + 1);
   // }


    /*---------------------------------------------
        [���� ����]
        -��ϵ� �������� ����Ͽ� ���͸� �����մϴ�.
    ---------------------------------------------*/

    public void SpawnMonster(string prefabId, PosInfo pos)
    {
        Debug.Log(prefabId);
        if (prefabMap.TryGetValue(prefabId, out GameObject prefab))
        {
            // 2D 게임에서는 rotation 기본값으로 Quaternion.identity 사용
            Instantiate(prefab, new Vector2(pos.X, pos.Y), Quaternion.identity);
            // Debug.Log($"Monster spawned: {prefabId} at {pos}");
        }
        else
        {
            Debug.LogError($"Prefab not found: {prefabId}");
        }
    }
}