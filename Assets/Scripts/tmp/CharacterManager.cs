using System.Collections.Generic;
using UnityEngine;
using Protocol;
using static UnityEditor.PlayerSettings;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    //public GameObject characterPrefab; // ĳ���� ������

    private Dictionary<string, Character> characters = new Dictionary<string, Character>(); // UUID�� ĳ���� ����
    private Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>(); 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ���� �����͸� ������� ĳ���� ����
    public void SpawnCharacter(GamePlayerData playerData)
    {
        if (characters.ContainsKey(playerData.Position.Uuid))
        {
            Debug.LogWarning($"�̹� ������ ĳ���Ͱ� �ֽ��ϴ�: {playerData.Position.Uuid}");
            return;
        }
        // �������� ���� ���� ��ġ ���
        Vector3 pos = new Vector3(playerData.Position.X, playerData.Position.Y, 0);

        if (!prefabMap.TryGetValue(playerData.PrefabId, out GameObject prefab))
        {
            Debug.LogError($"Prefab not found: {playerData.PrefabId}");
            return;
        }

        // ĳ���� ����
        // 2D ���ӿ����� rotation �⺻������ Quaternion.identity ���
        GameObject character = Instantiate(prefab, pos, Quaternion.identity);
        Character chara = character.GetComponent<Character>();

        if (chara != null)
        {
            chara.nickname = playerData.Nickname;
            chara.isLocalPlayer = playerData.Position.Uuid == PlayerInfoManager.instance.userId; // UUID ��� ���� �Ǻ�
            characters[playerData.Position.Uuid] = chara; // UUID�� ĳ���� ����
        }
        else
        {
            Debug.LogError("ĳ���� �����տ� Character ��ũ��Ʈ�� �����ϴ�.");
        }
    }

    // ��� �÷��̾� ĳ���� ����
    public async void InitializeCharacters()
    {
        await Utilities.RegisterPrefab("Prefab/Characters/Red", prefabMap);

        foreach (var playerData in PlayerManager.Instance.GetAllPlayers())
        {
            SpawnCharacter(playerData);
        }
    }

    // Ư�� ĳ���� ��������
    public Character GetCharacter(string uuid)
    {
        if (characters.TryGetValue(uuid, out Character character))
        {
            return character;
        }

        Debug.LogError($"UUID {uuid}�� �ش��ϴ� ĳ���͸� ã�� �� �����ϴ�.");
        return null;
    }
}
