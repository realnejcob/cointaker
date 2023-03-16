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

    private List<PlayerCard> playerCards = new List<PlayerCard>();
    private List<EnemyCard> enemyCards = new List<EnemyCard>();

    private List<int> coinDistribution;

    [Header("Debug:")]
    public StateIndicator stateIndicator;


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

        playerDeck.Shuffle();
        enemyDeck.Shuffle();


        InitializeCards();
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
}