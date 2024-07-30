using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; protected set; }
    public UIManager UIManager { get; protected set; }
    public ComboManager ComboManager { get; protected set; }

    public GameObject cardPrefab;
    public Transform cardParent;

    private List<Card> cards = new List<Card>();
    private Card previousCard = null;
    private Card currentCard = null;
    private int score = 0;
    private GridLayoutGroup gridLayoutGroup;
    private RectTransform cardParentRect;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        AudioManager = FindObjectOfType<AudioManager>();
        UIManager = FindObjectOfType<UIManager>();
        ComboManager = FindObjectOfType<ComboManager>();

        gridLayoutGroup = cardParent.GetComponent<GridLayoutGroup>();
        cardParentRect = cardParent.GetComponent<RectTransform>();

        UIManager.ShowMenu();
    }

    public void StartGame(int rows, int cols)
    {
        score = 0;
        ComboManager?.ResetCombo();
        SetCardLayout(rows, cols);
    }

    private void SetCardLayout(int rows, int cols)
    {
        ClearPreviousCards();
        CreateCardLayout(rows, cols);

        UIManager?.UpdateScoreText();
    }

    private void ClearPreviousCards()
    {
        foreach (Transform child in cardParent.transform)
        {
            Destroy(child.gameObject);
        }
        cards.Clear();
    }

    private void CreateCardLayout(int rows, int cols)
    {
        float containerWidth = cardParentRect.rect.width;
        float containerHeight = cardParentRect.rect.height;

        float maxCardWidth = containerWidth / cols;
        float maxCardHeight = containerHeight / rows;
        float cardSize = Mathf.Min(maxCardWidth, maxCardHeight);

        gridLayoutGroup.cellSize = new Vector2(cardSize, cardSize);
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = cols;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject cardObj = Instantiate(cardPrefab, cardParent);
                Card card = cardObj.GetComponent<Card>();
                cards.Add(card);
                card.Initialize((row * cols + col) / 2); // pairs
            }
        }

        ShuffleCards();
    }

    private void ShuffleCards()
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Card temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.SetSiblingIndex(i);
        }

        Invoke(nameof(FlipToBack), 1f);
    }

    private void FlipToBack()
    {
        foreach (Card card in cards)
        {
            card.ResetCard();
        }
    }

    public void CardSelected(Card card)
    {
        if (previousCard == null)
        {
            previousCard = card;
        }
        else if (currentCard == null)
        {
            currentCard = card;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.85f);

        if (previousCard.CardId == currentCard.CardId)
        {
            // Match
            previousCard.Match();
            currentCard.Match();
            score += 10 * ComboManager.GetComboMultiplier();
            ComboManager?.IncrementCombo();
            UIManager?.UpdateScoreText();
            AudioManager?.PlayMatchSound();
        }
        else
        {
            // Mismatch
            previousCard.Reset();
            currentCard.Reset();
            ComboManager?.ResetCombo();
            AudioManager?.PlayMismatchSound();
        }

        previousCard = null;
        currentCard = null;
    }

    public int GetScore()
    {
        return score;
    }
}