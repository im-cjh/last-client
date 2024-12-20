using UnityEngine;

public class HpBar : MonoBehaviour
{
    private Transform hpLeft;
    private Vector3 originalScale;
    private float maxHp;

    void Start()
    {
        hpLeft = transform.Find("HpLeft");
        originalScale = hpLeft.localScale;
    }

    public void SetMaxHp(float pMaxHp)
    {
        maxHp = pMaxHp;
    }

    public void SetHp(float hp)
    {
        Debug.Log(hp + ": " + maxHp);
        float scale = Mathf.Clamp(hp / maxHp, 0, 1);

        hpLeft.localScale = new Vector3(originalScale.x * scale, originalScale.y, originalScale.z);
    }
}
