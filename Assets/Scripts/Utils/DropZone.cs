using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using DG.Tweening;

public class DropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private CanvasGroup dropZoneCanvasAlpha;
    [SerializeField] private HandManager handManager;
    private Tween highlightTween;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedCard = eventData.pointerDrag;

        if (droppedCard != null && droppedCard.GetComponent<CardDragAndHover>() != null)
        {
            TowerPlacementManager.instance.SetPlacementState(true); // 타워 설치 모드 활성화
            handManager.RemoveCard(droppedCard);
            HideDropZone();
        }
    }

    public void ShowDropZone()
    {
        SetDropZoneAlpha(1f);
    }

    public void HideDropZone()
    {
        SetDropZoneAlpha(0f);
    }

    private void SetDropZoneAlpha(float alpha)
    {
        if (dropZoneCanvasAlpha != null)
        {
            dropZoneCanvasAlpha.alpha = alpha;
            dropZoneCanvasAlpha.blocksRaycasts = alpha > 0;
        }
    }
}
