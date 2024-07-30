using System.Collections;
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
    public float flipDuration = 0.5f;

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
            StartCoroutine(FlipCard());
            GameManager.Instance?.CardSelected(this);
        }
    }

    private IEnumerator FlipCard()
    {
        isFlipped = true;
        GameManager.Instance?.AudioManager.PlayFlipSound();

        float time = 0f;
        while (time < flipDuration)
        {
            float t = time / flipDuration;
            float angle = Mathf.Lerp(180f, 360f, t);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            time += Time.deltaTime;
            if(time >= flipDuration / 2)
                image.sprite = isFlipped ? frontImage : backImage;

            yield return null;
        }
    }

    public void Reset()
    {
        isFlipped = false;
        image.sprite = backImage;
        transform.rotation = Quaternion.Euler(0f, 0f, 180f);
    }

    public void Match()
    {
        // Logic for matching cards
        button.interactable = false;
    }
}