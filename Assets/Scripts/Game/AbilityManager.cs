using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using System.Collections;
using TMPro;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private Button abilityButton;
    [SerializeField] private Image cooldownImage;
    private float cooldown = 5f;
    private bool isCooldown = false;

    void Start()
    {
        abilityButton.onClick.AddListener(UseAbility);

        cooldownImage.fillAmount = 0;
    }

    void Update()
    {
        if (!isCooldown && IsMouseOverButton())
        {
            abilityButton.transform.DOScale(1.1f, 0.2f);
        }
        else
        {
            abilityButton.transform.DOScale(1f, 0.2f);
        }
    }

    void UseAbility()
    {
        if (isCooldown) return;

        Debug.Log("Ability 사용");
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        isCooldown = true;

        float remainingTime = cooldown;
        cooldownImage.fillAmount = 1;

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            cooldownImage.fillAmount = remainingTime / cooldown;
            yield return null;
        }

        cooldownImage.fillAmount = 0;
        isCooldown = false;
    }

    bool IsMouseOverButton()
    {
        RectTransform rectTransform = abilityButton.GetComponent<RectTransform>();
        Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);
        return rectTransform.rect.Contains(localMousePosition);
    }
}
