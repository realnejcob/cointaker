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
            coins += card.GetCoinsCount();
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
        if (!isMoveable)
            return;

        BoardManager.Instance.TargetSpace = this;
        BoardManager.Instance.ShowOriginActionIndicator();
    }

    public void OnPointerExit(PointerEventData eventData) {
        BoardManager.Instance.TargetSpace = null;
        BoardManager.Instance.HideOriginActionIndicator();
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (IsFree)
            return;

        if (GetTopCard().alignment != AlignmentType.PLAYER)
            return;

        if (eventData.button == PointerEventData.InputButton.Left) {
            if (!isMoveable)
                return;

            ConfigureNewOrigin();
            GetTopCard().DisplayOnTopOfAll();
        } else if (eventData.button == PointerEventData.InputButton.Right) {
            GetTopCard().StartZoomPreview();
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            if (BoardManager.Instance.OriginSpace != this)
                return;

            if (BoardManager.Instance.TargetSpace == null)
                return;

            //GetTopCard().DisplayOnTopOfStack();
            BoardManager.Instance.HideOriginActionIndicator();
            BoardManager.Instance.PerformAction();
            BoardManager.Instance.ClearSpaceReferences();
        } else if (eventData.button == PointerEventData.InputButton.Right) {
            if (GetTopCard() == null)
                return;

            GetTopCard().EndZoomPreview();
        }
    }

    #endregion

    private void ConfigureNewOrigin() {
        if (IsFree)
            return;

        BoardManager.Instance.OriginSpace = this;
        BoardManager.Instance.OriginSpace.GetTopCard().FindMovableSpaces();
    }

    public void UpdateStackCounter() {
        if (IsFree)
            return;

        GetTopCard().SetStackCounter(CardsCount);
    }
}