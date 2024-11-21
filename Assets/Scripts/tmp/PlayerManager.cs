using System.Collections.Generic;
using UnityEngine;
using Protocol; // Protobuf 메시지 사용

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private Dictionary<string, GamePlayerData> players = new Dictionary<string, GamePlayerData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않음
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
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

        Debug.LogError($"UUID {uuid}에 해당하는 플레이어를 찾을 수 없습니다.");
        return null;
    }

    public List<GamePlayerData> GetAllPlayers()
    {
        return new List<GamePlayerData>(players.Values);
    }
}
