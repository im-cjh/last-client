using System.Collections.Generic;
using UnityEngine;
using Protocol;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    //public GameObject characterPrefab; // 캐릭터 프리팹

    private Dictionary<string, Character> characters = new Dictionary<string, Character>(); // UUID와 캐릭터 매핑
    private Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>(); 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
            chara.nickname = playerData.Nickname;
            chara.isLocalPlayer = playerData.Position.Uuid == PlayerInfoManager.instance.userId; // UUID 기반 로컬 판별
            chara.cam.gameObject.SetActive(playerData.Position.Uuid == PlayerInfoManager.instance.userId);
            characters[playerData.Position.Uuid] = chara; // UUID로 캐릭터 매핑
        }
        else
        {
            Debug.LogError("캐릭터 프리팹에 Character 스크립트가 없습니다.");
        }
    }

    // 모든 플레이어 캐릭터 생성
    public async void InitializeCharacters()
    {
        await Utilities.RegisterPrefab("Prefab/Characters/Red", prefabMap);

        foreach (var playerData in PlayerManager.Instance.GetAllPlayers())
        {
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
}
