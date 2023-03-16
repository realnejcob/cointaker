using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    public static BoardManager Instance;
    public Board Board { get; private set; }
    [SerializeField] private Board boardPrefab;

    [SerializeField] private PlayerCard playerCardPrefab;
    [SerializeField] private EnemyCard enemyCardPrefab;

    [Space(15)]

    public List<CardEntity> playerDeck;
    public List<CardEntity> enemyDeck;

    private List<PlayerCard> playerCards = new List<PlayerCard>();
    private List<EnemyCard> enemyCards = new List<EnemyCard>();

    [Header("Dynamic References:")]
    [ReadOnly] public Space OriginSpace;
    [ReadOnly] public Space TargetSpace;

    private List<int> coinDistribution;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }

        Instance = this;
    }

    private void Start() {
        CreateBoard();
        SetupCards();
    }

    private void Update() {
        if (Input.GetMouseButtonUp(0)) {
            if (OriginSpace == null)
                return;

            OriginSpace = null;
        }
    }

    private void OnDrawGizmos() {
        Grid.DrawGizmo();
    }

    private void CreateBoard() {
        Board = Instantiate(boardPrefab);
        Board.Setup();
    }

    private void SetupCards() {
        InstantiateCards();

        playerDeck.Shuffle();
        enemyDeck.Shuffle();


        InitializeCards();
    }

    public void ShowOriginActionIndicator() {
        if (GetAction() == ActionType.NONE)
            return;

        OriginSpace.GetTopCard().ShowActionIndicator(GetDirectionFromToSpace(OriginSpace, TargetSpace));
    }

    public void HideOriginActionIndicator() {
        if (OriginSpace == null)
            return;

        OriginSpace.GetTopCard().HideActionIndicator();
    }

    private void InstantiateCards() {
        InstantiatePlayerCard(new Vector2Int(1, 1));
        InstantiatePlayerCard(new Vector2Int(2, 1));
        InstantiatePlayerCard(new Vector2Int(1, 2));
        InstantiatePlayerCard(new Vector2Int(2, 2));

        InstantiateOpponentCard(new Vector2Int(1, 0));
        InstantiateOpponentCard(new Vector2Int(2, 0));
        InstantiateOpponentCard(new Vector2Int(0, 1));
        InstantiateOpponentCard(new Vector2Int(3, 1));
        InstantiateOpponentCard(new Vector2Int(0, 2));
        InstantiateOpponentCard(new Vector2Int(3, 2));
        InstantiateOpponentCard(new Vector2Int(1, 3));
        InstantiateOpponentCard(new Vector2Int(2, 3));
    }

    private void InitializeCards() {
        coinDistribution = GetCoinDistribution();

        for (int i = 0; i < playerCards.Count; i++) {
            var card = playerCards[i];
            card.Initialize(playerDeck[i]);
            card.name = $"PlayerCard [{card.displayName}]";
        }

        for (int i = 0; i < enemyCards.Count; i++) {
            var card = enemyCards[i];
            card.SetCoins(coinDistribution[i]);
            card.Initialize(enemyDeck[i]);
            card.name = $"EnemyCard [{card.displayName}]";
        }
    }

    private List<int> GetCoinDistribution() {
        var newDistribution = new List<int>();
        newDistribution.Add(3);
        newDistribution.Add(2);
        newDistribution.Add(2);
        newDistribution.Add(1);
        newDistribution.Add(1);
        newDistribution.Add(1);
        newDistribution.Add(1);
        newDistribution.Add(1);
        newDistribution.Shuffle();
        return newDistribution;
    }

    public class CardInstanceSettings {
        public Card prefab;
        public Space space;
    }

    private Card InstantiateCard(CardInstanceSettings cardInstanceSettings) {
        var card = Instantiate(cardInstanceSettings.prefab, cardInstanceSettings.space.transform);
        return card;
    }

    private void InstantiatePlayerCard(Vector2Int coordinate) {
        var cardInstanceSettings = new CardInstanceSettings() {
            prefab = playerCardPrefab,
            space = Board.Spaces[coordinate.x, coordinate.y]
        };

        var newCard = (PlayerCard)InstantiateCard(cardInstanceSettings);
        newCard.alignment = AlignmentType.PLAYER;
        playerCards.Add(newCard);
        cardInstanceSettings.space.AddToSpace(newCard);
    }

    private void InstantiateOpponentCard(Vector2Int coordinate) {
        var cardInstanceSettings = new CardInstanceSettings() {
            prefab = enemyCardPrefab,
            space = Board.Spaces[coordinate.x, coordinate.y]
        };

        var newCard = (EnemyCard)InstantiateCard(cardInstanceSettings);
        newCard.alignment = AlignmentType.OPPONENT;
        enemyCards.Add(newCard);
        cardInstanceSettings.space.AddToSpace(newCard);
    }

    private Direction GetDirectionFromToSpace(Space from, Space to) {
        var originCoordinate = from.coordinate;
        var targetCoordinate = to.coordinate;

        if (targetCoordinate.y > originCoordinate.y) {
            return Direction.Up;
        } else if (targetCoordinate.y < originCoordinate.y) {
            return Direction.Down;
        } else if (targetCoordinate.x > originCoordinate.x) {
            return Direction.Right;
        } else {
            return Direction.Left;
        }
    }

    #region -- ACTIONS --

    public void PerformAction() {
        switch (GetAction()) {
            case ActionType.NONE:
                break;
            case ActionType.MOVE:
                SplitCoins();
                Move();
                break;
            case ActionType.STACK:
                SplitCoins();
                RestoreCardHitpoints();
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
        var cardToMove = OriginSpace.GetTopCard();
        OriginSpace.RemoveFromSpace(cardToMove);

        cardToMove.transform.SetParent(TargetSpace.transform);
        TargetSpace.AddToSpace(cardToMove);

        cardToMove.RefreshDesiredPosition();
        cardToMove.StartMoveZoomTween();

        TargetSpace.UpdateStackCounter();
        TargetSpace.GetTopCard().UpdateStackBuff();
    }

    private void Stack() {
        TargetSpace.MoveCoinsToTopCard();
        TargetSpace.GetTopCard().UpdateStackBuff();
    }

    private void Eliminate() {
        var originTopCard = OriginSpace.GetTopCard();
        var targetTopCard = TargetSpace.GetTopCard();

        StealCoins(originTopCard, targetTopCard, targetTopCard.GetCoinsCount());
        targetTopCard.EliminateCard();
    }

    private void Neutralize() {
        OriginSpace.GetTopCard().EliminateCard();
        TargetSpace.GetTopCard().EliminateCard();
    }

    private void Attack() {
        var originTopCard = OriginSpace.GetTopCard();
        var targetTopCard = TargetSpace.GetTopCard();

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
        if (OriginSpace.CardsCount <= 1)
            return;

        var topCard = OriginSpace.GetTopCard();
        var nextTopCard = OriginSpace.GetNextTopCard();

        var totalCoins = topCard.GetCoinsCount();
        var coinsToMove = Mathf.FloorToInt(totalCoins / 2f);
        StealCoins(nextTopCard, topCard, coinsToMove);
    }

    private void RestoreCardHitpoints() {
        OriginSpace.GetTopCard().RestoreHitpoints();
        TargetSpace.GetTopCard().RestoreHitpoints();
    }

    public void ClearSpaceReferences() {
        OriginSpace = null;
        TargetSpace = null;
    }

    private ActionType GetAction() {
        if (OriginSpace == null || TargetSpace == null)
            return ActionType.NONE;

        var originTopCard = OriginSpace.GetTopCard();
        if (originTopCard.MoveableSpaces.Contains(TargetSpace) && TargetSpace.IsFree)
            return ActionType.MOVE;
        else if (!originTopCard.MoveableSpaces.Contains(TargetSpace) && TargetSpace.IsFree)
            return ActionType.NONE;

        var targetTopCard = TargetSpace.GetTopCard();

        if (originTopCard.MoveableSpaces.Contains(TargetSpace)) {
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