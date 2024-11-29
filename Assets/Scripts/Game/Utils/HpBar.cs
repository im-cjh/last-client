using UnityEngine;

public class HpBar : MonoBehaviour
{
    private Transform hpLeft;
    private Vector3 originalScale;

    void Start()
    {
        hpLeft = transform.Find("HpLeft");
        originalScale = hpLeft.localScale;
    }

    public void SetHp(float hp, float maxHp)
    {
        float scale = Mathf.Clamp(hp / maxHp, 0, 1);

        hpLeft.localScale = new Vector3(originalScale.x * scale, originalScale.y, originalScale.z);
    }
}
