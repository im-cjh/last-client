using System.Collections.Generic;
using UnityEngine;
using Protocol;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance { get; private set; }

    // public GameObject characterPrefab; // 캐릭터 프리팹

    private Dictionary<string, Character> characters = new Dictionary<string, Character>(); // UUID와 캐릭터 매핑
    private Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>();
    private Character localPlayer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 서버 데이터를 기반으로 캐릭터 생성
    public void SpawnCharacter(GamePlayerData playerData)
    {
        if (characters.ContainsKey(playerData.Position.Uuid))
        {
            Debug.LogWarning($"이미 생성된 캐릭터가 있습니다: {playerData.Position.Uuid}");
            return;
        }
        // 서버에서 받은 스폰 위치 사용
        Vector3 pos = new Vector3(playerData.Position.X, playerData.Position.Y, 0);

        if (!prefabMap.TryGetValue(playerData.PrefabId, out GameObject prefab))
        {
            Debug.LogError($"Prefab not found: {playerData.PrefabId}");
            return;
        }

        // 캐릭터 생성
        // 2D 게임에서는 rotation 기본값으로 Quaternion.identity 사용
        GameObject character = Instantiate(prefab, pos, Quaternion.identity);
        Character chara = character.GetComponent<Character>();

        if (chara != null)
        {
            Debug.Log(playerData.PrefabId);
            chara.nickname = playerData.Nickname;
            chara.Init();
            if (playerData.Position.Uuid == PlayerInfoManager.instance.userId)
            {
                chara.isLocalPlayer = true;
                localPlayer = chara;
                chara.SetCharacterId(playerData.Position.Uuid);
                AbilityManager.instance.cooldown = playerData.CoolDown/1000;
                if (CameraFollow.instance != null)
                {
                    CameraFollow.instance.SetPlayer(chara.gameObject);
                }
                else
                {
                    Debug.LogError("CameraFollow.instance가 초기화되지 않았습니다.");
                }
            }
            else
            {
                Debug.Log("엘스");
                chara.isLocalPlayer = false;
            }

            characters[playerData.Position.Uuid] = chara; // UUID로 캐릭터 매핑
            chara.SetCharacterId(playerData.Position.Uuid);
        }
        else
        {
            Debug.LogError("캐릭터 프리팹에 Character 스크립트가 없습니다.");
        }
    }

    // 모든 플레이어 캐릭터 생성
    public async Task InitializeCharacters()
    {
        ClearCharacters(); // 기존 캐릭터 초기화
        ClearPrefabMap(); // 프리팹 초기화

        await Utilities.RegisterPrefab("Prefab/Characters/Red", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Characters/Shark", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Characters/Malang", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Characters/Frog", prefabMap);

        foreach (var playerData in PlayerManager.instance.GetAllPlayers())
        {
            Debug.Log(playerData.PrefabId);
            SpawnCharacter(playerData);
        }
    }

    // 특정 캐릭터 가져오기
    public Character GetCharacter(string uuid)
    {
        if (characters.TryGetValue(uuid, out Character character))
        {
            return character;
        }

        Debug.LogError($"UUID {uuid}에 해당하는 캐릭터를 찾을 수 없습니다.");
        return null;
    }

    public Character GetLocalPlayer()
    {
        return localPlayer;
    }

    private void ClearPrefabMap()
    {
        prefabMap.Clear(); // 프리팹 맵 초기화
    }

    public void ClearCharacters()
    {
        foreach (var character in characters.Values)
        {
            if (character != null)
            {
                Destroy(character.gameObject); 
            }
        }

        characters.Clear();
        localPlayer = null; 
    }
}
