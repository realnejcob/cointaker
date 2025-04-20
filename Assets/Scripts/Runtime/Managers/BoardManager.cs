using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    public static BoardManager Instance;
    public Board Board { get; private set; }
    [SerializeField] private Board boardPrefab;
    [SerializeField] private BoardLayout currentBoardLayout;
    [SerializeField] private List<BoardLayout> availableLayouts = new List<BoardLayout>();

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

        InitializeSavedLayout();

        battleSystem = GetComponentInChildren<BattleSystem>();
    }

    private void OnDrawGizmos() {
        Grid.DrawGizmo();
    }

    public void CreateBoard() {
        Board = Instantiate(boardPrefab);
        Board.Setup(currentBoardLayout);

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
        foreach (var space in Board.Spaces) {
            var spaceType = Board.GetSpaceTypeFromSpace(space);

            if (spaceType == SpaceType.PLAYER || spaceType == SpaceType.PLAYER_SPAWN) {
                InstantiatePlayerCard(space.coordinate);
                continue;
            }

            if (spaceType == SpaceType.ENEMY || spaceType == SpaceType.ENEMY_SPAWN) {
                InstantiateOpponentCard(space.coordinate, (Direction)UnityEngine.Random.Range(0,4));
            }
        }
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

    public void RecalculateBoard() {
        foreach (var space in Board.Spaces) {
            space.UpdateBuffIndicator();
            space.ResetTempStrengthOnAllCards();

            if (space.CardsCount == 0)
                continue;

            var topCard = space.GetTopCard();
            if (topCard.alignment == AlignmentType.ENEMY)
                continue;

            if (space.CardsCount >= 1) {
                // Add space buff
                topCard.TempStrength += space.TempStrength;
            }

            if (space.CardsCount > 1) {
                // Add stack buff
                for (int i = 0; i < space.CardsCount - 1; i++) {
                    topCard.TempStrength += space.Cards[i].InitStrength;
                }
            }

            space.UpdateAllCards();
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
        newCard.FeedbackFlash(FeedbackIndicatorType.DRAW);
    }

    public void DrawCards() {
        var drawSpaces = Board.PlayerDrawSpaces;

        foreach (var space in drawSpaces) {
            if (Board.Spaces[space.coordinate.x, space.coordinate.y].IsFree)
                DrawNewPlayerCard(space.coordinate);
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
            return Direction.UP;
        } else if (targetCoordinate.y < originCoordinate.y) {
            return Direction.DOWN;
        } else if (targetCoordinate.x > originCoordinate.x) {
            return Direction.RIGHT;
        } else {
            return Direction.LEFT;
        }
    }

    public Space GetSpaceFromDirection(Space from, Direction direction) {
        var coordinate = from.coordinate;
        switch (direction) {
            case Direction.LEFT:
                if (coordinate.x - 1 < 0)
                    return null;
                return Board.Spaces[coordinate.x - 1, coordinate.y];
            case Direction.RIGHT:
                if (coordinate.x + 1 > Grid.columns-1)
                    return null;
                return Board.Spaces[coordinate.x + 1, coordinate.y];
            case Direction.UP:
                if (coordinate.y + 1 > Grid.rows - 1)
                    return null;
                return Board.Spaces[coordinate.x, coordinate.y + 1];
            case Direction.DOWN:
                if (coordinate.y - 1 < 0)
                    return null;
                return Board.Spaces[coordinate.x, coordinate.y - 1];
            default:
                return null;
        }
    }

    #endregion

    private void InitializeSavedLayout() {
        if (!PlayerPrefs.HasKey("layout")) {
            PlayerPrefs.SetString("layout", "orig");
        }

        currentBoardLayout = GetLayoutFromKey(PlayerPrefs.GetString("layout"));
    }

    public void PrintLayoutKeys() {
        var keysString = "";

        foreach (var item in availableLayouts) {
            keysString += $" {item.key }";
        }

        if (string.IsNullOrEmpty(keysString)) {
            keysString = "No layouts added in the editor";
        }

        Debug.Log(keysString);
    }

    public bool SetBoardLayout(string key) {
        var newBoardLayout = GetLayoutFromKey(key);
        if (newBoardLayout == null) {
            Debug.LogError("Key invalid!");
            return false;
        }

        PlayerPrefs.SetString("layout", key);
        Debug.Log($"Layout set to '{key}'");

        return true;
    }

    private BoardLayout GetLayoutFromKey(string key) {
        foreach (var item in availableLayouts) {
            if (item.key == key) {
                return item;
            }
        }

        return null;
    }
}