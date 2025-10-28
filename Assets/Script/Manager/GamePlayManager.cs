using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    OnEnemyDie,
}

public class GamePlayManager : Singleton<GamePlayManager>
{
    [Header("Controller")]
    [SerializeField] private PlayerController _playerController;
    public PlayerController PlayerController => _playerController;

    [SerializeField] private EnemyManager _enemyManager;
    public EnemyManager EnemyManager => _enemyManager;

    [SerializeField] private WaveSpawnerController _waveSpawnerController;
    public WaveSpawnerController WaveSpawnerController => _waveSpawnerController;

    [SerializeField] private RewardManager _rewardManager;
    public RewardManager RewardManager => _rewardManager;

    [SerializeField] private BuffController _buffController;
    public BuffController BuffController => _buffController;

    private int _currentWave = 1;
    public int CurrentWave => _currentWave;
    private Tween _waveTween;
    private bool _isGamePaused = false;
    public bool IsGamePaused => _isGamePaused;
    private bool _isChoosingReward = false;
    public bool IsChoosingReward => _isChoosingReward;

    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 1;
    }

    private void Start()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        _playerController.Init();
        _enemyManager.Init();
        GameUIController.Instance.ShowEndGamePopup(false);
        GameUIController.Instance.ShowRewardPopup(false);
        _currentWave = 0;
        _buffController.Init();
        GameUIController.Instance.SetupStartGame();
        StartNewWave();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseBuff(BuffType.Buff1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseBuff(BuffType.Buff2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseBuff(BuffType.Buff3);
        }
    }

    public void StartNewWave()
    {
        _currentWave++;
        GameUIController.Instance.ShowWaveText(_currentWave);
        GameUIController.Instance.ShowWaveNotification();
    }

    public void UseBuff(BuffType buffType)
    {
        switch (buffType)
        {
            case BuffType.Buff1:
                _buffController.UseBuff1();
                break;
            case BuffType.Buff2:
                _buffController.UseBuff2();
                break;
            case BuffType.Buff3:
                _buffController.UseBuff3();
                break;
            default:
                break;
        }
    }

    public void EndWave()
    {
        DataManager.Instance.GameData.Coin += 10 * _currentWave;
        GameUIController.Instance.UpdateCoin(DataManager.Instance.GameData.Coin);
        ShowRewardEachWave();
    }

    public void ShowRewardEachWave()
    {
        _isChoosingReward = true;
        GameUIController.Instance.ShowRewardPopup(true);
        _rewardManager.GenerateReward();
        _playerController.PauseRegenShield();
    }    

    public void EndChooseReward()
    {
        _isChoosingReward = false;
        GameUIController.Instance.ShowRewardPopup(false);
        _playerController.ResumeRegenShield();
        StartNewWave();
    }

    public void PauseGame()
    {
        GameUIController.Instance.ShowSetting(true);
        _isGamePaused = true;
        _playerController.PauseRegenShield();
    }

    public void ResumeGame()
    {
        GameUIController.Instance.ShowSetting(false);
        _isGamePaused = false;
        _playerController.ResumeRegenShield();
    }

    public void EndGame()
    {
        _enemyManager.ClearAllEnemy();
        GameUIController.Instance.ShowEndGamePopup(true);
        _enemyManager.StopAllCoroutines();
        _enemyManager.ClearAllEnemy();
    }

    public void BackHome()
    {
        Time.timeScale = 1;
    }
}
