using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurn : State {
    public PlayerTurn(BattleSystem battlesystem) : base(battlesystem) {
    }

    public override IEnumerator Start() {
        BattleSystem.boardManager.debugView.SetIndicatorText("Player turn");
        yield break;
    }

    public override IEnumerator MouseUp() {
        if (BattleSystem.originSpace == null)
            yield break;

        BattleSystem.originSpace.GetTopCard().DisplayOnTopOfStack();

        var action = GetAction();
        if (action == ActionType.NONE)
            yield break;

        PerformAction(action);
        ClearSpaceReferences();

        if (BattleSystem.coinsReserve > 0) {
            BattleSystem.SetState(new CoinRedistribute(BattleSystem));
            yield break;
        } else {
            BattleSystem.SetState(new EnemyTurn(BattleSystem));
            yield break;
        }
        
    }

    public override IEnumerator MouseDown() {
        if (CanBeNewOrigin(BattleSystem.targetSpace) == false)
            yield break;

        BattleSystem.originSpace = BattleSystem.targetSpace;
        BattleSystem.originSpace.GetTopCard().CheckMoveableSpaces();
        BattleSystem.originSpace.GetTopCard().DisplayOnTopOfAll();
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
        var cardToMove = BattleSystem.originSpace.GetTopCard();
        BattleSystem.originSpace.RemoveFromSpace(cardToMove);

        cardToMove.transform.SetParent(BattleSystem.targetSpace.transform);
        BattleSystem.targetSpace.AddToSpace(cardToMove);

        cardToMove.RefreshDesiredPosition();
        cardToMove.StartMoveZoomTween();

        BattleSystem.targetSpace.GetTopCard().SetStackCounter(BattleSystem.targetSpace.CardsCount);
        BattleSystem.targetSpace.GetTopCard().UpdateStackBuff();
    }

    private void Stack() {
        BattleSystem.targetSpace.MoveCoinsToTopCard();
        BattleSystem.targetSpace.GetTopCard().UpdateStackBuff();
    }

    private void Eliminate() {
        var originTopCard = BattleSystem.originSpace.GetTopCard();
        var targetTopCard = BattleSystem.targetSpace.GetTopCard();

        StealCoins(originTopCard, targetTopCard, targetTopCard.CoinsCount());
        targetTopCard.EliminateCard();
    }

    private void Neutralize() {
        var totalCoins = 0;
        totalCoins += BattleSystem.originSpace.GetTopCard().CoinsCount();
        totalCoins += BattleSystem.targetSpace.GetTopCard().CoinsCount();

        BattleSystem.originSpace.GetTopCard().EliminateCard();
        BattleSystem.targetSpace.GetTopCard().EliminateCard();

        BattleSystem.coinsReserve += totalCoins;
    }

    private void Attack() {
        var originTopCard = BattleSystem.originSpace.GetTopCard();
        var targetTopCard = BattleSystem.targetSpace.GetTopCard();

        originTopCard.TakeHitPoint(out var isEliminated);

        StealCoins(originTopCard, targetTopCard, 1);
    }

    private void StealCoins(Card to, Card from, int amount) {
        if (from.CoinsCount() == 0)
            return;

        from.RemoveCoins(amount);
        to.AddCoins(amount);
    }

    private void SplitCoins() {
        if (BattleSystem.originSpace.CardsCount <= 1)
            return;

        var topCard = BattleSystem.originSpace.GetTopCard();
        var nextTopCard = BattleSystem.originSpace.GetNextTopCard();

        var totalCoins = topCard.CoinsCount();
        var coinsToMove = Mathf.FloorToInt(totalCoins / 2f);
        StealCoins(nextTopCard, topCard, coinsToMove);
    }

    private void RestoreAll() {
        BattleSystem.originSpace.GetTopCard().Restore();
        BattleSystem.targetSpace.GetTopCard().Restore();
    }

    public void ClearSpaceReferences() {
        BattleSystem.originSpace = null;
    }

    private ActionType GetAction() {
        if (BattleSystem.originSpace == null || BattleSystem.targetSpace == null)
            return ActionType.NONE;

        var originTopCard = BattleSystem.originSpace.GetTopCard();
        if (originTopCard.MoveableSpaces.Contains(BattleSystem.targetSpace) && BattleSystem.targetSpace.IsFree)
            return ActionType.MOVE;
        else if (!originTopCard.MoveableSpaces.Contains(BattleSystem.targetSpace) && BattleSystem.targetSpace.IsFree)
            return ActionType.NONE;

        var targetTopCard = BattleSystem.targetSpace.GetTopCard();

        if (originTopCard.MoveableSpaces.Contains(BattleSystem.targetSpace)) {
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