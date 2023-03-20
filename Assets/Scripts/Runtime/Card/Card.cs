using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour {
    [Header("Stats:")]
    [ReadOnly] public string displayName;
    [ReadOnly] public int strength;
    [ReadOnly] public int coins;
    [ReadOnly] public int hitPoints;
    [ReadOnly] public CardAbility.Type cardAbility;
    [ReadOnly] public string description;
    [ReadOnly] public AlignmentType alignment = AlignmentType.NONE;
    private Sprite graphic;

    [Header("References:")]
    public TMP_Text displayNameTMP;
    public TMP_Text strengthTMP;
    public TMP_Text coinsTMP;
    public TMP_Text descriptionTMP;
    public Image cardGraphicImage;
    public Image shadowImage;

    [Space(25)]
    [SerializeField] private ActionIndicator actionIndicator;
    [SerializeField] private HealthIndicator healthIndicator;
    [SerializeField] private StackCounter stackCounter;
    [SerializeField] private GameObject selectedOutline;

    public Canvas Canvas { get; private set; }

    public int InitStrength { get; private set; }

    private bool isZoomPreviewing = false;
    private Vector3 initPosition;
    private Vector3 screenToWorldPos;
    private Vector3 desiredPosition;

    private Vector3 smoothVelocity;

    public List<Space> MoveableSpaces { get; set; } = new List<Space>();

    private LTDescr zoomTween;

    private int desiredSortingOrder = 0;

    private void Awake() {
        Canvas = GetComponent<Canvas>();
    }

    public void Initialize(CardEntity cardtype) {
        stackCounter.SetCountText(0);

        InitializeStats(cardtype);
        InitializeDisplay();

        initPosition = transform.position;
        desiredPosition = initPosition;
        InitStrength = strength;
    }
    private void InitializeStats(CardEntity cardEntity) {
        displayName = cardEntity.displayName;
        strength = cardEntity.strength;
        cardAbility = cardEntity.cardAbility;
        graphic = cardEntity.cardGraphic;
        description = cardEntity.description;
    }

    private void InitializeDisplay() {
        displayNameTMP.text = displayName.ToString();
        UpdateStrengthDisplay();
        UpdateCoinsDisplay();
        descriptionTMP.text = GetDescriptionText();
        cardGraphicImage.sprite = graphic;

        selectedOutline.SetActive(false);
    }

    private string GetDescriptionText() {
        var abilityText = CardAbility.GetDescription(cardAbility);
        if (!string.IsNullOrEmpty(abilityText)) {
            return abilityText;
        }

        return description;
    }

    public Space GetSpace() {
        return transform.parent.GetComponent<Space>();
    }

    private void LateUpdate() {
        if (isZoomPreviewing) {
            screenToWorldPos = Camera.main.ScreenToWorldPoint(GetMousePosition());
            desiredPosition = screenToWorldPos;
        }

        LerpToDesiredPosition();
        DynamicRotation();
    }

    public void CheckMoveableSpaces() {
        MoveableSpaces.Clear();
        MoveableSpaces = BoardManager.Instance.Board.GetMoveableSpaces(GetSpace());
    }

    public void ShowActionIndicator() {
        var directions = BoardManager.Instance.GetDirectionsFromSpace(GetSpace()).ToArray();
        actionIndicator.Show(directions);
    }

    public void HideActionIndicator() {
        actionIndicator.HideAll();
    }

    public void SetStackCounter(int counter) {
        stackCounter.SetCountText(counter);
    }

    public void SetCoins(int newCoins) {
        coins = newCoins;
        UpdateCoinsDisplay();
    }

    public void AddCoins(int toAdd) {
        coins += toAdd;
        UpdateCoinsDisplay();
    }

    public void RemoveCoins(int toRemove) {
        coins -= toRemove;
        UpdateCoinsDisplay();
    }

    public int CoinsCount() {
        return coins;
    }

    public void UpdateStackBuff() {
        var space = GetSpace();
        strength = InitStrength;

        if (space.CardsCount > 1) {
            strength = strength + space.CardsCount - 1;
            strengthTMP.color = ColorManager.Instance.highlightText;
        } else {
            strengthTMP.color = ColorManager.Instance.lightCard;
        }

        UpdateStrengthDisplay();
    }

    public void RefreshDesiredPosition() {
        initPosition = GetSpace().transform.position;
        desiredPosition = initPosition;
    }

    public void TakeHitPoint(out bool hasEliminated) {
        hitPoints++;
        healthIndicator.UpdateIndicator(hitPoints);

        if (hitPoints > 1) { 
            hasEliminated = true;
            EliminateCard();
            return;
        }

        hasEliminated = false;
    }

    public void Restore() {
        hitPoints = 0;
        healthIndicator.UpdateIndicator(hitPoints);
    }

    public void EliminateCard() {
        GetSpace().RemoveFromSpace(this);
        if (alignment == AlignmentType.PLAYER)
            BoardManager.Instance.PlayerCards.Remove((PlayerCard)this);
        if (alignment == AlignmentType.ENEMY)
            BoardManager.Instance.EnemyCards.Remove((EnemyCard)this);
        Destroy(gameObject);
    }

    public void Move(Space targetSpace) {
        GetSpace().RemoveFromSpace(this);

        transform.SetParent(targetSpace.transform);
        targetSpace.AddToSpace(this);

        RefreshDesiredPosition();
        StartMoveZoomTween();
    }

    public void StealCoins(Card from, int amount) {
        if (from.CoinsCount() == 0)
            return;

        from.RemoveCoins(amount);
        AddCoins(amount);
    }

    #region MOVE
    private void LerpToDesiredPosition() {
        var speed = 0.1f;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref smoothVelocity, speed);
        shadowImage.transform.localPosition = screenToWorldPos * 0.01f;
    }

    private void DynamicRotation() {
        var newRotation = new Vector3(smoothVelocity.y, -smoothVelocity.x, 0) * 2;
        transform.rotation = Quaternion.Euler(newRotation);
    }

    private Vector3 GetMousePosition() {
        var mouse = Input.mousePosition;
        mouse.z = 25;
        return mouse;
    }

    public void StartZoomPreview() {
        isZoomPreviewing = true;
        DisplayOnTopOfAll();
        transform.localScale = Vector3.one * 1.5f;
        shadowImage.gameObject.SetActive(true);
    }

    public void EndZoomPreview() {
        isZoomPreviewing = false;
        DisplayOnTopOfStack();
        transform.localScale = Vector3.one;
        desiredPosition = initPosition;
        shadowImage.gameObject.SetActive(false);
    }

    #endregion

    #region DISPLAY METHODS
    public void StartMoveZoomTween() {
        DisplayOnTopOfAll();

        var originalSize = Vector3.one;
        var targetSize = Vector3.one * 1.5f;

        if (zoomTween != null) {
            LeanTween.cancel(zoomTween.id);
            transform.localScale = originalSize;
        }

        var seconds = 0.25f;
        var secondsHalf = seconds / 2;

        var sequence = LeanTween.sequence();
        sequence.append(LeanTween.value(0, 1, secondsHalf).setOnUpdate((float t) => {
            transform.localScale = Vector3.Lerp(originalSize, targetSize, t);
        }).setEaseInOutSine());
        sequence.append(LeanTween.value(0, 1, secondsHalf).setOnUpdate((float t) => {
            transform.localScale = Vector3.Lerp(targetSize, originalSize, t);
        }).setEaseInOutSine());
        sequence.append(DisplayOnTopOfStack);
    }

    public void DisplayOnTopOfStack() {
        desiredSortingOrder = GetSpace().TopCardIndex;
        Canvas.sortingOrder = desiredSortingOrder;
    }

    public void DisplayOnTopOfAll() {
        desiredSortingOrder = 1000;
        Canvas.sortingOrder = desiredSortingOrder;
    }

    private void UpdateStrengthDisplay() {
        strengthTMP.text = strength.ToString();
    }

    private void UpdateCoinsDisplay() {
        coinsTMP.text = coins.ToString();
    }

    public void ShowSelectedOutline() {
        selectedOutline.SetActive(true);
    }

    public void HideSelectedOutline() {
        selectedOutline.SetActive(false);
    }
    #endregion
}

public enum AlignmentType {
    NONE = 0,
    PLAYER = 1,
    ENEMY = 2
}