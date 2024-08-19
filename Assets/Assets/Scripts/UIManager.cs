using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject ingameDisplay;
    public GameObject gameOverDisplay;

    public TMP_Text timeElapsedText;
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text levelText;

    public TMP_Text destroyedScoreText;
    public TMP_Text livesScoreText;
    public TMP_Text timePenaltyText;
    public TMP_Text victoryBonusText;
    public TMP_Text roundScoreText;
    public TMP_Text totalScoreText;

    public TMP_Text countdownText;

    public Button nextLevelButton;
    public Button playAgainButton;

    private GameManager _gm;
    private InputManager _im;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        _gm = GameManager.instance;
        _im = InputManager.instance;

        ingameDisplay.SetActive(true);
        gameOverDisplay.SetActive(false);
    }

    public void UpdateTime(int secondsElapsed)
    {
        int minutes = secondsElapsed / 60;
        int seconds = secondsElapsed % 60;
        timeElapsedText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateLives(int lives)
    {
        livesText.text = "Lives: " + lives;
    }

    public void UpdateCountdown(int countdown)
    {
        countdownText.text = countdown.ToString();
    }

    public void UpdateLevel(int level)
    {
        levelText.text = "Level " + level;
    }

    public void ShowCountdown()
    {
        countdownText.gameObject.SetActive(true);
        ingameDisplay.SetActive(true);
        gameOverDisplay.SetActive(false);
    }

    public void ShowIngameUI()
    {
        countdownText.gameObject.SetActive(false);
        ingameDisplay.SetActive(true);
        gameOverDisplay.SetActive(false);
    }

    public void ShowGameOver(int destroyedScore, int livesScore, int timePenalty, int victoryBonus, int roundScore, int totalScore, bool canNextLevel)
    {
        countdownText.gameObject.SetActive(false);
        ingameDisplay.SetActive(false);
        gameOverDisplay.SetActive(true);

        nextLevelButton.gameObject.SetActive(canNextLevel);
        playAgainButton.gameObject.SetActive(!canNextLevel);

        destroyedScoreText.text = destroyedScore.ToString();
        livesScoreText.text = livesScore.ToString();
        timePenaltyText.text = timePenalty.ToString();
        victoryBonusText.text = victoryBonus.ToString();
        roundScoreText.text = roundScore.ToString();
        totalScoreText.text = totalScore.ToString();
    }
}
