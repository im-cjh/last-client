using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private int originalSiblingIndex;

    [SerializeField] private float hoverOffset = 20f;
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private Vector3 hoverScale = new Vector3(1f, 1f, 1f);

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.localPosition;
        originalScale = rectTransform.localScale;
    }

    // Update is called once per frame
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 원래 siblingIndex 저장
        originalSiblingIndex = transform.GetSiblingIndex();
        // 가장 위에 보이게 설정
        transform.SetAsLastSibling();

        // 마우스 올라오면 카드 떠오름
        Vector3 targetPosition = originalPosition + new Vector3(0, hoverOffset, 0);
        rectTransform.DOLocalMove(targetPosition, animationDuration).SetEase(Ease.OutQuad);
        rectTransform.DOScale(hoverScale, animationDuration).SetEase(Ease.OutQuad); // 크기 커짐
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 나가면 원래대로
        transform.SetSiblingIndex(originalSiblingIndex);
        rectTransform.DOLocalMove(originalPosition, animationDuration).SetEase(Ease.OutQuad);
        rectTransform.DOScale(originalScale, animationDuration).SetEase(Ease.OutQuad);
    }
}
