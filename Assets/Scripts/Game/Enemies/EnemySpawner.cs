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
    private Dictionary<string, Enemy> enemies = new Dictionary<string, Enemy>();


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

    /*---------------------------------------------
        [���� ����]
        -��ϵ� �������� ����Ͽ� ���͸� �����մϴ�.
    ---------------------------------------------*/

    public void SpawnMonster(string prefabId, PosInfo pos)
    {
        //Debug.Log(prefabId);
        if (prefabMap.TryGetValue(prefabId, out GameObject prefab))
        {
            // 2D 게임에서는 rotation 기본값으로 Quaternion.identity 사용
            GameObject monster = Instantiate(prefab, new Vector2(pos.X, pos.Y), Quaternion.identity);
            Enemy chara = monster.GetComponent<Enemy>();
            enemies[pos.Uuid] = chara;
            // Debug.Log($"Monster spawned: {prefabId} at {pos}");
        }
        else
        {
            Debug.LogError($"Prefab not found: {prefabId}");
        }
    }

    /*---------------------------------------------
        [몬스터 이동 처리]
    ---------------------------------------------*/
    public void HandleMonsterMove(PosInfo pos)
    {
        if (enemies.TryGetValue(pos.Uuid, out Enemy enemy))
        {
            // Rigidbody2D를 가져옴
            Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();

            if (rigid != null)
            {
                // 목표 위치를 Vector2로 변환
                Vector2 targetPosition = new Vector2(pos.X, pos.Y);

                
                enemy.SetNextPos(targetPosition);
                //Debug.Log($"Monster {pos.Uuid} moved to ({pos.X}, {pos.Y})");
            }
            else
            {
                Debug.LogError($"Rigidbody2D not found on Monster with UUID {pos.Uuid}");
            }
        }
        else
        {
            Debug.LogError($"Monster with UUID {pos.Uuid} not found");
        }
    }

}
