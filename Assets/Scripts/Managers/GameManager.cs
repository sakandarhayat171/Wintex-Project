using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    private GridLayoutGroup gridLayoutGroup;
    private RectTransform cardParentRect;
    private int score = 0;
    public int rows { get; private set; }
    public int cols { get; private set; }

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
        this.rows = rows;
        this.cols = cols;

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

            Invoke(nameof(CheckComplete), 1f);
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

    private void CheckComplete()
    {
        List<Card> tempCards = cards.Where(card => !card.isFlipped).ToList();

        if(tempCards.Count == 0 || tempCards.Count == 1)
        {
            UIManager?.ShowCompletePanel();
        }
    }

    public void SaveProgress()
    {
        SaveData saveData = new SaveData();
        saveData.score = score;
        saveData.rows = rows;
        saveData.cols = cols;
        saveData.cardStates = new List<int>();
        saveData.cardIDs = new List<int>();

        foreach (var card in cards)
        {
            saveData.cardStates.Add(card.isFlipped ? 1 : 0);
            saveData.cardIDs.Add(card.CardId);
        }

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + "/Wintex.json", json);
    }

    public void LoadProgress()
    {
        string path = Application.persistentDataPath + "/Wintex.json";
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);

                if (saveData != null)
                {
                    score = saveData.score;
                    SetCardLayout(saveData.rows, saveData.cols);

                    for (int i = 0; i < cards.Count; i++)
                    {
                        cards[i].Initialize(saveData.cardIDs[i]);
                        if (saveData.cardStates[i] == 1)
                        {
                            StartCoroutine(cards[i].FlipCard());
                        }
                        else
                        {
                            cards[i].ResetCard();
                        }
                    }
                }
                else
                {
                    Debug.LogError("Failed to load game: Save data is corrupted.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load game: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Save file not found.");
        }
    }
}