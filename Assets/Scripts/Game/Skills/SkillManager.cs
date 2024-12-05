using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance = null;
    private bool isSkillActive = false;
    private string skillPrefabId;
    private string cardId;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SetSkillActive(bool state, string prefabId, string uuid)
    {
        isSkillActive = state;
        skillPrefabId = prefabId;
        cardId = uuid;
        Debug.Log("스킬 활성화: " + state);
    }

    public string GetSkillPrefabId()
    {
        return skillPrefabId;
    }

    public string GetCardId()
    {
        return cardId;
    }

    public bool IsSkillActiveOn()
    {
        return isSkillActive;
    }
}
