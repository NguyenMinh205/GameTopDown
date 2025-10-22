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

    [Header("UI & Popups")]
    [SerializeField] private GameObject _endGamePopUp;
    [SerializeField] private GameObject _waveNotification;
    [SerializeField] private GameObject _rewardPopup;

    [Header("Config")]
    [SerializeField] private float _waveNotifScaleInDuration = 0.35f;
    [SerializeField] private float _waveNotifHoldDuration = 0.65f;
    [SerializeField] private float _waveNotifScaleOutDuration = 0.25f;

    private int _currentWave = 1;
    public int CurrentWave => _currentWave;
    private Tween _waveTween;

    protected override void Awake()
    {
        base.Awake();
        Time.timeScale = 1;
        if (_endGamePopUp != null) _endGamePopUp.SetActive(false);
        if (_waveNotification != null) _waveNotification.SetActive(false);
    }

    private void Start()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        _playerController.Init();
        _enemyManager.Init();
        _endGamePopUp.SetActive(false);
        _waveNotification.SetActive(false);
        _currentWave = 1;
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
        GameUIController.Instance.ShowWaveText(_currentWave);
        _waveNotification.SetActive(true);
        _waveNotification.transform.localScale = new Vector3(1, 0, 1);

        _waveTween?.Kill();

        _waveTween = DOTween.Sequence()
                .Append(_waveNotification.transform.DOScale(1f, _waveNotifScaleInDuration).SetEase(Ease.OutBack))
                .AppendInterval(_waveNotifHoldDuration)
                .Append(_waveNotification.transform.DOScale(0f, _waveNotifScaleOutDuration).SetEase(Ease.InBack))
                .OnComplete(() =>
                {
                    _waveNotification.SetActive(false);
                    _waveSpawnerController.SetUpWaveData();
                });
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
        _currentWave++;
        ShowRewardEachWave();
    }

    public void ShowRewardEachWave()
    {

    }    

    public void ChooseReward(RewardSO reward)
    {
        Time.timeScale = 1;
        StartNewWave();
    }    

    public void PauseGame()
    {
        GameUIController.Instance.OpenSetting(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        GameUIController.Instance.OpenSetting(false);
        Time.timeScale = 1;
    }

    public void EndGame()
    {
        _enemyManager.ClearAllEnemy();
        _endGamePopUp.SetActive(true);
        Time.timeScale = 0;
    }

    public void BackHome()
    {
        Time.timeScale = 1;
    }
}
