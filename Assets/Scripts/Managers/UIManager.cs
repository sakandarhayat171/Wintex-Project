using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Menu Panel")]
    public GameObject menuPanel;
    public Button playButton;
    public Button exitButton;
    public Toggle soundToggle;
    public TextMeshProUGUI soundToggleText;

    [Header("Layout Panel"), Space]
    public GameObject layoutSelectionPanel;
    public TMP_Dropdown rowsDropdown;
    public TMP_Dropdown colsDropdown;
    public Button startButton;
    public Button backButton;

    [Header("Gameplay Panel"), Space]
    public GameObject gameplayPanel;
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        playButton?.onClick.AddListener(OnPlayButtonClicked);
        exitButton?.onClick.AddListener(OnExitButtonClicked);
        soundToggle?.onValueChanged.AddListener(OnToggleSoundClicked);

        startButton?.onClick.AddListener(OnStartButtonClicked);
        backButton?.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        GameManager.Instance?.AudioManager?.PlayClickSound();
        ShowLayoutSelection();
        menuPanel?.SetActive(false);
    }

    public void OnToggleSoundClicked(bool isOn)
    {
        GameManager.Instance?.AudioManager?.PlayClickSound();
        soundToggleText.SetText(isOn ? "Un-mute" : "Mute");
        AudioListener.volume = isOn ? 0 : 1;
    }

    private void OnExitButtonClicked()
    {
        GameManager.Instance?.AudioManager?.PlayClickSound();
        Application.Quit();
    }

    private void OnStartButtonClicked()
    {
        GameManager.Instance?.AudioManager?.PlayClickSound();
        int rows = int.Parse(rowsDropdown.options[rowsDropdown.value].text);
        int cols = int.Parse(colsDropdown.options[colsDropdown.value].text);

        layoutSelectionPanel?.SetActive(false);
        gameplayPanel?.SetActive(true);
        GameManager.Instance?.StartGame(rows, cols);
    }

    private void OnBackButtonClicked()
    {
        GameManager.Instance?.AudioManager?.PlayClickSound();
        layoutSelectionPanel?.SetActive(false);
        menuPanel?.SetActive(true);
    }

    public void ShowLayoutSelection()
    {
        layoutSelectionPanel?.SetActive(true);
        gameplayPanel?.SetActive(false);
    }

    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        gameplayPanel?.SetActive(false);
    }

    public void UpdateScoreText()
    {
        scoreText?.SetText($"Score : {GameManager.Instance?.GetScore()}");
    }
}
