using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance = null;
    private bool isSkillActive = false;
    public string skillPrefabId;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SetSkillActive(bool state, string prefabId)
    {
        isSkillActive = state;
        skillPrefabId = prefabId;
        Debug.Log("스킬 활성화: " + state);
    }

    public string GetSkillPrefabId()
    {
        return skillPrefabId;
    }

    public bool IsSkillActiveOn()
    {
        return isSkillActive;
    }
}
