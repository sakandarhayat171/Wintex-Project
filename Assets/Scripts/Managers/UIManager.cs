using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Layout Panel")]
    public GameObject layoutSelectionPanel;
    public TMP_Dropdown rowsDropdown;
    public TMP_Dropdown colsDropdown;
    public Button startButton;

    [Header("Gameplay Panel"), Space]
    public GameObject gameplayPanel;
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        startButton?.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        int rows = int.Parse(rowsDropdown.options[rowsDropdown.value].text);
        int cols = int.Parse(colsDropdown.options[colsDropdown.value].text);

        layoutSelectionPanel?.SetActive(false);
        gameplayPanel?.SetActive(true);
        GameManager.Instance?.SetCardLayout(rows, cols);
    }

    public void ShowLayoutSelection()
    {
        layoutSelectionPanel?.SetActive(true);
        gameplayPanel?.SetActive(false);
    }

    public void UpdateScoreText()
    {
        scoreText?.SetText($"Score : {GameManager.Instance?.GetScore()}");
    }
}
