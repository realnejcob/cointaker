using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
    [ReadOnly] public List<EnemyCard> cardsToMove;

    public void ConfigureCardsToMove() {
        ResetCardsToMove();
        cardsToMove = GetNewCardsToMove();
        HighlightCardsToMove();

    }

    public void HighlightCardsToMove() {
        foreach (var card in cardsToMove) {
            card.ShowSelectedOutline();
        }
    }

    public void UnhighlightCardsToMove() {
        foreach (var card in cardsToMove) {
            if (!BoardManager.Instance.EnemyCards.Contains(card))
                continue;

            card.HideSelectedOutline();
        }
    }

    private void ResetCardsToMove() {
        UnhighlightCardsToMove();
        cardsToMove.Clear();
    }

    private List<EnemyCard> GetNewCardsToMove() {
        var newCardsToMove = new List<EnemyCard>();
        var availableCards = BoardManager.Instance.EnemyCards;
        if (availableCards.Count < 1)
            return newCardsToMove;

        var randomIndex = Random.Range(0, availableCards.Count);
        newCardsToMove.Add(availableCards[randomIndex]);
        return newCardsToMove;
    }
}
