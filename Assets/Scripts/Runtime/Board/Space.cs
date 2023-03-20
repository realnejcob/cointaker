using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Space : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {
    [ReadOnly] [SerializeField] private bool isMoveable;
    [ReadOnly] public Vector2Int coordinate;

    public bool IsFree { get { if(cards.Count == 0) { return true; } else { return false; };}}
    public int CardsCount { get { return cards.Count; } }
    public int TopCardIndex { get { return CardsCount - 1; } }

    public int CoinsOnSpace { get { return GetCoinsOnSpace(); } }

    public List<Card> Cards { get { return cards; } private set { } }

    [SerializeField] private List<Card> cards;
    public GameObject isMoveableIndicator;

    private Canvas canvas;

    private void Awake() {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    public Card GetTopCard() {
        return cards[TopCardIndex];
    }

    public Card GetNextTopCard() {
        return cards[TopCardIndex-1];
    }

    public void SetIsMoveable(bool value) {
        isMoveable = value;
        isMoveableIndicator.SetActive(value);
    }

    public bool GetIsMoveable() {
        return isMoveable;
    }

    public void AddToSpace(Card card) {
        cards.Add(card);
    }

    public void RemoveFromSpace(Card card) {
        if (!cards.Contains(card))
            return;

        cards.Remove(card);
    }

    public int GetCoinsOnSpace() {
        var coins = 0;
        foreach (Card card in cards) {
            coins += card.CoinsCount();
        }
        return coins;
    }

    public void ResetCoinsOnAllCards() {
        foreach (var card in cards) {
            card.SetCoins(0);
        }
    }

    public void MoveCoinsToTopCard() {
        var coinsOnSpace = GetCoinsOnSpace();
        ResetCoinsOnAllCards();
        GetTopCard().SetCoins(coinsOnSpace);
    }

    #region -- ON POINTER METHODS
    public void OnPointerEnter(PointerEventData eventData) {
        BoardManager.Instance.battleSystem.targetSpace = this;
    }

    public void OnPointerExit(PointerEventData eventData) {
        BoardManager.Instance.battleSystem.targetSpace = null;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right) {
            GetTopCard().StartZoomPreview();
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right) {
            if (IsFree)
                return;

            GetTopCard().EndZoomPreview();
        }
    }

    #endregion
}