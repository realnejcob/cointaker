using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurn : State {
    public PlayerTurn(BattleSystem battlesystem) : base(battlesystem) {
    }

    public override IEnumerator Start() {
        BattleSystem.boardManager.stateIndicator.SetIndicatorText("Player Turn");
        yield break;
    }

    public override IEnumerator MouseUp() {
        if (BattleSystem.OriginSpace == null)
            yield break;

        BattleSystem.OriginSpace.GetTopCard().DisplayOnTopOfStack();

        var action = GetAction();
        if (action == ActionType.NONE)
            yield break;

        PerformAction(action);
        ClearSpaceReferences();

        // Check if the player has coins to redistribute
            // Redestribute coins

        // If player has all coins, go to win state

        // Else, continue to enemy turn
        BattleSystem.SetState(new EnemyTurn(BattleSystem));
        yield break;
    }

    public override IEnumerator MouseDown() {
        if (CanBeNewOrigin(BattleSystem.TargetSpace) == false)
            yield break;

        BattleSystem.OriginSpace = BattleSystem.TargetSpace;
        BattleSystem.OriginSpace.GetTopCard().CheckMoveableSpaces();
        BattleSystem.OriginSpace.GetTopCard().DisplayOnTopOfAll();
    }

    public bool CanBeNewOrigin(Space spaceToCheck) {
        if (spaceToCheck == null)
            return false;

        if (spaceToCheck.IsFree)
            return false;

        if (spaceToCheck.GetTopCard().alignment != AlignmentType.PLAYER)
            return false;

        if (!spaceToCheck.GetIsMoveable()) 
            return false;

        return true;
    }

    #region -- ACTIONS --

    public void PerformAction(ActionType actionType) {
        switch (actionType) {
            case ActionType.NONE:
                break;
            case ActionType.MOVE:
                SplitCoins();
                Move();
                break;
            case ActionType.STACK:
                SplitCoins();
                RestoreAll();
                Move();
                Stack();
                break;
            case ActionType.ELIMINATE:
                SplitCoins();
                Eliminate();
                Move();
                break;
            case ActionType.NEUTRALIZE:
                Neutralize();
                break;
            case ActionType.ATTACK:
                Attack();
                break;
            default:
                break;
        }
    }

    private void Move() {
        var cardToMove = BattleSystem.OriginSpace.GetTopCard();
        BattleSystem.OriginSpace.RemoveFromSpace(cardToMove);

        cardToMove.transform.SetParent(BattleSystem.TargetSpace.transform);
        BattleSystem.TargetSpace.AddToSpace(cardToMove);

        cardToMove.RefreshDesiredPosition();
        cardToMove.StartMoveZoomTween();

        BattleSystem.TargetSpace.GetTopCard().SetStackCounter(BattleSystem.TargetSpace.CardsCount);
        BattleSystem.TargetSpace.GetTopCard().UpdateStackBuff();
    }

    private void Stack() {
        BattleSystem.TargetSpace.MoveCoinsToTopCard();
        BattleSystem.TargetSpace.GetTopCard().UpdateStackBuff();
    }

    private void Eliminate() {
        var originTopCard = BattleSystem.OriginSpace.GetTopCard();
        var targetTopCard = BattleSystem.TargetSpace.GetTopCard();

        StealCoins(originTopCard, targetTopCard, targetTopCard.GetCoinsCount());
        targetTopCard.EliminateCard();
    }

    private void Neutralize() {
        BattleSystem.OriginSpace.GetTopCard().EliminateCard();
        BattleSystem.TargetSpace.GetTopCard().EliminateCard();
    }

    private void Attack() {
        var originTopCard = BattleSystem.OriginSpace.GetTopCard();
        var targetTopCard = BattleSystem.TargetSpace.GetTopCard();

        originTopCard.TakeHitPoint(out var isEliminated);

        StealCoins(originTopCard, targetTopCard, 1);
    }

    private void StealCoins(Card to, Card from, int amount) {
        if (from.GetCoinsCount() == 0)
            return;

        from.RemoveCoins(amount);
        to.AddCoins(amount);
    }

    private void SplitCoins() {
        if (BattleSystem.OriginSpace.CardsCount <= 1)
            return;

        var topCard = BattleSystem.OriginSpace.GetTopCard();
        var nextTopCard = BattleSystem.OriginSpace.GetNextTopCard();

        var totalCoins = topCard.GetCoinsCount();
        var coinsToMove = Mathf.FloorToInt(totalCoins / 2f);
        StealCoins(nextTopCard, topCard, coinsToMove);
    }

    private void RestoreAll() {
        BattleSystem.OriginSpace.GetTopCard().Restore();
        BattleSystem.TargetSpace.GetTopCard().Restore();
    }

    public void ClearSpaceReferences() {
        BattleSystem.OriginSpace = null;
    }

    private ActionType GetAction() {
        if (BattleSystem.OriginSpace == null || BattleSystem.TargetSpace == null)
            return ActionType.NONE;

        var originTopCard = BattleSystem.OriginSpace.GetTopCard();
        if (originTopCard.MoveableSpaces.Contains(BattleSystem.TargetSpace) && BattleSystem.TargetSpace.IsFree)
            return ActionType.MOVE;
        else if (!originTopCard.MoveableSpaces.Contains(BattleSystem.TargetSpace) && BattleSystem.TargetSpace.IsFree)
            return ActionType.NONE;

        var targetTopCard = BattleSystem.TargetSpace.GetTopCard();

        if (originTopCard.MoveableSpaces.Contains(BattleSystem.TargetSpace)) {
            switch (targetTopCard.alignment) {
                case AlignmentType.NONE:
                    break;
                case AlignmentType.PLAYER:
                    if (targetTopCard.strength == originTopCard.strength)
                        return ActionType.STACK;
                    break;
                case AlignmentType.OPPONENT:
                    if (targetTopCard.strength > originTopCard.strength)
                        return ActionType.ATTACK;
                    if (originTopCard.strength > targetTopCard.strength)
                        return ActionType.ELIMINATE;
                    if (originTopCard.strength == targetTopCard.strength)
                        return ActionType.NEUTRALIZE;
                    break;
                default:
                    break;
            }
        }

        return ActionType.NONE;
    }
    #endregion
}

public enum ActionType {
    NONE = 0,
    MOVE = 1,
    STACK = 2,
    ELIMINATE = 3,
    NEUTRALIZE = 4,
    ATTACK = 5
}