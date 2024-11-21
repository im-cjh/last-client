using System.Collections.Generic;
using UnityEngine;
using Protocol; // Protobuf �޽��� ���

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private Dictionary<string, GamePlayerData> players = new Dictionary<string, GamePlayerData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� ���� ����
        }
    }

    public void AddPlayer(GamePlayerData playerData)
    {
        if (!players.ContainsKey(playerData.Position.Uuid))
        {
            players[playerData.Position.Uuid] = playerData;
        }
    }

    public GamePlayerData GetPlayer(string uuid)
    {
        if (players.TryGetValue(uuid, out GamePlayerData playerData))
        {
            return playerData;
        }

        Debug.LogError($"UUID {uuid}�� �ش��ϴ� �÷��̾ ã�� �� �����ϴ�.");
        return null;
    }

    public List<GamePlayerData> GetAllPlayers()
    {
        return new List<GamePlayerData>(players.Values);
    }
}
