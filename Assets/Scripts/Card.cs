using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int CardId { get; private set; }
    private bool isFlipped = false;
    private Button button;
    private Image image;
    public Sprite frontImage;
    public Sprite backImage;

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        button.onClick.AddListener(OnCardClicked);
    }

    public void Initialize(int id)
    {
        CardId = id;
        frontImage = Resources.Load<Sprite>($"Cards/{CardId}");
        image.sprite = backImage;
    }

    private void OnCardClicked()
    {
        if (!isFlipped)
        {
            Flip();
            GameManager.Instance?.CardSelected(this);
        }
    }

    public void Flip()
    {
        isFlipped = !isFlipped;
        image.sprite = isFlipped ? frontImage : backImage;
        GameManager.Instance?.AudioManager?.PlayFlipSound();
    }

    public void Reset()
    {
        isFlipped = false;
        image.sprite = backImage;
    }

    public void Match()
    {
        // Logic for matching cards
        button.interactable = false;
    }
}