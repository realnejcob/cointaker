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

    public void ResetCardsToMove() {
        UnhighlightCardsToMove();
        cardsToMove.Clear();
    }

    private void UnhighlightCardsToMove() {
        foreach (var card in cardsToMove) {
            if (!BoardManager.Instance.EnemyCards.Contains(card))
                continue;

            card.HideSelectedOutline();
        }
    }

    public bool HasCardsToMove() {
        if (cardsToMove.Count == 0)
            return false;

        return true;
    }

    public bool GetCardWillMove(EnemyCard card) {
        if (cardsToMove.Contains(card))
            return true;

        return false;
    }

    private List<EnemyCard> GetNewCardsToMove() {
        var newCardsToMove = new List<EnemyCard>();
        var availableCards = BoardManager.Instance.EnemyCards;

        if (availableCards.Count <= 0)
            return newCardsToMove;

        EnemyCard bestCard = availableCards[0];
        int bestCurrentValue = -1;
        foreach (var card in availableCards) {
            // Check values and compare
            int bestMoveValue = card.GetBestMove();
            if (bestMoveValue > bestCurrentValue) {
                bestCurrentValue = bestMoveValue;
                bestCard = card;
            }
        }

        newCardsToMove.Add(bestCard);
        return newCardsToMove;
    }
}
