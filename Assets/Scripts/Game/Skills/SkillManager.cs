using UnityEngine;
using Protocol;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance = null;
    private Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>(); // 설치할 타워 프리팹

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    async void Start()
    {
        await Utilities.RegisterPrefab("Prefab/Skills/OrbitalBeam", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Skills/TowerRepair", prefabMap);
    }

    public void UseSkill(string ownerId, SkillData skillData)
    {
        Debug.Log("UseSkill Called. skillData: " + skillData);
        Vector2 cellCenterWorld = new Vector2(skillData.SkillPos.X, skillData.SkillPos.Y);

        Instantiate(prefabMap[skillData.PrefabId], cellCenterWorld, Quaternion.identity);
        Debug.Log($"스킬이 {cellCenterWorld} 위치에 사용되었습니다.");
        CharacterManager.instance.GetCharacter(ownerId).isSkillActive = false;
    }
}
