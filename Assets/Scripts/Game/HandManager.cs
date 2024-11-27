using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> cardPrefabs;
    [SerializeField] private Transform handZone;
    [SerializeField] private int maxHand = 7;
    private List<GameObject> cards = new List<GameObject>(); // 현재 패에 있는 카드
    private GameObject highlightedCard = null; // 마우스가 올라가있어서 강조중인 카드

    public void AddCard()
    {
        if (cards.Count >= maxHand)
        {
            Debug.Log("패가 가득차 카드를 받을 수 없었습니다.");
            return;
        }

        int randomIndex = Random.Range(0, cardPrefabs.Count);
        GameObject newCard = Instantiate(cardPrefabs[randomIndex], handZone);
        newCard.transform.localScale = Vector3.zero;
        Vector3 cardScale = new Vector3(0.75f, 0.75f, 0);
        newCard.transform.DOScale(cardScale, 0.5f).SetEase(Ease.OutBack);
        cards.Add(newCard);

        UpdateCardPosition();
    }

    public void RemoveCard(GameObject card)
    {
        if (cards.Contains(card))
        {
            cards.Remove(card);
            Destroy(card);
            UpdateCardPosition();
        }
    }

    public void SetHighlightedCard(GameObject card)
    {
        highlightedCard = card;
        // UpdateCardPosition();
    }

    public void ClearHighlightedCard()
    {
        highlightedCard = null;
        // UpdateCardPosition();
    }

    public void UpdateCardPosition()
    {
        float cardSpacing = 70f;
        float startX = -200f;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == highlightedCard) continue;

            Vector3 targetPosition = new Vector3(startX + i * cardSpacing, -10, 0);
            cards[i].transform.DOLocalMove(targetPosition, 0.5f).SetEase(Ease.OutQuad);

            CardDragAndHover cardHover = cards[i].GetComponent<CardDragAndHover>();
            cardHover.SetOriginalPosition(targetPosition);
        }
    }
}
