using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurn : State {
    public PlayerTurn(BattleSystem battlesystem) : base(battlesystem) {
    }

    public override IEnumerator Start() {
        BattleSystem.boardManager.debugView.SetIndicatorText("Player turn", Color.green);
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

        BoardManager.Instance.RecalculateBoard();

        ClearSpaceReferences();

        yield return new WaitForSeconds(0.25f);

        BoardManager.Instance.DrawCards();

        BoardManager.Instance.RecalculateBoard();

        if (BoardManager.Instance.GetTotalPlayerCoins() == 12) {
            BattleSystem.SetState(new Win(BattleSystem));
            yield break;
        }

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

    public override IEnumerator Skip() {
        BattleSystem.SetState(new EnemyTurn(BattleSystem));
        yield break;
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
                TriggerAbility();
                SplitCoins();
                Move();
                break;
            case ActionType.STACK:
                TriggerAbility();
                SplitCoins();
                RestoreAll();
                Move();
                Stack();
                break;
            case ActionType.ELIMINATE:
                SplitCoins();
                Eliminate();
                TriggerAbility();
                Move();
                break;
            case ActionType.NEUTRALIZE:
                TriggerAbility();
                Neutralize();
                break;
            case ActionType.ATTACK:
                TriggerAbility();
                Attack();
                break;
            default:
                break;
        }
    }

    private void Move() {
        var cardToMove = BattleSystem.originSpace.GetTopCard();
        cardToMove.Move(BattleSystem.targetSpace);
        cardToMove.SetStackCounter(BattleSystem.targetSpace.CardsCount);
        //cardToMove.UpdateBuff();
    }

    private void Stack() {
        BattleSystem.targetSpace.MoveCoinsToTopCard();
        //BattleSystem.targetSpace.GetTopCard().UpdateBuff();
    }

    private void TriggerAbility() {
        var origin = BattleSystem.originSpace;
        var target = BattleSystem.targetSpace;
        var card = origin.GetTopCard();

        // Moving from empty to stack
        if (origin.CardsCount == 1 && target.CardsCount == 1 && !target.GetHasEnemy()) {
            card.TriggerOnStack(target);
            return;
        }

        // Moving from stack to stack
        if (origin.CardsCount > 1 && target.CardsCount >= 1 && !target.GetHasEnemy()) {
            card.TriggerOnDestack(origin);
            card.TriggerOnStack(target);
        }

        // Moving from stack to enemy eliminate
        if (origin.CardsCount > 1 && target.CardsCount >= 1 && target.GetHasEnemy() && card.GetTotalStrength() > target.GetTopCard().GetTotalStrength()) {
            card.TriggerOnDestack(origin);
        }

        // Moving from stack to enemy neutralize
        if (origin.CardsCount > 1 && target.CardsCount >= 1 && target.GetHasEnemy() && card.GetTotalStrength() == target.GetTopCard().GetTotalStrength()) {
            card.TriggerOnDestack(origin);
        }

        // Moving from stack to empty
        if (origin.CardsCount > 1 && target.CardsCount == 0) {
            card.TriggerOnDestack(origin);
        }
    }

    private void Eliminate() {
        var originTopCard = BattleSystem.originSpace.GetTopCard();
        var targetTopCard = BattleSystem.targetSpace.GetTopCard();

        originTopCard.StealCoins(targetTopCard, targetTopCard.CoinsCount());
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

        originTopCard.StealCoins(targetTopCard, 1);

        var enemyAI = BoardManager.Instance.battleSystem.enemyAI;
        if (!enemyAI.GetCardWillMove((EnemyCard)targetTopCard))
            return;

        enemyAI.ResetCardsToMove();
    }

    private void SplitCoins() {
        if (BattleSystem.originSpace.CardsCount <= 1)
            return;

        var topCard = BattleSystem.originSpace.GetTopCard();
        var nextTopCard = BattleSystem.originSpace.GetNextTopCard();

        var totalCoins = topCard.CoinsCount();
        var coinsToMove = Mathf.FloorToInt(totalCoins / 2f);
        nextTopCard.StealCoins(topCard, coinsToMove);
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
                    if (BattleSystem.targetSpace.CardsCount == 1)
                        return ActionType.STACK;
                    break;
                case AlignmentType.ENEMY:
                    if (targetTopCard.GetTotalStrength() > originTopCard.GetTotalStrength())
                        return ActionType.ATTACK;
                    if (originTopCard.GetTotalStrength() > targetTopCard.GetTotalStrength())
                        return ActionType.ELIMINATE;
                    if (originTopCard.GetTotalStrength() == targetTopCard.GetTotalStrength())
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
    ATTACK = 5,
    SHIFT = 6
}