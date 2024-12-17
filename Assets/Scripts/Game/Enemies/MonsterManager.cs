using Protocol;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum eBuffType
{
    ATK_INC = 1,
}

public class MonsterManager : MonoBehaviour
{
    /*---------------------------------------------
        [��� ����]
---------------------------------------------*/
    public static MonsterManager instance;
    private Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>();
    private Dictionary<string, Monster> enemies = new Dictionary<string, Monster>();
    private int numBuffMonsters = 0;

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

    public void SpawnMonster(string prefabId, PosInfo pos, int maxHp)
    {
        //Debug.Log(prefabId);
        if (prefabMap.TryGetValue(prefabId, out GameObject prefab))
        {
            // 2D 게임에서는 rotation 기본값으로 Quaternion.identity 사용
            GameObject monster = Instantiate(prefab, new Vector2(pos.X, pos.Y), Quaternion.identity);

            //Robot5가 존재하면 버프 상태 렌더링
            if(prefabId == "Robot5")
            {
                numBuffMonsters += 1;
            }

            Monster chara = monster.GetComponent<Monster>();
            chara.SetMonsterId(pos.Uuid);
            chara.SetPrefabId(prefabId);

            HpBar hpBar = chara.GetComponentInChildren<HpBar>();
            if (hpBar != null)
            {
                hpBar.SetMaxHp(maxHp);
            }
            else
            {
                Debug.LogWarning("HpBar를 찾을 수 없습니다.");
            }
            enemies[pos.Uuid] = chara;

        }
        else
        {
            Debug.LogError($"Prefab not found: {prefabId}");
        }
    }

    public void RemoveMonster(string uuid)
    {
        if (enemies.ContainsKey(uuid))
        {
            if(enemies[uuid].GetPrefabId() == "Robot5")
            {
                numBuffMonsters -= 1;
            }
            enemies.Remove(uuid);
            Debug.Log("몬스터 제거: uuid: " + uuid);
        }
    }

    public Monster GetMonsterByUuid(string uuid)
    {
        if (enemies.ContainsKey(uuid))
        {
            return enemies[uuid];
        }

        return null;
    }

    /*---------------------------------------------
        [몬스터 이동 처리]
    ---------------------------------------------*/
    public void HandleMonsterMove(PosInfo pos)
    {
        if (enemies.TryGetValue(pos.Uuid, out Monster monster))
        {
            monster.SetMoveMode();
            // Rigidbody2D를 가져옴
            Rigidbody2D rigid = monster.GetComponent<Rigidbody2D>();

            if (rigid != null)
            {
                // 목표 위치를 Vector2로 변환
                Vector2 targetPosition = new Vector2(pos.X, pos.Y);


                monster.SetNextPos(targetPosition);
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

    public void HandleMonsterAttackTower(string monsterId)
    {
        if (enemies.TryGetValue(monsterId, out Monster monster))
        {
            monster.SetAttackMode();
        }
        else
        {
            Debug.LogError($"not found");
        }
    }

    public bool GetBuffState(eBuffType buffType)
    {
        switch (buffType)
        {
            case eBuffType.ATK_INC:
                return numBuffMonsters > 0;
            default:
                return false;
        }
    }
}
