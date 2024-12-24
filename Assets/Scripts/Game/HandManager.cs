using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Google.Protobuf.Collections;
using Protocol;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class HandManager : MonoBehaviour
{
    private Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>();
    [SerializeField] private Transform handZone;
    [SerializeField] private int maxHand = 7;
    private List<GameObject> hands = new List<GameObject>(); // 현재 패에 있는 카드
    private GameObject highlightedCard = null; // 마우스가 올라가있어서 강조중인 카드
    public static HandManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            _ = InitializeCards();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

    }

    public async Task InitializeCards()
    {
        await Utilities.RegisterPrefab("Prefab/Cards/BasicTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Cards/BuffTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Cards/IceTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Cards/MissileTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Cards/StrongTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Cards/TankTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Cards/ThunderTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Cards/TowerRepair", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Cards/OrbitalBeam", prefabMap);
    }

    public void AddInitCard(Google.Protobuf.Collections.RepeatedField<Protocol.CardData> cardData)
    {
        Debug.Log("InitCardData: " + cardData);
        foreach (Protocol.CardData card in cardData)
        {
            AddCard(card);
        }
    }

    public void AddCard(Protocol.CardData card)
    {
        Debug.Log(card);
        if (hands.Count >= maxHand)
        {
            Debug.Log("패가 가득차 카드를 받을 수 없었습니다.");
            return;
        }

        if (!prefabMap.ContainsKey(card.PrefabId))
        {
            Debug.LogError("등록되지 않은 프리팹: " + card.PrefabId);
            return;
        }

        // 새로운 카드 생성
        Debug.Log("AddCard: prefabId: " + card.PrefabId);
        GameObject newCard = Instantiate(prefabMap[card.PrefabId], handZone);

        // 카드의 Card Script 불러오기
        Card cardScript = newCard.GetComponent<Card>();
        if (cardScript != null)
        {
            cardScript.SetCardId(card.CardId);
        }
        else
        {
            Debug.Log("카드에 Card 스크립트가 없음");
        }

        // 애니메이션 효과
        newCard.transform.localScale = Vector3.zero;
        Vector3 cardScale = new Vector3(0.75f, 0.75f, 0);
        newCard.transform.DOScale(cardScale, 0.5f).SetEase(Ease.OutBack);

        hands.Add(newCard);

        UpdateCardPosition();
    }

    public void RemoveCard(GameObject card)
    {
        if (hands.Contains(card))
        {
            hands.Remove(card);
            Destroy(card);
            UpdateCardPosition(); // 카드 정렬
        }
    }

    public void SetHighlightedCard(GameObject card)
    {
        highlightedCard = card;
    }

    public void ClearHighlightedCard()
    {
        highlightedCard = null;
    }

    public void UpdateCardPosition()
    {
        float cardSpacing = 70f; // 카드 간격
        float startX = -200f; // 첫 패 위치

        for (int i = 0; i < hands.Count; i++)
        {
            if (hands[i] == highlightedCard) continue; // 마우스가 올라가 있는 카드는 정렬 대상에서 제외

            Vector3 targetPosition = new Vector3(startX + i * cardSpacing, -10, 0);
            hands[i].transform.DOLocalMove(targetPosition, 0.5f).SetEase(Ease.OutQuad);

            CardDragAndHover cardHover = hands[i].GetComponent<CardDragAndHover>();
            cardHover.SetOriginalPosition(targetPosition);
        }
    }
}
