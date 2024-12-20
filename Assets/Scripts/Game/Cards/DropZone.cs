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
            string cardId = card.GetCardId(); // 카드의 uuid
            Character character = CharacterManager.instance.GetCharacter(PlayerInfoManager.instance.userId);

            // 타워 카드인지 스킬 카드인지
            if (card.isTowerCard)
            {
                string towerPrefabId = card.GetPrefabId();
                if (towerPrefabId != null)
                {
                    // 로컬 플레이어의 타워 설치 모드 활성화
                    character.SetPrefabId(towerPrefabId, cardId);
                    character.isTowerActive = true;
                    TowerPlacementManager.instance.SetTowerActive(true);
                    handManager.RemoveCard(droppedCard);
                    HideDropZone();
                }
            }
            else if (!card.isTowerCard)
            {
                string skillPrefabId = card.GetPrefabId();
                if (skillPrefabId != null)
                {
                    character.SetPrefabId(skillPrefabId, cardId);
                    character.isSkillActive = true;
                    SkillManager.instance.SetSkillActive(true);
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
