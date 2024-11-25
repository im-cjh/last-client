using Protocol;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    /*---------------------------------------------
        [멤버 변수]
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
        await RegisterPrefab("Prefab/Enemy/Robot1");
        await RegisterPrefab("Prefab/Enemy/Robot2");
        await RegisterPrefab("Prefab/Enemy/Robot3");
        await RegisterPrefab("Prefab/Enemy/Robot4");
        await RegisterPrefab("Prefab/Enemy/Robot5");
    }

    /*---------------------------------------------
        [프리팹 등록]
        -프리팹을 Addressables에서 로드하고 Dictionary에 등록합니다.
    ---------------------------------------------*/
    private async Task RegisterPrefab(string key)
    {
        // 키를 간소화 (예: Prefab/Enemy/Robot1 -> Robot1)
        string shortKey = ExtractShortKey(key);

        // 이미 등록된 프리팹은 무시
        if (prefabMap.ContainsKey(shortKey))
        {
            Debug.LogWarning($"Prefab '{shortKey}' is already registered.");
            return;
        }

        // 프리팹 로드
        GameObject prefab = await AssetManager.LoadAsset<GameObject>(key);

        if (prefab != null)
        {
            prefabMap[shortKey] = prefab;
            Debug.Log($"Prefab '{shortKey}' loaded and registered.");
        }
        else
        {
            Debug.LogError($"Failed to load prefab: {key}");
        }
    }
    /*---------------------------------------------
      [프리팹 키 추출]
   ---------------------------------------------*/
    private string ExtractShortKey(string key)
    {
        // 슬래시로 분리하여 마지막 부분만 반환
        return key.Substring(key.LastIndexOf('/') + 1);
    }


    /*---------------------------------------------
        [몬스터 스폰]
        -등록된 프리팹을 사용하여 몬스터를 스폰합니다.
    ---------------------------------------------*/

    public void SpawnMonster(string prefabId, PosInfo pos)
    {
        if (prefabMap.TryGetValue(prefabId, out GameObject prefab))
        {
            // 2D 게임에서는 rotation 기본값으로 Quaternion.identity 사용
            Instantiate(prefab, new Vector2(pos.X, pos.Y), Quaternion.identity);
            Debug.Log($"Monster spawned: {prefabId} at {pos}");
        }
        else
        {
            Debug.LogError($"Prefab not found: {prefabId}");
        }
    }
}
