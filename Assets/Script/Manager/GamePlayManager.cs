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

    [Header("Animation and Particle")]
    [SerializeField] private GameObject _explosionShotAnim;
    public GameObject ExplosionShotAnim => _explosionShotAnim;
    [SerializeField] private GameObject _explosionTankAnim;
    public GameObject ExplosionTankAnim => _explosionTankAnim;
    [SerializeField] private GameObject _explosionBulletAnim;
    public GameObject ExplosionBulletAnim => _explosionBulletAnim;
    [SerializeField] private Transform _objectPool;

    private int _currentWave = 1;
    public int CurrentWave => _currentWave;
    private Tween _waveTween;
    private bool _isGamePaused = false;
    public bool IsGamePaused => _isGamePaused;
    private bool _isChoosingReward = false;
    public bool IsChoosingReward
    {
        get { return _isChoosingReward; }
        set { _isChoosingReward = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 1;
    }

    private void Start()
    {
        if (DataManager.Instance.GameData.IsFirstTimePlay)
        {
            ShowInstruction();
            DataManager.Instance.GameData.IsFirstTimePlay = false;
        }    
        AudioManager.Instance.PlayMusicInGame();
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
        if (_isGamePaused) return;
        _currentWave++;
        GameUIController.Instance.ShowWaveText(_currentWave);
        GameUIController.Instance.ShowWaveNotification();
        AudioManager.Instance.PlayStartWaveSound();
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

    public void SpawnExplosionShotAnim(Vector3 position)
    {
        PoolingManager.Spawn(_explosionShotAnim, position, Quaternion.identity, _objectPool);
    }

    public void SpawnExplosionTankAnim(Vector3 position)
    {
        PoolingManager.Spawn(_explosionTankAnim, position, Quaternion.identity, _objectPool);
    }

    public void SpawnExplosionBullletAnim(Vector3 position)
    {
        PoolingManager.Spawn(_explosionBulletAnim, position, Quaternion.identity, _objectPool);
    }

    public void ShowInstruction()
    {
        AudioManager.Instance.PlayPopupSound();
        GameUIController.Instance.ShowInstruction(true);
        GameUIController.Instance.SetupInstructionDetail();
        _isGamePaused = true;
        _playerController.PauseRegenShield();
    }    

    public void FinishInstruction()
    {
        AudioManager.Instance.PlayPopupSound();
        GameUIController.Instance.ShowInstruction(false);
        _isGamePaused = false;
        _playerController.ResumeRegenShield();
    }    

    public void ShowRewardEachWave()
    {
        AudioManager.Instance.PlayPopupSound();
        _isChoosingReward = true;
        _isGamePaused = true;
        GameUIController.Instance.ShowRewardPopup(true);
        _rewardManager.GenerateReward();
        _playerController.PauseRegenShield();
    }    

    public void EndChooseReward()
    {
        AudioManager.Instance.PlayPopupSound();
        _isChoosingReward = false;
        _isGamePaused = false;
        GameUIController.Instance.ShowRewardPopup(false);
        _playerController.ResumeRegenShield();
        StartNewWave();
    }

    public void PauseGame()
    {
        AudioManager.Instance.PlayPopupSound();
        GameUIController.Instance.ShowSetting(true);
        _isGamePaused = true;
        _playerController.PauseRegenShield();
    }

    public void ResumeGame()
    {
        AudioManager.Instance.PlayButtonClick();
        GameUIController.Instance.ShowSetting(false);
        _isGamePaused = false;
        _playerController.ResumeRegenShield();
    }

    public void EndGame()
    {
        _enemyManager.ClearAllEnemy();
        GameUIController.Instance.ShowEndGamePopup(true);
        GameUIController.Instance.ShowEndGameDetails(_currentWave, _enemyManager.NumOfEnemyKilled);
        AudioManager.Instance.PlayEndGameSound();
        _enemyManager.StopAllCoroutines();
        _enemyManager.ClearAllEnemy();
    }

    public void BackHome()
    {
        AudioManager.Instance.PlayButtonClick();
        Time.timeScale = 1;
    }
}
