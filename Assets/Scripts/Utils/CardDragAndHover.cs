using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UIElements;

public class CardDragAndHover : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private DropZone dropZone;
    private HandManager handManager;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private Vector3 originalPosition; // 카드의 원래 위치
    private Vector3 originalScale;    // 카드의 원래 크기
    private int originalSiblingIndex; // 카드의 원래 Sibling Index
    private Vector2 dragOffset;       // 드래그 시작 시 마우스와 카드의 상대 위치
    private bool isDragging = false;  // 드래그 상태 확인

    [SerializeField] private float hoverOffset = 20f;  // 마우스 오버 시 위로 이동할 거리
    [SerializeField] private Vector3 hoverScale = new Vector3(1f, 1f, 1f); // 마우스 오버 시 크기
    [SerializeField] private float animationDuration = 0.2f; // 애니메이션 지속 시간

    private void Awake()
    {
        dropZone = FindAnyObjectByType<DropZone>();
        handManager = FindAnyObjectByType<HandManager>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        // 카드의 원래 크기를 저장
        originalScale = rectTransform.localScale;
    }

    public void SetOriginalPosition(Vector3 position)
    {
        originalPosition = position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging) return; // 드래그 중일 때는 Hover 무시

        handManager.SetHighlightedCard(gameObject);

        // 원래 Sibling Index 저장
        originalSiblingIndex = transform.GetSiblingIndex();

        // 강조된 카드를 부모의 마지막 자식으로 이동 (다른 카드 위로 표시)
        transform.SetAsLastSibling();

        // 마우스 오버 시 위로 이동 및 크기 증가
        Vector3 targetPosition = originalPosition + new Vector3(0, hoverOffset, 0);
        rectTransform.DOLocalMove(targetPosition, animationDuration).SetEase(Ease.OutQuad);
        rectTransform.DOScale(hoverScale, animationDuration).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging) return; // 드래그 중일 때는 Hover 무시

        handManager.ClearHighlightedCard();

        // 강조된 카드를 원래 Sibling Index로 복원
        transform.SetSiblingIndex(originalSiblingIndex);

        // 마우스가 나갔을 때 원래 위치와 크기로 복귀
        rectTransform.DOLocalMove(originalPosition, animationDuration).SetEase(Ease.OutQuad);
        rectTransform.DOScale(originalScale, animationDuration).SetEase(Ease.OutQuad);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (TowerPlacementManager.instance.IsPlacementActive()) return;

        isDragging = true; // 드래그 상태 설정
        canvasGroup.alpha = 0.6f; // 드래그 중 투명도 조정
        canvasGroup.blocksRaycasts = false; // 드래그 중 Raycast 차단

        // Canvas 기준으로 마우스와 카드의 상대 위치 계산
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 mousePositionInCanvas
        );

        dragOffset = mousePositionInCanvas - (Vector2)rectTransform.localPosition;

        // 드래그 시작 시 카드를 부모의 마지막 자식으로 이동
        transform.SetAsLastSibling();

        // Dropzone 보이게
        dropZone?.ShowDropZone();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (TowerPlacementManager.instance.IsPlacementActive()) return;

        // 마우스 위치를 Canvas의 로컬 좌표로 변환
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        // 마우스 위치에 dragOffset을 더해 보정된 카드 위치 설정
        rectTransform.localPosition = localPoint - dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (TowerPlacementManager.instance.IsPlacementActive()) return;

        isDragging = false; // 드래그 상태 해제
        canvasGroup.alpha = 1f; // 드래그 종료 후 투명도 복원
        canvasGroup.blocksRaycasts = true; // Raycast 활성화

        // 드래그 종료 시 원래 위치로 복귀
        rectTransform.DOLocalMove(originalPosition, animationDuration).SetEase(Ease.OutQuad);
        rectTransform.DOScale(originalScale, animationDuration).SetEase(Ease.OutQuad);

        // Sibling Index 복원
        transform.SetSiblingIndex(originalSiblingIndex);

        dropZone?.HideDropZone();
    }
}
