using System.Collections.Generic;
using UnityEngine;
using Protocol;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    public GameObject characterPrefab; // ĳ���� ������

    private Dictionary<string, Character> characters = new Dictionary<string, Character>(); // UUID�� ĳ���� ����

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
        Vector3 position = new Vector3(playerData.Position.X, playerData.Position.Y, 0);

        // ĳ���� ����
        GameObject character = Instantiate(characterPrefab, position, Quaternion.identity);
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
    public void InitializeCharacters()
    {
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
