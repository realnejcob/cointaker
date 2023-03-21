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

    public BattleSystem battleSystem { get; private set; }

    [Space(15)]

    public List<CardEntity> playerDeck;
    public List<CardEntity> enemyDeck;

    public List<CardEntity> PlayerHand { get; set; } = new List<CardEntity>();
    public List<CardEntity> EnemyHand { get; set; } = new List<CardEntity>();
    public List<PlayerCard> PlayerCards { get; set; } = new List<PlayerCard>();
    public List<EnemyCard> EnemyCards { get; set; } = new List<EnemyCard>();

    private List<int> coinDistribution;

    [Header("Debug:")]
    public DebugView debugView;


    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }

        Instance = this;

        battleSystem = GetComponentInChildren<BattleSystem>();
    }

    private void OnDrawGizmos() {
        Grid.DrawGizmo();
    }

    public void CreateBoard() {
        Board = Instantiate(boardPrefab);
        Board.Setup();

        SetupCards();
    }

    private void SetupCards() {
        InstantiateCards();

        PlayerHand.AddRange(playerDeck);
        PlayerHand.Shuffle();

        EnemyHand.AddRange(enemyDeck);
        EnemyHand.Shuffle();

        EnemyHand.Shuffle();

        InitializeCards();
    }

    private void InstantiateCards() {
        InstantiatePlayerCard(new Vector2Int(1, 1));
        InstantiatePlayerCard(new Vector2Int(2, 1));
        InstantiatePlayerCard(new Vector2Int(1, 2));
        InstantiatePlayerCard(new Vector2Int(2, 2));

        InstantiateOpponentCard(new Vector2Int(1, 0), Direction.Up);
        InstantiateOpponentCard(new Vector2Int(2, 0), Direction.Up);
        InstantiateOpponentCard(new Vector2Int(0, 1), Direction.Right);
        InstantiateOpponentCard(new Vector2Int(3, 1), Direction.Left);
        InstantiateOpponentCard(new Vector2Int(0, 2), Direction.Right);
        InstantiateOpponentCard(new Vector2Int(3, 2), Direction.Left);
        InstantiateOpponentCard(new Vector2Int(1, 3), Direction.Down);
        InstantiateOpponentCard(new Vector2Int(2, 3), Direction.Down);
    }

    private void InitializeCards() {
        coinDistribution = GetCoinDistribution();

        for (int i = 0; i < PlayerCards.Count; i++) {
            var card = PlayerCards[i];
            card.Initialize(GetNextCardInHand(PlayerHand));
        }

        for (int i = 0; i < EnemyCards.Count; i++) {
            var card = EnemyCards[i];
            card.SetCoins(coinDistribution[i]);
            card.Initialize(GetNextCardInHand(EnemyHand));
        }
    }

    private CardEntity GetNextCardInHand(List<CardEntity> hand) {
        if (hand.Count == 0) return null;
        var entity = hand[0];
        hand.Remove(entity);
        return entity;
    }

    private void DrawNewPlayerCard(Vector2Int coordinate) {
        if (PlayerHand.Count == 0) return;

        var newCard = InstantiatePlayerCard(coordinate);
        newCard.Initialize(GetNextCardInHand(PlayerHand));
    }

    public void DrawCards() {
        var coordinates = new List<Vector2Int>();
        coordinates.Add(new Vector2Int(1, 1));
        coordinates.Add(new Vector2Int(2, 1));
        coordinates.Add(new Vector2Int(1, 2));
        coordinates.Add(new Vector2Int(2, 2));

        foreach (var coordinate in coordinates) {
            if (Board.Spaces[coordinate.x, coordinate.y].IsFree)
                DrawNewPlayerCard(coordinate);
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

    public int GetTotalPlayerCoins() {
        var totalPlayerCoins = 0;
        foreach (var card in BoardManager.Instance.PlayerCards) {
            totalPlayerCoins += card.CoinsCount();
        }
        return totalPlayerCoins;
    }

    public class CardInstanceSettings {
        public Card prefab;
        public Space space;
    }

    private Card InstantiateCard(CardInstanceSettings cardInstanceSettings) {
        var card = Instantiate(cardInstanceSettings.prefab, cardInstanceSettings.space.transform);
        return card;
    }

    private PlayerCard InstantiatePlayerCard(Vector2Int coordinate) {
        var cardInstanceSettings = new CardInstanceSettings() {
            prefab = playerCardPrefab,
            space = Board.Spaces[coordinate.x, coordinate.y]
        };

        var newCard = (PlayerCard)InstantiateCard(cardInstanceSettings);
        newCard.alignment = AlignmentType.PLAYER;
        PlayerCards.Add(newCard);
        cardInstanceSettings.space.AddToSpace(newCard);

        return newCard;
    }

    private EnemyCard InstantiateOpponentCard(Vector2Int coordinate, Direction initMoveDirection) {
        var cardInstanceSettings = new CardInstanceSettings() {
            prefab = enemyCardPrefab,
            space = Board.Spaces[coordinate.x, coordinate.y]
        };

        var newCard = (EnemyCard)InstantiateCard(cardInstanceSettings);
        newCard.alignment = AlignmentType.ENEMY;
        newCard.MoveDirection = initMoveDirection;
        EnemyCards.Add(newCard);
        cardInstanceSettings.space.AddToSpace(newCard);

        return newCard;
    }

    #region -- HELPER FUNCTIONS

    public List<Direction> GetDirectionsFromSpace(Space from) {
        var directions = new List<Direction>();
        var moveableSpaces = from.GetTopCard().MoveableSpaces;
        foreach (var moveableSpace in moveableSpaces) {
            directions.Add(GetDirectionFromToSpace(from, moveableSpace));
        }
        return directions;
    }

    public Direction GetDirectionFromToSpace(Space from, Space to) {
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

    public Space GetSpaceFromDirection(Space from, Direction direction) {
        var coordinate = from.coordinate;
        switch (direction) {
            case Direction.Left:
                if (coordinate.x - 1 < 0)
                    return null;
                return Board.Spaces[coordinate.x - 1, coordinate.y];
            case Direction.Right:
                if (coordinate.x + 1 > Grid.columns-1)
                    return null;
                return Board.Spaces[coordinate.x + 1, coordinate.y];
            case Direction.Up:
                if (coordinate.y + 1 > Grid.rows - 1)
                    return null;
                return Board.Spaces[coordinate.x, coordinate.y + 1];
            case Direction.Down:
                if (coordinate.y - 1 < 0)
                    return null;
                return Board.Spaces[coordinate.x, coordinate.y - 1];
            default:
                return null;
        }
    }

    #endregion
}