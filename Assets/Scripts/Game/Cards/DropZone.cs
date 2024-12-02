using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using DG.Tweening;

public class DropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private CanvasGroup dropZoneCanvasAlpha;
    [SerializeField] private HandManager handManager;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedCard = eventData.pointerDrag;

        if (droppedCard != null)
        {
            // 카드 정보
            Card card = droppedCard.GetComponent<Card>();

            // 타워 카드인지 스킬 카드인지
            if (card.isTowerCard)
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
            else if (!card.isTowerCard)
            {
                string skillPrefabId = card.GetPrefabId();
                if (skillPrefabId != null)
                {
                    Debug.Log("skillPrefabId: " + skillPrefabId);
                    SkillManager.instance.SetSkillActive(true, skillPrefabId); // 스킬 사용 모드 활성화
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
