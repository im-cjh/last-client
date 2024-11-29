using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using DG.Tweening;

public class DropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private CanvasGroup dropZoneCanvasAlpha;
    [SerializeField] private HandManager handManager;
    [SerializeField] private TowerPlacer towerPlacer;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedCard = eventData.pointerDrag;

        if (droppedCard != null)
        {
            Card card = droppedCard.GetComponent<Card>();
            if (card != null)
            {
                string towerPrefabId = card.GetPrefabId();
                if (towerPrefabId != null)
                {
                    Debug.Log("towerPrefabId: " + towerPrefabId);
                    TowerPlacementManager.instance.SetPlacementState(true, towerPrefabId); // 타워 설치 모드 활성화
                    handManager.RemoveCard(droppedCard);
                    HideDropZone();
                }
            }
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
