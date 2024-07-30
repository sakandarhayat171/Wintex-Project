using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public AudioManager AudioManager { get; protected set; }

    public GameObject cardPrefab;
    public GridLayoutGroup cardParent;
    public int rows = 2;
    public int cols = 2;
    private List<Card> cards = new List<Card>();
    private Card previousCard = null;
    private Card currentCard = null;
    private int score = 0;

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
        InitializeGame();
    }

    private void InitializeGame()
    {
        if (cardParent.constraint.Equals(GridLayoutGroup.Constraint.FixedRowCount))
        {
            cardParent.constraintCount = rows;
        }
        else if (cardParent.constraint.Equals(GridLayoutGroup.Constraint.FixedColumnCount))
        {
            cardParent.constraintCount = cols;
        }

        // Initialize card layout and instantiate cards
        CreateCardLayout(rows, cols);
        ShuffleCards();
    }

    private void CreateCardLayout(int rows, int cols)
    {
        // Example of creating and positioning cards
        for (int i = 0; i < rows * cols; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardParent.transform);
            Card card = cardObj.GetComponent<Card>();
            cards.Add(card);
            card.Initialize(i / 2); // Assume pairs
        }
    }

    private void ShuffleCards()
    {
        // Shuffle the cards
        for (int i = 0; i < cards.Count; i++)
        {
            Card temp = cards[i];
            int randomIndex = Random.Range(i, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
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
        yield return new WaitForSeconds(0.5f);

        if (previousCard.CardId == currentCard.CardId)
        {
            // Match
            previousCard.Match();
            currentCard.Match();
            score++;
            AudioManager?.PlayMatchSound();
        }
        else
        {
            // Mismatch
            previousCard.Reset();
            currentCard.Reset();
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