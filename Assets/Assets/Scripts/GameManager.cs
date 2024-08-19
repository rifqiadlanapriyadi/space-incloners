using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int _score;
    private int _totalScore;

    private float _startingTime;
    public float GameTime()
    {
        return Time.time - _startingTime;
    }

    public int destroyEnemyScore = 100;
    public int remainingLivesScore = 500;
    public int penaltyPerSecond = 2;
    public int victoryBonus = 1000;

    public GameObject playerPrefab;
    public GameObject playerStartPosition;
    public GameObject enemies;
    public GameObject enemiesStartPosition;
    public GameObject bulletPool;

    public int countDownLength = 3;
    private bool _countingDown;
    private float _countdownFrom;

    public int lastLevel = 4;
    private int _level;

    private GameObject _player;

    private bool _isSuspended;
    public bool IsSuspended
    {
        get { return _isSuspended; }
        private set { _isSuspended = value; }
    }

    private InputManager _im;
    private UIManager _ui;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        _im = InputManager.instance;
        _ui = UIManager.instance;

        RestartFirstLevel();
    }

    private void Update()
    {
        if (_countingDown) UpdateCountdown();
        if (!_countingDown) _ui.UpdateTime((int)GameTime());
    }

    public void ResetAndPlay()
    {
        _im.InputAllowed = false;
        IsSuspended = true;

        enemies.SetActive(true);
        enemies.transform.position = enemiesStartPosition.transform.position;
        enemies.GetComponent<EnemiesController>().Reset();

        _startingTime = Time.time;
        _score = 0;

        _ui.UpdateTime((int)GameTime());
        _ui.UpdateScore(_score);
        _ui.UpdateLevel(_level);
        StartCountdown();
    }

    private void ResetPlayer(int lives = -1)
    {
        if (_player != null) Destroy(_player);
        _player = Instantiate(playerPrefab);
        _player.transform.position = playerStartPosition.transform.position;
        if (lives == -1) _player.GetComponent<PlayerController>().Reset();
        else _player.GetComponent<PlayerController>().Reset(lives);
    }

    public void SetupNextLevel()
    {
        int lives = _player == null ? -1 : _player.GetComponent<PlayerController>().Lives;
        ResetPlayer(lives);

        _level++;
        enemies.GetComponent<EnemiesController>().SetupNextLevelSettings();
        ResetAndPlay();
    }

    public void RestartFirstLevel()
    {
        ResetPlayer();

        _totalScore = 0;
        _level = 1;
        enemies.GetComponent<EnemiesController>().ResetFirstLevelSettings();
        ResetAndPlay();
    }

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void StartCountdown()
    {
        _countingDown = true;
        _countdownFrom = Time.time;

        _ui.ShowCountdown();
    }

    private void UpdateCountdown()
    {
        float elapsedTime = Time.time - _countdownFrom;
        _ui.UpdateCountdown(countDownLength - (int) elapsedTime);
        if (elapsedTime >= countDownLength)
        {
            _countingDown = false;
            _ui.ShowIngameUI();

            _im.InputAllowed = true;
            IsSuspended = false;
            _startingTime = Time.time;
        }
    }

    private void GameOver(bool won)
    {
        int enemiesContr = _score;
        int livesContr = remainingLivesScore * _player.GetComponent<PlayerController>().Lives;
        int timePenalty = -penaltyPerSecond * (int) GameTime();
        int victoryContr = won ? victoryBonus : 0;
        int roundScore = livesContr + enemiesContr + timePenalty + victoryContr;
        _totalScore += roundScore;
        bool canNextLevel = won && _level < lastLevel;
        _ui.ShowGameOver(enemiesContr, livesContr, timePenalty, victoryContr, roundScore, _totalScore, canNextLevel);

        _im.InputAllowed = false;

        foreach (Transform bulletTransform in bulletPool.transform)
            if (bulletTransform.gameObject.activeSelf)
                bulletTransform.GetComponent<Fireball>().ReturnToPool();

        IsSuspended = true;
    }

    public void HandleEnemyKilled(int killedCount)
    {
        _score += killedCount * destroyEnemyScore;
        _ui.UpdateScore(_score);
    }

    public void HandleEnemyEnterPlayerZone()
    {
        GameOver(false);
    }

    public void HandleAllEnemiesKilled()
    {
        GameOver(true);
    }

    public void HandlePlayerDeath()
    {
        GameOver(false);
    }
}
